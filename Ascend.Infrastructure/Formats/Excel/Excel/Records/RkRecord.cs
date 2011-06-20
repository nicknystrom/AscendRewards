using System;
using System.IO;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the RK (0x027e) Excel record found in Excel streams.
	/// </summary>
	public class RkRecord : RowColXfCellRecord
	{
		private RkValue _value;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the RK record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public RkRecord(GenericBiff biff) 
		{
			if(biff.Id == (ushort)RecordType.Rk)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				ReadRowColXf(reader);
				_value = new RkValue(reader.ReadInt32());
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Rk);
		}

		/// <summary>
		/// The value of the RK record.
		/// </summary>
		/// <value>Returns a double value.</value>
		public override object Value
		{
			get
			{
				return _value.Value;
			}
		}

	}
}
