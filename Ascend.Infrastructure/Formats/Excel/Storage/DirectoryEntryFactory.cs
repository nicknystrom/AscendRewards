using System;
using System.Text;
using Net.SourceForge.Koogra.Storage.Sectors;

namespace Net.SourceForge.Koogra.Storage
{
	/// <summary>
	/// Directory entry factory helper class.
	/// </summary>
	public class DirectoryEntryFactory
	{
		/// <summary>
		/// Builds a directory entry and populates its correct type, children and siblings.
		/// </summary>
		/// <param name="entry">The directory sector entry.</param>
		/// <param name="entries">The current entries.</param>
		/// <param name="sectors">The data sectors.</param>
		/// <param name="fat">The FAT.</param>
		/// <returns>The correct DirectoryEntry object.</returns>
		public static DirectoryEntry CreateEntry(DirectorySectorEntry entry, 
			DirectorySectorEntryCollection entries, 
			SectorCollection sectors, 
			Sect[] fat)
		{
			DirectoryEntry newEntry = null;
			switch(entry.Type)
			{
				case Stgty.Storage :
					newEntry = new StorageEntry(entry.Name);
					break;
				case Stgty.Stream :
					newEntry = new StreamEntry(entry.Name, entry.Size, entry.SectStart, sectors, fat);
					break;
				default:
					newEntry = new GenericDirectoryEntry(entry.Name, entry.Type);
					break;
			}

			if(!entry.LeftSibling.IsEof)
				newEntry.LeftSibling = CreateEntry(entries[entry.LeftSibling],
					entries,
					sectors,
					fat);

			if(!entry.RightSibling.IsEof)
				newEntry.RightSibling = CreateEntry(entries[entry.RightSibling],
					entries,
					sectors,
					fat);

			if(!entry.Child.IsEof)
				newEntry.Child = CreateEntry(entries[entry.Child],
					entries,
					sectors,
					fat);

			return newEntry;
		}
	}
}
