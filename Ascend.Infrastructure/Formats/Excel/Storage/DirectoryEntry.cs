using System;

namespace Net.SourceForge.Koogra.Storage
{
	/// <summary>
	/// Base class for Directory entries.
	/// </summary>
	public abstract class DirectoryEntry
	{
		private DirectoryEntry _leftSibling = null;
		private DirectoryEntry _rightSibling;
		private DirectoryEntry _child;
		private string _name;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the directory entry.</param>
		public DirectoryEntry(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Locates a directory entry by name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DirectoryEntry Find(string name)
		{
			DirectoryEntryNameComparer comparer = new DirectoryEntryNameComparer();
			DirectoryEntry child = _child;
			while(child != null)
			{
				int c = comparer.Compare(name, child.Name);
				if(c == 0)
					break;
				else if(c < 0)
					child = child.LeftSibling;
				else
					child = child.RightSibling;
			}

			return child;
		}

		/// <summary>
		/// The name of the directory entry.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// The left sibling.
		/// </summary>
		public virtual DirectoryEntry LeftSibling
		{
			get
			{
				return _leftSibling;
			}
			set
			{
				_leftSibling = value;
			}
		}

		/// <summary>
		/// The right sibling.
		/// </summary>
		public virtual DirectoryEntry RightSibling
		{
			get
			{
				return _rightSibling;
			}
			set
			{
				_rightSibling = value;
			}
		}

		/// <summary>
		/// The child.
		/// </summary>
		public virtual DirectoryEntry Child
		{
			get
			{
				return _child;
			}
			set
			{
				_child = value;
			}
		}
	}
}
