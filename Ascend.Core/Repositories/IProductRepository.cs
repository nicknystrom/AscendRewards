using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<KeyValuePair<string[], int>> GetAllCategories();
        IList<Product> GetProductsInCategory(string[] category);
        void AttachImage(Product p, string name, string contentType, Stream image);
        Product FindByProductId(string s);
        ProductSummary[] GetProductSummaries(bool? enabled = null);
        IDictionary<string, int> GetAllTags();
        Product[] GetEnabledProducts();
    }

    public class ProductSummary
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
        public string[] Category { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Upc { get; set; }
        public decimal? Msrp { get; set; }
        public decimal Price { get; set; }
        public decimal? Shipping { get; set; }
        public decimal? Handling { get; set; }
        public string Supplier { get; set; }
        public string SourceCategory { get; set; }
        public DateTime? SourceAdded { get; set; }
        public DateTime? SourceUpdated { get; set; }
        public DateTime? AscendAdded { get; set; }
        public DateTime? AscendUpdated { get; set; }
        public IList<string> Tags { get; set; }

        public ProductSummary()
        {
        }

        public ProductSummary(Product p)
        {
            Id = p.Document.Id;
            Enabled = p.Enabled ?? false;
            Category = p.Category;
            Name = p.Name;
            Model = p.Model;
            Brand = p.Brand;
            Upc = p.Upc;
            Tags = p.Tags;
            if (null != p.Pricing)
            {
                Msrp = p.Pricing.Msrp;
                Price = p.Pricing.Price;
            }
            if (null != p.Shipping)
            {
                Shipping = p.Shipping.Cost;
                Handling = p.Shipping.DropShipFee;
            }
            if (null != p.Supplier)
            {
                Supplier = p.Supplier.Name;
            }
            if (null != p.Source)
            {
                SourceCategory = p.Source.Category;
                SourceAdded = p.Source.Added;
                SourceUpdated = p.Source.Updated;
            }
            if (null != p.Created)
            {
                AscendAdded = p.Created.Date;
            }
            if (null != p.Updated)
            {
                AscendAdded = p.Updated.Date;
            }
        }
    }
}