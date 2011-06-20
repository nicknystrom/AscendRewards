using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the PALETTE (0x0092) Excel record found in Excel streams.
	/// </summary>
	public class PaletteRecord : Biff
	{
		private int[] _colors;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the PALETTE record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public PaletteRecord(GenericBiff biff)
		{
			if(biff.Id == (ushort)RecordType.Palette)
			{
				BinaryReader reader = new BinaryReader(biff.GetDataStream());

				ushort numColors = reader.ReadUInt16();
				_colors = new int[numColors];
				for(ushort i = 0; i < numColors; ++i)
				{
					_colors[i] = reader.ReadInt32();
				}
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Palette);
		}

		/// <summary>
		/// The array of colors in the PALETTE record.
		/// </summary>
		public int[] Colors
		{
			get
			{
				return _colors;
			}
		}
	}
}
