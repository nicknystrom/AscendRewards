using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Directory sector.
	/// </summary>
	public class DirectorySector : Sector
	{
		private DirectorySectorEntry[] _entries = new DirectorySectorEntry[4];

		/// <summary>
		/// Stream constructor.
		/// </summary>
		/// <param name="stream">The stream that contains the DirectorySector data.</param>
		public DirectorySector(Stream stream)
		{
			Debug.Assert(stream.Length >= Constants.SECTOR_SIZE);

			for(int i = 0; i < _entries.Length; ++i)
				_entries[i] = new DirectorySectorEntry(stream);
		}

		/// <summary>
		/// The array of directory sector entries.
		/// </summary>
		public DirectorySectorEntry[] Entries
		{
			get
			{
				return _entries;
			}
		}
	}
}
