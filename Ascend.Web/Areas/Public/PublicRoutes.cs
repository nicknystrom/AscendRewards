using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ascend.Web.Areas.Public
{
    public class PublicRoutes : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Public"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(null, "i/{productId}/{imageIndex}", new { controller = "Image", action = "Index", imageIndex = 0 });
            context.MapRoute(null, "reset", new { controller = "Login", action = "Reset" });
            context.MapRoute(null, "register", new { controller = "Login", action = "Register" });
            context.MapRoute(null, "activate/{code}", new { controller = "Login", action = "Activate" });
            context.MapRoute(null, "public/content/{page}", new { controller = "Login", action = "Page" });
            context.MapRoute(null, "redirect", new { controller = "Login", action = "Find" });
            context.MapRoute(null, "login", new { controller = "Login", action = "Index" });
            context.MapRoute(null, "logout", new { controller = "Login", action = "Logout" });
            context.MapRoute(null, "f/{folder}/{file}", new { controller = "File", action = "Index" });
            context.MapRoute(null, "useraward/{id}", new { controller = "Award", action = "Index" });
            context.MapRoute(null, "Content/Site.css", new { controller = "Theme", action = "Index", id = "theme-default" });
            context.MapRoute(null, "Content/Login.css", new { controller = "Theme", action = "Index", id = "theme-default", view="Login" });
        }
    }
}