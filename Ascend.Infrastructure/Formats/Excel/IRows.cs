using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Abstract for the rows in a worksheet.
    /// </summary>
    public interface IRows
    {
        /// <summary>
        /// Retries a row at the specified index.
        /// </summary>
        /// <param name="index">The index for the row.</param>
        /// <returns>
        /// Return a <see cref="IRow"/> at the specified index.
        /// 
        /// Returns null if no row exists at the specified index.
        /// </returns>
        IRow GetRow(uint index);
    }
}
