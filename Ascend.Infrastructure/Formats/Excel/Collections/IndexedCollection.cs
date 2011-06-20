using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra.Collections
{
    /// <summary>
    /// Indexed collection.
    /// </summary>
    public abstract class IndexedCollection<TKey, TValue> 
        where TKey : IComparable<TKey>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private bool _keysInitialized = false;
        private TKey _firstKey = default(TKey);
        private TKey _lastKey = default(TKey);

        private void SetKeys(TKey key)
        {
            if (!_keysInitialized)
            {
                _firstKey = key;
                _lastKey = key;

                _keysInitialized = true;
            }
            else
            {
                if (_firstKey.CompareTo(key) > 0)
                    _firstKey = key;

                if (_lastKey.CompareTo(key) < 0)
                    _lastKey = key;
            }
        }

        /// <summary>
        /// Add's the specified value at the specified key.
        /// </summary>
        /// <param name="key">The key where the value is to be inserted.</param>
        /// <param name="value">The value to insert.</param>
        protected void BaseAdd(TKey key, TValue value)
        {
            SetKeys(key);

            _dictionary.Add(key, value);
        }

        /// <summary>
        /// The lowest value key available in the collection.
        /// </summary>
        protected TKey BaseFirstKey
        {
            get
            {
                return _firstKey;
            }
        }

        /// <summary>
        /// The highest value key available in the collection.
        /// </summary>
        protected TKey BaseLastKey
        {
            get
            {
                return _lastKey;
            }
        }

        /// <summary>
        /// Returns the value at the specified key.
        /// </summary>
        /// <param name="key">The key for the value.</param>
        /// <returns>Returns the a default value (e.g. null) if the key does not exist. Else returns the value at the specified key.</returns>
        protected TValue BaseGet(TKey key)
        {
            if (!_dictionary.ContainsKey(key))
                return default(TValue);

            return _dictionary[key];
        }

        /// <summary>
        /// Returns true if the collection contains the specified key.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <returns></returns>
        protected bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Value enumerator.
        /// </summary>
        protected IEnumerable<TValue> BaseValues
        {
            get { return _dictionary.Values; }
        }
    }
}
