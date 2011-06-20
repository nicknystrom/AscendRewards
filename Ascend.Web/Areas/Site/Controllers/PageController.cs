using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class PageViewModel
    {
        public ContentViewModel Content { get; set; }

        public static PageViewModel FromDomain(Page p)
        {
            return new PageViewModel { 
                Content = ContentViewModel.FromDomain(p.Content),
            };
        }
    }

    public class ContentViewModel
    {
        public string Title { get; set; }
        public string Html { get; set; }

        public static ContentViewModel FromDomain(Content c)
        {
            c = (c ?? new Content());
            var a = new ContentViewModel();
            a.Title = c.Title;
            switch (c.Format)
            {
                case ContentFormat.Html:
                    a.Html = c.Body;
                    break;
                case ContentFormat.Markdown:
                    a.Html = new Markdown().Transform(c.Body);
                    break;
                case ContentFormat.Text:
                    a.Html = c.Body;
                    break;
            }
            return a;
        }
    }

    
    public partial class PageController : ResourceController<Page>
    {
        public IEntityCache<Page> PageCache { get; set; }

        protected override Page GetResource(string id)
        {
            return PageCache[id];
        }

        public virtual ActionResult Index(string id)
        {
            return View(PageViewModel.FromDomain(CurrentResource));
        }
    }
}