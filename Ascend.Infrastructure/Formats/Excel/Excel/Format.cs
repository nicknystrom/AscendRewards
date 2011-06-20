using System;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Represents a cell format.
	/// </summary>
	public class Format : ExcelObject
	{
		private string _formatValue;

		internal Format(Workbook wb, Net.SourceForge.Koogra.Excel.Records.FormatRecord f) : base(wb)
		{
			_formatValue = f.Format;
		}

		internal Format(Workbook wb, string formatValue) : base(wb)
		{
			_formatValue = formatValue;
		}

		/// <summary>
		/// The format string.
		/// </summary>
		public string FormatValue
		{
			get
			{
				return _formatValue;
			}
		}

	}
}
