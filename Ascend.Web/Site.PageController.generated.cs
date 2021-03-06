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
    public partial class PageController {
        public PageController() { }

        protected PageController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }

        [NonAction]
        public System.Web.Mvc.ActionResult Index() {
            return new T4MVC_ActionResult(Area, Name, Actions.Index);
        }

        public readonly string Area = "Site";
        public readonly string Name = "Page";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string Index = "~/Areas/Site/Views/Page/Index.spark";
        }
    }

    [CompilerGenerated]
    class T4MVC_PageController: Ascend.Web.Areas.Site.Controllers.PageController {
        public T4MVC_PageController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index(string id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
