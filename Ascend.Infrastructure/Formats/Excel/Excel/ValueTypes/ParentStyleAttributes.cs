using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Parent style usage attributes enumeration.
	/// </summary>
	[Flags()]
	public enum ParentStyleAttributes : byte
	{
		/// <summary>
		/// None 
		/// </summary>
		None = 0x00,
		/// <summary>
		/// Number 
		/// </summary>
		Number = 0x01,
		/// <summary>
		/// Font 
		/// </summary>
		Font = 0x02,
		/// <summary>
		/// Orientation 
		/// </summary>
		Orientation = 0x04,
		/// <summary>
		/// Border 
		/// </summary>
		Border = 0x08,
		/// <summary>
		/// Background 
		/// </summary>
		Background = 0x10,
		/// <summary>
		/// Protection 
		/// </summary>
		Protection = 0x20
	}
}
