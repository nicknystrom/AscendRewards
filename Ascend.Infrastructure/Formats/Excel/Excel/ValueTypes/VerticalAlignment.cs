using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Vertical alignment enumeration.
	/// </summary>
	public enum VerticalAlignment : byte
	{
		/// <summary>
		///Top 
		/// </summary>
		Top = 0x00,
		/// <summary>
		/// Centered
		/// </summary>
		Centered = 0x01,
		/// <summary>
		/// Bottom
		/// </summary>
		Bottom = 0x02,
		/// <summary>
		/// Justified
		/// </summary>
		Justified = 0x03,
		/// <summary>
		/// Distributed
		/// </summary>
		Distributed = 0x04
	}
}
