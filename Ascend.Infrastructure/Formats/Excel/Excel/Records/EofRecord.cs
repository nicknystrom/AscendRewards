using System;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts a EOF (0x000a) Excel record found in Excel streams.
	/// </summary>
	public class EofRecord : Biff
	{
		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the EOF record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public EofRecord(GenericBiff biff)
		{
			if(biff.Id != (ushort)RecordType.Eof)
				throw new InvalidRecordIdException(biff.Id, RecordType.Eof);
		}
	}
}
