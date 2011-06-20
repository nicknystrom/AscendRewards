using System;
using System.Collections.Generic;
using Net.SourceForge.Koogra.Collections;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Container class for Sector objects.
	/// </summary>
	public class SectorCollection : SimpleCollection<Sector>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SectorCollection()
		{
		}

		/// <summary>
		/// Initial capacity constructor.
		/// </summary>
		/// <param name="initialCapacity">The initial capacity of the collection.</param>
		public SectorCollection(int initialCapacity)
		{
			InnerList.Capacity = initialCapacity;
		}

		/// <summary>
		/// Indexer of the collection.
		/// </summary>
		public Sector this[Sect index]
		{
			get
			{
				return InnerList[index.ToInt()];
			}
			set
			{
				InnerList[index.ToInt()] = value;
			}
		}
	}
}
