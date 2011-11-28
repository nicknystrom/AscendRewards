using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure.Web;

using Spark;

namespace Ascend.Web.Areas.Mobile.Controllers
{
    public class MobileAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(MobileRoutes.Login, null);
        }
    }

    [Precompile("*")]
    public partial class MobileController : BaseController
    {
        public IEntityCache<User> Users { get; set; }
        public IAccountingService Accounting { get; set; }
        
        public User CurrentUser { get; private set; }
        public Ledger CurrentLedger { get; set; }
        public Ledger CurrentBudget { get; set; }
        public ShoppingCart CurrentWishlist { get; set; }

        /*

        protected TResponse GetResponse<TResponse>(string message = null) where TResponse : MobileResponse, new()
        {
            return new TResponse
            {
                Success = null == message,
                Message = message,

                Login = CurrentUser == null ? null : CurrentUser.Login,
                Name = CurrentUser == null ? null : CurrentUser.DisplayName,
                Wishlist = Wishlist == null ? 0 : Wishlist.Count,
                Points = CurrentLedger == null ? 0 : CurrentLedger.Balance,
                Budget = CurrentBudget == null ? -1 : CurrentBudget.Balance,
            };
        }
        */

        protected void Init(string identityName)
        {
            // much simplified version of the SiteController.OnActionExecuting system.. deemed simple enough
            // not to justify another shared base class)
            if (String.IsNullOrEmpty(identityName))
            {
                CurrentUser = null;
                CurrentLedger = null;
                CurrentBudget = null;
                CurrentWishlist = null;
            }
            else
            {
                CurrentUser = Users[identityName];
                CurrentLedger = Accounting.GetPointsLedger(CurrentUser);
                CurrentBudget = Accounting.GetBudgetLedger(CurrentUser);
                if (null != CurrentBudget &&
                    null == CurrentBudget.Account.Budget &&
                    CurrentBudget.Balance <= 0)
                {
                    // if the user had a budget, but the balance has gone to zero (either because
                    // they gave out all points or we revoked the points), and we unchecked 'has budget',
                    // don't show the budget display at all anymore.s
                    CurrentBudget = null;
                }
                CurrentWishlist = CurrentUser.Wishlist;

                ViewData["currentUser"] = CurrentUser;
                ViewData["currentUserLedger"] = CurrentLedger;
                ViewData["currentUserBudget"] = CurrentBudget;
                ViewData["wishlist"] = CurrentWishlist;
            }   
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!String.IsNullOrEmpty(Application.MaintenancePage))
            {
                filterContext.Result = RedirectToAction(MVC.Public.Login.Index());
            }
            else
            {
                Init(filterContext.HttpContext.User.Identity.Name);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}