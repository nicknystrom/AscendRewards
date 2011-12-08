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
        IDictionary<string, int> GetAllTags();
        Product[] GetEnabledProducts();
    }

    public interface IVendorRepository : IRepository<Vendor>
    {
        Vendor FindVendorByName(string name);
    }
}