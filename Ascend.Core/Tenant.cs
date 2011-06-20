using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Tenant : Entity
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Database { get; set; }
        public string[] Match { get; set; }
        public string EmailDomain { get; set; }
    }
}
