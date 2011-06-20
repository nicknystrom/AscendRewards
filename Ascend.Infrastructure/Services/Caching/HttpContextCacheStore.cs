using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

namespace Ascend.Infrastructure.Services.Caching
{
    public class HttpContextCacheStore : ICacheStore
    {
        public TValue Get<TValue>(string key) where TValue : class
        {
            return HttpContext.Current.Cache.Get(key) as TValue;
        }

        public void Replace<TValue>(string key, TValue value, TimeSpan duration) where TValue : class
        {
            HttpContext.Current.Cache.Remove(key);
            Put(key, value, duration);
        }

        public void Put<TValue>(string key, TValue value, TimeSpan duration) where TValue : class
        {
            HttpContext.Current.Cache.Add(
                key,
                value,
                null,
                DateTime.Now + duration,
                Cache.NoSlidingExpiration,
                CacheItemPriority.Normal,
                null);
        }

        public bool ContainsKey(string key)
        {
            return null != HttpContext.Current.Cache[key];
        }

        public void Invalidate(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public IList<string> Keys
        {
            get
            {
                var keys = new List<string>();
                foreach (DictionaryEntry pair in HttpContext.Current.Cache)
                {
                    keys.Add((string)pair.Key);
                }    
                return keys;
            }
        }

        public void Empty()
        {
            foreach (var x in Keys)
            {
                HttpContext.Current.Cache.Remove(x);
            }
        }
    }
}
