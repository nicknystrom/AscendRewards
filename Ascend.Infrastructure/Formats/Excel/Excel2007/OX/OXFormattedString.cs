using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXFormattedString
    {
        public OXFormattedString()
        {

        }

        private string[] _strings;

        [XmlElement("t")]
        public string[] Strings
        {
            get { return _strings; }
            set { _strings = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Strings != null)
            {
                foreach (string s in Strings)
                    sb.Append(s);
            }

            return sb.ToString();
        }
    }
}
