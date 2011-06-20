using System;
using System.Collections.Generic;
using System.Diagnostics;
using Net.SourceForge.Koogra.Collections;

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Collection class for Cell objects.
    /// </summary>
    public class CellCollection : ExcelIndexedCollection<byte, Cell>
    {
        internal CellCollection(Workbook wb)
            : base(wb)
        {
        }

        internal void Add(byte index, Cell cell)
        {
            Debug.Assert(!ContainsKey(index));
            BaseAdd(index, cell);
        }

        /// <summary>
        /// Obsolete. Use <see cref="MinCol"/>.
        /// </summary>
        [Obsolete("Please use MinCol.")]
        public byte FirstCol
        {
            get
            {
                return BaseFirstKey;
            }
        }

        /// <summary>
        /// The first column in the collection.
        /// </summary>
        public uint MinCol
        {
            get
            {
                return BaseFirstKey;
            }
        }

        /// <summary>
        /// Obsolete. Use <see cref="MaxCol"/>.
        /// </summary>
        [Obsolete("Please use MaxCol")]
        public byte LastCol
        {
            get
            {
                return BaseLastKey;
            }
        }

        /// <summary>
        /// The last column in the collection.
        /// </summary>
        public uint MaxCol
        {
            get
            {
                return BaseLastKey;
            }
        }

        /// <summary>
        /// The indexer for the collection.
        /// </summary>
        public Cell this[uint index]
        {
            get
            {
                if (index > byte.MaxValue)
                    throw new IndexOutOfRangeException();

                return BaseGet((byte)index);
            }
        }


    }
}
