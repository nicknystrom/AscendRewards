using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

namespace Ascend.Web.Areas.Dash.Controllers
{
    public partial class ReportController : DashController
    {
        public IUserRepository UserRepository { get; set; }
        public IAccountingService Accounting { get; set; }
        public IAccountRepository Accounts { get; set; }
        public IGroupSummaryCache Groups { get; set; }

		[HttpGet]
		public virtual ActionResult Index()
        {
		    return View();
        }		

        [HttpGet]
        public virtual ActionResult Wishlist()
        {
            if (String.IsNullOrEmpty(Request["tqx"]))
            {
                return View();    
            }

            var t = new GoogleVisualizationTable()
                    .AddColumn<string>("username", "User/Login Name", null)
                    .AddColumn<string>("firstname", "First Name", null)
                    .AddColumn<string>("lastname", "Last Name", null)
                    .AddColumn<string>("department", "Department", null)
                    .AddColumn<string>("manager", "Manager/Supervisor", null)
                    .AddColumn<string>("current-points", "Current Point Balance", "{0:n0}")
                    .AddColumn<string>("wishlist-item", "Wishlist Item Name", null)
                    .AddColumn<string>("wishlist-item-cost", "Wishlist Item Cost", "{0:n0}")
                    .AddColumn<string>("enough-points-item", "Enough Points Per Item", null)
                    .AddColumn<string>("points-needed-item", "Points Needed Per Item", "{0:n0}")
                    .AddColumn<string>("points-needed-all", "Points Needed For All Items", "{0:n0}");

            var users = UserRepository.GetUsersWithWishlists();
            foreach (var u in users.OrderBy(x => x.LastName))
            {
                var l = Accounting.GetPointsLedger(u);
                var balance = l.Balance;
                var needed = balance - u.Wishlist.Sum(x => x.Value.UnitPrice * x.Value.Quantity);
                foreach (var i in u.Wishlist)
                {
                    t.AddRow(
                        u.Login,
                        u.FirstName,
                        u.LastName,
                        String.IsNullOrWhiteSpace(u.Group) ? "" : Groups[u.Group].Name,
                        String.IsNullOrWhiteSpace(u.Manager) ? "" : Users[u.Manager].DisplayName,
                        balance,
                        i.Value.ProductName,
                        i.Value.UnitPrice * i.Value.Quantity,
                        (i.Value.UnitPrice * i.Value.Quantity) <= balance,
                        balance - (i.Value.UnitPrice * i.Value.Quantity),
                        needed
                    );
                }
            }

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = t;
            return r;
        }

        [HttpGet]
        public virtual ActionResult UsersReport()
        {
            if (String.IsNullOrEmpty(Request["tqx"]))
            {
                return View();
            }

            var t = new GoogleVisualizationTable()
                .AddColumn<string>("username", "User/Login Name", null)
                .AddColumn<string>("firstname", "First Name", null)
                .AddColumn<string>("lastname", "Last Name", null)
                .AddColumn<string>("department", "Department", null)
                .AddColumn<string>("manager", "Manager/Supervisor", null)
                .AddColumn<string>("status", "Status", null)
                .AddColumn<string>("account-created", "Account Creation Date", "{0:d}")
                .AddColumn<string>("account-terminated", "Account Term. Date", "{0:d}")
                .AddColumn<string>("last-login", "Last Logon Date", "{0:d}");

            var users = UserRepository.All().WithDocuments();
            foreach (var u in users.OrderBy(x => x.LastName))
            {
                t.AddRow(
                    u.Login,
                    u.FirstName,
                    u.LastName,
                    String.IsNullOrWhiteSpace(u.Group) ? "" : Groups[u.Group].Name,
                    String.IsNullOrWhiteSpace(u.Manager) ? "" : Users[u.Manager].DisplayName,
                    u.State.ToString(),
                    u.DateActivated.HasValue ? u.DateActivated.Value.ToShortDateString() : "",
                    u.DateTerminated.HasValue ? u.DateTerminated.Value.ToShortDateString() : "",
                    u.LastSuccesfulLogin.HasValue ? u.LastSuccesfulLogin.Value.ToShortDateString() : ""
                );
            }

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = t;
            return r;
        }

        [HttpGet]
        public virtual ActionResult ProgramLiability()
        {
            if (String.IsNullOrEmpty(Request["tqx"]))
            {
                return View();
            }

            var budgets = Accounts.Where(x => x.Type).Eq(AccountType.Program);
            // var programs = Programs

            var t = new GoogleVisualizationTable()
                .AddColumn<string>("account", "Account", null)
                .AddColumn<int>("budget-limit", "Budget Limit", "{0:n0}")
                .AddColumn<int>("budget-interval", "Budget Refresh Interval", null)
                .AddColumn<DateTime>("budget-last", "Last Budget Refresh", "{0:d}")
                .AddColumn<DateTime>("budget-next", "Next Budget Refresh", "{0:d}")
                .AddColumn<int>("balance", "Balance", "{0:n0}");

            foreach (var b in budgets)
            {
                var l = Accounting.GetLedger(b);
                t.AddRow(
                    b.Name,
                    b.Budget == null ? null : b.Budget.RefreshLimit,
                    b.Budget == null ? "" : b.Budget.RefreshInterval.ToString(),
                    b.Budget == null ? null : b.Budget.LastRefreshed,
                    b.Budget == null ? null : b.Budget.NextRefresh,
                    l.Balance

                );
            }

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = t;
            return r;
        }


        [HttpGet]
        public virtual ActionResult PointLiability()
        {
            if (String.IsNullOrEmpty(Request["tqx"]))
            {
                return View();
            }

            var t = new GoogleVisualizationTable()
                .AddColumn<string>("username", "User/Login Name", null)
                .AddColumn<string>("firstname", "First Name", null)
                .AddColumn<string>("lastname", "Last Name", null)
                .AddColumn<string>("department", "Department", null)
                .AddColumn<string>("manager", "Manager/Supervisor", null)
                .AddColumn<int>("points", "Current Point Balance", "{0:n0}")
                .AddColumn<int>("budget", "Current Budget Balance", "{0:n0}")
                .AddColumn<string>("status", "Status (active only)", null)
                .AddColumn<string>("account-created", "Account Creation Date", "{0:d}")
                .AddColumn<string>("account-terminated", "Account Term. Date", "{0:d}")
                .AddColumn<string>("last-login", "Last Logon Date", "{0:d}");

            var users = UserRepository.Where(x => x.State).Eq(UserState.Active).Spec().WithDocuments();
            foreach (var u in users.OrderBy(x => x.LastName))
            {
                var l = Accounting.GetPointsLedger(u);
                var b = Accounting.GetBudgetLedger(u);
                t.AddRow(
                    u.Login,
                    u.FirstName,
                    u.LastName,
                    String.IsNullOrWhiteSpace(u.Group) ? "" : Groups[u.Group].Name,
                    String.IsNullOrWhiteSpace(u.Manager) ? "" : Users[u.Manager].DisplayName,
                    null == l ? 0 : l.Balance,
                    null == b ? 0 : b.Balance,
                    u.State.ToString(),
                    u.DateActivated.HasValue ? u.DateActivated.Value.ToShortDateString() : "",
                    u.DateTerminated.HasValue ? u.DateTerminated.Value.ToShortDateString() : "",
                    u.LastSuccesfulLogin.HasValue ? u.LastSuccesfulLogin.Value.ToShortDateString() : ""
                );
            }

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = t;
            return r;
        }
    }
}