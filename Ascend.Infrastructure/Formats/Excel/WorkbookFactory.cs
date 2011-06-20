using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Net.SourceForge.Koogra
{
    /// <summary>
    /// Helper class for returning an appropriate <see cref="IWorkbook"/> implementation.
    /// </summary>
    public static class WorkbookFactory
    {
        /// <summary>
        /// Returns a <see cref="IWorkbook"/> for BIFF based Excel 97 and up BIFF based files.
        /// </summary>
        /// <param name="path">The path to the Excel file.</param>
        /// <returns>Returns a <see cref="IWorkbook"/> for BIFF based Excel 97 and up files.</returns>
        public static IWorkbook GetExcelBIFFReader(string path)
        {
            return new Excel.Workbook(path);
        }

        /// <summary>
        /// Returns a <see cref="IWorkbook"/> for BIFF based Excel 97 and up BIFF based files.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> that contains a BIFF based Excel file.</param>
        /// <returns>Returns a <see cref="IWorkbook"/> for BIFF based Excel 97 and up BIFF based files.</returns>
        public static IWorkbook GetExcelBIFFReader(Stream stream)
        {
            return new Excel.Workbook(stream);
        }

        /// <summary>
        /// Returns a <see cref="IWorkbook"/> for Excel 2007 .xlsx files.
        /// </summary>The path to the Excel file.
        /// <param name="path">The path to the Excel file.</param>
        /// <returns>Returns a <see cref="IWorkbook"/> for Excel 2007 .xlsx files.</returns>
        public static IWorkbook GetExcel2007Reader(string path)
        {
            return new Excel2007.Workbook(path);
        }

        /// <summary>
        /// Returns a <see cref="IWorkbook"/> for Excel 2007 .xlsx files.
        /// </summary>The path to the Excel file.
        /// <param name="stream"><see cref="Stream"/> that contains a xlsx based Excel file.</param>
        /// <returns>Returns a <see cref="IWorkbook"/> for Excel 2007 .xlsx files.</returns>
        public static IWorkbook GetExcel2007Reader(Stream stream)
        {
            return new Excel2007.Workbook(stream);
        }
    }
}
