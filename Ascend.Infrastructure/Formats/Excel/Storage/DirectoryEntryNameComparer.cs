using System;
using System.Collections.Generic;

namespace Net.SourceForge.Koogra.Storage
{
	/// <summary>
	/// Helper class for comparing directory entry names.
	/// </summary>
	public class DirectoryEntryNameComparer : IComparer<string>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DirectoryEntryNameComparer()
		{
		}

		/// <summary>
		/// Compares directory names.
		/// </summary>
		/// <param name="l"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public int Compare(string l, string r)
		{
			if(l.Length < r.Length)
				return -1;
			else if(l.Length > r.Length)
				return 1;
			else
				return string.Compare(l, r, true);
		}
    }
}
