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
    public partial class TicketController {
        public TicketController() { }

        protected TicketController(Dummy d) { }

        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = (IT4MVCActionResult)result;
            return RedirectToRoute(callInfo.RouteValues);
        }

        [NonAction]
        public System.Web.Mvc.ActionResult Local() {
            return new T4MVC_ActionResult(Area, Name, Actions.Local);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Venues() {
            return new T4MVC_ActionResult(Area, Name, Actions.Venues);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Performers() {
            return new T4MVC_ActionResult(Area, Name, Actions.Performers);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Venue() {
            return new T4MVC_ActionResult(Area, Name, Actions.Venue);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Performer() {
            return new T4MVC_ActionResult(Area, Name, Actions.Performer);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Event() {
            return new T4MVC_ActionResult(Area, Name, Actions.Event);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Ticket() {
            return new T4MVC_ActionResult(Area, Name, Actions.Ticket);
        }
        [NonAction]
        public System.Web.Mvc.ActionResult Order() {
            return new T4MVC_ActionResult(Area, Name, Actions.Order);
        }

        public readonly string Area = "Site";
        public readonly string Name = "Ticket";

        static readonly ActionNames s_actions = new ActionNames();
        public ActionNames Actions { get { return s_actions; } }
        public class ActionNames {
            public readonly string Index = "Index";
            public readonly string Local = "Local";
            public readonly string Venues = "Venues";
            public readonly string Performers = "Performers";
            public readonly string Venue = "Venue";
            public readonly string Performer = "Performer";
            public readonly string Event = "Event";
            public readonly string Ticket = "Ticket";
            public readonly string Order = "Order";
        }


        static readonly ViewNames s_views = new ViewNames();
        public ViewNames Views { get { return s_views; } }
        public class ViewNames {
            public readonly string _Search = "~/Areas/Site/Views/Ticket/_Search.spark";
            public readonly string Event = "~/Areas/Site/Views/Ticket/Event.spark";
            public readonly string Index = "~/Areas/Site/Views/Ticket/Index.spark";
            public readonly string Local = "~/Areas/Site/Views/Ticket/Local.spark";
            public readonly string Performer = "~/Areas/Site/Views/Ticket/Performer.spark";
            public readonly string Performers = "~/Areas/Site/Views/Ticket/Performers.spark";
            public readonly string Ticket = "~/Areas/Site/Views/Ticket/Ticket.spark";
            public readonly string Venue = "~/Areas/Site/Views/Ticket/Venue.spark";
            public readonly string Venues = "~/Areas/Site/Views/Ticket/Venues.spark";
            static readonly _DisplayTemplates s_DisplayTemplates = new _DisplayTemplates();
            public _DisplayTemplates DisplayTemplates { get { return s_DisplayTemplates; } }
            public partial class _DisplayTemplates{
                public readonly string Events = "~/Areas/Site/Views/Ticket/DisplayTemplates/Events.spark";
            }
        }
    }

    [CompilerGenerated]
    class T4MVC_TicketController: Ascend.Web.Areas.Site.Controllers.TicketController {
        public T4MVC_TicketController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Index() {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Index);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Local(string q) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Local);
            callInfo.RouteValues.Add("q", q);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Venues(string q) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Venues);
            callInfo.RouteValues.Add("q", q);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Performers(string q) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Performers);
            callInfo.RouteValues.Add("q", q);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Venue(string id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Venue);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Performer(string id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Performer);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Event(string id) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Event);
            callInfo.RouteValues.Add("id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Ticket(string id, int ticket) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Ticket);
            callInfo.RouteValues.Add("id", id);
            callInfo.RouteValues.Add("ticket", ticket);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Order(Ascend.Web.Areas.Site.Controllers.TicketPurchaseModel model) {
            var callInfo = new T4MVC_ActionResult(Area, Name, Actions.Order);
            callInfo.RouteValues.Add("model", model);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
#pragma warning restore 0108
