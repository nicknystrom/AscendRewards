using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// The height of the row.
	/// </summary>
	public struct RowHeight
	{
		private ushort _height;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="height">The row height.</param>
		public RowHeight(ushort height)
		{
			_height = height;
		}

		/// <summary>
		/// Method for converting the height to twips.
		/// </summary>
		/// <returns></returns>
		public ushort HeightInTwips()
		{
			return (ushort)(_height & 0x7FFF);
		}

		/// <summary>
		/// Determines if the current height is the default height.
		/// </summary>
		/// <returns>Returns true if the current height is the default height.</returns>
		public bool DefaultHeight()
		{
			return (_height & 0x8000) != 0;
		}
	}
}
