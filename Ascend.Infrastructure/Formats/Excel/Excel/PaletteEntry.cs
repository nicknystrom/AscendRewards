using System;
using System.Drawing;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Represents an entry in the Palette class.
	/// </summary>
	public class PaletteEntry : ExcelObject
	{
		private Color _color;

		internal PaletteEntry(Workbook wb, Color color) : base(wb)
		{
			_color = color;
		}

		/// <summary>
		/// The color of the entry.
		/// </summary>
		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}
	}
}
