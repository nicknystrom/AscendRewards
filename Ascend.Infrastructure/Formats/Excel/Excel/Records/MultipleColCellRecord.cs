using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Base class for cell records that span multiple cells.
	/// </summary>
	public abstract class MultipleColCellRecord : CellRecord
	{
		private ushort _firstCol;
		private ushort _lastCol;

		/// <summary>
		/// Method for reading the the first and last column information of the cell.
		/// </summary>
		/// <param name="reader">The reader for the record.</param>
		/// <returns>Returns a new reader that points to the data between the first and last column data.</returns>
		protected BinaryReader ReadRowColInfo(BinaryReader reader)
		{
			ReadRow(reader);
			_firstCol = reader.ReadUInt16();

			byte[] inBetween = reader.ReadBytes((int)(reader.BaseStream.Length - 6));

			_lastCol = reader.ReadUInt16();

			return new BinaryReader(new MemoryStream(inBetween));
		}

		/// <summary>
		/// The first column in the range the record pertains to.
		/// </summary>
		public ushort FirstCol
		{
			get
			{
				return _firstCol;
			}
		}

		/// <summary>
		/// The last column in the range the record pertains to.
		/// </summary>
		public ushort LastCol
		{
			get
			{
				return _lastCol;
			}
		}

		/// <summary>
		/// Method for getting the value of a particular column in the range pertained to.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public abstract object GetValue(ushort col);
	}
}
