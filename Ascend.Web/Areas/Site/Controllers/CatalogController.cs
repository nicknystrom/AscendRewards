using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;
using System.Text;
using Ascend.Web.Controllers;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public string Tag { get; set; }
        public IList<CatalogProduct> Products { get; set; }
        public string[] ItemsInCart { get; set; }
        public string[] ItemsInWishlist { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }

        public string Query { get; set; }
        public string Sort { get; set; }
        public int? Size { get; set; }
        public bool? Affordable { get; set; }
    }

    public static class __UrlExtensions
    {
        public static string GetPaginationUrl(this UrlHelper url, Category c, int? offset, int? count)
        {
            // reads the search, sorting, and filter parameters from the route data.
            // all other overrides do not (ie. jumping from category to category resets
            // those fields, but paging in a result set obviously should not).

            var q = url.RequestContext.HttpContext.Request.QueryString["q"];
            var s = url.RequestContext.HttpContext.Request.QueryString["sort"];
            var afford = url.RequestContext.HttpContext.Request.QueryString["afford"];
            var tag = url.RequestContext.HttpContext.Request.QueryString["tag"];

            return GetCategoryUrl(url, c, tag, q, s, String.IsNullOrEmpty(afford) ? (bool?)null : true, offset, count);
        }

        public static string GetTagUrl(this UrlHelper url, Category c, string tag)
        {
            return GetCategoryUrl(url, c, tag, null, null, null, null, null);
        }

        public static string GetCategoryUrl(this UrlHelper url, Category c)
        {
            return GetCategoryUrl(url, c, null, null);
        }

        public static string GetCategoryUrl(this UrlHelper url, Category c, int? offset, int? count)
        {
            return GetCategoryUrl(url, c, null, null, null, null, offset, count);
        }

        public static string GetCategoryUrl(this UrlHelper url, Category c, string tag, string q, string s, bool? afford, int? offset, int? count)
        {
            var d = new RouteValueDictionary
            {
                { "tag", tag },
                { "q", q },
                { "s", s },
                { "afford", afford },
                { "offset", offset },
                { "count", count },
            };

            var name = "CatalogKeys" + c.Key.Length;
            while (null != c && c.Key.Length > 1)
            {
                d["key" + (c.Key.Length-2)] = c.Slug;
                c = c.Parent;
            }

            return url.RouteUrl(name, d);
        }

        public static string GetProductImageUrl(this HtmlHelper html, string productId, int imageIndex, int? width, int? height)
        {
            var controller = html.ViewContext.Controller as BaseController;
            if (null == controller || null == controller.Images)
            {
                return "<span>No image</span>";
            }
            return controller.Images.GetExternalImageUrl(productId, imageIndex, (null == width || null == height) ? null : new Size() { Width = width.Value, Height = height.Value })
                ?? new UrlHelper(html.ViewContext.RequestContext).Action(MVC.Public.Image.Index(productId, imageIndex, width, height));
        }
    }
	
	public static class CatalogHtmlExtensions
	{
		public static MvcHtmlString CategoryLink(this HtmlHelper html, UrlHelper url, Category c)
		{
			return MvcHtmlString.Create("<a href=\"" + url.GetCategoryUrl(c) + "\">" + (c.Name == "Catalog" ? "All Products" : c.Name) + "</a>");
		}
		
		public static MvcHtmlString CategoryNode(this HtmlHelper html, UrlHelper url, Category c, Func<Category, bool> expanded = null)
		{
			var buf = new StringBuilder();
		    var exp = (null != expanded && expanded(c));
            buf.Append(exp ? "<li class=\"expanded\">" : "<li>");
            buf.AppendFormat("<span class=\"ui-icon {0}\" style=\"float: left;\"></span>",
                null != c.Children && c.Children.Count > 0
                    ? (exp ? "ui-icon-circlesmall-minus expandable" : "ui-icon-circlesmall-plus expandable")
                    : "ui-icon-placeholder");
			buf.Append(html.CategoryLink(url, c));
			if (null != c.Children && c.Children.Count > 0)
			{
                buf.Append("<ul>");
				foreach (var child in c.Children.Values)
				{
					buf.Append(html.CategoryNode(url, child, expanded));
				}
				buf.Append("</ul>");
			}
			buf.Append("</li>");
			return MvcHtmlString.Create(buf.ToString());
		}
	}
    
    public partial class CatalogController : SiteController
    {
        public IEntityCache<Product> Products { get; set; }
        public ICatalogService Catalog { get; set; }
        public IConciergeRepository ConciergeRepository { get; set; }

        [HttpGet]
        public virtual ActionResult Index(
            string key0,
            string key1,
            string key2,
            string key3,
            string key4,
            string tag,
            string q,
            string s,
            bool? afford,
            int? offset,
            int? count)
        {
            // determine the current category
            var c = Catalog.GetCategories(Application.DefaultCatalog);
            if (!String.IsNullOrEmpty(key0)) c = c.Children[key0];
            if (!String.IsNullOrEmpty(key1)) c = c.Children[key1];
            if (!String.IsNullOrEmpty(key2)) c = c.Children[key2];
            if (!String.IsNullOrEmpty(key3)) c = c.Children[key3];
            if (!String.IsNullOrEmpty(key4)) c = c.Children[key4];

            // load all products in that category
            // or search the catalog.
            var all = String.IsNullOrEmpty(q)
                ? Catalog.GetProducts(Application.DefaultCatalog, c)
                : Catalog.SearchProducts(Application.DefaultCatalog, c, q);
            var p = (IEnumerable<CatalogProduct>)all;

            // tag-based filters are done in memroy
            if (!String.IsNullOrWhiteSpace(tag))
            {
                p = p.Where(x => null != x.Tags && x.Tags.Contains(tag));
            }

            // points-based filters are done in memory
            if (afford.HasValue && afford.Value)
            {
                // only look for products that the user can purchase right now
                var budget = CurrentLedger.Balance;
                p = p.Where(x => x.Price <= budget);
            }

            // sort remaining
            s = String.IsNullOrEmpty(s) ? "name" : s;
            switch (s)
            {
                case "name": p = p.OrderBy(x => x.Name); break;
                case "!name": p = p.OrderByDescending(x => x.Name); break;
                case "price": p = p.OrderBy(x => x.Price); break;
                case "!price": p = p.OrderByDescending(x => x.Price); break;
            }
        
            // done filtering, record the total resultset size before doing skip/take
            var list = p.ToList();
            p = list;
            if (offset.HasValue) 
            {
                p = p.Skip(offset.Value);
            }
            if (count.HasValue)
            {
                p = p.Take(count.Value);
            }

            var model = new CategoryViewModel
                            {
                                Category = c,
                                Tag = tag,
                                Query = q,
                                Sort = s,
                                Size = count,
                                Affordable = afford,
                                Products = p.ToList(),
                                Offset = offset ?? 0,
                                Total = list.Count,
                                ItemsInCart =
                                    (CurrentUser.Cart ?? new ShoppingCart()).Select(x => x.Value.ProductId).ToArray(),
                                ItemsInWishlist =
                                    (CurrentUser.Wishlist ?? new ShoppingCart()).Select(x => x.Value.ProductId).ToArray(),
                            };

            return View("Index", model);
        }

        [HttpGet]
        public virtual ActionResult Product(string productId)
        {
            var p = Catalog.GetProduct(Application.DefaultCatalog, productId);
            if (null == p)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "Product not found.");
            }
            return View(p);
        }

        [HttpGet]
        public virtual ActionResult GiftCards()
        {
            var catalog = Catalog.GetCategories(Application.DefaultCatalog);
            if (catalog.Children.ContainsKey("gift-cards"))
            {
                return View(catalog.Children["gift-cards"].Products);    
            }
            return View(new List<CatalogProduct>());
        }

        [HttpGet]
        public virtual ActionResult Travel()
        {
            var catalog = Catalog.GetCategories(Application.DefaultCatalog);
            if (catalog.Children.ContainsKey("travel-packages"))
            {
                return View(catalog.Children["travel-packages"].Products);    
            }
            return View(new List<CatalogProduct>());
        }

        [HttpGet]
        public virtual ActionResult Tagged(string tag)
        {
            return Redirect(Url.GetTagUrl(Catalog.GetCategories(Application.DefaultCatalog), tag));
        }

        [HttpGet]
        public virtual ActionResult Concierge()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Concierge(ConciergeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticket = new Concierge
            {
                User = CurrentUser.Document.Id,
                Link = model.Link,
                Description = model.Description,
                Stage = ConciergeStage.Submitted,
            };

            ConciergeRepository.Save(ticket);

            Notifier.Notify(
                Severity.Success,
                "Thanks for your concierge request!",
                "We'll be reviewing your concierge request shortly and getting in touch with you. Typically we reply within 48 hours, although we may contact you sooner if we need some clarification on your request. Thanks!",
                ticket);

            return RedirectToAction("Index");
        }
    }

    public class ConciergeViewModel 
    {
        [StringLength(1000)] public string Link { get; set; }
        [Required, StringLength(4000), UIHint("TextArea")] public string Description { get; set; }
    }
}