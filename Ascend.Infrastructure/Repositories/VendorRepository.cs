using System.Collections.Generic;
using System.IO;
using System.Linq;

using Ascend.Core;
using Ascend.Core.Repositories;

using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class VendorRepository : Repository<Vendor>, IVendorRepository
    {
        public VendorRepository(Session sx) : base(sx)
        {
        }

        public Vendor FindVendorByName(string name)
        {
            return this.Where(x => x.Name).Eq(name).SingleOrDefault();
        }
    }
}