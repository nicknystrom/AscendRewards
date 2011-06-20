using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ascend.Core.Repositories;

namespace Ascend.Core
{
    public class Award : Program
    {
        public IList<Certificate> Certificates { get; set; }
        
        public bool OpenNomination { get; set; }
        public IList<string> NomineeUsers { get; set; }
        public IList<string> NomineeGroups { get; set; }

        public bool IsValidNominee(UserSummary user)
        {
            if (user.State != UserState.Active) return false;
            return OpenNomination ||
                   (null != NomineeGroups && NomineeGroups.Contains(user.Group)) ||
                   (null != NomineeUsers && NomineeUsers.Contains(user.Id));
        }
    }

    public class Certificate
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }

        public string BackgroundUrl { get; set; }
        public string BackgroundContentType { get; set; }
        public Size BackgroundSize { get; set; }

        [UIHint("TextArea"), DisplayName("Default Message")] public string DefaultMessage { get; set; }
        [DisplayName("Word Limit")] public int? MessageWordLimit { get; set; }

        public CertificateArea FromBox { get; set; }
        public CertificateArea DateBox { get; set; }
        public CertificateArea ToBox { get; set; }
        public CertificateArea MessageBox { get; set; }
        public CertificateArea AwardBox { get; set; }
    }

    public class UserAward : Entity
    {
        public string Nominator { get; set; }
        public string Recipient { get; set; }
        public string Award { get; set; }
        
        public bool EmailRecipient { get; set; }
        public int? Amount { get; set; }
        public string Message { get; set; }
        public DateTime? Sent { get; set; }
        public Certificate Certificate { get; set; }
    }

    public class CertificateArea
    {
        public bool Enabled { get; set; }

        public string FontFace { get; set; }
        public int FontSize { get; set; }
        public string FontColor { get; set; }
        public string Alignment { get; set; }

        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
