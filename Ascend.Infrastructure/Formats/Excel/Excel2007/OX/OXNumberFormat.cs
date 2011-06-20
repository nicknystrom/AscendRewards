using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXNumberFormat
    {
        public OXNumberFormat()
        {

        }

        private uint _id;

        [XmlAttribute("numFmtId")]
        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _format;

        [XmlAttribute("formatCode")]
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

    }
}
