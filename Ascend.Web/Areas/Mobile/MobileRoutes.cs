using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ascend.Web.Areas.Mobile
{
    public class MobileRoutes : AreaRegistration
    {
        public const string Login = "mobile-login";
        public const string Logout = "mobile-logout";
        public const string Home = "mobile-home";
        public const string Wishlist = "mobile-wishlist";
        public const string Product = "mobile-product";
        public const string Budget = "mobile-budget";
        public const string Distribute = "mobile-distribute";
        public const string Award = "mobile-award";
        public const string RewardCode = "mobile-rewardcode";

        public override string AreaName
        {
            get { return "Mobile"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(Home,       "m",               new { controller = "Home", action = "Index" });
            context.MapRoute(Login,      "m/login",         new { controller = "Login", action = "Index" });
            context.MapRoute(Logout,     "m/logout",        new { controller = "Login", action = "Logout" });
            context.MapRoute(Product,    "m/wishlist/{id}/{userId}", new { controller = "Home", action = "Product", userId = (string)null }); 
            context.MapRoute(Wishlist,   "m/wishlist",      new { controller = "Home", action = "Wishlist" });
            context.MapRoute(Budget,     "m/budget",        new { controller = "Home", action = "Budget" });
            context.MapRoute(Distribute, "m/budget/{id}",   new { controller = "Home", action = "Distribute" });
            context.MapRoute(Award,      "m/award/{id}",    new { controller = "Home", action = "Award" });
            context.MapRoute(RewardCode, "m/code",          new { controller = "Home", action = "RewardCode" });
        }
    }
}