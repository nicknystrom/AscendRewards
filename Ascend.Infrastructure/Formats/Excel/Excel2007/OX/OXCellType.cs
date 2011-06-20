using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public enum OXCellType
    {
        [XmlEnum("str")]
        String,
        [XmlEnum("b")]
        Boolean,
        [XmlEnum("e")]
        Error,
        [XmlEnum("inlineStr")]
        InlineString,
        [XmlEnum("n")]
        Number,
        [XmlEnum("s")]
        SharedString,
    }
}
