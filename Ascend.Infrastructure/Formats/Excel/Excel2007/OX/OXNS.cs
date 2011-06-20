using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Ionic.Utils.Zip;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007.OX
{
    public static class OXNS
    {
        public const string OpenXmlSpreadsheetNS = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        public const string OfficeRelationsNS = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        
        public const string PackageRelationsNS = "http://schemas.openxmlformats.org/package/2006/relationships";

        internal static Stream GetTempStream()
        {
            return new MemoryStream();
        }

        public static X Load<X>(ZipFile f, string path)
        {
            if (f == null)
                throw new ArgumentNullException("f");

            using (Stream s = GetTempStream())
            {
                if (s == null)
                    throw new NullReferenceException("GetTempStream returned null");

                try
                {
                    f.Extract(path, s);
                }
                catch (NullReferenceException)
                {
                    return default(X);
                }

                s.Seek(0, SeekOrigin.Begin);

                XmlSerializer x = new XmlSerializer(typeof(X));

                return (X)x.Deserialize(s);
            }
        }
    }
}
