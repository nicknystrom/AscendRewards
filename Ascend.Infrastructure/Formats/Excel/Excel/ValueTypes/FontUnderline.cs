using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Font underline.
	/// </summary>
	public enum FontUnderline : byte
	{
		/// <summary>
		/// None 
		/// </summary>
		None = 0x00,
		/// <summary>
		/// Single 
		/// </summary>
		Single = 0x01,
		/// <summary>
		/// Double 
		/// </summary>
		Double = 0x02,
		/// <summary>
		/// Single Accounting 
		/// </summary>
		SingleAccounting = 0x21,
		/// <summary>
		/// Double Accounting 
		/// </summary>
		DoubleAccounting = 0x22,
	}
}
