using System;
using System.Diagnostics;
using System.IO;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the FORMAT (0x041e) Excel record  found in Excel streams.
	/// </summary>
	public class FormatRecord : Biff
	{
		private ushort _index;
		private string _format;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the FORMAT record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public FormatRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Format)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				_index = reader.ReadUInt16();
				_format = Reader.ReadComplexString(reader);
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Format);
		}

		/// <summary>
		/// Index of the FORMAT record in the format table.
		/// </summary>
		public ushort Index
		{
			get
			{
				return _index;
			}
		}

		/// <summary>
		/// The format string for the FORMAT record.
		/// </summary>
		public string Format
		{
			get
			{
				return _format;
			}
		}

	}
}
