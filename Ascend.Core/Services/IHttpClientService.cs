using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Ascend.Core.Services
{
    public interface IHttpClientService
    {
        HttpWebResponse LoadResponse(Uri uri);
        Stream LoadStream(Uri uri);
    }

    public static class __StreamingExtensions
    {
        public static byte[] ToArray(this Stream s, long length)
        {
            var buf = new byte[length];
            var i = s.Read(buf, 0, 1024);
            var consumed = i;
            while (i > 0 && consumed < length)
            {
                i = s.Read(buf, consumed, (int)Math.Min(length - consumed, 1024));
                consumed += i;
            }
            return buf;
        }
    }
}
