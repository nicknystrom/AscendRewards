using System;
using System.IO;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Storage sector.
	/// </summary>
	public class StorageSector : Sector
	{
		private byte[] _data = new byte[Constants.SECTOR_SIZE];

		/// <summary>
		/// Stream constructor.
		/// </summary>
		/// <param name="stream"></param>
		public StorageSector(Stream stream)
		{
			int read = stream.Read(_data, 0, _data.Length);
			if(read < _data.Length)
				throw new IOException("The amount of data read was shorted than expected.");
		}

		/// <summary>
		/// The data in the StorageSector.
		/// </summary>
		public byte[] Data
		{
			get
			{
				return _data;
			}
		}

		/// <summary>
		/// Returns a stream to the data in the StorageSector.
		/// </summary>
		/// <returns></returns>
		public Stream GetStream()
		{
			return new MemoryStream(_data);
		}
	}
}
