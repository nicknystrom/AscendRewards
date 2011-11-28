using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
    #region BudgetEditModel

    public class BudgetEditModel
    {
        [DisplayName("Limit")] public int? RefreshLimit { get; set; }
        [DisplayName("Interval"), UIHint("Enum")] public BudgetRefreshInterval RefreshInterval { get; set; }

        [DisplayName("Last Refresh")] public DateTime? LastRefreshed { get; set; }
        [DisplayName("Next Refresh")] public DateTime? NextRefresh { get; set; }

        public static BudgetEditModel FromDomain(Budget b)
        {
            b = (b ?? new Budget());
            return new BudgetEditModel
                       {
                           RefreshLimit = b.RefreshLimit,
                           RefreshInterval = b.RefreshInterval,
                           LastRefreshed = b.LastRefreshed,
                           NextRefresh = b.NextRefresh,
                       };
        }

        public Budget ToBudget()
        {
            if (RefreshLimit.HasValue ||
                RefreshInterval != BudgetRefreshInterval.None)
            {
                return new Budget
                {
                    RefreshLimit = RefreshLimit,
                    RefreshInterval = RefreshInterval,
                    LastRefreshed = LastRefreshed,
                    NextRefresh = NextRefresh,
                };
            }
            return null;
        }
    }

    #endregion
    #region AccountEditModel

    public class AccountEditModel
    {
        public string Name { get; set; }
        [UIHint("TextArea")] public string Purpose { get; set; } 
        [UIHint("Enum")] public AccountType Type { get; set; }
        public BudgetEditModel Budget { get; set; }

        public static AccountEditModel FromDomain(Account a)
        {
            return new AccountEditModel {
                Name = a.Name,
                Purpose = a.Purpose,
                Type = a.Type,
                Budget = BudgetEditModel.FromDomain(a.Budget),
            };
        }

        public void Apply(Account a)
        {
            a.Name = Name;
            a.Purpose = Purpose;
            a.Type = Type;
            a.Budget = Budget.ToBudget();
        }
    }

    #endregion
    #region AccountCreateModel

    public class AccountCreateModel
    {
        public string Name { get; set; }
        [UIHint("TextArea")] public string Purpose { get; set; } 
        [UIHint("Enum")] public AccountType Type { get; set; }
        public int? Budget { get; set; }
        [UIHint("Enum")] public BudgetRefreshInterval Interval { get;set; }

        public Account CreateAccount()
        {
            var a = new Account {
                Document = new Document { Id = Document.For<Account>(Name.ToSlug()) },
                Name = Name,
                Purpose = Purpose,
                Type = Type,
            };
            if (Budget.HasValue && Budget.Value > 0)
            {
                a.Budget = new Budget {
                    RefreshLimit = Budget,
                    RefreshInterval = Interval,
                };
            }
            return a;
        }
    }

    #endregion

    public partial class AccountController : AdminController
    {
        public IRepository<Account> Accounts { get; set; }

        [HttpGet]
        public virtual ActionResult Choose(AccountType? type)
        {
            var accounts = (IEnumerable<Account>)Accounts.All();
            if (type.HasValue)
            {
                accounts = accounts.Where(x => x.Type == type.Value);
            }
            return View(accounts);
        }

		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Accounts.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(AccountCreateModel a)
        {
			ViewData["a"] = a;
			if (!ModelState.IsValid)
			{
                return View(Accounts.All().WithDocuments());
			}
			
			var x = a.CreateAccount();
            try
            {
				Accounts.Save(x);

                return this.RedirectToAction(c => c.Edit(x.Document.Id));
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Accounts.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    return View(AccountEditModel.FromDomain(Accounts.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, AccountEditModel a)
        {
			var x = Accounts.Get(id);
			if (!ModelState.IsValid)
			{
				return View(a);
			}
            try
            {
                a.Apply(x);
				Accounts.Save(x);
                return this.RedirectToAction(c => c.Index());
            }
            catch (Exception ex)
            {
                Notifier.Notify(ex);
                return View(a);
            }
        }
    }
}
