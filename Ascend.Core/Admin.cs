using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Admin : Entity
    {
        public Dictionary<string, AdminAccount> Accounts { get; set; }
    }

    public class AdminAccount
    {
        public string Password { get; set; }
    }
}
