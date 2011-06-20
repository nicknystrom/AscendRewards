using System;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Interface that must be implemented by all Excel objects.
	/// </summary>
	/// <remarks>
	/// Basically, Excel objects are simply objects that are inside a workbook.
	/// </remarks>
	public interface IExcelObject
	{
		/// <summary>
		/// Must return a reference to the objects workbook and must not return null.
		/// </summary>
		Workbook Workbook { get; }
	}
}
