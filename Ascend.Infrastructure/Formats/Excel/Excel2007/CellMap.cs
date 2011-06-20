using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    using OX;

    public class CellMap
    {
        public CellMap(OXWorksheet ws)
        {
            foreach (OXRow r in ws.Rows)
            {
                if (r.Cells == null)
                    continue;

                foreach (OXCell c in r.Cells)
                {
                    uint row = A1Translator.GetRowIndex(c.Reference) - 1;

                    if (!_firstRow.HasValue || row < _firstRow.Value)
                        _firstRow = row;

                    if (!_lastRow.HasValue || row > _lastRow.Value)
                        _lastRow = row;

                    ColMap cols;

                    if (!_rows.TryGetValue(row, out cols))
                        _rows.Add(row, cols = new ColMap());

                    uint col = A1Translator.GetCellIndex(c.Reference) - 1;

                    if (!_firstCol.HasValue || col < _firstCol.Value)
                        _firstCol = col;

                    if (!_lastCol.HasValue || col > _lastCol.Value)
                        _lastCol = col;

#if DEBUG
                    if (cols.ContainsKey(col))
                        throw new Exception(string.Format("Duplicate reference {0}", c.Reference));
#endif

                    if (!cols.ContainsKey(col))
                        cols.Add(col, c);
                }
            }
        }

        private uint? _firstRow;

        public uint FirstRow
        {
            get { return _firstRow.GetValueOrDefault(); }
        }

        private uint? _lastRow;

        public uint LastRow
        {
            get { return _lastRow.GetValueOrDefault(); }
        }

        private uint? _firstCol;

        public uint FirstCol
        {
            get { return _firstCol.GetValueOrDefault(); }
        }

        private uint? _lastCol;

        public uint LastCol
        {
            get { return _lastCol.GetValueOrDefault(); }
        }

        private class ColMap : Dictionary<uint, OXCell>
        {
        }

        private class RowMap : Dictionary<uint, ColMap>
        {
        }

        private RowMap _rows = new RowMap();

        internal OXCell GetCell(uint row, uint col)
        {
            OXCell ret = null;

            ColMap cols;

            if (_rows.TryGetValue(row, out cols))
                cols.TryGetValue(col, out ret);

            return ret;
        }
    }
}
