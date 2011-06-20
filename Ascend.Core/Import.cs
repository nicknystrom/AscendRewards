using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services.Import;

namespace Ascend.Core
{
    public enum ImportType
    {
        Users,
        Points,
        Products,
    }

    public class Import : Entity
    {
        public ImportType Type { get; set; }
        public string Location { get; set; }
        public bool Notify { get; set; }
        public ImportColumn[] Columns { get; set; }
        public List<ImportAttempt> Attempts  { get; set; }
    }

    public class ImportColumn
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public string CustomTarget { get; set; }
    }

    public class ImportAttempt
    {
        public DateTime Date { get; set; }
        public bool Success { get; set; }
        public ImportStep Step { get; set; }
        public List<string> Problems { get; set; }
        public int RowsProcessed { get; set; }
        public int RowsFailed { get; set; }
    }
}
