using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Abstract for a worksheet.
    /// </summary>
    public interface IWorksheet
    {
        /// <summary>
        /// The worksheet name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The worksheet rows.
        /// </summary>
        IRows Rows { get; }

        /// <summary>
        /// The first non-empty row index in the worksheet.
        /// </summary>
        uint FirstRow { get; }
        /// <summary>
        /// The Last non-empty row index in the worksheet.
        /// </summary>
        uint LastRow { get; }
        /// <summary>
        /// The first non-empty column index in the worksheet.
        /// </summary>
        uint FirstCol { get; }
        /// <summary>
        /// The last non-empty column index in the worksheet.
        /// </summary>
        uint LastCol { get; }
    }
}
