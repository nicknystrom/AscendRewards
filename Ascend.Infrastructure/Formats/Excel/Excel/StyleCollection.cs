using System;
using System.Diagnostics;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Collection class for Style objects.
	/// </summary>
	public class StyleCollection : ExcelCollection<Style>
	{
		internal StyleCollection(Workbook wb) : base(wb)
		{			
		}
	}
}
