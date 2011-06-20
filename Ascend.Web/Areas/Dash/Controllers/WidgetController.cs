using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Newtonsoft.Json;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Dash.Controllers
{

    public partial class WidgetController : DashController
    {
        public IUserSummaryCache UserSummaries { get; set; }
        public IUserRepository UserRepository { get; set; }

        public IEntityCache<Account> Accounts { get; set; }
        public IAccountingService Accounting { get; set; }

        [HttpGet]
        public virtual ActionResult TimelineControlAccount(FormCollection form)
        {
            var l = Accounting.GetLedger(Accounts[Application.GeneralControlAccount]);

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = new GoogleVisualizationTable()
                .AddColumn<DateTime>("d", "Date", null)
                .AddColumn<int>("balance", "Balance", null);

            l.Timeline((balance, tx) => r.Response.Table.AddRow(tx.Date, balance));

            return r;
        }

        [HttpGet]
        public virtual ActionResult TimelineExpenseAccount(FormCollection form)
        {
            var l = Accounting.GetLedger(Accounts[Application.GeneralExpenseAccount]);

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = new GoogleVisualizationTable()
                .AddColumn<DateTime>("d", "Date", null)
                .AddColumn<int>("balance", "Balance", null)
                .AddColumn<string>("title1", null, null);

            l.Timeline((balance, tx) => r.Response.Table.AddRow(tx.Date, balance, tx.Description));

            return r;
        }

        [HttpGet]
        public virtual ActionResult TimelineLiability(FormCollection form)
        {
            var l = new ProgramLiabilityLedger(
                Accounting.GetLedger(Accounts[Application.GeneralControlAccount]),
                Accounting.GetLedger(Accounts[Application.GeneralExpenseAccount])
            );
            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = new GoogleVisualizationTable()
                .AddColumn<DateTime>("d", "Date", null)
                .AddColumn<int>("issued", "Points Issued", null)
                .AddColumn<int>("spent", "Points Spent", null)
                .AddColumn<int>("liability", "Liability", null);

            l.Timeline((date, issued, spent, liability) =>
                r.Response.Table.AddRow(date, issued, spent, liability));

            return r;            
        }

        [HttpGet]
        public virtual ActionResult PercentLoginsThisWeek(FormCollection form)
        {
            var loggedIn = UserRepository.GetUniqueLoginsByDate(
                                        DateTime.UtcNow.AddDays(-7),
                                        DateTime.UtcNow);
            var active = UserRepository.GetUsersCountsByStates()[Core.UserState.Active];

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = new GoogleVisualizationTable()
                .AddColumn<string>("k", null, null)
                .AddColumn<int>("v", null, null)
                .AddRow("Active", loggedIn)
                .AddRow("Inactive", active - loggedIn);
            return r;
        }

        [HttpGet]
        public virtual ActionResult PercentLoginsThisMonth()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Org()
        {
            var t = new GoogleVisualizationTable()
                .AddColumn<string>("id", null, null)
                .AddColumn<string>("parent", null, null)
                .AddColumn<string>("tip", null, null);
            
            var mgrs = new Dictionary<string, int>();
            UserSummaries.All
                .Where(x => !String.IsNullOrEmpty(x.Manager))
                .Each(x => { 
                    if (mgrs.ContainsKey(x.Manager))
                        mgrs[x.Manager]++;
                    else
                        mgrs[x.Manager] = 1;
                });

            t.Rows = new List<GoogleVisualizationRow>();
            foreach (var entry in mgrs)
            {   
                var u = UserSummaries[entry.Key];
                t.Rows.Add(new GoogleVisualizationRow {
                    Cells = new List<GoogleVisualizationCell> {
                        new GoogleVisualizationCell { 
                            Value = u.Id,
                            Format = String.Format(
                            @"{0}<div style='color: red; font-style: italic;'>{1}</div>",
                            u.DisplayName,
                            u.Title) },
                        new GoogleVisualizationCell { Value = u.Manager },
                        new GoogleVisualizationCell { Value = entry.Value.ToString() },
                    },
                });
            }

            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = t;
            return r;
        }

        [HttpGet]
        public virtual ActionResult BudgetSpend()
        {
            var t = new GoogleVisualizationTable()
                .AddColumn<string>("label", null, null)
                .AddColumn<int>("spend", "Spend", "{0:n0}")
                .AddColumn<int>("budget", "Budget", "{0:n0}");

            var budgets = 
                UserRepository.GetUsersWithBudgets().ToDictionary(
                    x => x,
                    x => Accounting.GetBudgetLedger(x, true, null));
            
            var since = 
                new DateTime(
                    DateTime.UtcNow.Year,
                    DateTime.UtcNow.Month,
                    1);

            foreach (var x in budgets)
            {
                var spend = x.Value.Debits.Where(tx => tx.Date >= since).Sum(tx => tx.Amount);
                t.AddRow(
                    x.Key.DisplayName,
                    spend,
                    Math.Max(0, (x.Value.Account.Budget.RefreshLimit ?? 0) - spend)
                );
            }

            var rnd = new Random();
            var a = t.Rows ?? new List<GoogleVisualizationRow>();
            t.Rows = new List<GoogleVisualizationRow>();
            while (t.Rows.Count < 6 &&
                   a.Count > 0)
            {
                var i = rnd.Next(0, a.Count-1);
                t.Rows.Add(a[i]);
                a.RemoveAt(i);
            }
                   
            var r = new GoogleVisualizationResult(Request);
            r.Response.Table = t;
            return r;
        }
    }
}