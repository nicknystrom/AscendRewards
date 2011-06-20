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
    public partial class ConciergeController {
        public ConciergeController() { }

        protected ConciergeController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }

        [NonAction]
        public System.Web.Mvc.ActionResult Edit() {
            return new T4MVC_ActionResult(Area, Name, Actions.Edit);
        }

        public readonly string Area = "Admin";
        public readonly string Name = "Concierge";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Edit = "Edit";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string Edit = "~/Areas/Admin/Views/Concierge/Edit.spark";
            public readonly string Index = "~/Areas/Admin/Views/Concierge/Index.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_ConciergeController: Ascend.Web.Areas.Admin.Controllers.ConciergeController {
        public T4MVC_ConciergeController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(string id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Edit);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Edit(string id, Ascend.Web.Areas.Admin.Controllers.ConciergeEditModel model) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Edit);
            callInfo.RouteValues.Add("id", id);
            callInfo.RouteValues.Add("model", model);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
