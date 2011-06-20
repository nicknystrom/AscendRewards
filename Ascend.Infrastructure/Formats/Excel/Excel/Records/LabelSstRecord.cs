using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the LABELSST (0x00fd) Excel record  found in Excel streams.
	/// </summary>
	public class LabelSstRecord : RowColXfCellRecord
	{
		private uint _sstIndex;
		private string _value;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the LABELSST record.</param>
		/// <param name="sst">A reference to the shared string table for the workbook.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public LabelSstRecord(GenericBiff biff, SstRecord sst)
		{
			if(biff.Id == (ushort)RecordType.LabelSst)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				ReadRowColXf(reader);
				_sstIndex = reader.ReadUInt32();		
				_value = sst.Strings[_sstIndex];
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.LabelSst);
		}

		/// <summary>
		/// Index in the sst of the LABELSST record's value.
		/// </summary>
		public uint SstIndex
		{
			get
			{
				return _sstIndex;
			}
		}

		/// <summary>
		/// The value of the LABELSST record.
		/// </summary>
		/// <value>
		/// Returns a string value.
		/// </value>
		public override object Value
		{
			get
			{
				return _value;
			}
		}

	}
}
