using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Anniversary : Program
    {
        // determine exactly when anniversary awards are issued:
        // * for hire date or birth day
        // * every year (null or empty intervals), or on a 1st, 3rd, 5th ([1,3,5]) schedule
        // * whether the intervals repeat. for example, to setup an every-other year program:
        //   - Intervals: [2],
        //   - Repeating: true,
        public AnniversaryType Type { get; set; }
        public int[] Intervals { get; set; }     
        public bool Repeating { get; set; }

        // award details
        public string Budget { get; set; }
        public int Amount { get; set; }
    }

    public enum AnniversaryType
    {
        Hire = 0,
        Birth = 1,
    }
}
