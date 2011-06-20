using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// This class abstracts the value of a formula.
	/// </summary>
	public class FormulaValue
	{
		private object _value;
		private bool _stringFollows;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value">The value of the formula.</param>
		public FormulaValue(double value)
		{
			_stringFollows = false;
			_value = null;
			byte[] bytes = BitConverter.GetBytes(value);
			
			if(BitConverter.ToUInt16(bytes, 6) == 0xFFFF)
			{
				byte type = bytes[0];

				if(type == 0)
				{
					_stringFollows = true;
					return;
				}
				else if(type == 1)
				{
					_value = new BoolErrValue(bytes[2], 0);
					return;
				}
				else if(type == 2)
				{
					_value = new BoolErrValue(bytes[2], 1);
					return;
				}
				else if(type == 3)
				{
					_value = "";
					return;
				}
			}

			_value = value;
		}

		/// <summary>
		/// Returns true if the value of the formula is in a STRING record that follows.
		/// </summary>
		public bool StringFollows
		{
			get
			{
				return _stringFollows;
			}
		}

		/// <summary>
		/// The value of the formula.
		/// </summary>
		/// <value>Returns a double, string or bool value.</value>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Method for getting the string value of the formula.
		/// </summary>
		/// <returns>The string value/convertion of the formula.</returns>
		public override string ToString()
		{
			return _value.ToString();
		}

	}
}
