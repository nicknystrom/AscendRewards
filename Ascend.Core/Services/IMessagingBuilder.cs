using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ascend.Core.Services
{
    public class TemplateData : Dictionary<string, string>
    {

    }


    public interface IMessagingBuilder
    {
        Email Transform(Template template, TemplateData data);
        Email Transform(Template template, TemplateData data, HttpRequestBase request);
    }
}
