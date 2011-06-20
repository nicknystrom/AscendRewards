using System;
using Net.SourceForge.Koogra.Storage.Sectors;

namespace Net.SourceForge.Koogra.Storage
{
	/// <summary>
	/// GenericDirectoryEntry. Class for entries whose contents are never interpreted.
	/// </summary>
	public class GenericDirectoryEntry : DirectoryEntry
	{
		private Stgty _type;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the entry.</param>
		/// <param name="type">The entry type.</param>
		public GenericDirectoryEntry(string name, Stgty type) : base(name)
		{
			_type = type;
		}

		/// <summary>
		/// The entry type.
		/// </summary>
		public Stgty Type
		{
			get
			{
				return _type;
			}
		}
	}
}
