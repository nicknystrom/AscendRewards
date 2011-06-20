using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the MULBLANK (0x00be) Excel record found in Excel streams.
	/// </summary>
	public class MulBlankRecord : MultipleColCellRecord
	{
		private ushort[] _xfIndex;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the MULBLANK record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public MulBlankRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.MulBlank)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				reader = ReadRowColInfo(reader);
				_xfIndex = new ushort[reader.BaseStream.Length / 2];
				for(int n = 0; n < _xfIndex.Length; ++n)
					_xfIndex[n] = reader.ReadUInt16();

			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.MulBlank);
		}

		/// <summary>
		/// The array of formatting (XF) records for each column in the range.
		/// </summary>
		public ushort[] XfIndex
		{
			get
			{
				return _xfIndex;
			}
		}

		/// <summary>
		/// Method for getting the array of values in the range of cells.
		/// </summary>
		public override object Value
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Method for getting the value of a particular column.
		/// </summary>
		/// <param name="col">The column index.</param>
		/// <returns>Always returns null.</returns>
		/// <remarks>Currently doesn't check if the col parameter is within the acceptable range of values.</remarks>
		public override object GetValue(ushort col)
		{
			return null;
		}

	}
}
