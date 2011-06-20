using System;
using System.Collections.Generic;
using Net.SourceForge.Koogra.Collections;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Base class for collections that are part of a workbook.
	/// </summary>
	public class ExcelCollection<T> : ReadOnlyCollection<T>, IExcelObject
	{
		private Workbook _wb;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="wb">The workbook.</param>
		/// <exception cref="ArgumentNullException">Exception is thrown if wb is null.</exception>
		public ExcelCollection(Workbook wb)
		{
			if(wb == null)
				throw new ArgumentNullException("wb");
			_wb = wb;
		}

		/// <summary>
		/// The workbook.
		/// </summary>
		public Workbook Workbook
		{
			get
			{
				return _wb;
			}
		}

        internal void Add(T item)
        {
            InnerList.Add(item);
        }

        /// <summary>
        /// Default indexer for the collection.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>Returns the item at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                return InnerList[index];
            }
        }
	}
}
