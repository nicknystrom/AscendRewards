using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ascend.Core.Services;

namespace Ascend.Infrastructure
{
    public class HttpClientService : IHttpClientService
    {
        public HttpWebResponse LoadResponse(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            return (HttpWebResponse)request.GetResponse();
        }

        public Stream LoadStream(Uri uri)
        {
            return LoadResponse(uri).GetResponseStream();
        }
    }
}
