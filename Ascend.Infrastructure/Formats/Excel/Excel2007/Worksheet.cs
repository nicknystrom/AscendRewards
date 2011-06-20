using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    using OX;

    public class Worksheet : IWorksheet, IRows
    {
        [DllImport("oleaut32.dll")]
        private static extern int VarFormat(
            ref object o,
            [MarshalAs(UnmanagedType.BStr)]
		    string format,
            int firstDay,
            int firstWeek,
            uint flags,
            [MarshalAs(UnmanagedType.BStr)]
		    ref string output);

        internal Worksheet(Workbook wb, string name, OXWorksheet s)
        {
#if DEBUG
            _cellMap = new CellMap(s);
#endif

            _wb = wb;

            _s = s;

            _name = name;
        }

        private OXWorksheet _s;

        private Workbook _wb;

        private CellMap _cellMap;

        public Workbook Workbook
        {
            get { return _wb; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public CellMap CellMap
        {
            get
            {
                if (_cellMap == null)
                    _cellMap = new CellMap(_s);

                return _cellMap;
            }
        }

        public string GetFormattedValue(uint row, uint cell)
        {
            OXCell c = CellMap.GetCell(row, cell);

            if (c != null)
            {
                string value = GetCellValue(c);

                string format = GetFormat(c);

                if (!string.IsNullOrEmpty(format))
                {
                    object fvalue = value;

                    string output = null;

                    int ret = VarFormat(ref fvalue, format, 0, 0, 0, ref output);

                    if (ret < 0)
                    {
#if DEBUG
                        Marshal.ThrowExceptionForHR(ret);
#else
                        return value;
#endif
                    }

                    return output;
                }

                return value;
            }

            return "";
        }

        private string GetFormat(OXCell c)
        {
            if (Workbook != null)
                return Workbook.GetFormat(c);

            return "";
        }

        public bool IsEmpty(uint row)
        {
            if (row >= CellMap.FirstRow && row <= CellMap.LastRow)
            {
                for (uint col = CellMap.FirstCol; col <= CellMap.LastCol; ++col)
                {
                    OXCell c = CellMap.GetCell(row, col);

                    if (c != null && !string.IsNullOrEmpty(GetCellValue(c)))
                        return false;
                }
            }

            return true;
        }

        internal string GetCellValue(OXCell c)
        {
            switch (c.Type)
            {
                case OXCellType.InlineString:
                    if (c.FormattedString == null)
                    {
#if DEBUG
                        throw new Exception("Cell type is InlineString but FormattedString is null");
#else
                        return "";
#endif
                    }

                    return c.FormattedString.ToString();
                case OXCellType.SharedString:
                    bool valid = false;

                    uint idx;

                    valid = uint.TryParse(c.Value, out idx);

                    if (Workbook.SST == null ||
                        !valid ||
                        idx >= Workbook.SST.Entries.Length)
                    {
#if DEBUG
                        throw new Exception(string.Format("Cell type is SharedString and refers to an invalid shared string table reference {0} for cell {1}", idx, c.Reference));
#else
                        return "";
#endif
                    }

                    return Workbook.SST.Entries[idx].ToString();
                default:
                    return c.Value;
            }
        }

        private Dictionary<uint, Row> _rows;

        public Row GetRow(uint index)
        {
            if (_rows == null)
                _rows = new Dictionary<uint, Row>();

            Row ret = null;

            if (!_rows.TryGetValue(index, out ret))
                _rows.Add(index, ret = new Row(this, index));

            return ret;
        }

        IRows IWorksheet.Rows
        {
            get { return this; }
        }

        uint IWorksheet.FirstRow
        {
            get { return CellMap.FirstRow; }
        }

        uint IWorksheet.LastRow
        {
            get { return CellMap.LastRow; }
        }

        uint IWorksheet.FirstCol
        {
            get { return CellMap.FirstCol; ; }
        }

        uint IWorksheet.LastCol
        {
            get { return CellMap.LastCol; }
        }

        string IWorksheet.Name
        {
            get { return Name; }
        }

        IRow IRows.GetRow(uint index)
        {
            return GetRow(index);
        }

    }
}
