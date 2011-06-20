using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Utils.Zip;
using Net.SourceForge.Koogra.Excel2007.OX;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    public class Workbook : IWorkbook, IWorksheets
    {
        private OXWorkbook _wb;
        private OXRelationCollection _excelRelations;
        private OXStyleSheet _styleSheet;
        private OXSST _sst;

        private ZipFile _f;

        public Workbook(string path)
            : this(new ZipFile(path))
        {

        }

        public Workbook(Stream s)
            : this(ZipFile.Read(s))
        {

        }

        public Workbook(ZipFile f)
        {
            _wb = OXNS.Load<OXWorkbook>(f, OXWorkbook.DefaultPath);

            if (_wb == null)
                throw new InvalidOperationException("File contains no workbook");

            _excelRelations = OXNS.Load<OXRelationCollection>(f, OXRelationCollection.DefaultExcelRelationPath);

            if (_excelRelations == null)
                throw new InvalidOperationException("File contains no excel relationship map");

            _styleSheet = OXNS.Load<OXStyleSheet>(f, OXStyleSheet.DefaultPath);
            _sst = OXNS.Load<OXSST>(f, OXSST.DefaultPath);

            _f = f;
        }

        internal OXSST SST
        {
            get { return _sst; }
        }

        internal OXStyleSheet StyleSheet
        {
            get { return _styleSheet; }
        }

        public Worksheet GetWorksheetByName(string name)
        {
            return GetWorksheetByName(name, true);
        }

        public Worksheet GetWorksheetByName(string name, bool ignoreCase)
        {
            foreach (OXWorkbookWorksheetEntry e in _wb.Sheets)
            {
                if (string.Compare(name, e.Name, ignoreCase) == 0)
                    return ProcessGetWorksheet(e);
            }

            return null;
        }

        private Worksheet ProcessGetWorksheet(OXWorkbookWorksheetEntry e)
        {
            OXRelation r = _excelRelations.GetRelation(e.RelationId);

            if (r == null)
                throw new Exception(string.Format("Relationship id {0} does not exist in the relationships for worksheet name {1}", e.RelationId, e.Name));

            OXWorksheet ows = OXNS.Load<OXWorksheet>(
                _f,
                @"xl\" + r.Target.Replace('/', '\\')
            );

            if (ows == null)
                throw new Exception(string.Format("Worksheet target {0} does not exist for relation id {1}", r.Target, r.Id));

            return new Worksheet(this, e.Name, ows);
        }

        public Worksheet GetWorksheet(int index)
        {
            if (index < _wb.Sheets.Length)
                return ProcessGetWorksheet(_wb.Sheets[index]);

            return null;
        }

        public IEnumerable<Worksheet> GetWorksheets()
        {
            foreach (OXWorkbookWorksheetEntry e in _wb.Sheets)
                yield return ProcessGetWorksheet(e);
        }

        private Dictionary<uint, string> _formats;

        internal string GetFormat(OXCell c)
        {
            if (StyleSheet != null &&
                StyleSheet.CellXFS != null &&
                StyleSheet.CellXFS.XFS != null)
            {
                if (c.StyleIndex < StyleSheet.CellXFS.XFS.Length)
                {
                    OXXf xf = StyleSheet.CellXFS.XFS[c.StyleIndex];

                    if (xf.ApplyNumberFormat == 1)
                    {
                        string f = OXBuiltInFormats.GetFormat(xf.NumFormatID);

                        if (string.IsNullOrEmpty(f))
                        {
                            if (StyleSheet.NumberFormats != null &&
                                StyleSheet.NumberFormats.NumberFormatEntries != null)
                            {
                                if (_formats == null)
                                {
                                    _formats = new Dictionary<uint, string>();

                                    foreach (OXNumberFormat nf in StyleSheet.NumberFormats.NumberFormatEntries)
                                    {
                                        if (!_formats.ContainsKey(nf.Id))
                                            _formats.Add(nf.Id, nf.Format);
                                        else
                                        {
#if DEBUG
                                            throw new Exception(string.Format("Duplicate number format id {0}", nf.Id));
#endif
                                        }
                                    }
                                }

                                if (!_formats.TryGetValue(xf.NumFormatID, out f))
                                {
#if DEBUG
                                    throw new Exception(string.Format("Invalid number format id {0} for cell {1}", xf.NumFormatID, c.Reference));
#endif
                                }
                            }
                        }

                        return f ?? "";
                    }
                }
                else
                {
#if DEBUG
                    throw new Exception(string.Format("Cell {0} has non existent cell xf {1}", c.Reference, c.StyleIndex));
#endif
                }
            }

            return "";
        }

        IWorksheets IWorkbook.Worksheets
        {
            get { return this; }
        }

        IEnumerable<string> IWorksheets.EnumerateWorksheetNames()
        {
            foreach (OXWorkbookWorksheetEntry e in _wb.Sheets)
                yield return e.Name;
        }

        IWorksheet IWorksheets.GetWorksheetByName(string name)
        {
            return GetWorksheetByName(name);
        }

        IWorksheet IWorksheets.GetWorksheetByName(string name, bool ignoreCase)
        {
            return GetWorksheetByName(name, ignoreCase);
        }

        IWorksheet IWorksheets.GetWorksheetByIndex(int index)
        {
            return GetWorksheet(index);
        }

        int IWorksheets.Count
        {
            get { return _wb.Sheets.Length; }
        }
    }
}
