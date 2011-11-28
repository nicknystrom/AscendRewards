using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Infrastructure.Web;

namespace Ascend.Web.Admin.Controllers
{
    #region ProductEditModel

    public class ProductOptionEditModel
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal? Price { get; set; }
    }

    public class ProductEditModel
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Manufacturer { get; set; }
        public string Brand { get; set; }
        [UIHint("TextArea")] public string Description { get; set; }
        [UIHint("TextArea")] public string Details { get; set; }
        public string Warranty { get; set; }
        public string CountryOfOrigin { get; set; }
        public List<ProductOptionEditModel> Options { get; set; }

        public ProductLockedFields Locks { get; set; }

        public static ProductEditModel FromDomain(Product p)
        {
            return new ProductEditModel {
                Id = p.ProductId,
                Enabled = p.Enabled ?? false,
                Category = null == p.Category ? String.Empty : String.Join("/", p.Category),
                Name = p.Name,
                Sku = p.Sku,
                Manufacturer = p.Manufacturer,
                Brand = p.Brand,
                Description = p.Description,
                Details = p.Details,
                Warranty = p.Warranty,
                CountryOfOrigin = p.CountryOfOrigin,
                Options = null == p.Options
                    ? new List<ProductOptionEditModel>()
                    : p.Options.Select(x => new ProductOptionEditModel
                                                {
                                                    Name = x.Name,
                                                    Sku = x.Sku,
                                                    Price = x.Price != null ? (decimal?)x.Price.Price : (decimal?)null
                                                }).ToList(),
                Locks = p.Locks,
            };
        }

        public void Apply(Product p)
        {
            p.Locks = Locks;
            p.Enabled = Enabled;
            p.Category = Category.Split('/');
            p.Name = Name;
            p.Sku = Sku;
            p.Manufacturer = Manufacturer;
            p.Brand = Brand;
            p.Description = Description;
            p.Details = Details;
            p.Warranty = Warranty;
            p.CountryOfOrigin = CountryOfOrigin;
            if (null != Options)
            {
                p.Options = Options.Select(x => new ProductOption
                {
                    Name = x.Name,
                    Sku = x.Sku,
                    Price = x.Price != null ? new ProductPricing { Price = x.Price.Value } : null
                }).ToList();
            }
        }
    }

    #endregion
    #region ProductFilterModel

    public enum ProductDetailSort
    {
        None,
        Enabled,
        Price,
        Added,
        Updated,
        SourceCategory,
        Brand,
    }

    public class ProductFilterModel
    {
        public bool? Enabled { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Updated { get; set; }
        public string Category { get; set; }
        [UIHint("Enum")] public ProductDetailSort Sort { get; set; }

        public ProductSummary[] Filter(IEnumerable<ProductSummary> source)
        {
            if (Enabled.HasValue)
            {
                source = source.Where(x => x.Enabled == Enabled.Value);
            }
            if (Added.HasValue)
            {
                source = source.Where(x => x.AscendAdded.HasValue && x.AscendAdded.Value.Date >= Added.Value);
            }
            if (Updated.HasValue)
            {
                source = source.Where(x => x.AscendUpdated.HasValue && x.AscendUpdated.Value >= Updated.Value);
            }
            if (!String.IsNullOrEmpty(Category))
            {
                if (Category == "-")
                {
                    source = source.Where(x => x.Category == null || x.Category.Length == 0);
                }
                else
                {
                    var parts = Product.ParseCategory(Category);
                    if (parts.Length > 1)
                    {
                        source = source.Where(x => x.Category != null && parts.SequenceEqual(x.Category));
                    }
                }
            }
            switch (Sort)
            {
                case ProductDetailSort.Enabled:
                    source = source.OrderBy(x => x.Enabled);
                    break;
                case ProductDetailSort.Added:
                    source = source.OrderBy(x => x.AscendAdded);
                    break;
                case ProductDetailSort.Updated:
                    source = source.OrderBy(x => x.AscendUpdated);
                    break;
                case ProductDetailSort.SourceCategory:
                    source = source.OrderBy(x => x.SourceCategory);
                    break;
                case ProductDetailSort.Brand:
                    source = source.OrderBy(x => x.Brand);
                    break;
                case ProductDetailSort.Price:
                    source = source.OrderBy(x => x.Price + x.Shipping + x.Handling);
                    break;
            }
            return source.ToArray();
        }
    }

    #endregion

    public partial class ProductController : AdminController
    {
        public IProductRepository Products { get; set; }
        public ILog Log { get; set; }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public virtual ActionResult Index(ProductFilterModel model)
        {
            var products = model.Filter(Products.GetProductSummaries(model.Enabled));

            // get all used categories and backfill for parent categories that don't actually
            // have any products in it.
            IDictionary<string, int> categories = Products.GetAllCategories().ToDictionary(x => Product.FormatCategory(x.Key), x=> x.Value);
            var changed = false;
            do
            {
                changed = false;
                foreach (var cat in categories)
                {
                    var parts = cat.Key.Split('/');
                    if (parts.Length > 2)
                    {
                        var parent = String.Join("/", parts.Take(parts.Length - 1));
                        if (!categories.ContainsKey(parent))
                        {
                            categories.Add(parent, 0);
                            changed = true;
                            break;
                        }
                    }
                }
            } while (changed);
            categories = new SortedDictionary<string, int>(categories);

            // get all used tags, union in the default tags
            var tags = Products.GetAllTags();
            StarTags.Where(x => !tags.ContainsKey(x))
                    .Each(x => tags.Add(x, 0));
            tags = new SortedDictionary<string, int>(tags);

            ViewData["filter"] = model ?? new ProductFilterModel();
            ViewData["categories"] = categories;
            ViewData["tags"] = tags;

            if (products.Length > 10000)
            {
                products = products.Take(10000).ToArray();
                Notifier.Notify(Severity.Warn, "Too may products, try filtering more.", "You're only seeing the first 10,000 products that matched your criteria, try using the filter options to narrow your search a bit.", null);
            }
            return View(products);
        }

        [HttpPost]
        public virtual ActionResult RenameCategory(string from, string to)
        {
            var categoryFrom = Product.ParseCategory(from);
            var categoryTo = Product.ParseCategory(to);

            if (null == categoryFrom ||
                null == categoryTo ||
                categoryFrom.Length <= 1 ||
                categoryTo.Length <= 1)
            {
                throw new Exception("Specify valid cateories.");
            }

            // if used to start with Catalog, it still needs to
            if (categoryFrom[0] == "Catalog" && categoryTo[0] != "Catalog")
            {
                throw new Exception("Don't rename from Catalog/ to a non-Catalog/ category.");
            }

            var count = 0;
            var products = Products.GetProductSummaries();
            Parallel.ForEach(products, x =>
            {
                if (x.Category != null &&
                    x.Category.Length >= categoryFrom.Length &&
                    Enumerable.Range(0, categoryFrom.Length).All(i => x.Category[i] == categoryFrom[i]))
                {
                    var p = Products.Get(x.Id);
                    p.Category = categoryTo.Concat(x.Category.Skip(categoryFrom.Length)).ToArray();
                    Products.Save(p);
                    Interlocked.Increment(ref count);
                }
            });

            return Content(count.ToString());
        }

        [HttpGet]
        public virtual ActionResult ClearImages()
        {
            Images.ClearExternalImages();
            return this.RedirectToAction(c => c.Index(null));
        }

        [HttpGet]
        public virtual ActionResult RefreshImages()
        {
            var products = Products.All().Execute().Rows;
            Log.InfoFormat("Loading images for {0} products...", products.Length);
            var failed = new List<string>();
            var n = 0;
            Parallel.ForEach(
                products,
                new ParallelOptions() { MaxDegreeOfParallelism = 1 },
                row =>
            {
                Interlocked.Increment(ref n);
                if (null != row.Entity.Images)
                {
                    for (int i = 0; i < row.Entity.Images.Length; i++)
                    {
                        try
                        {
                            var e = row.Entity;
                            if (null == Images.GetExternalImageUrl(e.ProductId, i, null))
                            {
                                Images.GetItemImage(e, i);
                            }
                            if (null == Images.GetExternalImageUrl(e.ProductId, i, new Size { Width = 250, Height = 250 }))
                            {
                                Images.GetItemImage(e, i, new Size { Width = 250, Height = 250 });
                            }
                            if (null == Images.GetExternalImageUrl(e.ProductId, i, new Size { Width = 170, Height = 170 }))
                            {
                                Images.GetItemImage(e, i, new Size { Width = 170, Height = 170 });
                            }
                            if (null == Images.GetExternalImageUrl(e.ProductId, i, new Size { Width = 114, Height = 114 }))
                            {
                                Images.GetItemImage(e, i, new Size { Width = 114, Height = 114 });
                            }
                            if (null == Images.GetExternalImageUrl(e.ProductId, i, new Size { Width = 100, Height = 100 }))
                            {
                                Images.GetItemImage(e, i, new Size { Width = 100, Height = 100 });
                            }
                            Log.InfoFormat(" ... Loaded images for {0} ({1}/{2})", e.Name, n, products.Length);
                        }
                        catch (Exception ex)
                        {
                            Log.WarnFormat(" ... Failed to load images for {0} with exception '{1}' ({1}/{2})", row.Entity.Name, ex.Message, n, products.Length);
                            lock(failed) { failed.Add(row.Entity.Images[i]); }
                        }
                    }
                }
            });

            if (failed.Count > 0)
            {
                Notifier.Notify(
                    Severity.Warn,
                    "Some images failed to load.",
                    String.Format("{0} images could not be loaded. The system will try again each time that product is viewed in the catalog.", failed.Count),
                    null);
            }
            else
            {
                Notifier.Notify(Severity.Success, "All product images refreshed.", null, null);
            }

            return this.RedirectToAction(c => c.Index(null));
        }

        [HttpPost]
        public virtual ActionResult Enable(string[] ids, bool? enable)
        {
            return Json(ids.Select(x =>
            {
                var p = Products.Get(x);
                p.Enabled = enable ?? true;
                if (null == p.Locks)
                {
                    p.Locks = new ProductLockedFields();
                }
                p.Locks.EnabledLocked = true;
                Products.Save(p);
                return new ProductSummary(p);
            }));
        }

        [HttpPost]
        public virtual ActionResult Categorize(string[] ids, string category)
        {
            return Json(ids.Select(x =>
            {
                var p = Products.Get(x);
                p.Category = Product.ParseCategory(category);
                if (null == p.Locks)
                {
                    p.Locks = new ProductLockedFields();
                }
                p.Locks.CategoryLocked = true;
                Products.Save(p);
                return new ProductSummary(p);
            }));
        }

        public static readonly string[] StarTags = new string[] 
        {
            "reject",
            "1-star",
            "2-star",
            "3-star",
            "4-star",
            "5-star",
        };

        [HttpPost]
        public virtual ActionResult Tag(string[] ids, string[] tag)
        {
            return Json(ids.Select(x =>
            {
                var p = Products.Get(x);
                if (null == p.Tags)
                {
                    p.Tags = new List<string>();   
                }
                foreach (var t in tag)
                {
                    if (t.StartsWith("-"))
                    {
                        // remove the tag
                        var u = t.Substring(1);
                        if (p.Tags.Contains(u))
                        {
                            p.Tags.Remove(u);
                        }
                    }
                    else
                    {
                        // add the tag
                        if (!p.Tags.Contains(t))
                        {
                            // 'star tags' are mutually exclusive
                            if (StarTags.Contains(t))
                            {
                                foreach (var starTag in StarTags)
                                {
                                    if (starTag != t && p.Tags.Contains(starTag))
                                    {
                                        p.Tags.Remove(starTag);
                                    }
                                }

                                // if the tag is reject, disable the product
                                if ("reject" == t)
                                {
                                    p.Enabled = false;
                                }
                                else
                                {
                                    // otherwise enable the product
                                    p.Enabled = true;
                                }
                            }

                            p.Tags.Add(t);
                        }
                    }
                }
                Products.Save(p);
                return new ProductSummary(p);
            }));
        }

        [HttpGet]
        public virtual ActionResult Edit(string id)
        {
            if (!id.StartsWith("product-"))
            {
                id = "product-" + id;
            }
            return View(ProductEditModel.FromDomain(Products.Get(id)));
        }

        [HttpPost]
        public virtual ActionResult Edit(string id, ProductEditModel model)
        {
            if (!id.StartsWith("product-"))
            {
                id = "product-" + id;
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var p = Products.Get(id);
            model.Apply(p);
            Products.Save(p);
            Notifier.Notify(Severity.Success, "Product updated.", null, p);
            return RedirectToAction("Index");
        }
    }
}