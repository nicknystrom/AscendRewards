using System;
using System.Collections.Generic;
using Net.SourceForge.Koogra.Collections;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Collection class for DirectorySectorEntry objects.
	/// </summary>
	public class DirectorySectorEntryCollection : SimpleCollection<DirectorySectorEntry>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DirectorySectorEntryCollection()
		{
		}

		/// <summary>
		/// Adds a directory sector entry and returns its Sid
		/// </summary>
		/// <param name="entry">The directory sector entry.</param>
		/// <returns>Returns the Sid of the entry.</returns>
		new public Sid Add(DirectorySectorEntry entry)
		{
            InnerList.Add(entry);
			return new Sid((uint)InnerList.Count - 1);
		}

		/// <summary>
		/// Indexer for retrieving a DirectorySectorEntry
		/// </summary>
		public DirectorySectorEntry this[Sid index]
		{
			get
			{
				return (DirectorySectorEntry)InnerList[index.ToInt()];
			}
			set
			{
				InnerList[index.ToInt()] = value;
			}
		}

	}
}
