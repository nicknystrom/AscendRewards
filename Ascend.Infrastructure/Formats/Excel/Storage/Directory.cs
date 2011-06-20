using System;
using Net.SourceForge.Koogra.Storage.Sectors;

namespace Net.SourceForge.Koogra.Storage
{
	/// <summary>
	/// Directory.
	/// </summary>
	public class Directory
	{
		private DirectoryEntry _root;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entries">The directory sector entries.</param>
		/// <param name="sectors">All the sectors.</param>
		/// <param name="fat">The FAT.</param>
		public Directory(DirectorySectorEntryCollection entries, SectorCollection sectors, Sect[] fat)
		{
			_root = DirectoryEntryFactory.CreateEntry(entries[Sid.ZERO], entries, sectors, fat);			
		}

		/// <summary>
		/// The root directory entry.
		/// </summary>
		public DirectoryEntry Root
		{
			get
			{
				return _root;
			}
		}
	}
}
