using System;
using System.Collections.Generic;
using Net.SourceForge.Koogra.Collections;

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Collection class of Worksheet objects.
    /// </summary>
    public class WorksheetCollection : SimpleCollection<Worksheet>, IWorksheets
    {
        internal WorksheetCollection()
        {
        }

        /// <summary>
        /// Retrieves a worksheet given its name.
        /// </summary>
        /// <param name="index">The name of the worksheet.</param>
        /// <returns>Returns null if the worksheet cannot be found.</returns>
        /// <remarks>Search is case sensitive.</remarks>
        public Worksheet GetByName(string index)
        {
            return GetByName(index, true);
        }

        /// <summary>
        /// Retrieves a worksheet given its name.
        /// </summary>
        /// <param name="index">The name of the worksheet.</param>
        /// <param name="ignoreCase">Set to True to perform a case insenstive match, False if otherwise.</param>
        /// <returns>Returns null if the worksheet cannot be found.</returns>
        /// <remarks>Search is case sensitive.</remarks>
        public Worksheet GetByName(string index, bool ignoreCase)
        {
            foreach (Worksheet sheet in this)
            {
                if (string.Compare(sheet.Name, index, true) == 0)
                    return sheet;
            }

            return null;
        }

        IEnumerable<string> IWorksheets.EnumerateWorksheetNames()
        {
            throw new NotImplementedException();
        }

        IWorksheet IWorksheets.GetWorksheetByName(string name)
        {
            return GetByName(name);
        }

        IWorksheet IWorksheets.GetWorksheetByName(string name, bool ignoreCase)
        {
            return GetByName(name, ignoreCase);
        }

        IWorksheet IWorksheets.GetWorksheetByIndex(int index)
        {
            return this[index];
        }

        int IWorksheets.Count
        {
            get { return Count; }
        }
    }
}
