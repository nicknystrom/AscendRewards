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
    public partial class HomeController {
        public HomeController() { }

        protected HomeController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }


        public readonly string Area = "Site";
        public readonly string Name = "Home";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Terms = "Terms";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string Index = "~/Areas/Site/Views/Home/Index.spark";
            public readonly string Terms = "~/Areas/Site/Views/Home/Terms.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_HomeController: Ascend.Web.Areas.Site.Controllers.HomeController {
        public T4MVC_HomeController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Terms() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Terms);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Terms(bool accept) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Terms);
            callInfo.RouteValues.Add("accept", accept);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
