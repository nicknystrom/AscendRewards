using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Ascend.Core
{
    public class Email : Entity
    {
        public EmailAddress Sender { get; set; }
        public EmailAddress Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public DateTime? Queued { get; set; }
        public DateTime? Sent { get; set; }
        public IList<EmailSendAttempt> Attempts { get; set; }

        public void AddAttempt(EmailSendAttempt attempt)
        {
            (Attempts ?? (Attempts = new List<EmailSendAttempt>(1))).Add(attempt);
        }

        public virtual MailMessage ToMailMessage()
        {
            var msg = new MailMessage
            {
                From = Sender.ToMailAddress(),
                Subject = Subject,
                Body = Body,
                IsBodyHtml = true,
            };
            msg.To.Add(Recipient.ToMailAddress());
            return msg;
        }
    }

    public class EmailAddress
    {
        public string Address { get; set; }
        public string Name { get; set; }

        public MailAddress ToMailAddress()
        {
            return new MailAddress(Address, Name);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Address);
        }
    }

    public class EmailSendAttempt
    {
        public string Server { get; set; }
        public DateTime Date { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
