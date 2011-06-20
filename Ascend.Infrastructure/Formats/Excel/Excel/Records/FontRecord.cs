using System;
using System.IO;
using Net.SourceForge.Koogra.Excel.ValueTypes;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the FONT (0x0031) Excel record found in Excel streams.
	/// </summary>
	public class FontRecord : Biff
	{
		private ushort _fontHeight;
		private FontOptions _options;
		private ushort _colorIdx;
		private FontBoldness _boldness;
		private FontEscape _escapement;
		private FontUnderline _underline;
		private FontFamily _fontFamily;
		private FontCharacterSet _characterSet;
		private string _fontName;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the FONT record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public FontRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Font)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				_fontHeight = reader.ReadUInt16();
				_options = (FontOptions)reader.ReadUInt16();
				_colorIdx = reader.ReadUInt16();
				_boldness = new FontBoldness(reader.ReadUInt16());
				_escapement = (FontEscape)reader.ReadUInt16();
				_underline = (FontUnderline)reader.ReadByte();
				_fontFamily = (FontFamily)reader.ReadByte();
				_characterSet = (FontCharacterSet)reader.ReadByte();
				/* byte reserved = */ reader.ReadByte();
				byte len = reader.ReadByte();
				_fontName = Reader.ReadComplexString(reader, len);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Font);
		}

		/// <summary>
		/// The height of the font in twips.
		/// </summary>
		public ushort FontHeight
		{
			get
			{
				return _fontHeight;
			}
		}

		/// <summary>
		/// Options for the font.
		/// </summary>
		public FontOptions Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Index in the COLOR table for the font.
		/// </summary>
		public ushort ColorIdx
		{
			get
			{
				return _colorIdx;
			}
		}

		/// <summary>
		/// Boldness of the font.
		/// </summary>
		public FontBoldness Boldness
		{
			get
			{
				return _boldness;
			}
		}

		/// <summary>
		/// Escapement of the font.
		/// </summary>
		public FontEscape Escapement
		{
			get
			{
				return _escapement;
			}
		}

		/// <summary>
		/// Underline options for the font.
		/// </summary>
		public FontUnderline Underline
		{
			get
			{
				return _underline;
			}
		}

		/// <summary>
		/// Font family options for the font.
		/// </summary>
		public FontFamily FontFamily
		{
			get
			{
				return _fontFamily;
			}
		}

		/// <summary>
		/// Character set of the font.
		/// </summary>
		public FontCharacterSet CharacterSet
		{
			get
			{
				return _characterSet;
			}
		}

		/// <summary>
		/// Name of the font.
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
