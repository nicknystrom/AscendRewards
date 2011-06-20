using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the SHRFMLA (0x00bc) Excel record found in Excel streams.
	/// </summary>
	public class SharedFormulaRecord : FormulaChildRangedRecord
	{
		private ushort _reserved;
		private byte[] _data;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the SHRFMLA record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public SharedFormulaRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.ShrFmla)
			{
				Stream stream = biff.GetDataStream();
				BinaryReader reader = new BinaryReader(stream);

				ReadRangeValues(reader);

				_reserved = reader.ReadUInt16();
				_data = reader.ReadBytes((int)(stream.Length - stream.Position));
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.ShrFmla);
		}

		/// <summary>
		/// Reserved field.
		/// </summary>
		public ushort Reserved
		{
			get
			{
				return _reserved;
			}
		}

		/// <summary>
		/// The data for the SHRFMLA record.
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
