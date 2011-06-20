using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Horizontal alignment enumeration.
	/// </summary>
	public enum HorizontalAlignment : byte
	{
		/// <summary>
		/// General 
		/// </summary>
		General = 0x00,
		/// <summary>
		/// Left 
		/// </summary>
		Left = 0x01,
		/// <summary>
		/// Centered 
		/// </summary>
		Centered = 0x02,
		/// <summary>
		/// Right 
		/// </summary>
		Right = 0x03,
		/// <summary>
		/// Filled 
		/// </summary>
		Filled = 0x04,
		/// <summary>
		/// Justified 
		/// </summary>
		Justified = 0x05,
		/// <summary>
		/// Centered Across Selection 
		/// </summary>
		CenteredAcrossSelection = 0x06,
		/// <summary>
		/// Distributed 
		/// </summary>
		Distributed = 0x07
	}
}
