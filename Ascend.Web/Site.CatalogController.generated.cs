// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#pragma warning disable 0108
#region T4MVC

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace Ascend.Web.Areas.Site.Controllers {
    [CompilerGenerated]
    public partial class CatalogController {
        public CatalogController() { }

        protected CatalogController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }

        [NonAction]
        public System.Web.Mvc.ActionResult Index() {
            return new T4MVC_ActionResult(Area, Name, Actions.Index);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Product() {
            return new T4MVC_ActionResult(Area, Name, Actions.Product);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Tagged() {
            return new T4MVC_ActionResult(Area, Name, Actions.Tagged);
        }

        public readonly string Area = "Site";
        public readonly string Name = "Catalog";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Product = "Product";
            public readonly string GiftCards = "GiftCards";
            public readonly string Travel = "Travel";
            public readonly string Tagged = "Tagged";
            public readonly string Concierge = "Concierge";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string _Nav = "~/Areas/Site/Views/Catalog/_Nav.spark";
            public readonly string Categories = "~/Areas/Site/Views/Catalog/Categories.spark";
            public readonly string Concierge = "~/Areas/Site/Views/Catalog/Concierge.spark";
            public readonly string GiftCards = "~/Areas/Site/Views/Catalog/GiftCards.spark";
            public readonly string Index = "~/Areas/Site/Views/Catalog/Index.spark";
            public readonly string Product = "~/Areas/Site/Views/Catalog/Product.spark";
            public readonly string Travel = "~/Areas/Site/Views/Catalog/Travel.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_CatalogController: Ascend.Web.Areas.Site.Controllers.CatalogController {
        public T4MVC_CatalogController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index(string key0, string key1, string key2, string key3, string key4, string tag, string q, string s, bool? afford, int? offset, int? count) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            callInfo.RouteValues.Add("key0", key0);
            callInfo.RouteValues.Add("key1", key1);
            callInfo.RouteValues.Add("key2", key2);
            callInfo.RouteValues.Add("key3", key3);
            callInfo.RouteValues.Add("key4", key4);
            callInfo.RouteValues.Add("tag", tag);
            callInfo.RouteValues.Add("q", q);
            callInfo.RouteValues.Add("s", s);
            callInfo.RouteValues.Add("afford", afford);
            callInfo.RouteValues.Add("offset", offset);
            callInfo.RouteValues.Add("count", count);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Product(string productId) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Product);
            callInfo.RouteValues.Add("productId", productId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GiftCards() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.GiftCards);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Travel() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Travel);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Tagged(string tag) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Tagged);
            callInfo.RouteValues.Add("tag", tag);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Concierge() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Concierge);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Concierge(Ascend.Web.Areas.Site.Controllers.ConciergeViewModel model) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Concierge);
            callInfo.RouteValues.Add("model", model);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
