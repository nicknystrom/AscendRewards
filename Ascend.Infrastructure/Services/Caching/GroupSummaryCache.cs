using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Services.Caching
{
    public class GroupSummaryCache : IGroupSummaryCache
    {
        /// <summary>
        /// Observes a Session and keeps a GrouSummaryCache up-to-date.
        /// </summary>
        public class SessionObserver : BaseObserver<Group>
        {
            readonly ICacheStore _cache;
            readonly ITenantResolverService _tenants;

            public SessionObserver(ICacheStore cache, ITenantResolverService tenants)
            {  
                _cache = cache;
                _tenants = tenants;
            }

            public override void AfterSave(Group entity, Document document)
            {
                var s = new TenantCacheStore(
                    _cache,
                    _tenants.GetTenantForRequest(HttpContext.Current)
                );
                s.Invalidate(Key);
            }
        }

        public IGroupRepository Groups { get; set; }
        public ICacheStore Store { get; set; }

        private const string Key = "__GroupSummaryCache_Data";
        private IDictionary<string, GroupSummary> Data
        {
            get
            {
                var a = Store.Get<IDictionary<string, GroupSummary>>(Key);
                if (null == a)
                {
                    a = Groups.GetSummaries().ToDictionary(
                        x => x.Id,
                        x => x
                    );
                    Store.Put(Key, a, TimeSpan.FromHours(12));
                }
                return a;
            }
        }

        public GroupSummary this[string id]
        {
            get
            {
                var a = Data;
                return a.ContainsKey(id) ? a[id] : null;
            }
        }

        public IEnumerable<GroupSummary> this[IEnumerable<string> ids]
        {
            get
            {
                var a = Data;
                return ids.Select(x => a[x]);
            }
        }

        public IEnumerable<GroupSummary> All
        {
            get { return Data.Values; }
        }
    }
}
