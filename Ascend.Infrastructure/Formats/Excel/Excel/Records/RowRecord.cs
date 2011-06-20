using System;
using System.Diagnostics;
using System.IO;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the ROW (0x0208) Excel record found in Excel streams.
	/// </summary>
	public class RowRecord : Biff
	{
		private ushort _rowNumber;
		private ushort _firstCol;
		private ushort _lastCol;
		private RowHeight _rowHeight;
		private ushort _optimizer;
		private RowOptionFlags _options;
		private ushort _xf;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the ROW record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public RowRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Row)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());
				_rowNumber = reader.ReadUInt16();
				_firstCol = reader.ReadUInt16();
				_lastCol = reader.ReadUInt16();
				_rowHeight = new RowHeight(reader.ReadUInt16());
				_optimizer = reader.ReadUInt16();
				_options = new RowOptionFlags(reader.ReadUInt16());
				_xf = reader.ReadUInt16();
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Row);
		}

		/// <summary>
		/// The row number of the ROW record in the worksheet.
		/// </summary>
		public ushort RowNumber
		{
			get
			{
				return _rowNumber;
			}
		}

		/// <summary>
		/// The first column that can be found in the ROW record.
		/// </summary>
		public ushort FirstCol
		{
			get
			{
				return _firstCol;
			}
		}

		/// <summary>
		/// The last column that can be found in the ROW record.
		/// </summary>
		public ushort LastCol
		{
			get
			{
				return _lastCol;
			}
		}

		/// <summary>
		/// The height of the row in twips.
		/// </summary>
		public RowHeight RowHeight
		{
			get
			{
				return _rowHeight;
			}
		}

		/// <summary>
		/// Microsoft specific row optimizer.
		/// </summary>
		public ushort Optimizer
		{
			get
			{
				return _optimizer;
			}
		}

		/// <summary>
		/// Options.
		/// </summary>
		public RowOptionFlags Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// The formatting (XF) information for the row.
		/// </summary>
		public ushort Xf
		{
			get
			{
				return _xf;
			}
		}

	}
}
