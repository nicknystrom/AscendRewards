using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Base class for Excel Cell type records that span only a specific column.
	/// </summary>
	public abstract class SingleColCellRecord : CellRecord
	{
		private ushort _col;

		/// <summary>
		/// Method for reading the row and column to which the cell belongs to.
		/// </summary>
		/// <param name="reader">The reader for the record.</param>
		protected void ReadRowCol(BinaryReader reader)
		{
			ReadRow(reader);
			_col = reader.ReadUInt16();
		}

		/// <summary>
		/// The column that the cell pertains to.
		/// </summary>
		public ushort Col
		{
			get
			{
				return _col;
			}
		}

	}
}
