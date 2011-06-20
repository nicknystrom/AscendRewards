using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the INDEX (0x020B) Excel record  found in Excel streams.
	/// </summary>
	public class IndexRecord : Biff
	{
		private uint _firstRow;
		private uint _lastRow;
		private uint[] _rows;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the INDEX record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public IndexRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Index)
			{
				Stream stream = biff.GetDataStream();
				BinaryReader reader = new BinaryReader(stream);

				/* uint reserved = */ reader.ReadUInt32();
				_firstRow = reader.ReadUInt32();
				_lastRow = reader.ReadUInt32();
				/* reserved = */ reader.ReadUInt32();

				int nrows = (int)(stream.Length - stream.Position) / 4; 
				_rows = new uint[nrows];
				nrows = 0;
				while(stream.Position < stream.Length)
					_rows[nrows++] = reader.ReadUInt32();

				Debug.Assert(stream.Position == stream.Length);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Index);
		}

		/// <summary>
		/// The first row that can be found in the worksheet.
		/// </summary>
		public uint FirstRow
		{
			get
			{
				return _firstRow;
			}
		}

		/// <summary>
		/// The last row that can be found in the worksheet.
		/// </summary>
		public uint LastRow
		{
			get
			{
				return _lastRow;
			}
		}

		/// <summary>
		/// The collection of ROW cluster offsets to the DBCELL records for each ROW cluster.
		/// </summary>
		public uint[] Rows
		{
			get
			{
				return _rows;
			}
		}
	}
}
