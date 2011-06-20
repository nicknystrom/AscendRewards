using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class BudgetDistributionViewModel
    {
        public string Id { get; set; }
        [DisplayName("Name")] public string DisplayName { get; set; }
        [DisplayName("Current Balance")]public int Balance { get; set; }
        [DisplayName("Last Budget Award")] public DateTime? LastBudgetDistribution { get; set; }
        [DisplayName("Last Budget Award")] public string LastBudgetDistributionText { get; set; }
        public ShoppingCart Wishlist { get; set; }

        public static BudgetDistributionViewModel Create(Ledger managerBudget, User employee, Ledger employeePoints)
        {
            var last = managerBudget.Debits
                .Where(x => x.Credit == employeePoints.Account.Document.Id)
                .OrderBy(x => x.Date)
                .Select(x => (DateTime?)x.Date)
                .LastOrDefault();

            return new BudgetDistributionViewModel
            {
                Id = employee.Document.Id,
                DisplayName = employee.DisplayName,
                Balance = employeePoints.Balance,
                LastBudgetDistribution = last,
                LastBudgetDistributionText = last.HasValue ? last.Value.ToShortDateString() : "Never",
                Wishlist = employee.Wishlist,
            };
        }
    }

    public class BudgetDistributionModel
    {
        public int? Amount { get; set; }
        public string Message { get; set; }
    }

    
    public partial class BudgetController : SiteController
    {
        public IUserSummaryCache UserSummaries { get; set; }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Index()
        {
            var users = Accounting.GetUsersManangerCanDisitributeBudgetTo(CurrentUser);
            return Request.IsAjaxRequest()
                ? (ActionResult) Json(users.ToDictionary(x => x.Id, x => x.DisplayName), JsonRequestBehavior.AllowGet)
                : View(users);
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Distribute(string userId)
        {
            if (!Accounting.CanDistributeBudget(CurrentUser, UserSummaries[userId]))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot distribute points to this user.");
            }

            var employee = Users[userId];
            var model = BudgetDistributionViewModel.Create(
                CurrentBudget,
                employee,
                Accounting.GetPointsLedger(employee));

            //return Request.IsAjaxRequest() ? (ActionResult)Json(model, JsonRequestBehavior.AllowGet) : PartialView(model);
            return PartialView(model);
        }

        [HttpPost]
        public virtual ActionResult Distribute(string userId, BudgetDistributionModel model)
        {
            if (!Accounting.CanDistributeBudget(CurrentUser, UserSummaries[userId]))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot distribute points to this user.");
            }

            Accounting.CreateBudgetTransfer(
                CurrentUser,
                Users[userId],
                model.Amount ?? 0,
                model.Message);

            return Content("");
        }
    }
}