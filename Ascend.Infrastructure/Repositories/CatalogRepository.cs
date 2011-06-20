using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class CatalogRepository : Repository<Catalog>, ICatalogRepository
    {
        public CatalogRepository(Session sx) : base(sx)
        {
        }
    }
}
