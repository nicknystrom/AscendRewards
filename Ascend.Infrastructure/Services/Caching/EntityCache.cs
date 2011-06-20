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
    public class EntityCache<TEntity> : IEntityCache<TEntity> where TEntity : Entity
    {
        public class SessionObserver : BaseObserver<TEntity>
        {
            readonly ICacheStore _cache;
            readonly ITenantService _tenants;

            public SessionObserver(ICacheStore cache, ITenantService tenants)
            {  
                _cache = cache;
                _tenants = tenants;
            }

            public override void AfterSave(TEntity entity, Document document)
            {
                var s = new TenantCacheStore(
                    _cache,
                    _tenants.GetTenantForRequest(HttpContext.Current.Request)
                );
                var k = BuildKey(entity);
                if (s.ContainsKey(k))
                {
                    s.Replace(k, entity, TimeSpan.FromHours(1));
                }
            }
        }

        private const string Key = "__entity-cache-";
        public static string BuildKey(TEntity entity)
        {
            return BuildKey(entity.Document.Id);
        }
        public static string BuildKey(string id)
        {
            return Key + id;
        }

        IRepository<TEntity> _repository { get; set; }
        ICacheStore _store { get; set; }

        public EntityCache(IRepository<TEntity> repo, ICacheStore store)
        {
            _repository = repo;
            _store = store;
        }

        public TEntity this[string id]
        {
            get
            {
                var k = BuildKey(id);
                var u = _store.Get<TEntity>(k);
                if (null == u)
                {
                    u = _repository.Get(id);
                    _store.Put(k, u, TimeSpan.FromHours(1));
                }
                return u;
            }
            set
            {
                _store.Put(BuildKey(id), value, TimeSpan.FromHours(1));
            }
        }

        public TEntity TryGet(string id)
        {
            try { return this[id]; }
            catch { return null; }
        }
    }
}
