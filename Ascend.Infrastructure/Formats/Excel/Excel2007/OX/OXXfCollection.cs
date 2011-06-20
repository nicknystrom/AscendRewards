using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXXfCollection
    {
        public OXXfCollection()
        {

        }

        private int _count;

        [XmlAttribute("count")]
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private OXXf[] _xfs;

        [XmlElement("xf")]
        public OXXf[] XFS
        {
            get { return _xfs; }
            set { _xfs = value; }
        }
    }
}
