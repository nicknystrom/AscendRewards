using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the BOF (0x0809) Excel record found in Excel streams.
	/// </summary>
	public class BofRecord : Biff
	{
		/// <summary>
		/// Sub stream types that determine the data that follows the BOF record.
		/// </summary>
		public enum SubstreamType : ushort
		{
			/// <summary>
			/// Sub stream type for data that apply to the entire workbook.
			/// </summary>
			WorkbookGlobals = 0x0005,
			/// <summary>
			/// Sub stream type for visual basic modules.
			/// </summary>
			VisualBasicModule = 0x0006,
			/// <summary>
			/// Sub stream type for Worksheet streams.
			/// </summary>
			Worksheet = 0x0010,
			/// <summary>
			/// Sub stream type for Chart streams.
			/// </summary>
			Chart = 0x0020,
			/// <summary>
			/// Sub stream type for Excel 4.0 Macros.
			/// </summary>
			Excel4MacroSheet = 0x0040,
			/// <summary>
			/// Sub stream type for Workspaces.
			/// </summary>
			Workspace = 0x0100
		}

		private ushort _version;
		private SubstreamType _type;
		private ushort _buildId;
		private ushort _buildYear;
		private uint _history;
		private uint _lowestBiff;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the BOF record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public BofRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Bof)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());
				_version = reader.ReadUInt16();
				_type = (SubstreamType)reader.ReadUInt16();
				_buildId = reader.ReadUInt16();
				_buildYear = reader.ReadUInt16();
				_history = reader.ReadUInt32();
				_lowestBiff = reader.ReadUInt32();
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Bof);
		}

		/// <summary>
		/// The version of the sub-stream of data that follows this BOF record.
		/// </summary>
		public ushort Version
		{
			get
			{
				return _version;
			}
		}

		/// <summary>
		/// The type of the sub-stream of data that follows this BOF record.
		/// </summary>
		public SubstreamType Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// The build id for the sub-stream of data that follows this BOF record.
		/// </summary>
		public ushort BuildId
		{
			get
			{
				return _buildId;
			}
		}

		/// <summary>
		/// The build year for the sub-stream of data that follows this BOF record.
		/// </summary>
		public ushort BuildYear
		{
			get
			{
				return _buildYear;
			}
		}

		/// <summary>
		/// The history for the sub-stream of data that follows this BOF record.
		/// </summary>
		public uint History
		{
			get
			{
				return _history;
			}
		}

		/// <summary>
		/// The lowest biff version that can read the data found in the sub-stream of data that follows this BOF record.
		/// </summary>
		public uint LowestBiff
		{
			get
			{
				return _lowestBiff;
			}
		}

	}
}
