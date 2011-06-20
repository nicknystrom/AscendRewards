using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public class OXWorkbookWorksheetEntry
    {
        public OXWorkbookWorksheetEntry()
        {

        }

        private string _name;

        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _sheetId;

        [XmlAttribute("sheetId")]
        public int SheetId
        {
            get { return _sheetId; }
            set { _sheetId = value; }
        }

        private string _relId;

        [XmlAttribute("id", Namespace = OXNS.OfficeRelationsNS)]
        public string RelationId
        {
            get { return _relId; }
            set { _relId = value; }
        }

    }
}
