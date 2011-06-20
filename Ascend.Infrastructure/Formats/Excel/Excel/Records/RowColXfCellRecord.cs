using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Base class for Excel Cell type records that span only a specific column and have formatting (XF) information.
	/// </summary>
	public abstract class RowColXfCellRecord : SingleColCellRecord
	{
		private ushort _xf;

		/// <summary>
		/// Method for reading the row, column and formatting information for the cell.
		/// </summary>
		/// <param name="reader">The reader for the cell.</param>
		protected void ReadRowColXf(BinaryReader reader)
		{
			ReadRowCol(reader);
			_xf = reader.ReadUInt16();
		}

		/// <summary>
		/// The index to the formatting information for retrieving formatting information from the format table.
		/// </summary>
		public ushort Xf
		{
			get
			{
				return _xf;
			}
		}
	}
}
