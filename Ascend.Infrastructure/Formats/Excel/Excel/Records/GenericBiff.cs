using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts a Generic Microsoft (MS) Binary Interchange File Format (BIFF) record.
	/// </summary>
	public class GenericBiff : Biff
	{
		/// <summary>
		/// The minimum size of any BIFF records.
		/// </summary>
		public const int MinimumSize = 4;

		private ushort _id;
		private ushort _length;
		private byte[] _data;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="stream">The stream to read the record data from.</param>
		public GenericBiff(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);
			_id = reader.ReadUInt16();
			_length = reader.ReadUInt16();
			_data = reader.ReadBytes(_length);
		}

		/// <summary>
		/// The Id of the BIFF record.
		/// </summary>
		public ushort Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// The length of data in the BIFF record.
		/// </summary>
		public ushort Length
		{
			get
			{
				return _length;
			}
		}

		/// <summary>
		/// The data in the BIFF record.
		/// </summary>
		public byte[] Data
		{
			get
			{
				return _data;
			}
		}

		/// <summary>
		/// Method for getting a stream for the data in the BIFF record.
		/// </summary>
		/// <returns>Returns a stream for the data in the BIFF record.</returns>
		public Stream GetDataStream()
		{
			return new MemoryStream(_data);
		}

	}
}
