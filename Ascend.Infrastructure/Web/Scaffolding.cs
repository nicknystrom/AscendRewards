using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Ascend.Infrastructure.Web
{
    public interface IScaffolding<TModel>
    {
        void Open();
        void Close();
        void Custom(string label, string field);
        void Field<TProperty>(Expression<Func<TModel, TProperty>> expression);
        void Field<TProperty>(Expression<Func<TModel, TProperty>> expression, string help);
        void Notes(string notes);
        void Seperator();
        void Submit(string text);
        void Display<TProperty>(Expression<Func<TModel, TProperty>> expression);
    }

    public abstract class BaseScaffolding<TModel> : IScaffolding<TModel>
    {
        protected StringBuilder Buffer { get; private set; }
        protected HtmlHelper<TModel> Html { get; private set; }

        protected abstract string SubmitFormat { get; }
        protected abstract string SeperatorFormat { get; }
        protected abstract string DefaultFieldFormat { get; }
        protected abstract string BooleanFieldFormat { get; }
        protected abstract string DisplayFieldFormat { get; }


        protected BaseScaffolding(HtmlHelper<TModel> html)
        {
            Buffer = new StringBuilder(1024);
            Html = html;
        }

        public virtual void Field<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Field(expression, null);
        }

        public virtual void Field<TProperty>(Expression<Func<TModel, TProperty>> expression, string help)
        {
            var format = DefaultFieldFormat;
            var meta = ModelMetadata.FromLambdaExpression(expression, Html.ViewData);
            if (meta.ModelType == typeof(bool) ||
                meta.ModelType == typeof(bool?))
            {
                format = BooleanFieldFormat;
            }

            Buffer.AppendFormat(
                format,
                Html.LabelFor(expression),
                Html.EditorFor(expression),
                Html.ValidationMessageFor(expression),
                help ?? meta.Description
            );    
        }

        public virtual void Notes(string notes)
        {
            Buffer.AppendFormat(
                DisplayFieldFormat,
                "",
                notes);
        }

        public virtual void Display<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Buffer.AppendFormat(
                DisplayFieldFormat,
                Html.LabelFor(expression),
                Html.DisplayFor(expression)
            );    
        }

        public virtual void Custom(string label, string field)
        {
            Buffer.AppendFormat(
                DefaultFieldFormat,
                label,
                field,
                "");
        }

        public virtual void Seperator()
        {
            Buffer.Append(SeperatorFormat);
        }

        public virtual void Submit(string text)
        {
            Buffer.AppendFormat(SubmitFormat, text ?? "Submit");
        }

        public abstract void Open();
        public abstract void Close();

        public override string ToString()
        {
            return Buffer.ToString();
        }
    }

    class TableScaffolding<TModel> : BaseScaffolding<TModel>
    {
        public TableScaffolding(HtmlHelper<TModel> html) : base(html)
        {
        }

        protected override string DisplayFieldFormat
        {
            get { return
                  @"
                  <tr>
                     <th>{0}</th>
                     <td colspan=""2"">{1}</td>
                  </tr>";
            }
        }

        protected override string DefaultFieldFormat
        {
            get
            {
                return
                @"
                <tr>
                   <th>{0}</th>
                   <td>{1}</td>
                   <td>{2}</td>
                </tr>";
            }
        }

        protected override string BooleanFieldFormat
        {
            get
            {
                return
                @"
                <tr>
                   <td colspan=""3"">
                        {1}
                        {0}
                        {3}
                        {2}
                    </td>
                </tr>";
            }
        }

        protected override string SeperatorFormat
        {
            get { return "<tr class=\"seperator\"><td colspan=\"3\"></td></tr>"; }
        }

        protected override string SubmitFormat
        {
            get
            { 
                return
                @"
                <tr>
                    <td colspan=""3"">
                        <input type=""submit"" value=""{0}"">
                    </td>
                </tr>";
            }
        }

        public override void Open()
        {
            Buffer.Append("<table class=\"scaffold\">");
        }

        public override void Close()
        {
            Buffer.Append("\n</table>");
        }
    }

    public static class Scaffolding
    {
        public static MvcHtmlString Scaffold<TModel>(
            this HtmlHelper<TModel> html,
            Action<IScaffolding<TModel>> builder)
        {
            var s = new TableScaffolding<TModel>(html);
            builder(s);
            return MvcHtmlString.Create(s.ToString());
        }
    }
}