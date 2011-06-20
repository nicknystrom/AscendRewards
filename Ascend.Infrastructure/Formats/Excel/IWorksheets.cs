using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Abstract for a collection of worksheets.
    /// </summary>
    public interface IWorksheets
    {
        /// <summary>
        /// Retrieves the available worksheet names for the workbook.
        /// </summary>
        /// <returns>Enumerator for the worksheet names.</returns>
        IEnumerable<string> EnumerateWorksheetNames();

        /// <summary>
        /// Retrieves a worksheet by it's name.
        /// 
        /// This method is case sensitive.
        /// </summary>
        /// <param name="name">The case sensitive worksheet name.</param>
        /// <returns>
        /// A <see cref="IWorksheet"/> that matches the requested name.
        /// 
        /// Null if no matching name is found.</returns>
        IWorksheet GetWorksheetByName(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The case sensitive worksheet name.</param>
        /// <param name="ignoreCase">Set to True for a case insensitive match, False if otherwise.</param>
        /// <returns>
        /// A <see cref="IWorksheet"/> that matches the requested name.
        /// 
        /// Null if no matching name is found.</returns>
        IWorksheet GetWorksheetByName(string name, bool ignoreCase);

        /// <summary>
        /// Retrieve a worksheet by ordinal position.
        /// </summary>
        /// <param name="index">The index where to retrieve the worksheet.</param>
        /// <returns>A <see cref="IWorksheet"/> at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Is thrown if there is no worksheet at the specified index.</exception>
        IWorksheet GetWorksheetByIndex(int index);

        /// <summary>
        /// Returns the count of available worksheets in this workbook.
        /// </summary>
        int Count { get; }
    }
}
