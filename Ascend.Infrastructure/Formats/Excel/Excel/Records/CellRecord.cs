using System;
using System.IO;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Base class for all Excel Cell type records.
	/// Cell type records may pertain to a single column or a range of columns and may or may not have formatting information.
	/// All Cell type records must return a value.
	/// </summary>
	public abstract class CellRecord : Biff
	{
		private ushort _row;

		/// <summary>
		/// Method for retrieving the row to which the cell belongs to.
		/// </summary>
		/// <param name="reader">The reader for the record.</param>
		protected void ReadRow(BinaryReader reader)
		{
			_row = reader.ReadUInt16();
		}

		/// <summary>
		/// Method that must be overriden in inheriting classes and must return the value for that class.
		/// </summary>
		public abstract object Value { get; }

		/// <summary>
		/// The row to which the cell belongs to.
		/// </summary>
		public ushort Row 
		{ 
			get
			{
				return _row;
			}
		}
	}
}
