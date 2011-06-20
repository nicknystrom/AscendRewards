using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Version enumeration.
	/// </summary>
	public enum Version : ushort
	{
		/// <summary>
		/// Unsupported
		/// </summary>
		Unsupported = 0,
		/// <summary>
		/// Biff5_7 - Excel 95 - 97
		/// </summary>
		Biff5_7 = 0x0,
		/// <summary>
		/// Biff 8 - Excel 2000
		/// </summary>
		Biff8 = 0x0600
	}
}
