using System;
using System.Diagnostics;
using System.IO;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Header sector.
	/// </summary>
	public class HeaderSector : Sector
	{
		/// <summary>
		/// The magic number that starts any compound file.
		/// </summary>
		public const ulong MAGIC_NUMBER = 0xe11ab1a1e011cfd0; //0xD0CF11E0A1B11AE1;

		private Guid _clsId;
		private ushort _minorVer;
		private ushort _dllVer;
		private ushort _byteOrder;
		private ushort _sectShift;
		private ushort _miniSectShift;
		private ushort _reserved;
		private uint _reserved1;
		private uint _reserved2;
		private uint _sectFatCount;
		private Sect _sectDirStart;
		private uint _signature;
		private uint _miniSectorCutoff;
		private Sect _sectMiniFatStart;
		private uint _sectMiniFatCount;
		private Sect _sectDifStart;
		private uint _sectDifCount;
		private Sect[] _sectFat = new Sect[109];
			
		/// <summary>
		/// Stream constructor.
		/// </summary>
		/// <param name="stream">The stream that contains the HeaderSector data.</param>
		/// <exception cref="Exception">Throws an exception if it encounters any invalid data in the stream.</exception>
		public HeaderSector(Stream stream) 
		{
			Debug.Assert(stream.Length >= Constants.SECTOR_SIZE);
			BinaryReader reader = new BinaryReader(stream);

			ulong magicNumber = reader.ReadUInt64();
			if(magicNumber != MAGIC_NUMBER)
				throw new Exception("Invalid header magic number.");

			_clsId = new Guid(reader.ReadBytes(16));
			_minorVer = reader.ReadUInt16();
			_dllVer = reader.ReadUInt16();
			_byteOrder = reader.ReadUInt16();
			_sectShift = reader.ReadUInt16();
			_miniSectShift = reader.ReadUInt16();
			_reserved = reader.ReadUInt16();
			_reserved1 = reader.ReadUInt32();
			_reserved2 = reader.ReadUInt32();
			_sectFatCount = reader.ReadUInt32();
			_sectDirStart = new Sect(reader.ReadUInt32());
			_signature = reader.ReadUInt32();
			_miniSectorCutoff = reader.ReadUInt32();
			_sectMiniFatStart = new Sect(reader.ReadUInt32());
			_sectMiniFatCount = reader.ReadUInt32();
			_sectDifStart = new Sect(reader.ReadUInt32());
			_sectDifCount = reader.ReadUInt32();

			for(int i = 0; i < _sectFat.Length; ++i)
				_sectFat[i] = new Sect(reader.ReadUInt32());
		}

		/// <summary>
		/// Class Id.
		/// </summary>
		public Guid ClsId
		{
			get
			{
				return _clsId;
			}
		}

		/// <summary>
		/// Minor Version.
		/// </summary>
		public ushort MinorVer
		{
			get
			{
				return _minorVer;
			}
		}

		/// <summary>
		/// Dll Version.
		/// </summary>
		public ushort DllVer
		{
			get
			{
				return _dllVer;
			}
		}

		/// <summary>
		/// Byte Order.
		/// </summary>
		public ushort ByteOrder
		{
			get
			{
				return _byteOrder;
			}
		}

		/// <summary>
		/// Sector size in powers of 2.
		/// </summary>
		public ushort SectShift
		{
			get
			{
				return _sectShift;
			}
		}

		/// <summary>
		/// Mini-sector size in powers of 2.
		/// </summary>
		public ushort MiniSectShift
		{
			get
			{
				return _miniSectShift;
			}
		}

		/// <summary>
		/// Reserved.
		/// </summary>
		public ushort Reserved
		{
			get
			{
				return _reserved;
			}
		}

		/// <summary>
		/// Reserved.
		/// </summary>
		public uint Reserved1
		{
			get
			{
				return _reserved1;
			}
		}

		/// <summary>
		/// Reserved.
		/// </summary>
		public uint Reserved2
		{
			get
			{
				return _reserved2;
			}
		}

		/// <summary>
		/// FAT sector chain count.
		/// </summary>
		public uint SectFatCount
		{
			get
			{
				return _sectFatCount;
			}
		}

		/// <summary>
		/// Directory sector start.
		/// </summary>
		public Sect SectDirStart
		{
			get
			{
				return _sectDirStart;
			}
		}

		/// <summary>
		/// Transaction signature.
		/// </summary>
		public uint Signature
		{
			get
			{
				return _signature;
			}
		}

		/// <summary>
		/// Maximum ministream size.
		/// </summary>
		public uint MiniSectorCutoff
		{
			get
			{
				return _miniSectorCutoff;
			}
		}

		/// <summary>
		/// Mini sector start.
		/// </summary>
		public Sect SectMiniFatStart
		{
			get
			{
				return _sectMiniFatStart;
			}
		}

		/// <summary>
		/// Mini FAT sector chain count.
		/// </summary>
		public uint SectMiniFatCount
		{
			get
			{
				return _sectMiniFatCount;
			}
		}

		/// <summary>
		/// Double Indirect FAT Sector start.
		/// </summary>
		public Sect SectDifStart
		{
			get
			{
				return _sectDifStart;
			}
		}

		/// <summary>
		/// Double Indirect FAT count.
		/// </summary>
		public uint SectDifCount
		{
			get
			{
				return _sectDifCount;
			}
		}

		/// <summary>
		/// First 109 Sects of the FAT
		/// </summary>
		public Sect[] SectFat
		{
			get
			{
				return _sectFat;
			}
		}
	}
}
