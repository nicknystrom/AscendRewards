using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the NUMBER (0x0203) Excel record found in Excel streams.
	/// </summary>
	public class NumberRecord : RowColXfCellRecord
	{
		private double _value;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the NUMBER record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public NumberRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Number)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				ReadRowColXf(reader);
				_value = reader.ReadDouble();
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Number);
		}

		/// <summary>
		/// Returns the value of the NUMBER record.
		/// </summary>
		/// <value>Returns a double value.</value>
		public override object Value
		{
			get
			{
				return _value;
			}
		}
	}
}
