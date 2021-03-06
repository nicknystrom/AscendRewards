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
    public partial class ProfileController {
        public ProfileController() { }

        protected ProfileController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }


        public readonly string Area = "Site";
        public readonly string Name = "Profile";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Activity = "Activity";
            public readonly string Orders = "Orders";
            public readonly string RecognitionsSent = "RecognitionsSent";
            public readonly string RecognitionsReceived = "RecognitionsReceived";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string _Tabs = "~/Areas/Site/Views/Profile/_Tabs.spark";
            public readonly string Activity = "~/Areas/Site/Views/Profile/Activity.spark";
            public readonly string Index = "~/Areas/Site/Views/Profile/Index.spark";
            public readonly string Orders = "~/Areas/Site/Views/Profile/Orders.spark";
            public readonly string Recognitions = "~/Areas/Site/Views/Profile/Recognitions.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_ProfileController: Ascend.Web.Areas.Site.Controllers.ProfileController {
        public T4MVC_ProfileController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Index(Ascend.Web.Areas.Site.Controllers.ProfileEditModel p) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            callInfo.RouteValues.Add("p", p);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Activity() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Activity);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Orders() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Orders);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RecognitionsSent() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.RecognitionsSent);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RecognitionsReceived() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.RecognitionsReceived);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
