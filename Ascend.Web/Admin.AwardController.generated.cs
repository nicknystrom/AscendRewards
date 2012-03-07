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
namespace Ascend.Web.Areas.Admin.Controllers {
    [CompilerGenerated]
    public partial class AwardController {
        public AwardController() { }

        protected AwardController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }

        [NonAction]
        public System.Web.Mvc.ActionResult Details() {
            return new T4MVC_ActionResult(Area, Name, Actions.Details);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Edit() {
            return new T4MVC_ActionResult(Area, Name, Actions.Edit);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Certificate() {
            return new T4MVC_ActionResult(Area, Name, Actions.Certificate);
        }

        public readonly string Area = "Admin";
        public readonly string Name = "Award";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Details = "Details";
            public readonly string Edit = "Edit";
            public readonly string Certificate = "Certificate";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string AwardCreateModel = "~/Areas/Admin/Views/Award/AwardCreateModel.spark";
            public readonly string Certificate = "~/Areas/Admin/Views/Award/Certificate.spark";
            public readonly string Edit = "~/Areas/Admin/Views/Award/Edit.spark";
            public readonly string Index = "~/Areas/Admin/Views/Award/Index.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_AwardController: Ascend.Web.Areas.Admin.Controllers.AwardController {
        public T4MVC_AwardController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Details(int id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Details);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Index(Ascend.Web.Areas.Admin.Controllers.AwardCreateModel a) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            callInfo.RouteValues.Add("a", a);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(string id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Edit);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(string id, Ascend.Web.Areas.Admin.Controllers.AwardEditModel a) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Edit);
            callInfo.RouteValues.Add("id", id);
            callInfo.RouteValues.Add("a", a);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Certificate(Ascend.Web.Areas.Admin.Controllers.CertificateCreateModel c) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Certificate);
            callInfo.RouteValues.Add("c", c);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108