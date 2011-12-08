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
        Msrp,
        Cost,
        ShippingFee,
        ShippingCost,
        Custom,
        OptionName,
        OptionSku,
        SourceAdded,
        SourceUpdated,
        Vendor,
        VendorProductId,
        VendorPrice,
    }

    public class ProductImportService : BaseImportService<Product, ProductColumnMappingTargets>
    {
        public IProductRepository Products { get; set; }
        public IVendorRepository Vendors { get; set; }
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
                else if (layout[i] == ProductColumnMappingTargets.VendorPrice)
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
                case ProductColumnMappingTargets.ShippingFee:
                case ProductColumnMappingTargets.ShippingCost:
                case ProductColumnMappingTargets.VendorPrice:
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
            var features = new List<string>();
            var images = new List<string>();

            // select the option that this row refers, or use the default option
            // creates the named option if it does not exist.
            var optionName = (string)r[ProductColumnMappingTargets.OptionName] ?? "";
            if (null == p.Options)
            {
                p.Options = new List<ProductOption>();
            }
            var option = p.Options.FirstOrDefault(x => x.Name == optionName);
            if (null == option)
            {
                option = new ProductOption
                {
                    Name = optionName,
                };
                p.Options.Add(option);
            }

            // select the product source or use a default source
            var vendorName = (string)r[ProductColumnMappingTargets.Vendor] ?? "";
            if (null == option.Sources)
            {
                option.Sources = new List<ProductSource>();
            }
            var source = String.IsNullOrEmpty(vendorName)
                ? option.Sources.FirstOrDefault(x => x.Vendor == null)
                : option.Sources.FirstOrDefault(x => null != x.Vendor && x.Vendor.Value.Name == vendorName);
            if (null == source)
            {
                source = new ProductSource
                {
                    Pricing = new ProductPricing(),
                    Stock = new ProductStock(),
                    Vendor = null,
                };
                if (!String.IsNullOrEmpty(vendorName))
                {
                    var vendor = Vendors.FindVendorByName(vendorName);
                    if (null == vendor)
                    {
                        vendor = new Vendor
                        {
                            Document = new Document() { Id = Document.For<Vendor>(vendorName.ToSlug()) },
                            Name = vendorName,
                        };
                        Vendors.Save(vendor);
                        source.Vendor = Reference.To(vendor);
                    }
                }
                option.Sources.Add(source);
            }
            var pricing = source.Pricing ?? (source.Pricing = new ProductPricing());
            var stock = source.Stock ?? (source.Stock = new ProductStock());


            for (var i=0; i<r.Layout.Length; i++)
            {
                var value = r.Values[i];
                var target = r.Layout[i];
    
                switch (target)
                {
                    case ProductColumnMappingTargets.Enabled:           p.Set(x => x.Enabled, (bool)value); break;
                    case ProductColumnMappingTargets.Category:          p.Set(x => x.Category, (Product.ParseCategory((string)value))); break;
                    case ProductColumnMappingTargets.Name:              p.Set(x => x.Name, (string)value); break;
                    case ProductColumnMappingTargets.Manufacturer:      p.Set(x => x.Manufacturer, (string)value); break;
                    case ProductColumnMappingTargets.Brand:             p.Set(x => x.Brand, (string)value); break;
                    case ProductColumnMappingTargets.Description:       p.Set(x => x.Description, (string)value); break;
                    case ProductColumnMappingTargets.Details:           p.Set(x => x.Details, (string)value); break;
                    case ProductColumnMappingTargets.Warranty:          p.Set(x => x.Warranty, (string)value); break;
                    case ProductColumnMappingTargets.CountryOfOrigin:   p.Set(x => x.CountryOfOrigin, (string)value); break;
                    case ProductColumnMappingTargets.Model:             p.Model = ((string)value); break;
                    case ProductColumnMappingTargets.LeadTime:          stock.Leadtime = (string)value; break;
                    case ProductColumnMappingTargets.Feature:           features.Add((string)value); break;
                    case ProductColumnMappingTargets.Sku:               p.Set(x => x.Sku, (string)value); break;
                    case ProductColumnMappingTargets.Upc:               p.Set(x => x.Upc, (string)value); break;
                    case ProductColumnMappingTargets.PackageL:          (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Length = (decimal?)value; break;
                    case ProductColumnMappingTargets.PackageW:          (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Width = (decimal?)value; break;
                    case ProductColumnMappingTargets.PackageH:          (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Height = (decimal?)value; break;
                    case ProductColumnMappingTargets.PackageWeight:     (p.PackageDimensions ?? (p.PackageDimensions = new ProductDimensions())).Weight = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductL:          (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Length = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductW:          (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Width = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductH:          (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Height = (decimal?)value; break;
                    case ProductColumnMappingTargets.ProductWeight:     (p.ProductDimensions ?? (p.ProductDimensions = new ProductDimensions())).Weight = (decimal?)value; break;
                    case ProductColumnMappingTargets.Image:             images.Add((string)value); break;
                    case ProductColumnMappingTargets.SourceAdded:       source.Added = (DateTime?)value; break;
                    case ProductColumnMappingTargets.SourceUpdated:     source.Updated = (DateTime?)value; break;
                    case ProductColumnMappingTargets.Msrp:              pricing.Msrp = (decimal?)value; break;
                    case ProductColumnMappingTargets.Cost:              pricing.Cost = ((decimal?)value).Value; break;
                    case ProductColumnMappingTargets.ShippingFee:       pricing.ShippingFee = (decimal?)value; break;
                    case ProductColumnMappingTargets.ShippingCost:      pricing.ShippingCost = (decimal?)value; break;

                    case ProductColumnMappingTargets.OptionSku:         option.Sku = (string) value; break;
                    case ProductColumnMappingTargets.VendorPrice:       pricing.Cost = ((decimal?)value) ?? 0; break;
                }
            }

            if (features.Count > 0)
            {
                p.Set(x => x.Features, features.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray());
            }
            if (images.Count > 0)
            {
                p.Set(x => x.Images, images.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray());
            }
        }
    }
}
