using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    [XmlRoot("worksheet", Namespace = OXNS.OpenXmlSpreadsheetNS)]
    public class OXWorksheet
    {
        public OXWorksheet()
        {

        }

        private OXRow[] _rows;

        [XmlArray("sheetData")]
        [XmlArrayItem("row")]
        public OXRow[] Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }
    }
}
