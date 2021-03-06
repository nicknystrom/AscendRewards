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
namespace Ascend.Web.Areas.Dash.Controllers {
    [CompilerGenerated]
    public partial class ReportController {
        public ReportController() { }

        protected ReportController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }


        public readonly string Area = "Dash";
        public readonly string Name = "Report";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Wishlist = "Wishlist";
            public readonly string UsersReport = "UsersReport";
            public readonly string ProgramLiability = "ProgramLiability";
            public readonly string PointLiability = "PointLiability";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string Index = "~/Areas/Dash/Views/Report/Index.spark";
            public readonly string PointLiability = "~/Areas/Dash/Views/Report/PointLiability.spark";
            public readonly string ProgramLiability = "~/Areas/Dash/Views/Report/ProgramLiability.spark";
            public readonly string UsersReport = "~/Areas/Dash/Views/Report/UsersReport.spark";
            public readonly string Wishlist = "~/Areas/Dash/Views/Report/Wishlist.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_ReportController: Ascend.Web.Areas.Dash.Controllers.ReportController {
        public T4MVC_ReportController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Wishlist() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Wishlist);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult UsersReport() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.UsersReport);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ProgramLiability() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.ProgramLiability);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult PointLiability() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.PointLiability);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
