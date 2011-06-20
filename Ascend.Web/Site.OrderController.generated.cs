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
    public partial class OrderController {
        public OrderController() { }

        protected OrderController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }


        public readonly string Area = "Site";
        public readonly string Name = "Order";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Checkout = "Checkout";
            public readonly string Complete = "Complete";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string Checkout = "~/Areas/Site/Views/Order/Checkout.spark";
            public readonly string Complete = "~/Areas/Site/Views/Order/Complete.spark";
            public readonly string Index = "~/Areas/Site/Views/Order/Index.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_OrderController: Ascend.Web.Areas.Site.Controllers.OrderController {
        public T4MVC_OrderController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Checkout() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Checkout);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Checkout(Ascend.Web.Areas.Site.Controllers.OrderEditModel model) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Checkout);
            callInfo.RouteValues.Add("model", model);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Complete() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Complete);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
