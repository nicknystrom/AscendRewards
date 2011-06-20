using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Abstract for a row.
    /// </summary>
    public interface IRow
    {
        /// <summary>
        /// Method to determine if the row is empty.
        /// </summary>
        /// <returns>Returns True if the row mostly contains null or empty string cells.</returns>
        bool IsEmpty();

        /// <summary>
        /// Returns a cell at the specified index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>
        /// Returns a <see cref="ICell"/> that exists at the specified index.
        /// 
        /// Should return null if no cell exists.
        /// </returns>
        ICell GetCell(uint index);
    }
}
