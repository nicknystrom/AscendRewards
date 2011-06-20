using System;
using System.Collections.Generic;
using System.Diagnostics;
using Net.SourceForge.Koogra.Collections;

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Collection class for Row objects.
    /// </summary>
    public class RowCollection : ExcelIndexedCollection<ushort, Net.SourceForge.Koogra.Excel.Row>, IRows
    {
        internal RowCollection(Workbook wb)
            : base(wb)
        {
        }

        internal void Add(ushort rowNumber, Net.SourceForge.Koogra.Excel.Row row)
        {
            Debug.Assert(!ContainsKey(rowNumber));
            BaseAdd(rowNumber, row);
        }

        /// <summary>        
        /// Obsolete. Use <see cref="MinRow"/>.
        /// </summary>
        [Obsolete("Please use MinRow")]
        public ushort FirstRow
        {
            get
            {
                return BaseFirstKey;
            }
        }

        /// <summary>
        /// The first row in the collection.
        /// </summary>
        public uint MinRow
        {
            get
            {
                return BaseFirstKey;
            }
        }

        /// <summary>
        /// Obsolete. Use <see cref="MaxRow"/>.
        /// </summary>
        [Obsolete("Please use MaxRow")]
        public ushort LastRow
        {
            get
            {
                return BaseLastKey;
            }
        }

        /// <summary>
        /// The last row in the collection.
        /// </summary>
        public uint MaxRow
        {
            get
            {
                return BaseLastKey;
            }
        }

        /// <summary>
        /// The indexer for the collection.
        /// </summary>
        public Row this[uint index]
        {
            get
            {
                if (index > ushort.MaxValue)
                    throw new IndexOutOfRangeException();

                return BaseGet((ushort)index);
            }
        }

        IRow IRows.GetRow(uint index)
        {
            return this[index];
        }
    }
}
