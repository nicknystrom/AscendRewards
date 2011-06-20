using System;
using System.Collections.Generic;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Abstract for a workbook.
    /// </summary>
    public interface IWorkbook
    {
        /// <summary>
        /// Returns the worksheets available in this workbook.
        /// </summary>
        IWorksheets Worksheets { get; }
    }
}
