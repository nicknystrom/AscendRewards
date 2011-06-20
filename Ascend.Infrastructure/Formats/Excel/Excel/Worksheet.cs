using System;
using System.Collections.Generic;
using System.Diagnostics;
using Net.SourceForge.Koogra.Excel.Records;

#pragma warning disable 0219

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Represents a worksheet in a workbook.
    /// </summary>
    public class Worksheet : ExcelObject, IWorksheet
    {
        private string _name;
        private RowCollection _rows;
        private HyperLinkCollection _hyperlinks;

        internal Worksheet(Workbook wb, BoundSheetRecord sheet, SortedList<long, Biff> records)
            : base(wb)
        {
            _name = sheet.Name;

            int idx = records.IndexOfKey((long)sheet.BofPos);

            _hyperlinks = new HyperLinkCollection(wb);

            for (int i = idx + 1; i < records.Count; ++i)
            {
                Biff biff = records.Values[i];
                if (biff is HyperLinkRecord)
                    _hyperlinks.Add((HyperLinkRecord)biff);
                else if (biff is EofRecord)
                    break;
            }

            BofRecord bof = (BofRecord)records.Values[idx++];

            Biff seeker = records.Values[idx++];

            while (!(seeker is IndexRecord))
                seeker = records.Values[idx++];

            IndexRecord index = (IndexRecord)seeker;

            _rows = new RowCollection(wb);
            foreach (uint indexPos in index.Rows)
            {
                long dbCellPos = indexPos;
                int dbCellIdx = records.IndexOfKey(dbCellPos);
                DbCellRecord dbCell = (DbCellRecord)records[dbCellPos];

                if (dbCell.RowOffset > 0)
                {
                    long rowPos = dbCellPos - dbCell.RowOffset;
                    int recIndex = records.IndexOfKey(rowPos);
                    Debug.Assert(recIndex != -1);

                    Biff record = records.Values[recIndex++];
                    while (record is RowRecord)
                    {
                        RowRecord row = (RowRecord)record;
                        Row currentRow = new Row(Workbook, row);
                        _rows.Add(row.RowNumber, currentRow);

                        record = records.Values[recIndex++];
                    }

                    while (recIndex <= dbCellIdx)
                    {
                        if (!(record is CellRecord))
                        {
                            record = records.Values[recIndex++];
                            continue;
                        }

                        CellRecord thecell = (CellRecord)record;
                        Row currentRow = _rows[thecell.Row];

                        if (thecell is SingleColCellRecord)
                        {
                            SingleColCellRecord cell = (SingleColCellRecord)thecell;
                            object val = cell.Value;

                            Cell newCell = new Cell(Workbook, val);
                            if (cell is RowColXfCellRecord)
                            {
                                RowColXfCellRecord xfCell = (RowColXfCellRecord)cell;

                                Style style = Workbook.Styles[xfCell.Xf];
                                Debug.Assert(style != null);
                                newCell.Style = style;
                            }
                            currentRow.Cells.Add((byte)cell.Col, newCell);
                        }
                        else
                        {
                            MultipleColCellRecord cells = (MultipleColCellRecord)thecell;
                            for (ushort i = cells.FirstCol; i <= cells.LastCol; ++i)
                            {
                                object val = cells.GetValue(i);
                                if (val != null)
                                {
                                    Cell newCell = null;
                                    if (val is RkRec)
                                    {
                                        RkRec rk = (RkRec)val;

                                        newCell = new Cell(Workbook, rk.Value);
                                        Style style = Workbook.Styles[rk.Xf];
                                        Debug.Assert(style != null);
                                        newCell.Style = style;
                                    }
                                    else
                                        newCell = new Cell(Workbook, val);

                                    currentRow.Cells.Add((byte)i, newCell);
                                }
                            }
                        }

                        record = records.Values[recIndex++];
                    }
                }
            }
        }

        /// <summary>
        /// The name of the worksheet.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// The collection of rows in the worksheet.
        /// </summary>
        public RowCollection Rows
        {
            get
            {
                return _rows;
            }
        }

        /// <summary>
        /// The hyperlink table/collection in the worksheet.
        /// </summary>
        public HyperLinkCollection HyperLinks
        {
            get
            {
                return _hyperlinks;
            }
        }

        IRows IWorksheet.Rows
        {
            get { return Rows; }
        }

        uint IWorksheet.FirstRow
        {
            get { return Rows.MinRow; }
        }

        uint IWorksheet.LastRow
        {
            get { return Rows.MaxRow; }
        }

        private uint? _minCol;
        private uint? _maxCol;

        private void InitMinMaxCol()
        {
            if (!_minCol.HasValue)
            {
                foreach (Row r in Rows.InternalValues)
                {
                    if (!_minCol.HasValue || r.Cells.MinCol < _minCol.Value)
                        _minCol = r.Cells.MinCol;

                    if (!_maxCol.HasValue || r.Cells.MaxCol > _maxCol.Value)
                        _maxCol = r.Cells.MaxCol;
                }

                if (!_minCol.HasValue)
                    _minCol = _maxCol = 0;
            }
        }

        uint IWorksheet.FirstCol
        {
            get
            {
                InitMinMaxCol();

                return _minCol.Value;
            }
        }

        uint IWorksheet.LastCol
        {
            get
            {
                InitMinMaxCol();

                return _maxCol.Value;
            }
        }
    }
}


