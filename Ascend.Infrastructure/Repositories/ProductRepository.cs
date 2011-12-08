using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;

using Ascend.Core;
using Ascend.Core.Repositories;

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
                    (int)((JValue)x.Value).Value)
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
                x => (string)((JValue)x.Key).Value,
                x => (int)((JValue)x.Value).Value);
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
    }
}
