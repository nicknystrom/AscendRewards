using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Abstract for a cell.
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// The raw value of the cell.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Retrieves the formatted value of the cell.
        /// </summary>
        /// <remarks>
        /// Uses the Win32 VarFormat function.
        /// </remarks>
        /// <returns>Returns the formatted value of the cell of possibly null if the cell has a null value.</returns>
        string GetFormattedValue();
    }
}
