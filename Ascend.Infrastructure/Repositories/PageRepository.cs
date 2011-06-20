using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class PageRepository : Repository<Page>, IPageRepository
    {
        public PageRepository(Session sx) : base(sx)
        {
        }
    }
}
