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
    public class Product : Entity
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

        // options
        public IList<ProductOption> Options { get; set; }

        // fulfillment
        public ProductSource Source { get; set; }
        public ProductSupplier Supplier {get; set; }
        public ProductStock Stock { get; set; }
        public ProductPricing Pricing { get; set; }
        public ProductShipping Shipping { get; set; }

        public ProductLockedFields Locks { get; set; }

        public decimal CalculateTotalCost()
        {
            var x = null == Pricing ? 0 : Pricing.Price;
            if (null != Shipping)
            {
                x += Shipping.DropShipFee ?? 0;
                x += Shipping.Cost ?? 0;
            }
            return x;
        }

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
    }

    #region ProductLockedFields

    /// <summary>
    /// Locked fields are simply not updated during a product import.
    /// </summary>
    public class ProductLockedFields
    {
        public bool? EnabledLocked { get; set; }
        public bool? CategoryLocked { get; set; }
        public bool? NameLocked { get; set; }
        public bool? FeaturesLocked { get; set; }
        public bool? ImagesLocked { get; set; }
        public bool? SkuLocked { get; set; }
        public bool? ManufacturerLocked { get; set; }
        public bool? BrandLocked { get; set; }
        public bool? DescriptionLocked { get; set; }
        public bool? DetailsLocked { get; set; }
        public bool? WarrantyLocked { get; set; }
        public bool? CountryOfOriginLocked { get; set; }

        public void SetEnabled(Product p, bool value)
        {
            if (!EnabledLocked.GetValueOrDefault())
               p.Enabled = value;
        }

        public void SetCategory(Product p, string[] value)
        {
            if (!CategoryLocked.GetValueOrDefault())
                p.Category = value;
        }

        public void SetName(Product p, string value)
        {
            if (!NameLocked.GetValueOrDefault())
                p.Name = value;
        }

        public void SetSku(Product p, string value)
        {
            if (!SkuLocked.GetValueOrDefault())
                p.Sku = value;
        }

        public void SetManufacturer(Product p, string value)
        {
            if (!ManufacturerLocked.GetValueOrDefault())
                p.Manufacturer = value;
        }

        public void SetBrand(Product p, string value)
        {
            if (!BrandLocked.GetValueOrDefault())
                p.Brand = value;
        }

        public void SetDescription(Product p, string value)
        {
            if (!DescriptionLocked.GetValueOrDefault())
                p.Description = value;
        }

        public void SetDetails(Product p, string value)
        {
            if (!DetailsLocked.GetValueOrDefault())
                p.Details = value;
        }

        public void SetWarranty(Product p, string value)
        {
            if (!WarrantyLocked.GetValueOrDefault())
                p.Warranty = value;
        }

        public void SetCountryOfOrigin(Product p, string value)
        {
            if (!CountryOfOriginLocked.GetValueOrDefault())
                p.CountryOfOrigin = value;
        }

        public void SetFeatures(Product p, string[] value)
        {
            if (!FeaturesLocked.GetValueOrDefault())
                p.Features = value;
        }

        public void SetImages(Product p, string[] value)
        {
            if (!ImagesLocked.GetValueOrDefault())
                p.Images = value;
        }
    }

    #endregion

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
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime? Added { get; set; }
        public DateTime? Updated { get; set; }
    }

    public class ProductSupplier
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductPricing
    {
        public decimal? Msrp { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductShipping
    {
        public decimal? DropShipFee { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Weight { get; set; }
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

        /// <summary>
        /// Only set if different from product price (e.g. gift cards)
        /// </summary>
        public ProductPricing Price { get; set; }
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
