using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the BLANK (0x0201) Excel record found in Excel streams.
	/// </summary>
	public class BlankRecord : RowColXfCellRecord
	{
		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the BLANK record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException if thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public BlankRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Blank)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());
				
				ReadRowColXf(reader);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Blank);
		}

		/// <summary>
		/// Method for retrieving the value of the Record.
		/// </summary>
		/// <value>
		/// Currently returns null since BLANK records have no value.
		/// </value>
		public override object Value
		{
			get
			{
				return null;
			}
		}

	}
}
