using System;
using System.Collections.Generic;
using System.Text;
using Net.SourceForge.Koogra.Excel2007.OX;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    public sealed class Cell : WorksheetChild, ICell
    {
        internal Cell(Row r, uint index)
            : base(r.Worksheet, index)
        {
            Row = r;
        }

        public Row Row { get; private set; }

        private bool _valueInit;

        private string _value;

        public string Value
        {
            get
            {
                InitValue();

                return _value;
            }
        }

        private void InitValue()
        {
            if (!_valueInit)
            {
                OXCell cell = Worksheet.CellMap.GetCell(Row.Index, Index);

                if (cell != null)
                {
                    switch (cell.Type)
                    {
                        case OXCellType.Boolean:
                            _value = cell.Value;
                            _typedValue = _value == "1";
                            break;
                        case OXCellType.Number:
                            _value = cell.Value;
                            _typedValue = decimal.Parse(cell.Value);
                            break;
                        default:
                            _typedValue =
                                _value = Worksheet.GetCellValue(cell);
                            break;
                    }
                }

                _valueInit = true;
            }
        }

        private object _typedValue;

        public object TypedValue
        {
            get
            {
                InitValue();

                return _typedValue;
            }
        }

        object ICell.Value
        {
            get
            {
                return TypedValue;
            }
        }

        public string GetFormattedValue()
        {
            return Worksheet.GetFormattedValue(Row.Index, Index);
        }

        string ICell.GetFormattedValue()
        {
            return GetFormattedValue();
        }
    }
}
