using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface IImportRepository : IRepository<Import>
    {
        IList<Import> FindRecentImports();
    }
}