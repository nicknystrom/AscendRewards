using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXXf
    {
        public OXXf()
        {

        }

        private uint _numFmtId;

        [XmlAttribute("numFmtId")]
        public uint NumFormatID
        {
            get { return _numFmtId; }
            set { _numFmtId = value; }
        }

        private int _applyNumberFormat;

        [XmlAttribute("applyNumberFormat")]
        public int ApplyNumberFormat
        {
            get { return _applyNumberFormat; }
            set { _applyNumberFormat = value; }
        }
    }
}
