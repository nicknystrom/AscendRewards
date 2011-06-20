using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    public sealed class Row : WorksheetChild, IRow
    {
        internal Row(Worksheet ws, uint index)
            : base(ws, index)
        {

        }

        bool IRow.IsEmpty()
        {
            return Worksheet.IsEmpty(Index);
        }

        private Dictionary<uint, Cell> _cells;

        public ICell GetCell(uint index)
        {
            if (_cells == null)
                _cells = new Dictionary<uint, Cell>();

            Cell ret;

            if (!_cells.TryGetValue(index, out ret))
                _cells.Add(index, ret = new Cell(this, index));

            return ret;
        }
    }
}
