using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Services.Caching
{
    public class TenantCacheStore : ICacheStore
    {
        readonly ICacheStore _inner;
        readonly string _prefix;

        public TenantCacheStore(ICacheStore innerStore, Tenant currentTenant)
        {
            _inner = innerStore;
            _prefix = currentTenant.Database + "-";
        }

        public TValue Get<TValue>(string key) where TValue : class
        {
            return _inner.Get<TValue>(_prefix + key);
        }

        public void Replace<TValue>(string key, TValue value, TimeSpan duration) where TValue : class
        {
            _inner.Replace(_prefix + key, value, duration);
        }

        public void Put<TValue>(string key, TValue value, TimeSpan duration) where TValue : class
        {
            _inner.Put(_prefix + key, value, duration);
        }

        public bool ContainsKey(string key)
        {
            return _inner.ContainsKey(_prefix + key);
        }

        public void Invalidate(string key)
        {
            _inner.Invalidate(_prefix + key);
        }

        public IList<string> Keys
        {
            get 
            {
                return _inner.Keys.Where(x => x.StartsWith(_prefix))
                                  .Select(x => x.Substring(_prefix.Length))
                                  .ToList();
            }
        }

        public void Empty()
        {
            foreach (var x in _inner.Keys.Where(x => x.StartsWith(_prefix)))
            {
                _inner.Invalidate(x);
            }
        }
    }
}
