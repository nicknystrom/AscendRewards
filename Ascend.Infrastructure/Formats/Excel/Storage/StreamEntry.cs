using System;
using System.Diagnostics;
using System.IO;
using Net.SourceForge.Koogra.Storage.Sectors;

namespace Net.SourceForge.Koogra.Storage
{
	/// <summary>
	/// StreamEntry.
	/// </summary>
	public class StreamEntry : DirectoryEntry
	{
		private byte[] _data;
		private long _length;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The entry Name.</param>
		/// <param name="length">The stream length.</param>
		/// <param name="dataOffset">Data offset.</param>
		/// <param name="sectors">The sectors.</param>
		/// <param name="fat">The FAT.</param>
		public StreamEntry(string name, long length, Sect dataOffset, SectorCollection sectors, Sect[] fat) : base(name)
		{
			_length = length;
			_data = new byte[length];

			if(!dataOffset.IsEndOfChain)
			{
				int left = (int)length;
				MemoryStream stream = new MemoryStream(_data);
				Sect sect = dataOffset;

				do
				{
					try
					{
						StorageSector sector = (StorageSector)sectors[sect];

						int toWrite = Math.Min(sector.Data.Length, left);
						Debug.Assert(toWrite <= 512);
						stream.Write(sector.Data, 0, toWrite);
						left -= toWrite;

						sect = fat[sect.ToInt()];
					}
					catch(Exception err)
					{
						Debug.WriteLine("Stream name is " + name);
						Debug.WriteLine(err.Message);
						Debug.WriteLine(err.StackTrace);

						return;
					}
				} while(!sect.IsEndOfChain);

				Debug.Assert(left == 0);
				Debug.Assert(stream.Length == stream.Position);
			}
		}

		/// <summary>
		/// The data.
		/// </summary>
		public byte[] Data
		{
			get
			{
				return _data;
			}
		}

		/// <summary>
		/// Data length.
		/// </summary>
		public long Length
		{
			get
			{
				return _length;
			}
		}
	}
}
