using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Group : Entity
    {
        [Required] public string Name { get; set; }
        public string Number { get; set; }
        
        public string Catalog { get; set; }
        public string HomePage { get; set; }
        public string TermsOfService { get; set; }
        public string BannerImage { get; set; }
    }
}
