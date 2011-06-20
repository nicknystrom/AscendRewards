using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Sheet type enumeration.
	/// </summary>
	public enum SheetType : byte
	{
		/// <summary>
		/// WorkSheet
		/// </summary>
		WorkSheet = 0,
		/// <summary>
		/// Chart
		/// </summary>
		Chart,
		/// <summary>
		/// VBModule
		/// </summary>
		VBModule
	}
}
