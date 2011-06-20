using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXSSTEntry
    {
        public OXSSTEntry()
        {

        }

        private object[] _strings;

        [XmlElement("t", Type = typeof(string))]
        [XmlElement("r", Type = typeof(OXFormattedString))]
        public object[] Strings
        {
            get { return _strings; }
            set { _strings = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in GetStrings())
                sb.Append(s);

            return sb.ToString();
        }

        public IEnumerable<string> GetStrings()
        {
            if (Strings != null)
            {
                foreach (object o in Strings)
                {
                    if (o is string)
                        yield return (string)o;
                    else if (o is OXFormattedString)
                    {
                        OXFormattedString fs = (OXFormattedString)o;

                        if (fs.Strings != null)
                            foreach (string s in fs.Strings)
                                yield return s;
                    }
                }
            }
        }
    }
}
