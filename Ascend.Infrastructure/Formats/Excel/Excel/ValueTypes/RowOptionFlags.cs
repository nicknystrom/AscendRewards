using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Row options.
	/// </summary>
	public struct RowOptionFlags
	{
		private ushort _options;

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="options"></param>
		public RowOptionFlags(ushort options)
		{
			_options = options;
		}

		/// <summary>
		/// Determines the outline level.
		/// </summary>
		public int OutlineLevel
		{
			get
			{
				return _options & 0x0007;
			}
		}

		/// <summary>
		/// Determines if the row is collapsed.
		/// </summary>
		public bool Collapsed
		{
			get
			{
				return (_options & 0x0010) == 1;
			}
		}

		/// <summary>
		/// Determines if the row height is zero.
		/// </summary>
		public bool ZeroHeight
		{
			get
			{
				return (_options & 0x0020) == 1;
			}
		}

		/// <summary>
		/// Detemines if the row is unsynced.
		/// </summary>
		public bool Unsynced
		{
			get
			{
				return (_options & 0x0040) == 1;
			}
		}

		/// <summary>
		/// Determines if a row is formatted.
		/// </summary>
		/// <returns></returns>
		public bool Formatted
		{
			get
			{
				return (_options & 0x0080) == 1;
			}
		}
	}

}
