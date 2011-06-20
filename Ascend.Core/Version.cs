using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Version : Entity
    {
        public int Revision { get; set; }
        public IList<MigrationHistory> History { get; set; }
    }

    public class MigrationHistory
    {
        public DateTime Applied { get; set; }
        public int FromRevision { get; set; }
        public int ToRevision { get; set; }

        public int Created { get; set; }
        public int Updated { get; set; }
        public int Deleted { get; set; }
        public int Unchanged { get; set; }
    }
}
