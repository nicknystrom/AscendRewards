using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ascend.Core.Repositories;
using Ascend.Core.Services.Caching;
using Newtonsoft.Json;
using RedBranch.Hammock;

namespace Ascend.Core.Services
{
    #region CatalogData

    public class CatalogData
    {
        public string Catalog { get; private set; }
        public Category Categories { get; private set; }
        public IDictionary<string, CatalogProduct> Products { get; private set; }
        
        public CatalogData(
            IApplicationConfiguration configuration,
            Catalog catalog,
            Product[] products)
        {

            // set the root category
            Categories = new Category
            {
                Key = new string[] {"catalog"},
                Slug = "catalog",
                Name = "Catalog",
                Products = new List<CatalogProduct>(products.Length),
            };

            // convert products
            Products = new Dictionary<string, CatalogProduct>(products.Length);
            foreach (var p in products)
            {
                // find/build the category
                var category = Categories;
                for (int i = 1; i < p.Category.Length; i++)
                {
                    var slug = p.Category[i].ToSlug();
                    if (null == category.Children)
                    {
                        category.Children = new Dictionary<string, Category>();
                    }
                    if (!category.Children.ContainsKey(slug))
                    {
                        category.Children[slug] = new Category
                                                        {
                                                            Key = category.Key.Concat(new[] {slug}).ToArray(),
                                                            Name = p.Category[i],
                                                            Parent = category,
                                                            Slug = slug,
                                                            Products = new List<CatalogProduct>(100),
                                                        };
                    }
                    category = category.Children[slug];
                }

                // tranform the product
                var x = new CatalogProduct
                {
                    Catalog = catalog,
                    Id = p.ProductId,
                    Name = p.Name,
                    Sku = p.Sku,
                    Manufacturer = p.Manufacturer,
                    Brand = p.Brand,
                    Model = p.Model,
                    Description = p.Description,
                    Features = p.Features,
                    Warranty = p.Warranty,
                    CountryOfOrigin = p.CountryOfOrigin,
                    Category = category,
                    Tags = p.Tags,
                    Price = (int)Math.Ceiling(catalog.GetPrice(p) * configuration.PointsPerDollar),
                    Options = null == p.Options
                        ? new List<CatalogProductOption>()
                        : p.Options.Select(i => new CatalogProductOption
                            {
                                Sku = i.Sku,
                                Name = i.Name,
                                Price = i.Price == null ? null : (int?)Math.Ceiling(configuration.PointsPerDollar * catalog.GetPrice(p, i.Name)),
                            }).ToList(),
                };
                Products[x.Id] = x;

                // add to various categories
                do
                {
                    category.Products.Add(x);
                    category = category.Parent;
                } while (category != null);
            }

        }
    }

    #endregion
    #region Category

    public class Category
    {
        public string Slug { get; set; }
        public string[] Key { get; set; }

        public Category Parent { get; set; }
        public IDictionary<string, Category> Children { get; set; }

        public string Name { get; set; }
        public IList<CatalogProduct> Products { get; set; }
		
		public Category Root
		{
			get
			{
				var c = this;
				while (null != c.Parent) c = c.Parent;
				return c;
			}
		}
		
        public bool Contains(CatalogProduct product)
        {
            return product.Category == this || (
                Key.Length < product.Category.Key.Length &&
                Key.Take(product.Category.Key.Length).SequenceEqual(product.Category.Key)
            );
        }
    }

    #endregion
    #region CatalogProduct

    public class CatalogProduct
    {
        [JsonIgnore] public Catalog Catalog { get; set; }
        [JsonIgnore] public Category Category { get; set; }
        [JsonIgnore] public IList<string> Tags { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore] public string Model { get; set; }
        [JsonIgnore] public string Sku { get; set; }
        [JsonIgnore] public string Manufacturer { get; set; }
        public string Brand { get; set; }
        [JsonIgnore] public string Description { get; set; }
        [JsonIgnore] public string[] Features { get; set; }
        [JsonIgnore] public string Warranty { get; set; }
        [JsonIgnore] public string CountryOfOrigin { get; set; }
        [JsonIgnore] public ProductStock Stock { get; set; }

        public int Price { get; set; }

        [JsonIgnore] public IList<CatalogProductOption> Options { get; set; }
    }

    #endregion
    #region CatalogProductOption

    public class CatalogProductOption
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public int? Price { get; set; }
    }

    #endregion

    public interface ICatalogService
    {
        Category GetCategories(string catalog);
        IList<CatalogProduct> GetProducts(string catalog, Category category);
        CatalogProduct GetProduct(string catalog, string productId);
        IList<CatalogProduct> SearchProducts(string catalog, Category category, string query);
    }

    public class CatalogService : ICatalogService
    {
        public class CatalogCacheObserver : BaseObserver
        {
            public ICacheStore Cache { get; set; }
            public override void AfterSave(object entity, Document document)
            {
                if (entity is Product ||
                    entity is Catalog)
                {
                    var invalid = Cache.Keys.Where(x => x.StartsWith("__CatalogService_")).ToList();
                    invalid.Each(Cache.Invalidate);
                }
            }    
        }

        public IApplicationConfiguration Configuration { get; set; }
        public IProductRepository Products { get; set; }
        public IEntityCache<Catalog> Catalogs { get; set; }
        public ICacheStore Cache { get; set; }
        public IProductImageService Images { get; set; }

        private CatalogData GetCatalogData(string catalog)
        {
            var key = string.Format("__CatalogService_Catalog_{0}", catalog);
            if (Cache.ContainsKey(key))
            {
                return Cache.Get<CatalogData>(key);
            }

            var data = new CatalogData(
                Configuration,
                Catalogs[catalog],
                Products.GetEnabledProducts());

            Cache.Put(key, data, TimeSpan.FromHours(24));
            return data;
        }

        public Category GetCategories(string catalog)
        {
            return GetCatalogData(catalog).Categories;
        }

        public IList<CatalogProduct> GetProducts(string catalog, Category category)
        {
            return category.Products;
        }

        public CatalogProduct GetProduct(string catalog, string productId)
        {
            return GetCatalogData(catalog).Products[productId];
        }

        public IList<CatalogProduct> SearchProducts(string catalog, Category category, string query)
        {
            query = query.ToLowerInvariant();
            return category.Products.Where(x => x.Name.ToLowerInvariant().Contains(query)).ToList();
        }
    }
}
