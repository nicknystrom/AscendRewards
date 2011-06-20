using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts a CONTINUE (0x003c) Excel record found in Excel streams.
	/// </summary>
	public class ContinueRecord : Biff
	{
		private byte[] _data;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the CONTINUE record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public ContinueRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Continue)
			{
				_data = biff.Data;
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Continue);
		}

		/// <summary>
		/// The data in the CONTINUE record.
		/// </summary>
		public byte[] Data
		{
			get
			{
				return _data;
			}
		}

		/// <summary>
		/// Method for getting a stream for the data in the CONTINUE record.
		/// </summary>
		/// <returns>Returns a Stream for the data in the CONTINUE record.</returns>
		public Stream GetDataStream()
		{
			return new MemoryStream(_data);
		}
	}
}
