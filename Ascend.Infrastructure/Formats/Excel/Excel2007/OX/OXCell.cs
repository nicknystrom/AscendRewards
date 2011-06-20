using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXCell
    {
        public OXCell()
        {

        }	

        private string _reference;

        [XmlAttribute("r")]
        public string Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        private uint _styleIndex;

        [XmlAttribute("s")]
        public uint StyleIndex
        {
            get { return _styleIndex; }
            set { _styleIndex = value; }
        }

        private OXFormattedString _fstr;

        [XmlElement("is")]
        public OXFormattedString FormattedString
        {
            get { return _fstr; }
            set { _fstr = value; }
        }

        private OXCellType _type;

        [XmlAttribute("t")]
        public OXCellType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _value;

        [XmlElement("v")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

    }
}
