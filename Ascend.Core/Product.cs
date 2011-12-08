using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using Newtonsoft.Json;
using RedBranch.Hammock;

namespace Ascend.Core
{
    public class Product : ImportableEntity<Product>
    {
        // identification
        public bool? Enabled { get; set; }      // products are disabled by default
        public string ProductId { get; set; }
        public string[] Category { get; set; }
        public List<string> Tags { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Upc { get; set; }

        // product info 
        public string[] Images { get; set; }
        public ProductCondition? Condition { get; set; }
        
        public string Manufacturer { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Warranty { get; set; }
        public string CountryOfOrigin { get; set; }

        public ProductDimensions PackageDimensions { get; set; }
        public ProductDimensions ProductDimensions { get; set; }
        
        public string Description { get; set; }
        public string Details { get; set; }
        public string[] Features { get; set; }
        public IDictionary<string, string> Attributes { get; set; }

        // options
        public IList<ProductOption> Options { get; set; }

        public override string ToString()
        {
            return (Upc ?? Sku) + ": " + Name;
        }

        public static string[] ParseCategory(string category)
        {
            var parts = category.Split('/', '\\');
            return parts
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x.Substring(0, 1).ToUpperInvariant() + x.Substring(1).ToLowerInvariant())
                .ToArray();
        }

        public static string FormatCategory(string[] category)
        {
            return String.Join("/", category);
        }

        public ProductPricing GetReferencePricing()
        {
            if (null == Options || Options.Count == 0) return null;
            if (Options.Count == 1) return Options[0].GetBestSource().Pricing;
            return Options.Select(x => x.GetBestSource()).Where(x => null != x).Low(x => x.Pricing.Total).Pricing;
        }
    }

    public class ProductDimensions
    {
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public override string  ToString()
        {
            return String.Format("{0:n2}L x {1:n2}W x {2:n2}H", Length, Width, Height);
        }
    }

    public class ProductSource
    {
        public Reference<Vendor> Vendor {get; set; }
        public string VendorProductId { get; set; }
        public ProductStock Stock { get; set; }
        public ProductPricing Pricing { get; set; }

        public DateTime? Added { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class Vendor : Entity
    {
        public string Name { get; set; }
    }

    public class ProductPricing
    {
        /// <summary>
        /// Suggest Retail Price
        /// </summary
        public decimal? Msrp { get; set; }

        /// <summary>
        /// Actual Cost from Vendor.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Flat shipping fee charged by vendor, typically for white-label drop shipping.
        /// </summary>
        public decimal? ShippingFee { get; set; }

        /// <summary>
        /// Variable shipping cost, based on product size. If vendor charges variable shipping
        /// based on destination, this field should be the hightest estimated shipping cost in the
        /// lower United States.
        /// </summary>
        public decimal? ShippingCost { get; set; }

        /// <summary>
        /// Total estimated cost to acquire the product from vendor.
        /// </summary>
        public decimal Total
        {
            get
            {
                return Cost + (ShippingCost ?? 0) + (ShippingFee ?? 0);
            }
        }
    }

    public class ProductStock
    {
        public StockStatus? Status { get; set; }
        public DateTime? Changed { get; set; }
        public int? Available { get; set; }
        public string Replacement { get; set; }
        public string Leadtime { get; set; }
    }

    public class ProductOption
    {
        /// <summary>
        /// Optional, only if different from product sku.
        /// </summary>
        public string Sku { get; set; }
        
        /// <summary>
        /// Red XXL, Blue XL, Green XL, etc: the specific orderable item
        /// </summary>
        public string Name { get; set; }

        public IList<ProductSource> Sources { get; set; }

        public ProductSource GetBestSource()
        {
            if (Sources == null || Sources.Count == 0) return null;
            if (Sources.Count == 1) return Sources[0];
            return Sources.Where(x => x.Stock == null || x.Stock.Status != StockStatus.Discontinued).Low(x => x.Pricing.Total);
        }
    }

    public enum ProductCondition
    {
        New,
        Refurbished,
    }

    public enum StockStatus
    {
        InStock,
        OutOfStock,
        Discontinued,
    }
}