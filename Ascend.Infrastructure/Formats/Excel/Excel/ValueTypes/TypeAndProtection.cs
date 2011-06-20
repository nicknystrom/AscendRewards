using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Type and protection options.
	/// </summary>
	public struct TypeAndProtection 
	{
		private bool _locked;
		private bool _hidden;
		private bool _isStyle;
		private bool _123;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="val">The raw type and protection options.</param>
		public TypeAndProtection(int val) : this((byte)(val & 0x000F))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="val">The raw type and protection options.</param>
		public TypeAndProtection(byte val)
		{
			_locked = (val & 0x01) != 0;
			_hidden = (val & 0x02) != 0;
			_isStyle = (val & 0x04) != 0;
			_123 = (val & 0x08) != 0;
		}

		/// <summary>
		/// Locked.
		/// </summary>
		public bool Locked
		{
			get
			{
				return _locked;
			}
		}

		/// <summary>
		/// Hidden.
		/// </summary>
		public bool Hidden
		{
			get
			{
				return _hidden;
			}
		}

		/// <summary>
		/// Is style.
		/// </summary>
		public bool IsStyle
		{
			get
			{
				return _isStyle;
			}
		}

		/// <summary>
		/// Is cell.
		/// </summary>
		public bool IsCell
		{
			get
			{
				return !_isStyle;
			}
		}

		/// <summary>
		/// Lotus 123.
		/// </summary>
		public bool Lotus123
		{
			get
			{
				return _123;
			}
		}
	}
}
