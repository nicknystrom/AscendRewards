using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Public.Controllers
{
    public class ThemeViewModel
    {
        Theme _theme;
        HttpRequestBase _request;

        public ThemeViewModel(Theme theme, HttpRequestBase request)
        {
            _theme = theme;
            _request = request;
        }

        public string Banner
        {
            get { return String.IsNullOrEmpty(_theme.Banner) ? "" : _theme.Banner.ToAbsoluteUrl(_request).ToString(); }
        }

        public string C(string color)
        {
            return _theme.ResolveColor(color);
        }

        public string F(string font)
        {
            return _theme.ResolveFont(font);
        }

        public string B(string box)
        {
            return _theme.ResolveBox(box);
        }

        public string CustomSite { get { return _theme.CustomSite; } }
        public string CustomLogin { get { return _theme.CustomLogin; } }

        public string[] LoginBanners { get { return _theme.LoginBanners;  } }
        public string[] LoginInfos { get { return _theme.LoginInfos;  } }
    }

    public partial class ThemeController : PublicController
    {
        public IApplicationConfiguration Configuration { get; set; }
        public IEntityCache<Theme> Themes { get; set; }

        public virtual ActionResult Index(string id, string view)
        {
            var t = Themes[id];
            Response.ContentType = "text/css";
            return PartialView(view ?? t.Stylesheet, new ThemeViewModel(t, Request));  
        }
    }
}
