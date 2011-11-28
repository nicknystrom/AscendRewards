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
    public class UserSummaryCache : IUserSummaryCache
    {
        /// <summary>
        /// Observes a Session and keeps the UserSummaryCache up-to-date.
        /// </summary>
        public class SessionObserver : BaseObserver<User>
        {
            readonly ICacheStore _cache;
            readonly ITenantResolverService _tenants;

            public SessionObserver(ICacheStore cache, ITenantResolverService tenants)
            {
                _cache = cache;
                _tenants = tenants;
            }

            public override void AfterSave(User entity, Document document)
            {
                var s = new TenantCacheStore(
                    _cache,
                    _tenants.GetTenantForRequest(HttpContext.Current.Request)
                );
                var a = s.Get<IDictionary<string, UserSummary>>(Key);
                if (null != a)
                {
                    lock (a)
                    {
                        var summary = UserSummary.FromDomain(entity);
                        a[summary.Id] = summary;
                    }
                }
            }
        }

        public IUserRepository Users { get; set; }
        public ICacheStore Store { get; set; }

        private const string Key = "__UserSummaryCache_Data";
        private IDictionary<string, UserSummary> Data
        {
            get
            {
                var a = Store.Get<IDictionary<string, UserSummary>>(Key);
                if (null == a)
                {
                    a = Users.GetSummaries().ToDictionary(
                        x => x.Id,
                        x => x
                    );
                    Store.Put(Key, a, TimeSpan.FromHours(12));
                }
                return a;
            }
        }

        public UserSummary this[string userId]
        {
            get
            {
                var a = Data;
                return a.ContainsKey(userId) ? a[userId] : null;
            }
        }

        public IEnumerable<UserSummary> this[IEnumerable<string> userIds]
        {
            get
            {
                var a = Data;
                return userIds.Select(x => a[x]);
            }
        }

        public IEnumerable<UserSummary> All
        {
            get { return Data.Values; }
        }

        public UserSummary TryGet(string userId)
        {
            return this[userId];
        }
    }
}
