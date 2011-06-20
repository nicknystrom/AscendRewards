using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    [XmlRoot("styleSheet", Namespace = OXNS.OpenXmlSpreadsheetNS)]
    public class OXStyleSheet
    {
        public const string DefaultPath = @"xl\styles.xml";

        public OXStyleSheet()
        {

        }

        private OXXfCollection _cellXFS;

        [XmlElement("cellXfs")]
        public OXXfCollection CellXFS
        {
            get { return _cellXFS; }
            set { _cellXFS = value; }
        }

        private OXNumberFormatCollection _numberFormats;

        [XmlElement("numFmts")]
        public OXNumberFormatCollection NumberFormats
        {
            get { return _numberFormats; }
            set { _numberFormats = value; }
        }

    }
}
