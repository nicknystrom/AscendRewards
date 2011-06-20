using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXNumberFormatCollection
    {
        public OXNumberFormatCollection()
        {

        }

        private uint _count;

        [XmlAttribute("count")]
        public uint Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private OXNumberFormat[] _numberFormats;

        [XmlElement("numFmt")]
        public OXNumberFormat[] NumberFormatEntries
        {
            get { return _numberFormats; }
            set { _numberFormats = value; }
        }

    }
}
