using System;
using System.Diagnostics;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Class that abstracts a boolean or error Excel value.
	/// </summary>
	public class BoolErrValue
	{
		private object _value;
		private bool _error;

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="value">The value to read.</param>
		/// <param name="error">Error indicator.</param>
		public BoolErrValue(byte value, byte error)
		{
			_value = null;

			_error = (error == 1);

			if(_error)
			{
				switch(value)
				{
					case 0x00 :
						_value = "#NULL!";
						break;
					case 0x07 :
						_value = "#DIV/0";
						break;
					case 0x0f :
						_value = "#VALUE!";
						break;
					case 0x17 :
						_value = "#REF!";
						break;
					case 0x1d :
						_value = "#NAME?";
						break;
					case 0x24 :
						_value = "#NUM!";
						break;
					case 0x2a :
						_value = "#N/A";
						break;
					default :
						Debug.Assert(false);
						_value = "";
						break;
				}
			}
			else
				_value = (value == 1);
		}

		/// <summary>
		/// The value of the object.
		/// </summary>
		/// <value>Returns a bool or string value.</value>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Returns true if the class reprents an error and false if a boolean.
		/// </summary>
		public bool IsError
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Method for getting the string value of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _value.ToString();
		}

	}
}
