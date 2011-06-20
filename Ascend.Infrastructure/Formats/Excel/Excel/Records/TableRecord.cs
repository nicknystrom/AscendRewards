using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the TABLE (0x0036) Excel record found in Excel streams.
	/// </summary>
	public class TableRecord : FormulaChildRangedRecord
	{
		private ushort _options;
		private ushort _inputRowIdx;
		private ushort _inputColIdx;
		private ushort _inputRowIdxForColInp;
		private ushort _inputColIdxForColInp;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the TABLE record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public TableRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Table)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				ReadRangeValues(reader);

				_options = reader.ReadUInt16();
				_inputRowIdx = reader.ReadUInt16();
				_inputColIdx = reader.ReadUInt16();
				_inputRowIdxForColInp = reader.ReadUInt16();
				_inputColIdxForColInp = reader.ReadUInt16();
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Table);
		}

		/// <summary>
		/// Options.
		/// </summary>
		public ushort Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// InputRowIdx.
		/// </summary>
		public ushort InputRowIdx
		{
			get
			{
				return _inputRowIdx;
			}
		}

		/// <summary>
		/// InputColIdx.
		/// </summary>
		public ushort InputColIdx
		{
			get
			{
				return _inputColIdx;
			}
		}

		/// <summary>
		/// InputRowIdxForColInp.
		/// </summary>
		public ushort InputRowIdxForColInp
		{
			get
			{
				return _inputRowIdxForColInp;
			}
		}

		/// <summary>
		/// InputColIdxForColInp.
		/// </summary>
		public ushort InputColIdxForColInp
		{
			get
			{
				return _inputColIdxForColInp;
			}
		}

	}
}
