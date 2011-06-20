using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra.Collections
{
    /// <summary>
    /// Simple collection class.
    /// </summary>
    /// <typeparam name="T">The data type held by the collection</typeparam>
    public abstract class SimpleCollection<T> : ReadOnlyCollection<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected SimpleCollection()
        {
        }

        /// <summary>
        /// Add's an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public virtual void Add(T item)
        {
            InnerList.Add(item);
        }

        /// <summary>
        /// Default indexer.
        /// </summary>
        /// <param name="index">The index to the item in the collection.</param>
        /// <returns>Returns the item at the specified index.</returns>
        public virtual T this[int index]
        {
            get
            {
                return InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }
    }
}
