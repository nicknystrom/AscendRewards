using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXRelation
    {
        public const string WorksheetType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";

        public OXRelation()
        {

        }

        private string _id;

        [XmlAttribute()]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _type;

        [XmlAttribute()]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _target;

        [XmlAttribute()]
        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }
}

