using System;
using System.Collections.Generic;
using System.Diagnostics;
using Net.SourceForge.Koogra.Excel.Records;

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Represents a row of cells.
    /// </summary>
    public class Row : ExcelObject, IRow
    {
        private CellCollection _cells;

        internal Row(Workbook wb, RowRecord row)
            : base(wb)
        {
            _cells = new CellCollection(wb);
        }

        /// <summary>
        /// The collection of cells in the row.
        /// </summary>
        public CellCollection Cells
        {
            get
            {
                return _cells;
            }
        }

        /// <summary>
        /// Method to determine if a row is empty.
        /// </summary>
        /// <returns>Returns True if the row is composed of null or empty string formatted value cells.</returns>
        public bool IsEmpty()
        {
            for (uint i = Cells.MinCol; i <= Cells.MaxCol; ++i)
            {
                Cell c = Cells[i];

                if (c != null && c.Value != null && !string.IsNullOrEmpty(c.FormattedValue()))
                    return false;
            }

            return true;
        }

        bool IRow.IsEmpty()
        {
            return IsEmpty();
        }


        ICell IRow.GetCell(uint index)
        {
            return Cells[index];
        }
    }
}
