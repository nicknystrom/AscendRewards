using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Net.SourceForge.Koogra.Text;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Directory sector entry.
	/// </summary>
	public class DirectorySectorEntry
	{
		private string _name;
		private ushort _nameLength;
		private Stgty _type;
		private DeColor _color;
		private Sid _leftSibling;
		private Sid _rightSibling;
		private Sid _child;
		private Guid _clsId;
		private uint _userFlags;
		private ulong _createTimeStamp;
		private ulong _modifyTimeStamp;
		private Sect _sectStart;
		private uint _size;
		private ushort _propType;
		/* private ushort _padding; */

		/// <summary>
		/// Stream constructor.
		/// </summary>
		/// <param name="stream">The stream that contains the DirectorySectorEntry data.</param>
		public DirectorySectorEntry(Stream stream)
		{
			Debug.Assert(stream.Length >= Constants.DIR_ENTRY_SIZE);
			BinaryReader reader = new BinaryReader(stream);

			_name = Reader.ReadSimpleUnicodeString(reader, 64);
			_nameLength = reader.ReadUInt16();
			if(_nameLength > 0)
			{
				_nameLength /= 2;
				--_nameLength;
				_name = _name.Substring(0, _nameLength);
			}
			else
				_name = "";

			_type = (Stgty)reader.ReadByte();
			_color = (DeColor)reader.ReadByte();
			_leftSibling = new Sid(reader.ReadUInt32());
			_rightSibling = new Sid(reader.ReadUInt32());
			_child = new Sid(reader.ReadUInt32());
			_clsId = new Guid(reader.ReadBytes(16));
			_userFlags = reader.ReadUInt32();
			_createTimeStamp = reader.ReadUInt64();
			_modifyTimeStamp = reader.ReadUInt64();
			_sectStart = new Sect(reader.ReadUInt32());
			_size = reader.ReadUInt32();
			_propType = reader.ReadUInt16();
			/* _padding = */ reader.ReadUInt16();
		}

		/// <summary>
		/// Entry name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Name length.
		/// </summary>
		public ushort NameLength
		{
			get
			{
				return _nameLength;
			}
		}

		/// <summary>
		/// Type.
		/// </summary>
		public Stgty Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Color.
		/// </summary>
		public DeColor Color
		{
			get
			{
				return _color;
			}
		}

		/// <summary>
		/// Pointer to left sibling.
		/// </summary>
		public Sid LeftSibling
		{
			get
			{
				return _leftSibling;
			}
		}

		/// <summary>
		/// Pointer to right sibling.
		/// </summary>
		public Sid RightSibling
		{
			get
			{
				return _rightSibling;
			}
		}

		/// <summary>
		/// Pointer to child.
		/// </summary>
		public Sid Child
		{
			get
			{
				return _child;
			}
		}

		/// <summary>
		/// Class id.
		/// </summary>
		public Guid ClsId
		{
			get
			{
				return _clsId;
			}
		}

		/// <summary>
		/// User flags.
		/// </summary>
		public uint UserFlags
		{
			get
			{
				return _userFlags;
			}
		}

		/// <summary>
		/// Creation time stamp.
		/// </summary>
		public ulong CreateTimeStamp
		{
			get
			{
				return _createTimeStamp;
			}
		}

		/// <summary>
		/// Modification time stamp.
		/// </summary>
		public ulong ModifyTimeStamp
		{
			get
			{
				return _modifyTimeStamp;
			}
		}

		/// <summary>
		/// Stream start.
		/// </summary>
		public Sect SectStart
		{
			get
			{
				return _sectStart;
			}
		}

		/// <summary>
		/// Stream size.
		/// </summary>
		public uint Size
		{
			get
			{
				return _size;
			}
		}

		/// <summary>
		/// Property type.
		/// </summary>
		public ushort PropType
		{
			get
			{
				return _propType;
			}
		}

	}
}
