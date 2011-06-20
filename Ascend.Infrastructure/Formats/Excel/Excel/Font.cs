using System;
using Net.SourceForge.Koogra.Excel.Records;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Class that represents a font.
	/// </summary>
	public class Font : ExcelObject
	{
		private ushort _height;
		private FontOptions _options;
		private PaletteEntry _color;
		private FontBoldness _boldNess;
		private FontEscape _escapement;
		private FontUnderline _underline;
		private FontFamily _family;
		private FontCharacterSet _characterSet;
		private string _fontName;

		internal Font(Workbook wb, FontRecord f) : base(wb)
		{
			_height = f.FontHeight;
			_options = f.Options;
			_color = wb.Palette.GetColor(f.ColorIdx);
			_boldNess = f.Boldness;
			_escapement = f.Escapement;
			_underline = f.Underline;
			_family = f.FontFamily;
			_characterSet = f.CharacterSet;
			_fontName = f.FontName;
		}

		/// <summary>
		/// Font height.
		/// </summary>
		public ushort Height
		{
			get
			{
				return _height;
			}
		}

		/// <summary>
		/// Font options.
		/// </summary>
		public FontOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Font color.
		/// </summary>
		public PaletteEntry Color
		{
			get
			{
				return _color;
			}
		}

		/// <summary>
		/// Font boldness.
		/// </summary>
		public FontBoldness BoldNess
		{
			get
			{
				return _boldNess;
			}
		}

		/// <summary>
		/// Font escapement.
		/// </summary>
		public FontEscape Escapement
		{
			get
			{
				return _escapement;
			}
		}

		/// <summary>
		/// Font underline.
		/// </summary>
		public FontUnderline Underline
		{
			get
			{
				return _underline;
			}
		}

		/// <summary>
		/// Font family.
		/// </summary>
		public FontFamily Family
		{
			get
			{
				return _family;
			}
		}

		/// <summary>
		/// Font character set.
		/// </summary>
		public FontCharacterSet CharacterSet
		{
			get
			{
				return _characterSet;
			}
		}

		/// <summary>
		/// Font name.
		/// </summary>
		public string FontName
		{
			get
			{
				return _fontName;
			}
		}

	}
}
