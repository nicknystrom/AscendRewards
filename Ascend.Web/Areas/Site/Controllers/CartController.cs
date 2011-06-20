using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Site.Controllers
{
    
    public partial class CartController : SiteController
    {
        public IUserRepository UserRepository { get; set; }
        public ICatalogService Catalog { get; set; }

        [HttpGet]
        public virtual ActionResult Index()
        {
            if (null == CurrentUser.Cart ||
                CurrentUser.Cart.Count == 0)
            {
                ViewData["what"] = "shopping cart";
                return View("Empty");
            }

            ViewData["checkout"] = (CurrentLedger.Balance >= CurrentUser.Cart.Total);
            ViewData["deficit"] = (CurrentUser.Cart.Total - CurrentLedger.Balance);
            return View(CurrentUser.Cart);
        }

        [HttpPost]
        public virtual ActionResult Index(string productId, string optionName)
        {
            var c = CurrentUser.Cart ?? (CurrentUser.Cart = new ShoppingCart());
            if (!c.Contains(productId, optionName))
            {
                var product = Catalog.GetProduct(Application.DefaultCatalog, productId);
                c.Add(
                    product,
                    optionName,
                    1
                );
            }
            c.Update(Catalog, Application.DefaultCatalog);
            UserRepository.Save(CurrentUser);

            if (Request.IsAjaxRequest())
            {
                return Content("");
            }
            return RedirectToAction(Actions.Index);
        }

        [HttpGet]
        public virtual ActionResult Wishlist()
        {
            if (null == CurrentUser.Wishlist ||
                CurrentUser.Wishlist.Count == 0)
            {
                ViewData["what"] = "wishlist";
                return View("Empty");
            }
            return View(CurrentUser.Wishlist);
        }

        /// <summary>
        /// Handles both cart and wishlist updates.
        /// </summary>
        [HttpPost]
        public virtual ActionResult Update(string type, string productId, string optionName, int? quantity)
        {
            var c = type == "Cart" 
                ? (CurrentUser.Cart ?? (CurrentUser.Cart = new ShoppingCart()))
                : (CurrentUser.Wishlist ?? (CurrentUser.Wishlist = new ShoppingCart()));

            if (quantity == 0 && c.Contains(productId, optionName))
            {
                c.Remove(productId, optionName);
            }
            if (quantity > 0)
            {
                if (c.Contains(productId, optionName))
                {
                    c[productId, optionName].Quantity = quantity ?? 1;
                }
                else
                {
                    var product = Catalog.GetProduct(Application.DefaultCatalog, productId);
                    c.Add(
                        product,
                        optionName,
                        quantity ?? 1
                    );
                }
            }
            c.Update(Catalog, Application.DefaultCatalog);
            UserRepository.Save(CurrentUser);

            if (c == CurrentUser.Wishlist)
            {
                ViewData["wishlist"] = c;
                if (!quantity.HasValue || quantity.Value > 0)
                {
                    ViewData["wishlistItem"] = ShoppingCart.CreateItemKey(productId, optionName);
                }
                return PartialView("Modules/Wishlist");
            }
            return Content("");
        }
    }
}