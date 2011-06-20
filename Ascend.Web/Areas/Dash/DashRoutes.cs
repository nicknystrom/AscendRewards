using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ascend.Web.Areas.Dash
{
    public class DashRoutes : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Dash"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(null, "dash",                    new {Controller = "Home", Action = "Index"});
            context.MapRoute(null, "dash/org",                new {Controller = "Home", Action = "Org"});
            context.MapRoute(null, "dash/reports",            new {Controller = "Report", Action = "Index"});
            context.MapRoute(null, "dash/reports/wishlist",   new {Controller = "Report", Action = "Wishlist"});
            context.MapRoute(null, "dash/reports/users",      new {Controller = "Report", Action = "UsersReport"});
            context.MapRoute(null, "dash/reports/points",     new {Controller = "Report", Action = "PointLiability"});
            context.MapRoute(null, "dash/reports/program",    new {Controller = "Report", Action = "ProgramLiability"});

            context.MapRoute(null, "dash/data/percentloginsthisweek", new { Controller = "Widget", Action = "PercentLoginsThisWeek" });
            context.MapRoute(null, "dash/data/timelines/control", new { Controller = "Widget", Action = "TimelineControlAccount" });
            context.MapRoute(null, "dash/data/timelines/expense", new { Controller = "Widget", Action = "TimelineExpenseAccount" });
            context.MapRoute(null, "dash/data/timelines/liability", new { Controller = "Widget", Action = "TimelineLiability" });
            context.MapRoute(null, "dash/data/org", new { Controller = "Widget", Action = "Org" });
            context.MapRoute(null, "dash/data/budgetspend", new { Controller = "Widget", Action = "BudgetSpend" });
        }
    }
}
