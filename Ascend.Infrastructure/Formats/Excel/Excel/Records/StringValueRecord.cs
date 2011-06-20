using System;
using System.IO;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the STRING (0x0207) Excel record found in Excel streams.
	/// </summary>
	public class StringValueRecord : FormulaChildRecord
	{
		private string _value;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the SST record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public StringValueRecord(GenericBiff biff) 
		{
			if(biff.Id == (ushort)RecordType.String)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());
				_value = Reader.ReadComplexString(reader);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.String);
		}

		/// <summary>
		/// Returns the value of the STRING record.
		/// </summary>
		/// <value>Returns a string value.</value>
		public string Value
		{
			get
			{
				return _value;
			}
		}

	}
}
