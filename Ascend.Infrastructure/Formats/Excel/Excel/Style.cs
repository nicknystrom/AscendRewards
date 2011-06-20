using System;
using System.Collections.Generic;
using System.Diagnostics;
using Net.SourceForge.Koogra.Excel.Records;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Represents a cell style.
	/// </summary>
	public class Style : ExcelObject
	{
		private Font _font;
		private Format _format;
		private Style _parentStyle;
		private TypeAndProtection _typeAndProtection;
		private HorizontalAlignment _horizontalAlignment;
		private bool _wrapped;
		private VerticalAlignment _verticalAlignment;
		private Rotation _rotation;
		private Nibble _indentLevel;
		private bool _shrinkContent;
		private ParentStyleAttributes _parentStyleAttributes;
		private LineStyle _leftLineStyle;
		private LineStyle _rightLineStyle;
		private LineStyle _topLineStyle;
		private LineStyle _bottomLineStyle;
		private PaletteEntry _leftLineColor;
		private PaletteEntry _rightLineColor;
		private bool _diagonalRightTopToLeftBottom;
		private bool _diagonalLeftBottomToTopRight;
		private PaletteEntry _topLineColor;
		private PaletteEntry _bottomLineColor;
		private PaletteEntry _diagonalLineColor;
		private LineStyle _diagonalLineStyle;
		private FillPattern _fillPattern;
		private PaletteEntry _patternColor;
		private PaletteEntry _patternBackground;

		internal Style(Workbook wb, XfRecord xf) : base(wb)
		{	
			if(xf.FontIdx > 0 && xf.FontIdx < wb.Fonts.Count)
    	        _font = wb.Fonts[xf.FontIdx - 1];
		    _format = wb.Formats[xf.FormatIdx];
			_typeAndProtection = xf.TypeAndProtection;
			if(_typeAndProtection.IsCell)
				_parentStyle = wb.Styles[xf.ParentIdx];
			_horizontalAlignment = xf.HorizontalAlignment;
			_wrapped = xf.Wrapped;
			_verticalAlignment = xf.VerticalAlignment;
			_rotation = xf.Rotation;
			_indentLevel = xf.IndentLevel;
			_shrinkContent = xf.ShrinkContent;
			_parentStyleAttributes = xf.ParentStyle;
			_leftLineStyle = xf.LeftLineStyle;
			_rightLineStyle = xf.RightLineStyle;
			_topLineStyle = xf.TopLineStyle;
			_bottomLineStyle = xf.BottomLineStyle;
			_leftLineColor = wb.Palette.GetColor(xf.LeftLineColor);
			_rightLineColor = wb.Palette.GetColor(xf.RightLineColor);
			_diagonalRightTopToLeftBottom = xf.DiagonalRightTopToLeftBottom;
			_diagonalLeftBottomToTopRight = xf.DiagonalLeftBottomToTopRight;
			_topLineColor = wb.Palette.GetColor(xf.TopLineColor);
			_bottomLineColor = wb.Palette.GetColor(xf.BottomLineColor);
			_diagonalLineColor = wb.Palette.GetColor(xf.DiagonalLineColor);
			_diagonalLineStyle = xf.DiagonalLineStyle;
			_fillPattern = xf.FillPattern;
			_patternColor = wb.Palette.GetColor(xf.PatternColor);
			_patternBackground = wb.Palette.GetColor(xf.PatternBackground);
		}

		/// <summary>
		/// Font.
		/// </summary>
		public Font Font
		{
			get
			{
				return _font;
			}
		}

		/// <summary>
		/// Format.
		/// </summary>
		public Format Format
		{
			get
			{
				return _format;
			}
		}

		/// <summary>
		/// Parent style.
		/// </summary>
		public Style ParentStyle
		{
			get
			{
				return _parentStyle;
			}
			set
			{
				if(!object.ReferenceEquals(value, this))
				{
					Workbook.CheckIfMember(value);
					_parentStyle = value;
				}
				else
					throw new ArgumentException("Cannot assign self as parent");
			}
		}

		/// <summary>
		/// Type and protection.
		/// </summary>
		public TypeAndProtection TypeAndProtection
		{
			get
			{
				return _typeAndProtection;
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
		/// Wrapped.
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
		/// IndentLevel.
		/// </summary>
		public Nibble IndentLevel
		{
			get
			{
				return _indentLevel;
			}
		}

		/// <summary>
		/// Shrink content.
		/// </summary>
		public bool ShrinkContent
		{
			get
			{
				return _shrinkContent;
			}
		}

		/// <summary>
		/// Parent style attributes usage.
		/// </summary>
		public ParentStyleAttributes ParentStyleAttributes
		{
			get
			{
				return _parentStyleAttributes;
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
		public PaletteEntry LeftLineColor
		{
			get
			{
				return _leftLineColor;
			}
		}

		/// <summary>
		/// Right line color.
		/// </summary>
		public PaletteEntry RightLineColor
		{
			get
			{
				return _rightLineColor;
			}
		}

		/// <summary>
		/// Diagonal right top to left bottom.
		/// </summary>
		public bool DiagonalRightTopToLeftBottom
		{
			get
			{
				return _diagonalRightTopToLeftBottom;
			}
		}

		/// <summary>
		/// Diagonal left bottom to top right.
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
		public PaletteEntry TopLineColor
		{
			get
			{
				return _topLineColor;
			}
		}

		/// <summary>
		/// Bottom line color.
		/// </summary>
		public PaletteEntry BottomLineColor
		{
			get
			{
				return _bottomLineColor;
			}
		}

		/// <summary>
		/// Diagonal line color.
		/// </summary>
		public PaletteEntry DiagonalLineColor
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
		public PaletteEntry PatternColor
		{
			get
			{
				return _patternColor;
			}
		}

		/// <summary>
		/// Pattern background.
		/// </summary>
		public PaletteEntry PatternBackground
		{
			get
			{
				return _patternBackground;
			}
		}

	}
}
