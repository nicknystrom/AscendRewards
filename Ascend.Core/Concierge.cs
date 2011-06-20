using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Concierge : Entity
    {
        public ConciergeStage Stage { get; set; }
        public string User { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }

    public enum ConciergeStage
    {
        Submitted = 0,
        Replied   = 10,
        Sourced   = 20,
        Ordered   = 30,
    }
}
