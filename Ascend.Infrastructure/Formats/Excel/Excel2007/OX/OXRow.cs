using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXRow
    {
        public OXRow()
        {

        }

        private uint _rowIndex;

        [XmlAttribute("r")]
        public uint RowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }

        private int _customFormat;

        [XmlAttribute("customFormat")]
        public int CustomFormat
        {
            get { return _customFormat; }
            set { _customFormat = value; }
        }

        private uint _styleIndex;

        public uint StyleIndex
        {
            get { return _styleIndex; }
            set { _styleIndex = value; }
        }

        private OXCell[] _cell;

        [XmlElement("c")]
        public OXCell[] Cells
        {
            get { return _cell; }
            set { _cell = value; }
        }
    }
}
