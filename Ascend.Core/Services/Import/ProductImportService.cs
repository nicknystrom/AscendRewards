using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Core.Services.Import
{
    public enum ProductColumnMappingTargets
    {
        None = -1,
        ProductId,
        Enabled,
        Category,
        Name,
        Manufacturer,
        Brand,
        Description,
        Details,
        Warranty,
        CountryOfOrigin,
        Model,
        LeadTime,
        Sku,
        Upc,
        Feature,
        PackageL,
        PackageW,
        PackageH,
        PackageWeight,
        ProductL,
        ProductW,
        ProductH,
        ProductWeight,
        Image,
        Source,
        SourceCategory,
        SourceAdded,
        SourceUpdated,
        Supplier,
        Msrp,
        Cost,
        DropShipFee,
        Shipping,
        ShippingWeight,
        Custom,
        OptionName,
        OptionPrice,
        OptionSku,
    }

    public class ProductImportService : BaseImportService<Product, ProductColumnMappingTargets>
    {
        public IProductRepository Products { get; set; }
        public IGroupRepository Groups { get; set; }

        public override ImportResult ValidateLayout(ProductColumnMappingTargets[] layout)
        {
            // must include product id, name, and cost
            var result = new ImportResult();
            if (!layout.Any(x => x == ProductColumnMappingTargets.ProductId))
            {
                result.AddProblem("ProductId column is required.");
            }
            if (!layout.Any(x => x == ProductColumnMappingTargets.Name))
            {
                result.AddProblem("Name column is required.");
            }
            if (!layout.Any(x => x == ProductColumnMappingTargets.Cost))
            {
                result.AddProblem("Cost column is required.");
            }

            // option columns must alternate between OptionName and [OptionSku,OptionPrice] columns
            bool optionSkuOk = false;
            bool optionPriceOk = false;
            for (int i = 0; i < layout.Length; i++)
            {
                if (layout[i] == ProductColumnMappingTargets.OptionName)
                {
                    optionSkuOk = true;
                    optionPriceOk = true;
                }
                else if (layout[i] == ProductColumnMappingTargets.OptionSku)
                {
                    if (!optionSkuOk)
                    {
                        result.AddProblem(string.Format("Column {0} indicates OptionSku, but you must include and OptionName first.", i));
                    }
                    optionSkuOk = false;
                }
                else if (layout[i] == ProductColumnMappingTargets.OptionPrice)
                {
                    if (!optionPriceOk)
                    {
                        result.AddProblem(string.Format("Column {0} indicates OptionPrice, but you must include and OptionName first.", i));
                    }
                    optionPriceOk = false;
                }
            }

            return result;
        }

        public override void ValidateRow(ImportRow row)
        {
            // product id is required
            if (String.IsNullOrEmpty((string)row[ProductColumnMappingTargets.ProductId]))
            {
                row.AddProblem("Row has an empty ProductId column. ProductId is a required field.");
            }

            // name is also required if on the sheet
            if (String.IsNullOrEmpty((string)row[ProductColumnMappingTargets.Name]))
            {
                row.AddProblem("Row has an empty name value. Name is a required field.");
            }

            var cost = (decimal?)row[ProductColumnMappingTargets.Cost];
            if (!cost.HasValue)
            {
                row.AddProblem("Row has no cost value. Cost is a required field.");
            }
        }

        public override Product Find(ImportRow row)
        {
            // search by employee id first
            if (row.Has(ProductColumnMappingTargets.ProductId))
            {
                return Products.FindByProductId((string)row[ProductColumnMappingTargets.ProductId]);
            }

            row.AddProblem("Import contains no ProductId.");
            return null;
        }

        public override Product Create(ImportRow row)
        {
            var pid = (string)row[ProductColumnMappingTargets.ProductId];
            return new Product
            {
                Document = new Document { Id = Document.For<Product>(pid) },
                ProductId = pid,
                Enabled = false,
            };
        }

        public override void Save(Product entity)
        {
            Products.Save(entity);
        }

        public override ImportDataType GetColumnType(ProductColumnMappingTargets x)
        {
            switch (x)
            {
                case ProductColumnMappingTargets.PackageL:
                case ProductColumnMappingTargets.PackageW:
                case ProductColumnMappingTargets.PackageH:
                case ProductColumnMappingTargets.PackageWeight:
                case ProductColumnMappingTargets.ProductL:
                case ProductColumnMappingTargets.ProductW:
                case ProductColumnMappingTargets.ProductH:
                case ProductColumnMappingTargets.ProductWeight:
                case ProductColumnMappingTargets.Msrp:
                case ProductColumnMappingTargets.Cost:
                case ProductColumnMappingTargets.DropShipFee:
                case ProductColumnMappingTargets.Shipping:
                case ProductColumnMappingTargets.ShippingWeight:
                case ProductColumnMappingTargets.OptionPrice:
                    return ImportDataType.Number;

                case ProductColumnMappingTargets.Enabled:
                    return ImportDataType.Boolean;

                case ProductColumnMappingTargets.SourceAdded:
                case ProductColumnMappingTargets.SourceUpdated:
                    return ImportDataType.Date;

                default:
                    return ImportDataType.String;
            }    
        }

        public override void Apply(Product p, ImportRow r)
        {
            var pricing = p.Pricing ?? (p.Pricing = new ProductPricing());
            var shipping = p.Shipping ?? (p.Shipping = new ProductShipping());
            var locks = p.Locks ?? new ProductLockedFields();
            var features = new List<string>();
            var images = new List<string>();
            var source = p.Source ?? new ProductSource();
            var options = new List<ProductOption>();

            for (var i=0; i<r.Layout.Length; i++)
            {
                var value = r.Values[i];
                var target = r.Layout[i];
    
                switch (target)
                {
                    case ProductColumnMappingTargets.Enabled:           locks.SetEnabled(p, (bool)value); break;
                    case ProductColumnMappingTargets.Category:          locks.SetCategory(p, (Product.ParseCategory((string)value))); break;
                    case ProductColumnMappingTargets.Name:              locks.SetName(p, (string)value); break;
                    case ProductColumnMappingTargets.Manufacturer:      locks.SetManufacturer(p, (string)value); break;
                    case ProductColumnMappingTargets.Brand:             locks.SetBrand(p, (string)value); break;
                    case ProductColumnMappingTargets.Description:       locks.SetDescription(p, (string)value); break;
                    case ProductColumnMappingTargets.Details:           locks.SetDetails(p, (string)value); break;
                    case ProductColumnMappingTargets.Warranty:          locks.SetWarranty(p, (string)value); break;
                    case ProductColumnMappingTargets.CountryOfOrigin:   locks.SetCountryOfOrigin(p, (string)value); break;
                    case ProductColumnMappingTargets.Model:             p.Model = ((string)value); break;
                    case ProductColumnMappingTargets.LeadTime:          (p.Stock ?? (p.Stock = new ProductStock())).Leadtime = ((string)value); break;
                    case ProductColumnMappingTargets.Feature:           features.Add((string)value); break;
                    case ProductColumnMappingTargets.Sku: 
                        p.Sku = (string)value;
                        locks.SetSku(p, (string)value);
                        break;
                    case ProductColumnMappingTargets.Upc:               p.Upc = (string)value; break;
                    case ProductColumnMappingTargets.PackageL:          (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Length = (decimal?)value; break;
                    case ProductColumnMappingTargets.PackageW:          (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Width = (decimal?)value; break;
                    case ProductColumnMappingTargets.PackageH:          (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Height = (decimal?)value; break;
                    case ProductColumnMappingTargets.PackageWeight:     (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Weight = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductL:          (p.ProductDimensions ?? (p.ProductDimensions= new ProductDimensions())).Length = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductW:          (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Width = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductH:          (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Height = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductWeight:     (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Weight = (decimal?)value; break;
                    case ProductColumnMappingTargets.Image:             images.Add((string)value); break;
                    case ProductColumnMappingTargets.Supplier:          p.Supplier = new Supplier { Name = (string)value }; break;
                    case ProductColumnMappingTargets.Source:            source.Name = (string)value; break;
                    case ProductColumnMappingTargets.SourceCategory:    source.Category = (string)value; break;
                    case ProductColumnMappingTargets.SourceAdded:       source.Added = (DateTime?)value; break;
                    case ProductColumnMappingTargets.SourceUpdated:     source.Updated = (DateTime?)value; break;
                    case ProductColumnMappingTargets.Msrp:              pricing.Msrp = (decimal?)value; break;
                    case ProductColumnMappingTargets.Cost:              pricing.Price = ((decimal?)value).Value; break;
                    case ProductColumnMappingTargets.DropShipFee:       shipping.DropShipFee = (decimal?)value; break;
                    case ProductColumnMappingTargets.Shipping:          shipping.Cost = (decimal?)value; break;
                    case ProductColumnMappingTargets.ShippingWeight:    shipping.Weight = (decimal?)value; break;
                    
                    case ProductColumnMappingTargets.OptionName:        if (null != value && !String.IsNullOrWhiteSpace((string)value)) options.Add(new ProductOption { Name = (string)value }); break;
                    case ProductColumnMappingTargets.OptionSku:         if (null != value && !String.IsNullOrWhiteSpace((string)value)) options.Last().Sku = (string) value; break;
                    case ProductColumnMappingTargets.OptionPrice:       if (null != value && null != (decimal?)value) options.Last().Price = new ProductPricing { Price =  (decimal)value }; break;
                }
            }

            if (source.Name != null ||
                source.Category != null ||
                source.Added.HasValue ||
                source.Updated.HasValue)
            {
                p.Source = source;
            }

            if (features.Count > 0)
            {
                locks.SetFeatures(p, features.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray());
            }
            if (images.Count > 0)
            {
                locks.SetImages(p, images.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray());
            }
            if (options.Count > 0)
            {
                p.Options = options;
            }
        }
    }
}
