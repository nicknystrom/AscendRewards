using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RedBranch.Hammock;

namespace Ascend.Core
{
    public class Entity : IHasDocument
    {
        [JsonIgnore] public Document Document { get; set; }

        public EntityActivity Created { get; set; }
        public EntityActivity Updated { get; set; }
    }

    public class EntityActivity
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
    }
}
