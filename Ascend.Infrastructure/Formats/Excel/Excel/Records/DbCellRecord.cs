using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts a DBCELL (0x00d7) Excel record found in Excel streams.
	/// </summary>
	public class DbCellRecord : Biff
	{
		private uint _rowOffset;
		private ushort[] _streamOffsets;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the DBCELL record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public DbCellRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.DbCell)
			{
				Stream stream = biff.GetDataStream();
				BinaryReader reader = new BinaryReader(stream);
				
				_rowOffset = reader.ReadUInt32();

				int noffsets = (int)(stream.Length - stream.Position) / 2;
				_streamOffsets = new ushort[noffsets];
				noffsets = 0;
				while(stream.Position < stream.Length)
					_streamOffsets[noffsets++] = reader.ReadUInt16();

				Debug.Assert(stream.Position == stream.Length);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.DbCell);
		}

		/// <summary>
		/// The offset of the first row in the ROW cluster that the DBCELL record indexes.
		/// </summary>
		public uint RowOffset
		{
			get
			{
				return _rowOffset;
			}
		}

		/// <summary>
		/// Row offsets in the ROW cluster that the DBCELL record indexes.
		/// </summary>
		public ushort[] StreamOffsets
		{
			get
			{
				return _streamOffsets;
			}
		}

	}
}
