using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Web.Areas.Site.Controllers;

namespace Ascend.Web.Areas.Mobile.Controllers
{
    public partial class HomeController : MobileController
    {
        public IEntityCache<Award> Awards { get; set; }
        public IUserSummaryCache UserSummaries { get; set; }
        public IUserAwardRepository UserAwards { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IMenuRepository MenusRepository { get; set; }
        public IUserMessaging Messaging { get; set; }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        [MobileAuthorize]
        public virtual ActionResult Index()
        {
            ViewData["awards"] = MenusRepository.GetResourcesForUser(CurrentUser).Where(x => x.Type == MenuItemType.Award).Select(x => Awards[x.Id]).ToList();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("Index") : View("Index");
        }

        [MobileAuthorize]
        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Wishlist()
        {
            return PartialView();
        }

        [MobileAuthorize]
        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Product(string userId, string id)
        {
            var user = String.IsNullOrEmpty(userId) ? CurrentUser : Users[userId];
            var i = user.Wishlist.First(x => x.Value.ProductId == id);
            return PartialView(i.Value);
        }
        
        [MobileAuthorize]
        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Budget()
        {
            var users = Accounting.GetUsersManangerCanDisitributeBudgetTo(CurrentUser);
            return PartialView(users);
        }

        [MobileAuthorize]
        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Distribute(string id)
        {
            if (!Accounting.CanDistributeBudget(CurrentUser, UserSummaries[id]))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot distribute points to this user.");
            }

            var employee = Users[id];
            var model = BudgetDistributionViewModel.Create(
                CurrentBudget,
                employee,
                Accounting.GetPointsLedger(employee));

            return PartialView(model);
        }


        [MobileAuthorize]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Distribute(string id, BudgetDistributionModel model)
        {
            if (!Accounting.CanDistributeBudget(CurrentUser, UserSummaries[id]))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot distribute points to this user.");
            }

            if (null == model.Amount ||
                0 == model.Amount ||
                String.IsNullOrEmpty(model.Message))
            {
		        return Index();
            }

            Accounting.CreateBudgetTransfer(
                CurrentUser,
                Users[id],
                model.Amount ?? 0,
                model.Message);

            return Index();
        }

        [MobileAuthorize]
        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Award(string id)
        {
            var award = Awards[id];
            var model = AwardViewModel.FromDomain(CurrentUser, award, UserRepository, Accounting, Request);
            return PartialView(model);
        }

        [MobileAuthorize]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult Award(string id, UserAwardViewModel model)
        {
            var award = Awards[id];

            var ua = model.CreateAward(CurrentUser, award);
            if (ua.Amount.HasValue)
            {
                Accounting.CreateProgramAward(
                    award,
                    CurrentUser,
                    Users[ua.Recipient],
                    ua.Amount.Value,
                    award.Content.Title
                );
            }
            UserAwards.Save(ua);
            Messaging.SendAward(ControllerContext.RequestContext, ua);
        
            return Index();
        }

        [MobileAuthorize]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None)]
        public virtual ActionResult RewardCode(string rewardcode)
        {
            if (String.IsNullOrWhiteSpace(rewardcode) || rewardcode != "1234")
            {
                return Index();
            }

            Accounting.CreateTransaction(
                Application.GeneralControlAccount,
                CurrentUser.PointsAccount,
                70,
                "Reward code accepted! Thanks!",
                "",
                DateTime.Now,
                null);

            ViewData["rewardCodeMessage"] = "Your code has been accepted, 70 points have been added to your account!";
            return Index();
        }
    }
}