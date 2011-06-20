using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Font options.
	/// </summary>
	[Flags()]
	public enum FontOptions : ushort
	{
		/// <summary>
		/// None 
		/// </summary>
		None = 0x0000,
		/// <summary>
		/// Italic 
		/// </summary>
		Italic = 0x0002,
		/// <summary>
		/// Strike Out 
		/// </summary>
		StrikeOut = 0x0008,
	}
}
