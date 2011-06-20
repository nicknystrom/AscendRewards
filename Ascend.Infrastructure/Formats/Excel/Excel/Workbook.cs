using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Net.SourceForge.Koogra.Excel.Records;
using Net.SourceForge.Koogra.Storage;

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Represents a workbook.
    /// </summary>
    public class Workbook : IWorkbook
    {
        private WorksheetCollection _sheets;
        private StyleCollection _styles;
        private FormatCollection _formats;
        private FontCollection _fonts;
        private Palette _palette;
        private HyperLinkCollection _hyperLinks;

        private void Load(CompoundFile doc)
        {
            Stream stream;
            try
            {
                // see if workbook works
                stream = doc.OpenStream("Workbook");
            }
            catch (IOException)
            {
                // see if book works, if not then leak the exception
                stream = doc.OpenStream("Book");
            }

            SstRecord sst = null;
            /* long sstPos = 0; */

            // record position dictionary
            SortedList<long, Biff> records = new SortedList<long, Biff>();

            _styles = new StyleCollection(this);
            _formats = new FormatCollection(this);
            _fonts = new FontCollection(this);
            _palette = new Palette(this);
            _hyperLinks = new HyperLinkCollection(this);

            while (stream.Length - stream.Position >= GenericBiff.MinimumSize)
            {
                // capture the current stream position
                long pos = stream.Position;

                // decode the record if possible
                Biff record = GetCorrectRecord(new GenericBiff(stream), stream, sst);

                // capture 
                // shared string table 
                if (record is SstRecord)
                {
                    Debug.Assert(sst == null);
                    sst = (SstRecord)record;
                    /* sstPos = pos; */
                }
                // formatting records
                else if (record is FormatRecord)
                {
                    FormatRecord f = (FormatRecord)record;
                    _formats.Add(f.Index, new Format(this, f));
                }
                else if (record is FontRecord)
                    _fonts.Add(new Font(this, (FontRecord)record));
                else if (record is PaletteRecord)
                    _palette.Initialize((PaletteRecord)record);
                else if (record is XfRecord)
                    _styles.Add(new Style(this, (XfRecord)record));
                else if (record is HyperLinkRecord)
                    _hyperLinks.Add((HyperLinkRecord)record);

                Debug.Assert(!records.ContainsKey(pos));
                // store the position and corresponding record
                records[pos] = record;
            }

            // generate the worksheets
            _sheets = new WorksheetCollection();
            foreach (Biff record in records.Values)
            {
                if (record is BoundSheetRecord)
                    _sheets.Add(new Worksheet(this, (BoundSheetRecord)record, records));
            }
        }

        /// <summary>
        /// File path constructor.
        /// </summary>
        /// <param name="path">The path to the excel file.</param>
        public Workbook(string path)
        {
            // open the stream, load the stream to read only but set share to read write so that we can open files open by other applications such as excel itself
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // load the compound file
                Load(new CompoundFile(stream));
                // release the stream
                stream.Close();
            }
        }

        /// <summary>
        /// Stream constructor.
        /// </summary>
        /// <param name="stream">The stream that has excel file data.</param>
        public Workbook(Stream stream)
        {
            Load(new CompoundFile(stream));
        }

        /// <summary>
        /// CompoundFile constructor.
        /// </summary>
        /// <param name="doc">The compound file that has excel file data.</param>
        public Workbook(CompoundFile doc)
        {
            Load(doc);
        }

        private Biff GetCorrectRecord(GenericBiff record, Stream stream, SstRecord sst)
        {
            Biff ret = record;
            switch (record.Id)
            {
                case (ushort)RecordType.Bof:
                    BofRecord bof = new BofRecord(record);
                    if (bof.Version < 0x0600)
                        throw new Exception("Versions below Excel 97/2000 are currently not supported.");

                    ret = bof;
                    break;
                case (ushort)RecordType.Boundsheet:
                    ret = new BoundSheetRecord(record);
                    break;
                case (ushort)RecordType.Index:
                    ret = new IndexRecord(record);
                    break;
                case (ushort)RecordType.DbCell:
                    ret = new DbCellRecord(record);
                    break;
                case (ushort)RecordType.Row:
                    ret = new RowRecord(record);
                    break;
                case (ushort)RecordType.Continue:
                    ret = new ContinueRecord(record);
                    break;
                case (ushort)RecordType.Blank:
                    ret = new BlankRecord(record);
                    break;
                case (ushort)RecordType.BoolErr:
                    ret = new BoolErrRecord(record);
                    break;
                case (ushort)RecordType.Formula:
                    ret = new FormulaRecord(record, stream);
                    break;
                case (ushort)RecordType.Label:
                    ret = new LabelRecord(record);
                    break;
                case (ushort)RecordType.LabelSst:
                    ret = new LabelSstRecord(record, sst);
                    break;
                case (ushort)RecordType.MulBlank:
                    ret = new MulBlankRecord(record);
                    break;
                case (ushort)RecordType.MulRk:
                    ret = new MulRkRecord(record);
                    break;
                case (ushort)RecordType.String:
                    ret = new StringValueRecord(record);
                    break;
                case (ushort)RecordType.Xf:
                    ret = new XfRecord(record);
                    break;
                case (ushort)RecordType.Rk:
                    ret = new RkRecord(record);
                    break;
                case (ushort)RecordType.Number:
                    ret = new NumberRecord(record);
                    break;
                case (ushort)RecordType.Array:
                    ret = new ArrayRecord(record);
                    break;
                case (ushort)RecordType.ShrFmla:
                    ret = new SharedFormulaRecord(record);
                    break;
                case (ushort)RecordType.Table:
                    ret = new TableRecord(record);
                    break;
                case (ushort)RecordType.Sst:
                    ret = new SstRecord(record, stream);
                    break;
                case (ushort)RecordType.Eof:
                    ret = new EofRecord(record);
                    break;
                case (ushort)RecordType.Font:
                    ret = new FontRecord(record);
                    break;
                case (ushort)RecordType.Format:
                    ret = new Net.SourceForge.Koogra.Excel.Records.FormatRecord(record);
                    break;
                case (ushort)RecordType.Palette:
                    ret = new PaletteRecord(record);
                    break;
                case (ushort)RecordType.Hyperlink:
                    ret = new HyperLinkRecord(record);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// The worksheets in the workbook.
        /// </summary>
        public WorksheetCollection Sheets
        {
            get
            {
                return _sheets;
            }
        }

        /// <summary>
        /// The style collection table.
        /// </summary>
        public StyleCollection Styles
        {
            get
            {
                return _styles;
            }
        }

        /// <summary>
        /// The format collection table.
        /// </summary>
        public FormatCollection Formats
        {
            get
            {
                return _formats;
            }
        }

        /// <summary>
        /// The font collection table.
        /// </summary>
        public FontCollection Fonts
        {
            get
            {
                return _fonts;
            }
        }

        /// <summary>
        /// The color palette collection table.
        /// </summary>
        public Palette Palette
        {
            get
            {
                return _palette;
            }
        }

        internal void CheckIfMember(IExcelObject o)
        {
            if (!object.ReferenceEquals(this, o.Workbook))
                throw new ArgumentException("Specified object is not a member of the workbook");
        }

        /// <summary>
        /// The hyperlink table
        /// </summary>
        public HyperLinkCollection HyperLinks
        {
            get
            {
                return _hyperLinks;
            }
        }

        IWorksheets IWorkbook.Worksheets
        {
            get { return Sheets; }
        }
    }
}
