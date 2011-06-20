using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Mini FAT Sector.
	/// </summary>
	public class MiniFatSector : Sector
	{
		private Sect[] _sect = new Sect[Constants.MAX_SECT];

		/// <summary>
		/// Stream constructor.
		/// </summary>
		/// <param name="stream">The stream that contains the MiniFatSector data.</param>
		public MiniFatSector(Stream stream)
		{
			Debug.Assert(stream.Length >= Constants.SECTOR_SIZE);
			BinaryReader reader = new BinaryReader(stream);

			for(int i = 0; i < _sect.Length; ++i)
				_sect[i] = new Sect(reader.ReadUInt32());
		}

		/// <summary>
		/// The chain array of mini fat sectors.
		/// </summary>
		public Sect[] SectFat
		{
			get
			{
				return _sect;
			}
		}
	}
}
