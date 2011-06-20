using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public static class OXBuiltInFormats
    {
        private static readonly Dictionary<uint, string> _formats;

        static OXBuiltInFormats()
        {
            _formats = new Dictionary<uint, string>();

            _formats.Add(1, "0");
            _formats.Add(2, "0.00");
            _formats.Add(3, "#,##0");
            _formats.Add(4, "#,##0.00");
            _formats.Add(9, "0%");
            _formats.Add(10, "0.00%");
            _formats.Add(11, "0.00E+00");
            _formats.Add(12, "# ?/?");
            _formats.Add(13, "# ??/??");
            _formats.Add(14, "mm-dd-yy");
            _formats.Add(15, "d-mmm-yy");
            _formats.Add(16, "d-mmm");
            _formats.Add(17, "mmm-yy");
            _formats.Add(18, "h:mm AM/PM");
            _formats.Add(19, "h:mm:ss AM/PM");
            _formats.Add(20, "h:mm");
            _formats.Add(21, "h:mm:ss");
            _formats.Add(22, "m/d/yy h:mm");
            _formats.Add(37, "#,##0 ;(#,##0)");
            _formats.Add(38, "#,##0 ;(#,##0)");
            _formats.Add(39, "#,##0.00;(#,##0.00)");
            _formats.Add(40, "#,##0.00;(#,##0.00)");
            _formats.Add(45, "mm:ss");
            _formats.Add(46, "[h]:mm:ss");
            _formats.Add(47, "mmss.0");
            _formats.Add(48, "##0.0E+0");
            _formats.Add(49, "@");
        }

        public static string GetFormat(uint code)
        {
            string ret;

            _formats.TryGetValue(code, out ret);

            return ret;
        }
    }
}
