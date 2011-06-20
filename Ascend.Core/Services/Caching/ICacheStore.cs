using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services.Caching
{
    public interface ICacheStore
    {
        TValue Get<TValue>(string key) where TValue : class;
        void Put<TValue>(string key, TValue value, TimeSpan duration) where TValue : class;
        void Invalidate(string key);
        bool ContainsKey(string key);
        void Replace<TValue>(string key, TValue value, TimeSpan duration) where TValue : class;
        void Empty();
        IList<string> Keys { get; }
    }
}
