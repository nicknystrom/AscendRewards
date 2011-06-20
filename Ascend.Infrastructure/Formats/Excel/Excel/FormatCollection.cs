using System;
using System.Diagnostics;
using System.Globalization;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Collection class for Format objects.
	/// </summary>
	public class FormatCollection : ExcelIndexedCollection<int, Format>
	{
		private static readonly string[] _builtIn1 = new string[] { "",
			"0", "0.00", "#,##0", "#,##0.00", 
			"\"$\"#,##0_);(\"$\"#,##0)", "\"$\"#,##0_);(\"$\"#,##0)",
			"\"$\"#,##0.00_);(\"$\"#,##0.00)", "\"$\"#,##0.00_);(\"$\"#,##0.00)",
			"0%", "0.00%", "0.00E+00", "#?/?", "#??/??", 
			"M/D/YY", "D-MMM-YY", "D-MMM", "MMM-YY", 
			"h:mm AM/PM", "h:mm:ss AM/PM",
			"M/D/YY h:mm", 
																	};
		private static readonly string[] _builtIn2 = new string[] {
			"_(#,##0_);(#,##0)", "_(#,##0_);(#,##0)",
			"_(#,##0.00_);(#,##0.00)", "_(#,##0.00_);(#,##0.00)",
			"_(\"$\"* #,##0_);_(\"$\"* (#,##0);_(\"$\"* \"-\"_);_(@_)",
			"_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)",
			"_(\"$\"* #,##0.00_);_(\"$\"* (#,##0.00);_(\"$\"* \"-\"??_);_(@_)",
			"_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)",
			"mm:ss", "[h]:mm:ss", "mm:ss.0", "##0.0E+0", "@",
																  };
		internal FormatCollection(Workbook wb) : base(wb)
		{
			CultureInfo culture = CultureInfo.CurrentCulture;
			string currencySymbol = culture.NumberFormat.CurrencySymbol;

			int idx = 0;
			foreach(string format in _builtIn1)
			{
				Format f = new Format(wb, format.Replace("\"$\"", currencySymbol));
				BaseAdd(idx++, f);
			}

			idx = 37;
			foreach(string format in _builtIn2)
			{
				Format f = new Format(wb, format.Replace("\"$\"", currencySymbol));
				BaseAdd(idx++, f);
			}
		}

		internal void Add(int index, Format f)
		{
			if(!ContainsKey(index))
				BaseAdd(index, f);
	}

		/// <summary>
		/// Indexer for the collection.
		/// </summary>
		public Format this[int index]
		{
			get
			{
				return BaseGet(index);
			}
		}
	}
}
