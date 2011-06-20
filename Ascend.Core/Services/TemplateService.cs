using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ascend.Core.Services
{
    public class BasicTemplateService : IMessagingBuilder
    {
        public Email Transform(Template template, TemplateData data)
        {
            return Transform(template, data, new HttpRequestWrapper(HttpContext.Current.Request));
        }

        public Email Transform(Template template, TemplateData data, HttpRequestBase request)
        {
            return new Email {
                Subject = Render(template.Subject, data, request),
                Body = Render(template.Content, data, request),
            };
        }
        
        public string Render(string input, TemplateData data, HttpRequestBase request)
        {
            // perform template replacement
            var a = data.Aggregate(
                input,
                (current, pair) => current.Replace("{" + pair.Key + "}", pair.Value)
            );

            // perform url replacements
            var x = a.IndexOf("\"~/");
            while (x >= 0)
            {
                var y = a.IndexOf("\"", x+3);
                if (y < x)
                {
                    y = x;
                }
                else
                {
                    a = a.Substring(0, x+1) + 
                        a.Substring(x+1, y - x).ToAbsoluteUrl(request) +
                        a.Substring(y+1);
                }
                x = a.IndexOf("\"~/", y);
            }
            
            return a;
        }
    }
}
