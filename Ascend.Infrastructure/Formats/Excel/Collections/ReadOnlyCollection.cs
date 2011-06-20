using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra.Collections
{
    /// <summary>
    /// Read-only Collection.
    /// </summary>
    public abstract class ReadOnlyCollection<T> : IEnumerable<T>
    {
        private List<T> _list = new List<T>();

        /// <summary>
        /// Collection item count.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Internal list.
        /// </summary>
        protected List<T> InnerList
        {
            get
            {
                return _list;
            }
        }

        /// <summary>
        /// Returns an enumerator for the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
