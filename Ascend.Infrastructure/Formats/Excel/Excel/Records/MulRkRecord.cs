using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the MULRK (0x00bd) Excel record found in Excel streams.
	/// </summary>
	public class MulRkRecord : MultipleColCellRecord
	{
		private RkRec[] _values;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the MULRK record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public MulRkRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.MulRk)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				reader = ReadRowColInfo(reader);

				_values = new RkRec[reader.BaseStream.Length / 6];
				for(int n = 0; n < _values.Length; ++n)
				{
					ushort xf = reader.ReadUInt16();
					int rk = reader.ReadInt32();
					_values[n] = new RkRec(xf, rk);
				}
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.MulRk);
		}

		/// <summary>
		/// The array of RkRec values within the range of records.
		/// </summary>
		public RkRec[] Values
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Returns the array of RkRec values within the range of records.
		/// </summary>
		public override object Value
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Returns the value of a particular column in the range.
		/// </summary>
		/// <param name="col">The index of the column.</param>
		/// <returns>Returns an RkRec value.</returns>
		public override object GetValue(ushort col)
		{
			return _values[col - FirstCol];
		}

	}
}
