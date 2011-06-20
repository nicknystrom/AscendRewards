using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class ImportRepository : Repository<Import>, IImportRepository
    {
        public ImportRepository(Session sx) : base(sx)
        {
        }

        public IList<Import> FindRecentImports()
        {
            return Where(x => x.Created.Date).Bw(DateTime.UtcNow, DateTime.UtcNow.AddYears(-1))
                   .Spec()
                   .WithDocuments()
                   .Descending()
                   .Limit(100)
                   .ToList();
        }
    }
}
