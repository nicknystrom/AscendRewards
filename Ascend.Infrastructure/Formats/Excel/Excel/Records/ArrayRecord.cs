using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the ARRAY (0x0221) Excel record found in Excel streams.
	/// </summary>
	public class ArrayRecord : FormulaChildRangedRecord
	{
		private ushort _options;
		private uint _reserved;
		private byte[] _data;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the ARRAY record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public ArrayRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Array)
			{
				Stream stream = biff.GetDataStream();
				BinaryReader reader = new BinaryReader(stream);
				
				ReadRangeValues(reader);

				_options = reader.ReadUInt16();
				_reserved = reader.ReadUInt32();
				_data = reader.ReadBytes((int)(stream.Length - stream.Position));
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Array);
		}

		/// <summary>
		/// The options for the ARRAY record.
		/// </summary>
		public ushort Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// The reserved field for the ARRAY record.
		/// </summary>
		public uint Reserved
		{
			get
			{
				return _reserved;
			}
		}

		/// <summary>
		/// The raw byte data for the ARRAY record.
		/// </summary>
		public byte[] Data
		{
			get
			{
				return _data;
			}
		}
	}
}
