using System;
using Net.SourceForge.Koogra.Excel.ValueTypes;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Structure that represents a formatted RK value.
	/// </summary>
	public struct RkRec
	{
		private ushort _xf;
		private RkValue _value;

		/// <summary>
		/// Constructor for the formatted RK value.
		/// </summary>
		/// <param name="xf">The index in the format table for the RK value.</param>
		/// <param name="rk">The RK value.</param>
		public RkRec(ushort xf, int rk)
		{
			_xf = xf;
			_value = new RkValue(rk);
		}

		/// <summary>
		/// The index in the format table for the RK value.
		/// </summary>
		public ushort Xf
		{
			get
			{
				return _xf;
			}
		}

		/// <summary>
		/// The value of the RK record.
		/// </summary>
		public double Value
		{
			get
			{
				return _value.Value;
			}
		}

	}
}
