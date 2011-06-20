using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Double indirect FAT sector.
	/// </summary>
	public class DifSector : Sector
	{
		private Sect[] _sect = new Sect[127];
		private Sect _nextDif;

		/// <summary>
		/// Stream constructor.
		/// </summary>
		/// <param name="stream">The stream to read the sector contents from.</param>
		public DifSector(Stream stream)
		{
			Debug.Assert(stream.Length >= Constants.SECTOR_SIZE);
			BinaryReader reader = new BinaryReader(stream);

			for(int i = 0; i < _sect.Length; ++i)
				_sect[i] = new Sect(reader.ReadUInt32());

			_nextDif = new Sect(reader.ReadUInt32());
		}

		/// <summary>
		/// The collection of FAT sectors.
		/// </summary>
		public Sect[] SectFat
		{
			get
			{
				return _sect;
			}
		}

		/// <summary>
		/// Pointer to the next DIFSector.
		/// </summary>
		public Sect NextDif
		{
			get
			{
				return _nextDif;
			}
		}
	}
}
