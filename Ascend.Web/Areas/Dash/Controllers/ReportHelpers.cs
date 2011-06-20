using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Ascend.Web.Areas.Dash.Controllers
{
    public static class ReportHelpers
    {
        public static MvcHtmlString DashboardReport<TModel>(this HtmlHelper<IEnumerable<TModel>> html)
        {
            var builder = new StringBuilder(1024*100);
            builder.Append("<table><thead><tr>");
            
            var meta = new ViewDataDictionary<TModel>().ModelMetadata;
            foreach (var p in meta.Properties)
            {
                builder.Append("<th>");
                builder.Append(html.Encode(p.DisplayName ?? p.PropertyName));
                builder.Append("</th>");
            }
            builder.Append("</tr></thead><tbody>");
            foreach (var i in html.ViewData.Model)
            {
                builder.Append("<tr>");
                foreach (var p in meta.Properties)
                {
                    var value = p.ContainerType.GetProperty(p.PropertyName).GetValue(i, null);
                    builder.Append("<td>");
                    builder.Append(
                        html.Encode(null == p.DisplayFormatString
                            ? Convert.ToString(value ?? p.NullDisplayText)
                            : String.Format(p.DisplayFormatString, value ?? p.NullDisplayText))
                    );
                    builder.Append("</td>");
                }
                builder.Append("</tr>");
            }

            builder.Append("</tbody></table>");

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}