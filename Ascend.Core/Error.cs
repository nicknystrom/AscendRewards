using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Error : Entity
    {
        public string Url { get; set; }
        public IDictionary<string, string> FormValues { get; set; }
        public IDictionary<string, string> QueryValues { get; set; }

        public string Type { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}
