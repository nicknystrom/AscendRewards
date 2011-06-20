using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using Newtonsoft.Json.Linq;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(Session sx) : base(sx)
        {
        }

        public void AttachImage(Product p, string name, string contentType, Stream image)
        {
            if (!Session.IsEnrolled(p))
            {
                Session.Enroll(p.Document, p);
            }
            Session.AttachFile(p, name, contentType, image);
        }

        public Product FindByProductId(string s)
        {
            return Where(x => x.ProductId).Eq(s).SingleOrDefault();
        }

        public IEnumerable<KeyValuePair<string[], int>> GetAllCategories()
        {
            var categories = 
                WithView(
                    "_all_categories",
                    @"
                    function(doc) {
                      if (doc._id.indexOf('product-') === 0 && doc.Category && doc.Category.length > 0) {
                        emit(doc.Category, null);
                      }
                    }",
                    @"
                    function (key, values, rereduce) {
                      if (!rereduce) {   
                        return values.length;
                      }
                      var x = 0;
                      for (var i in values) {
                        x += values[i];
                      }
                      return x;
                    }")
                .All()
                .Group()
                .Execute();
            return categories.Rows.Select(x => 
                new KeyValuePair<string[], int>(
                    x.Key.Values<string>().ToArray(),
                    x.Value.Value<int>())
            );
        }

        public IDictionary<string, int> GetAllTags()
        {
            var tags =
                WithView(
                    "_all_tags",
                    @"
                    function(doc) {
                      if (doc._id.indexOf('product-') === 0 && doc.Tags && doc.Tags.length > 0) {
                        for (i in doc.Tags) {
                          emit(doc.Tags[i], null);
                        }
                      }
                    }",
                    @"
                    function (key, values, rereduce) {
                      if (!rereduce) {   
                        return values.length;
                      }
                      var x = 0;
                      for (var i in values) {
                        x += values[i];
                      }
                      return x;
                    }")
                .All()
                .Group()
                .Execute();
            return tags.Rows.ToDictionary(
                x => x.Key.Value<string>(),
                x => x.Value.Value<int>());
        }

        public IList<Product> GetProductsInCategory(string[] category)
        {
            return
                WithView(
                    "_by_category",
                    @"
                    function(doc) {
                      if (doc._id.indexOf('product-') === 0 && doc.Category) {
                        emit(doc.Category, null);
                      }
                    }")
                .From(category)
                .To(category.Concat(new [] { new object() }))
                .WithDocuments()
                .ToList();
        }

        public Product[] GetEnabledProducts()
        {
            return  
                WithView(
                    "_valid_products",
                    @"
                    function(doc) {
                      if (doc._id.indexOf('product-') === 0 &&
                          doc.Category != undefined && 
                          doc.Category &&
                          doc.Enabled != undefined &&
                          doc.Enabled == true) {
                        emit(null, null);
                      }
                    }")
                .All()
                .WithDocuments()
                .ToArray();
        }

        public ProductSummary[] GetProductSummaries(bool? enabled = null)
        {
            var query = WithView(
                "_summaries",
                @"
                function (doc) {
                  if (doc._id.indexOf('product-') === 0) {
                    emit(
                        doc.Enabled == undefined ? 0 : doc.Enabled ? 1 : 0,
                        [doc.Enabled == undefined ? null : doc.Enabled,
                         doc.Category == undefined ? null : doc.Category,
                         doc.Name == undefined ? null : doc.Name,
                         doc.Model == undefined ? null : doc.Model,
                         doc.Brand == undefined ? null : doc.Brand,
                         doc.Upc == undefined ? null : doc.Upc,
                         doc.Pricing == undefined ? null : doc.Pricing.Msrp,
                         doc.Pricing == undefined ? null : doc.Pricing.Price,
                         doc.Shipping == undefined ? null : doc.Shipping.Cost,
                         doc.Shipping == undefined ? null : doc.Shipping.DropShipFee,
                         doc.Supplier == undefined ? null : doc.Supplier.Name,
                         doc.Source == undefined ? null : doc.Source.Category,
                         doc.Source == undefined ? null : doc.Source.Added,
                         doc.Source == undefined ? null : doc.Source.Added,
                         doc.Created == undefined ? null : doc.Created.Date,
                         doc.Updated == undefined ? null : doc.Updated.Added,
                         doc.Tags == undefined ? null : doc.Tags]
                    );
                  }
                }");

            var q = query.All();
            if (null != enabled)
            {
                q = query.From((enabled ?? false) ? 1 : 0)
                         .To((enabled ?? true) ? 2 : 1);
            }
            return q.Execute().Rows.Select(
                x =>
                    {
                        string[] categories = null;
                        var category = x.Value[1];
                        if (null != category && category.Type != JTokenType.Null)
                        {
                            categories = category.Select(y => y.Value<string>()).ToArray();
                        }
                        
                        List<string> tags = null;
                        var tag = x.Value[16];
                        if (null != tag && tag.Type != JTokenType.Null)
                        {
                            tags = tag.Select(y => y.Value<string>()).ToList();
                        }

                        return new ProductSummary
                                   {
                                       Id = x.Id,
                                       Enabled = x.Value.Value<bool>(0),
                                       Category = categories,
                                       Name = x.Value.Value<string>(2),
                                       Model = x.Value.Value<string>(3),
                                       Brand = x.Value.Value<string>(4),
                                       Upc = x.Value.Value<string>(5),
                                       Msrp = x.Value.Value<decimal?>(6),
                                       Price = x.Value.Value<decimal?>(7) ?? 0,
                                       Shipping = x.Value.Value<decimal?>(8),
                                       Handling = x.Value.Value<decimal?>(9) ?? 0,
                                       Supplier = x.Value.Value<string>(10),
                                       SourceCategory = x.Value.Value<string>(11),
                                       SourceAdded = x.Value.Value<DateTime?>(12),
                                       SourceUpdated = x.Value.Value<DateTime?>(13),
                                       AscendAdded = x.Value.Value<DateTime?>(14),
                                       AscendUpdated = x.Value.Value<DateTime?>(15),
                                       Tags = tags
                                   };
                    }
                ).ToArray();
        }
    }


}
