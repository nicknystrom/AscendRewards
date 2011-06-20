using System;
using System.IO;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the LABEL (0x0204) Excel record  found in Excel streams.
	/// </summary>
	public class LabelRecord : RowColXfCellRecord
	{
		private string _value;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the LABEL record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public LabelRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Label)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				ReadRowColXf(reader);

				_value = Reader.ReadSimpleUnicodeString(reader);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Label);
		}

		/// <summary>
		/// Returns the value of the LABEL record.
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
