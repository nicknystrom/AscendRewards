using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using Ascend.Infrastructure;
using Ascend.Infrastructure.Web;
using Spark;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class MenuViewModel : List<MenuViewModelItem>
    {
        public string Revision { get; set; }

        public MenuViewModel()
        {
        }

        public MenuViewModel(IEnumerable<MenuViewModelItem> items) : base (items)
        {
            
        }
    }

    public class MenuViewModelItem
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public IList<MenuViewModelItem> Items { get; set; }
        public static MenuViewModelItem FromDomain(
            MenuItem item,
            UrlHelper url,
            IEnumerable<AllowedResource> allowed)
        {
            var a = new MenuViewModelItem();
            a.Name = item.Name;
            switch (item.Type)
            {
                case MenuItemType.None:
                    break;
                case MenuItemType.Catalog:
                    a.Location = url.Action(MVC.Site.Catalog.Index());
                    break;
                case MenuItemType.Url:
                    a.Location = item.Location;
                    break;
                case MenuItemType.Page:
                    a.Location = url.Action(MVC.Site.Page.Index(item.Location));
                    break;
                case MenuItemType.Quiz:
                    a.Location = url.Action(MVC.Site.Quiz.Index(item.Location));
                    break;
                case MenuItemType.Award:
                    a.Location = url.Action(MVC.Site.Award.Index(item.Location));
                    break;                
                case MenuItemType.Survey:
                    a.Location = url.Action(MVC.Site.Survey.Index(item.Location));
                    break;
                case MenuItemType.Game:
                    a.Location = url.Action(MVC.Site.Game.Index(item.Location));
                    break;
            }
            if (!String.IsNullOrEmpty(a.Location))
            {
                a.Location = a.Location.ToAbsoluteUrl(url.RequestContext.HttpContext.Request).ToString();
            }
            if (item.Items != null && null != allowed)
            {
                a.Items = item.Items.Where(x => IsVisible(x, allowed))
                                    .Select(x => FromDomain(x, url, allowed))
                                    .ToList();
                if (a.Items.Count == 0) a.Items = null;
            }
            return a;
        }

        public static bool IsVisible(
            MenuItem item,
            IEnumerable<AllowedResource> allowed)
        {
            return item.Type == MenuItemType.None ||
                   item.Type == MenuItemType.Catalog ||
                   item.Type == MenuItemType.Url ||
                   (null != allowed && allowed.Any(y => y.Allows(item)));
        }
    }
    
    [Authorize, Precompile("*")]
    public partial class SiteController : BaseController
    {
        public IMenuRepository MenusRepository { get; set; }
        public IEntityCache<Menu> Menus { get; set; }
        public IEntityCache<Page> Pages { get; set; }

        public IAccountingService Accounting { get; set; }
        public ShoppingCart CurrentCart { get; set; }
        public Ledger CurrentLedger { get; set; }
        public Ledger CurrentBudget { get; set; }

        public IEntityCache<User> Users { get; set; }
        public IEntityCache<Group> Groups { get; set; }
        public User CurrentUser { get; private set; }
        public Group CurrentGroup { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!String.IsNullOrEmpty(Application.MaintenancePage))
            {
                filterContext.Result = RedirectToAction(MVC.Public.Login.Index());
                return;
            }
            
            base.OnActionExecuting(filterContext);

            CurrentUser = Users[filterContext.HttpContext.User.Identity.Name];
            if (!String.IsNullOrEmpty(CurrentUser.Group))
            {
                CurrentGroup = Groups[CurrentUser.Group];
            }
            ViewData["currentUser"] = CurrentUser;

            if (!Request.IsAjaxRequest() ||
                filterContext.Controller.GetType() == typeof(BudgetController))
            {
                // cart, wishlist, menu and ledger info must be loaded on every page request
                ResolveCart(filterContext);
                ResolveWishlist(filterContext);
                ResolveMenu(filterContext, Application.DefaultHeaderMenu, "headerMenu");
                ResolveMenu(filterContext, Application.DefaultNavigationMenu, "navigationMenu");
                ResolveMenu(filterContext, Application.DefaultFooterMenu, "footerMenu");
                ResolveLedger(filterContext);
            }

            // ensure that the tos and profile pages have been accepted/updated before
            // allowing the user to enter the rest of the site.
            if (!IsToS(filterContext))
            {
                ResolveTos(filterContext);
                if (!IsProfile(filterContext))
                {
                    ResolveProfile(filterContext);
                }
            }
        }

        protected void ResolveWishlist(ActionExecutingContext filterContext)
        {
            ViewData["wishlist"] = CurrentUser.Wishlist ?? new ShoppingCart();
        }

        protected void ResolveCart(ActionExecutingContext filterContext)
        {
            CurrentCart = (CurrentUser.Cart ?? new ShoppingCart());
            ViewData["cart"] = CurrentCart;
        }

        protected static bool IsToS(ActionExecutingContext filterContext)
        {
            return (filterContext.RouteData.GetRequiredString("controller") == "Home" &&
                    filterContext.RouteData.GetRequiredString("action") == "Terms");
        }

        protected static bool IsProfile(ActionExecutingContext filterContext)
        {
            return (filterContext.RouteData.GetRequiredString("controller") == "Profile" &&
                    filterContext.RouteData.GetRequiredString("action") == "Index");
        }

        protected void ResolveTos(ActionExecutingContext filterContext)
        {
            // don't bother if we've already had a result
            if (filterContext.Result != null) return;

            // determine the tos page
            var page = Application.DefaultTermsOfService;
            if (null != CurrentGroup &&
                !String.IsNullOrEmpty(CurrentGroup.TermsOfService))
            {
                page = CurrentGroup.TermsOfService;
            }
            if (!String.IsNullOrEmpty(page))
            {
                // determine last updated date on the tos
                var tos = Pages[page];
                var d = (tos.Updated ?? tos.Created ?? new EntityActivity {Date = DateTime.MinValue}).Date;

                // determine if the user has accepted after that date
                if (!CurrentUser.DateAcceptedTermsOfService.HasValue ||
                    CurrentUser.DateAcceptedTermsOfService.Value < d)
                {
                    // redirect to the tos page
                    filterContext.Result = RedirectToAction("Terms", "Home");
                }
            }
        }

        protected void ResolveProfile(ActionExecutingContext filterContext)
        {
            // don't bother if we've already had a result
            if (filterContext.Result != null) return;

            if (Application.ShowProfileOnActivate &&
                !CurrentUser.LastUpdatedProfile.HasValue)
            {
                filterContext.Result = RedirectToAction("Index", "Profile");
            }
        }

        private const string MenuKey = "__BaseController_";

        protected void ResolveMenu(ActionExecutingContext filterContext, string menuId, string viewdataName)
        {
            if (String.IsNullOrEmpty(menuId)) return;

            var menu = Menus[menuId];
            var key = MenuKey + menuId;
            var menuViewModel = Session[key] as MenuViewModel;
            if (null == menuViewModel ||
                menuViewModel.Revision != menu.Document.Revision)
            {
                var allowed = MenusRepository.GetResourcesForUser(CurrentUser);
                menuViewModel = new MenuViewModel(
                    menu.Items.Where(x => MenuViewModelItem.IsVisible(x, allowed))
                              .Select(x => MenuViewModelItem.FromDomain(x, Url, allowed))
                );
                menuViewModel.Revision = menu.Document.Revision;
                Session[key] = menuViewModel;
            }
            ViewData[viewdataName] = menuViewModel;
        }

        protected void ResolveLedger(ActionExecutingContext filterContext)
        {
            CurrentLedger = Accounting.GetPointsLedger(CurrentUser);
            ViewData["currentUserLedger"] = CurrentLedger;

            // budget will be null if user has no budget
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
            ViewData["currentUserBudget"] = CurrentBudget;
        }
    }
}