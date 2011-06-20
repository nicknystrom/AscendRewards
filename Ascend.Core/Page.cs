using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Ascend.Core
{
    public class Page : Resource
    {
        public Content Content { get; set; }

        [JsonIgnore]
        public string Title { get { return null == Content ? "-" : Content.Title.Or("-"); } }
    }
}
