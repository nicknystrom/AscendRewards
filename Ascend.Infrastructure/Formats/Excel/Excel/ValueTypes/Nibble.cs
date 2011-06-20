using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Represents a nibble which is half a byte.
	/// </summary>
	public struct Nibble
	{
		/// <summary>
		/// The minimum value allowed for the nibble.
		/// </summary>
		public const byte MinValue = 0x00;
		/// <summary>
		/// The maximum value allowed for the nibble.
		/// </summary>
		public const byte MaxValue = 0x0F;

		private byte _value;

		private static void CheckVal(byte val)
		{
			if(val < MinValue || val > MaxValue)
				throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		/// Constructs a nibble from an integer. The integer is cast to a byte.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <exception cref="ArgumentOutOfRangeException">Exception is thrown when value is not between MinValue and MaxValue</exception>
		public Nibble(int value) : this((byte)value)
		{
		}

		/// <summary>
		/// Constructs a nibble from a byte.
		/// </summary>
		/// <param name="value">The value to read.</param>
		/// <exception cref="ArgumentOutOfRangeException">Exception is thrown when value is not between MinValue and MaxValue</exception>
		public Nibble(byte value)
		{
			CheckVal(value);
			_value = value;
		}

		/// <summary>
		/// The value of the Nibble.
		/// </summary>
		public byte Value
		{
			get
			{
				return _value;
			}
			set
			{
				CheckVal(value);
				_value = value;
			}
		}
	}
}
