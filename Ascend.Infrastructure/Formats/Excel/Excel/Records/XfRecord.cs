using System;
using System.Diagnostics;
using System.IO;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the XF (0x00e0) Excel record found in Excel streams.
	/// </summary>
	public class XfRecord : Biff
	{
		private ushort _fontIdx;
		private ushort _formatIdx;
		private TypeAndProtection _typeAndProtection;
		private ushort _parentIdx;
		private ParentStyleAttributes _parentStyle;
		private HorizontalAlignment _horizontalAlignment;
		private bool _wrapped;
		private VerticalAlignment _verticalAlignment;
		private Rotation _rotation;
		private Nibble _indentLevel;
		private bool _shrinkContent;
		private LineStyle _leftLineStyle;
		private LineStyle _rightLineStyle;
		private LineStyle _topLineStyle;
		private LineStyle _bottomLineStyle;
		private ushort _leftLineColor;
		private ushort _rightLineColor;
		private bool _diagonalRightTopToLeftBottom;
		private bool _diagonalLeftBottomToTopRight;
		private ushort _topLineColor;
		private ushort _bottomLineColor;
		private ushort _diagonalLineColor;
		private LineStyle _diagonalLineStyle;
		private FillPattern _fillPattern;
		private ushort _patternColor;
		private ushort _patternBackground;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the XF record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public XfRecord(GenericBiff biff) 
		{
			if(biff.Id == (ushort)RecordType.Xf)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());
				
				_fontIdx = reader.ReadUInt16();
				_formatIdx = reader.ReadUInt16();

				ushort data = reader.ReadUInt16();
				_typeAndProtection = new TypeAndProtection(data & 0x000F);
				_parentIdx = (ushort)((data & 0xFFF0) >> 8);

				byte data2 = reader.ReadByte();
				_horizontalAlignment = (HorizontalAlignment)(data2 & 0x07);
				_wrapped = (data & 0x088) != 0;
				_verticalAlignment = (VerticalAlignment)((data2 & 0x70) >> 4);
				
				data2 = reader.ReadByte();
				_rotation = new Rotation(data2);

				data2 = reader.ReadByte();
				_indentLevel = new Nibble((byte)(data2 & 0x0F));
				_shrinkContent = (data2 & 0x10) == 1;

				_parentStyle = (ParentStyleAttributes)(reader.ReadByte());
					
				uint data3 = reader.ReadUInt32();
				_leftLineStyle = (LineStyle)(data3 & 0x0000000F);
				_rightLineStyle = (LineStyle)((data3 & 0x000000F0) >> 1);
				_topLineStyle = (LineStyle)((data3 & 0x00000F00) >> 2);
				_bottomLineStyle = (LineStyle)((data3 & 0x0000F000) >> 3);
				_rightLineColor = (ushort)((data3 & 0x007F0000) >> 4);
				_leftLineColor = (ushort)((data3 & 0x3F800000) >> 5);
				_diagonalRightTopToLeftBottom = (data & 0x40000000) == 1;
				_diagonalLeftBottomToTopRight = (data & 0x80000000) == 1;

				data3 = reader.ReadUInt32();
				_topLineColor = (ushort)(data3 & 0x0000007F);
				_bottomLineColor = (ushort)((data3 & 0x00003F80) >> 1);
				_diagonalLineColor = (ushort)((data3 & 0x001FC000) >> 3);
				_diagonalLineStyle = (LineStyle)((data3 & 0x01E00000) >> 5);
				_fillPattern = (FillPattern)((data3 & 0xFC000000) >> 6);

				data = reader.ReadUInt16();
				_patternColor = (ushort)(data & 0x007F);
				_patternBackground = (ushort)((data & 0x3F80) >> 7);

				Debug.Assert(reader.BaseStream.Position == reader.BaseStream.Length);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Xf);
		}

		/// <summary>
		/// Index in the font table for the formatting record font.
		/// </summary>
		public ushort FontIdx
		{
			get
			{
				return _fontIdx;
			}

		}

		/// <summary>
		/// Index in the format table for the formatting record format.
		/// </summary>
		public ushort FormatIdx
		{
			get
			{
				return _formatIdx;
			}
		}

		/// <summary>
		/// Type and protection option for the format.
		/// </summary>
		public TypeAndProtection TypeAndProtection
		{
			get
			{
				return _typeAndProtection;
			}
		}

		/// <summary>
		/// The parent format for the FORMAT record.
		/// </summary>
		public ushort ParentIdx
		{
			get
			{
				return _parentIdx;
			}
		}

		/// <summary>
		/// Parent style usage attributes.
		/// </summary>
		public ParentStyleAttributes ParentStyle
		{
			get
			{
				return _parentStyle;
			}
		}

		/// <summary>
		/// Horizontal alignment.
		/// </summary>
		public HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return _horizontalAlignment;
			}
		}

		/// <summary>
		/// Text wrapping.
		/// </summary>
		public bool Wrapped
		{
			get
			{
				return _wrapped;
			}
		}

		/// <summary>
		/// Vertical alignment.
		/// </summary>
		public VerticalAlignment VerticalAlignment
		{
			get
			{
				return _verticalAlignment;
			}
		}

		/// <summary>
		/// Rotation.
		/// </summary>
		public Rotation Rotation
		{
			get
			{
				return _rotation;
			}
		}

		/// <summary>
		/// Left line style.
		/// </summary>
		public LineStyle LeftLineStyle
		{
			get
			{
				return _leftLineStyle;
			}
		}

		/// <summary>
		/// Right line style.
		/// </summary>
		public LineStyle RightLineStyle
		{
			get
			{
				return _rightLineStyle;
			}
		}

		/// <summary>
		/// Top line style.
		/// </summary>
		public LineStyle TopLineStyle
		{
			get
			{
				return _topLineStyle;
			}
		}

		/// <summary>
		/// Bottom line style.
		/// </summary>
		public LineStyle BottomLineStyle
		{
			get
			{
				return _bottomLineStyle;
			}
		}

		/// <summary>
		/// Left line color.
		/// </summary>
		public ushort LeftLineColor
		{
			get
			{
				return _leftLineColor;
			}
		}

		/// <summary>
		/// Right line color.
		/// </summary>
		public ushort RightLineColor
		{
			get
			{
				return _rightLineColor;
			}
		}

		/// <summary>
		/// Diagonal line right top to left bottom.
		/// </summary>
		public bool DiagonalRightTopToLeftBottom
		{
			get
			{
				return _diagonalRightTopToLeftBottom;
			}
		}

		/// <summary>
		/// Diagonal line left bottom to right top.
		/// </summary>
		public bool DiagonalLeftBottomToTopRight
		{
			get
			{
				return _diagonalLeftBottomToTopRight;
			}
		}

		/// <summary>
		/// Top line color.
		/// </summary>
		public ushort TopLineColor
		{
			get
			{
				return _topLineColor;
			}
		}

		/// <summary>
		/// Bottom line color.
		/// </summary>
		public ushort BottomLineColor
		{
			get
			{
				return _bottomLineColor;
			}
		}

		/// <summary>
		/// Diagonal line color.
		/// </summary>
		public ushort DiagonalLineColor
		{
			get
			{
				return _diagonalLineColor;
			}
		}

		/// <summary>
		/// Diagonal line style.
		/// </summary>
		public LineStyle DiagonalLineStyle
		{
			get
			{
				return _diagonalLineStyle;
			}
		}

		/// <summary>
		/// Fill pattern.
		/// </summary>
		public FillPattern FillPattern
		{
			get
			{
				return _fillPattern;
			}
		}

		/// <summary>
		/// Pattern color.
		/// </summary>
		public ushort PatternColor
		{
			get
			{
				return _patternColor;
			}
		}

		/// <summary>
		/// Pattern background.
		/// </summary>
		public ushort PatternBackground
		{
			get
			{
				return _patternBackground;
			}
		}

		/// <summary>
		/// Indent level.
		/// </summary>
		public Nibble IndentLevel
		{
			get
			{
				return _indentLevel;
			}
		}

		/// <summary>
		/// Shrink.
		/// </summary>
		public bool ShrinkContent
		{
			get
			{
				return _shrinkContent;
			}
		}

	}
}
