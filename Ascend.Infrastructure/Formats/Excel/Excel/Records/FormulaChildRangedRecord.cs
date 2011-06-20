using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This is the base class for FORMULA (0x006) Excel records whose formula and values pertain to a range of cells.
	/// </summary>
	public abstract class FormulaChildRangedRecord : FormulaChildRecord
	{
		private ushort _firstRowIdx;
		private ushort _lastRowIdx;
		private byte _firstColIdx;
		private byte _lastColIdx;

		/// <summary>
		/// The default constructor
		/// </summary>
		public FormulaChildRangedRecord()
		{
		}

		/// <summary>
		/// Method for populating the range values of the record given a reader for the stream.
		/// </summary>
		/// <param name="reader">The reader for the stream.</param>
		protected void ReadRangeValues(BinaryReader reader)
		{
			_firstRowIdx = reader.ReadUInt16();
			_lastRowIdx = reader.ReadUInt16();
			_firstColIdx = reader.ReadByte();
			_lastColIdx = reader.ReadByte();
		}

		/// <summary>
		/// The first row in the range to which the formula applies.
		/// </summary>
		public ushort FirstRowIdx
		{
			get
			{
				return _firstRowIdx;
			}
		}

		/// <summary>
		/// The last row in the range to which the formula applies.
		/// </summary>
		public ushort LastRowIdx
		{
			get
			{
				return _lastRowIdx;
			}
		}

		/// <summary>
		/// The first column in the range to which the formula applies.
		/// </summary>
		public byte FirstColIdx
		{
			get
			{
				return _firstColIdx;
			}
		}

		/// <summary>
		/// The last column in the range to which the formula applies.
		/// </summary>
		public byte LastColIdx
		{
			get
			{
				return _lastColIdx;
			}
		}
	}
}
