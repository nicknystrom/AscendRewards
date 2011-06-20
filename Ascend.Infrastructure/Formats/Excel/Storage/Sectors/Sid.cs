using System;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Represents a DirectorySectorEntry index.
	/// </summary>
	public struct Sid : IComparable
	{
		/// <summary>
		/// Represents the end of the file.
		/// </summary>
		public static readonly uint EOF = 0xFFFFFFFF;
		/// <summary>
		/// Represents a zero Sid.
		/// </summary>
		public static readonly Sid ZERO = new Sid(0);

		private uint _value;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value">The index of the Sid.</param>
		public Sid(uint value)
		{
			_value = value;
		}

		/// <summary>
		/// Returns true if this Sid indicates end of file.
		/// </summary>
		public bool IsEof
		{
			get
			{
				return _value == EOF;
			}
		}

		/// <summary>
		/// Returns the value of this Sid.
		/// </summary>
		public uint Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Converts a Sid to an integer .
		/// </summary>
		/// <returns>Returns the integer equivalent of the Sid.</returns>
		public int ToInt()
		{
			return (int)_value;
		}

		/// <summary>
		/// Compares Value to obj.
		/// </summary>
		/// <param name="obj">The value to compare.</param>
		/// <returns>Returns true if obj == Value .</returns>
		public override bool Equals(object obj)
		{
			return _value.Equals(obj);
		}

		/// <summary>
		/// Returns the hashcode of Value.
		/// </summary>
		/// <returns>Returns the hashcode of Value.</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		/// <summary>
		/// Compares a Sid to another Sid for equality.
		/// </summary>
		/// <param name="l">The left Sid.</param>
		/// <param name="r">The right Sid.</param>
		/// <returns>Returns true if l is equal to r.</returns>
		public static bool operator == (Sid l, Sid r)
		{
			return r._value == l._value;
		}

		/// <summary>
		/// Compares a Sid to another Sid for inequality.
		/// </summary>
		/// <param name="l">The left Sid.</param>
		/// <param name="r">The right Sid.</param>
		/// <returns>Returns true if l is not equal to r.</returns>
		public static bool operator != (Sid l, Sid r)
		{
			return l._value != r._value;
		}

		/// <summary>
		/// Compares a Sid to another Sid.
		/// </summary>
		/// <param name="l">The left Sid.</param>
		/// <param name="r">The right Sid.</param>
		/// <returns>Returns true if l is less than r.</returns>
		public static bool operator < (Sid l, Sid r)
		{
			return l._value < r._value;
		}

		/// <summary>
		/// Compares a Sid to another Sid.
		/// </summary>
		/// <param name="l">The left Sid.</param>
		/// <param name="r">The right Sid.</param>
		/// <returns>Returns true if l is greater than r.</returns>
		public static bool operator > (Sid l, Sid r)
		{
			return l._value > r._value;
		}

		/// <summary>
		/// Compares a Sid to another Sid.
		/// </summary>
		/// <param name="l">The left Sid.</param>
		/// <param name="r">The right Sid.</param>
		/// <returns>Returns true if l less than or equal to r.</returns>
		public static bool operator <= (Sid l, Sid r)
		{
			return l._value <= r._value;
		}

		/// <summary>
		/// Compares a Sid to another Sid for equality.
		/// </summary>
		/// <param name="l">The left Sid.</param>
		/// <param name="r">The right Sid.</param>
		/// <returns>Returns true if l is greater than or equal to r.</returns>
		public static bool operator >= (Sid l, Sid r)
		{
			return l._value >= r._value;
		}

		#region IComparable Members

		/// <summary>
		/// Compares this Sid to another Sid.
		/// </summary>
		/// <param name="obj">The other Sid.</param>
		/// <returns>Returns 0, &lt; 0 or &gt; 0 if obj is equal, less or greater than this Sid respectively.</returns>
		public int CompareTo(object obj)
		{
			Sid l = (Sid)obj;

			return _value.CompareTo(l.Value);
		}

		#endregion
	}
}
