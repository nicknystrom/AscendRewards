using System;
using System.IO;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class abstracts the FORMULA (0x006) Excel record  found in Excel streams.
	/// </summary>
	public class FormulaRecord : RowColXfCellRecord
	{
		private object _value;
		private ushort _options;
		private uint _reserved;
		private ushort _formulaLen;
		private byte[] _formulaData;

		/// <summary>
		/// The constructor for the record.
		/// </summary>
		/// <param name="biff">The GenericBiff record that should contain the correct type and data for the FORMULA record.</param>
		/// <param name="recordsStream">The stream into the records to which the FORMULA record belongs to. The record stream must be positioned just after the FORMULA record.</param>
		/// <exception cref="InvalidRecordIdException">
		/// An InvalidRecordIdException is thrown if biff contains an invalid type or invalid data.
		/// </exception>
		public FormulaRecord(GenericBiff biff, Stream recordsStream)
		{
			if(biff.Id == (ushort)RecordType.Formula)
			{
				Stream stream = biff.GetDataStream();
				BinaryReader reader = new BinaryReader(stream);
				
				ReadRowColXf(reader);
				FormulaValue val = new FormulaValue(reader.ReadDouble());
				
				_options = reader.ReadUInt16();
				_reserved = reader.ReadUInt32();
				_formulaLen = reader.ReadUInt16();
				_formulaData = reader.ReadBytes((int)(stream.Length - stream.Position));

				if(val.StringFollows)
				{
					GenericBiff r = new GenericBiff(recordsStream);
					while(r.Id != (ushort)RecordType.String)
					{
						r = new GenericBiff(recordsStream);
					}

					StringValueRecord stringValue = new StringValueRecord(r);
					_value = stringValue.Value;
				}
				else
					_value = val.Value;
			}
			else
				throw new InvalidRecordIdException(biff.Id, RecordType.Formula);
		}

		internal void SetStringValue(string val)
		{
			_value = val;
		}

		/// <summary>
		/// The value of the formula.
		/// </summary>
		/// <value>
		/// The return value maybe a double, string or bool value.
		/// </value>
		public override object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Options for the FORMULA record.
		/// </summary>
		public ushort Options
		{
			get
			{
				return _options;
			}
		}

		/// <summary>
		/// Reserved field for the FORMULA record.
		/// </summary>
		public uint Reserved
		{
			get
			{
				return _reserved;
			}
		}

		/// <summary>
		/// Length of the formula data.
		/// </summary>
		public ushort FormulaLen
		{
			get
			{
				return _formulaLen;
			}
		}

		/// <summary>
		/// The unparsed formula data.
		/// </summary>
		public byte[] FormulaData
		{
			get
			{
				return _formulaData;
			}
		}

	}
}
