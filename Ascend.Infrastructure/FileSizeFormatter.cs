using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Infrastructure
{
    /// <summary>
    /// Courtesy http://blogs.interakting.co.uk/brad/archive/2008/01/24/c-getting-a-user-friendly-file-size-as-a-string.aspx
    /// </summary>
    public static  class FileSizeFormatter
    {
        private static readonly string[] _format = new []
                                                       {
                                                           "{0:###,###,##0.##} bytes", 
                                                           "{0:###,###,##0.##} KB", 
                                                           "{0:###,###,##0.##} MB", 
                                                           "{0:###,###,##0.##} GB", 
                                                           "{0:###,###,##0.##} TB", 
                                                           "{0:###,###,##0.##} PB", 
                                                           "{0:###,###,##0.##} EB", 
                                                           "{0:###,###,##0.##} ZB", 
                                                           "{0:###,###,##0.##} YB"
                                                       };

        public static string ToFileSizeString(this int size)
        {
            return ToFileSizeString((long) size);
        }

        public static string ToFileSizeString(this long size)
        {
            double s = size;    
            foreach (var t in _format)
            {
                if (s <= 1024) return string.Format(t, s);
                s /= 1024;
            }
            throw new ArgumentOutOfRangeException("size");
        }
    }
}
