using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    [XmlRoot("sst", Namespace = OXNS.OpenXmlSpreadsheetNS)]
    public class OXSST
    {
        public const string DefaultPath = @"xl\sharedStrings.xml";

        public OXSST()
        {

        }

        private int _count;

        [XmlAttribute("count")]
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private int _uniqueCount;

        [XmlAttribute("uniqueCount")]
        public int UniqueCount
        {
            get { return _uniqueCount; }
            set { _uniqueCount = value; }
        }	

        private OXSSTEntry[] _strings;

        [XmlElement("si")]
        public OXSSTEntry[] Entries
        {
            get { return _strings; }
            set { _strings = value; }
        }

    }
}
