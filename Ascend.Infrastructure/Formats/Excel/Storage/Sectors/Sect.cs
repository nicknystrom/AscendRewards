using System;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Sector index.
	/// </summary>
	public struct Sect
	{
		/// <summary>
		/// DifSect index.
		/// </summary>
		public const uint DIFSECT = 0xFFFFFFFC;
		/// <summary>
		/// FatSect index.
		/// </summary>
		public const uint FATSECT = 0xFFFFFFFD;
		/// <summary>
		/// End of chain index.
		/// </summary>
		public const uint END_OF_CHAIN = 0xFFFFFFFE;
		/// <summary>
		/// Free sector index.
		/// </summary>
		public const uint FREE_SECT = 0xFFFFFFFF;

		private uint _value;

		/// <summary>
		/// Zero value.
		/// </summary>
		public static readonly Sect ZERO = new Sect(0);

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value">The index of the Sect.</param>
		public Sect(uint value)
		{
			_value = value;
		}

		/// <summary>
		/// Returns true is Sect is a DifSect
		/// </summary>
		public bool IsDifSect
		{
			get
			{
				return _value == DIFSECT;
			}
		}

		/// <summary>
		/// Returns true if Sect is a FatSect
		/// </summary>
		public bool IsFatSect
		{
			get
			{
				return _value == FATSECT;
			}
		}

		/// <summary>
		/// Returns true if Sect is the end of the chain.
		/// </summary>
		public bool IsEndOfChain
		{
			get
			{
				return _value == END_OF_CHAIN;
			}
		}

		/// <summary>
		/// Returns true if Sect has no data.
		/// </summary>
		public bool IsFree
		{
			get
			{
				return _value == FREE_SECT;
			}
		}

		/// <summary>
		/// Returns the index of the Sect.
		/// </summary>
		public uint Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Casts the sect to an integer.
		/// </summary>
		/// <returns>The integer equivalent of the Sect.</returns>
		public int ToInt()
		{
			return (int)_value;
		}

		/// <summary>
		/// Compares a Sect.
		/// </summary>
		/// <param name="obj">The other value to compare.</param>
		/// <returns>Returns true if obj == Value .</returns>
		public override bool Equals(object obj)
		{
			return _value.Equals (obj);
		}

		/// <summary>
		/// Returns the hashcode based on Value.
		/// </summary>
		/// <returns>Returns the hashcode based on Value.</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode ();
		}

	}
}
