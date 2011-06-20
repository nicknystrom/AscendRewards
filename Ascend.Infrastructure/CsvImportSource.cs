using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using LumenWorks.Framework.IO.Csv;

using Ascend.Core.Services.Import;

namespace Ascend.Infrastructure
{
    public class CsvImportSource : IImportSource
    {
        public string[] ReadHeaders(string absoluteUrl)
        {
            return ReadRows(absoluteUrl, 0, true)[0];
        }

        public string[][] ReadRows(string absoluteUrl, int rows, bool includeHeader)
        {
            var csv = GetReader();
            if (includeHeader)
            {
                rows++;
            }
            var a = new string[rows][];
            var i = 0;
            if (includeHeader)
            {
                a[i++] = csv.GetFieldHeaders();
            }
            while (i < rows && csv.ReadNextRecord())
            {
                var x = new string[csv.FieldCount];
                csv.CopyCurrentRecordTo(x);
                a[i++] = x;
            }
            return a;
        }

        private readonly string _url;
        private CsvReader _reader;

        public CsvImportSource(string url)
        {
            _url = url;
        }

        private CsvReader GetReader()
        {
            if (null == _reader)
            {
                var www = new WebClient();
                var buf = www.DownloadData(_url);
                var txt = new StreamReader(new MemoryStream(buf), Encoding.Default);
                _reader = new CsvReader(txt, true);
            }
            return _reader;
        }

        public string[] Fields
        {
            get { return GetReader().GetFieldHeaders(); }
        }

        public int? Rows
        {
            get { return null; }
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return new __Enumerator(GetReader());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new __Enumerator(GetReader());
        }

        private class __Enumerator : IEnumerator<object[]>
        {
            private IDataReader _reader;
            private object[] _buf;

            public __Enumerator(IDataReader reader)
            {
                _reader = reader;
            }

            public void Dispose()
            {
                _reader = null;
                _buf = null;
            }

            public bool MoveNext()
            {
                var result = _reader.Read();
                if (null == _buf)
                {
                    _buf = new object[_reader.FieldCount];
                }
                _reader.GetValues(_buf);
                return result;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public object[] Current
            {
                get { return _buf; }
            }

            object IEnumerator.Current
            {
                get { return _buf; }
            }
        }
    }
}
