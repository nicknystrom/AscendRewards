using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Ionic.Utils.Zip;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    [XmlRoot("workbook", Namespace = OXNS.OpenXmlSpreadsheetNS)]
    public class OXWorkbook
    {
        public const string DefaultPath = @"xl\workbook.xml";
        public OXWorkbook()
        {

        }

        private OXWorkbookWorksheetEntry[] _sheets;

        [XmlArray("sheets")]
        [XmlArrayItem("sheet")]
        public OXWorkbookWorksheetEntry[] Sheets
        {
            get { return _sheets; }
            set { _sheets = value; }
        }
    }
}
