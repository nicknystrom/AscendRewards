using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Net.SourceForge.Koogra.Excel.Records;

namespace Net.SourceForge.Koogra.Excel
{
    /// <summary>
    /// Class that represents the individual cells in a row.
    /// </summary>
    public class Cell : ExcelObject, ICell
    {
        [DllImport("oleaut32.dll")]
        private static extern int VarFormat(
            ref object o,
            [MarshalAs(UnmanagedType.BStr)]
			string format,
            int firstDay,
            int firstWeek,
            uint flags,
            [MarshalAs(UnmanagedType.BStr)]
			ref string output);

        private object _value;
        private Style _style;

        /// <summary>
        /// Use this method to format a value.
        /// </summary>
        /// <param name="str">The value to format.</param>
        /// <param name="format">The format string. This format string is the same as strings used in old OLE applications.</param>
        /// <returns>The formatted value of str.</returns>
        /// <permission cref="System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode">This method requires the specified permission.</permission>
        public static string Format(object str, string format)
        {
            return Format(str, format, 0, 0, 0);
        }

        /// <summary>
        /// Use this method to format a value.
        /// </summary>
        /// <param name="str">The value to format.</param>
        /// <param name="format">The format string. This format string is the same as strings used in old OLE applications.</param>
        /// <param name="firstDay">The first day of the week for date formats.</param>
        /// <param name="firstWeek">The first week of the year.</param>
        /// <param name="flags">Flags that control the formatting process.</param>
        /// <returns>The formatted value of str.</returns>
        /// <remarks>For further documentation on firstDay, firstWeek and flags, lookup VarFormat in the MSDN library.</remarks>
        /// <permission cref="System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode">This method requires the specified permission.</permission>
        public static string Format(object str, string format, int firstDay, int firstWeek, uint flags)
        {
            // initilize our return value.
            string output = "";

            // format the value
            int ret = VarFormat(ref str, format, firstDay, firstWeek, flags, ref output);

            // check for errors, similar to the c++ macro FAILED(hr)
            if (ret < 0)
                // raise the proper exception
                Marshal.ThrowExceptionForHR(ret);

            // return the value if there are no errors
            return output;
        }

        internal Cell(Workbook wb, object value)
            : base(wb)
        {
            _value = value;
        }

        /// <summary>
        /// The value of the cell.
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Formatting.
        /// </summary>
        public Style Style
        {
            get
            {
                return _style;
            }
            set
            {
                Workbook.CheckIfMember(value);
                _style = value;
            }
        }

        /// <summary>
        /// Use this method to get the value of the cell with formatting applied.
        /// </summary>
        /// <returns>The formatted value of the cell</returns>
        /// <permission cref="System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode">This method requires the specified permission.</permission>
        public string FormattedValue()
        {
            // initialize the return value
            string ret = "";

            // check if there is a value and a style
            if (_value != null && _style != null)
            {
                // get the string value
                ret = _value.ToString();

                // retrieve the format if any
                Format format = _style.Format;
                if (format != null)
                {
                    // get the format string
                    string formatString = format.FormatValue;

                    if (formatString != null && formatString != "")
                    {
                        try
                        {
                            // try to format the value, raise no exception as this is the behavior for excel
                            ret = Format(_value, formatString);
                        }
                        catch (Exception err)
                        {
                            Debug.WriteLine("Cell Format Error");
                            Debug.WriteLine(err.Message);
                            Debug.WriteLine(err.StackTrace);
                        }
                    }
                }
            }
            // if there is only a value, just return the string value
            else if (_value != null)
                ret = _value.ToString();

            // return the formatted value
            return ret;
        }

        object ICell.Value
        {
            get { return Value; }
        }

        string ICell.GetFormattedValue()
        {
            return FormattedValue();
        }
    }
}
