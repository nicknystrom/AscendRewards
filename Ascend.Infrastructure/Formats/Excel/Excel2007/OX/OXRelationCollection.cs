using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    [XmlRoot("Relationships", Namespace = OXNS.PackageRelationsNS)]
    public class OXRelationCollection
    {
        public const string DefaultExcelRelationPath = @"xl\_rels\workbook.xml.rels";

        public OXRelationCollection()
        {

        }

        private List<OXRelation> _relationships;

        [XmlElement("Relationship")]
        public List<OXRelation> Relationships
        {
            get { return _relationships; }
            set { _relationships = value; }
        }

        [Obsolete()]
        public IEnumerable<OXRelation> GetRelations(string type)
        {
            foreach (OXRelation r in Relationships)
            {
                if (r.Type == type)
                    yield return r;
            }
        }

        public OXRelation GetRelation(string id)
        {
            foreach (OXRelation r in Relationships)
            {
                if (r.Id == id)
                    return r;
            }

            return null;
        }
    }
}
