using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ascend.Core.Services.Import;
using Net.SourceForge.Koogra;

namespace Ascend.Infrastructure
{
    public class ExcelImportSource : IImportSource
    {
        private readonly string _url;
        private IWorkbook _workbook;
        private IWorksheet _worksheet;
        
        public ExcelImportSource(string url)
        {
            _url = url;
        }

        private IWorksheet GetWorksheet()
        {
            if (null == _worksheet)
            {
                var www = new WebClient();
                var buf = www.DownloadData(_url);

                // test for xml or pkzip (ie. packed xlsx format) leadin
                // also account for leading text encoding markers in xml
                if (buf.Length <= 10) return null;
                if ((buf[0] == 'P' && buf[1] == 'K') ||
                    (buf[0] == '<' && buf[1] == '?' && buf[2] == 'x') ||
                    (buf[1] == '<' && buf[2] == '?' && buf[3] == 'x') ||
                    (buf[2] == '<' && buf[3] == '?' && buf[4] == 'x') ||
                    (buf[3] == '<' && buf[4] == '?' && buf[5] == 'x') ||
                    (buf[4] == '<' && buf[5] == '?' && buf[6] == 'x'))
                {

                    using (var ms = new MemoryStream(buf))
                        _workbook = WorkbookFactory.GetExcel2007Reader(ms);
                }
                else
                {
                    using (var ms = new MemoryStream(buf))
                        _workbook = WorkbookFactory.GetExcelBIFFReader(ms);
                }

                _worksheet = _workbook.Worksheets.GetWorksheetByIndex(0);
            }
            return _worksheet;
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return new __Enumerator(GetWorksheet());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string[] Fields
        {
            get
            {
                var ws = GetWorksheet();
                var r = ws.Rows.GetRow(0);
                var a = new string[1 + ws.LastCol - ws.FirstCol];
                for (uint i = 0; i <= ws.LastCol - ws.FirstCol; i++)
                {
                    a[i] = r.GetCell(ws.FirstCol + i).GetFormattedValue();
                }
                return a;
            }
        }

        public int? Rows
        {
            get
            {
                var ws = GetWorksheet();
                return (int?)(1 + ws.LastRow - ws.FirstRow);
            }
        }

        private class __Enumerator : IEnumerator<object[]>
        {
            private IWorksheet _ws;
            private uint _cursor;
            private object[] _data;

            public __Enumerator(IWorksheet worksheet)
            {
                _ws = worksheet;
                _cursor = 0;
            }

            public void Dispose()
            {
                _ws = null;
            }

            public bool MoveNext()
            {
                return ++_cursor <= _ws.LastRow;
            }

            public void Reset()
            {
                _cursor = 0;
            }

            public object[] Current
            {
                get
                {
                    if (null == _data)
                    {
                        _data = new object[1 + _ws.LastCol - _ws.FirstCol];
                    }
                    var r = _ws.Rows.GetRow(_cursor);
                    for (uint i = 0; i <= _ws.LastCol - _ws.FirstCol; i++)
                    {
                        var cell = r.GetCell(_ws.FirstCol + i);
                        _data[i] = null == cell ? null : cell.Value;
                    }
                    return _data;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
