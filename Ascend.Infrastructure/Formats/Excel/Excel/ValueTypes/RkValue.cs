using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
    /// <summary>
    /// Structure that represents the value of an RK record.
    /// </summary>
    public struct RkValue
    {
        private double _value;

        /// <summary>
        /// The constructor for the RK record.
        /// </summary>
        /// <param name="rk">The raw RK value to interpret.</param>
        public RkValue(int rk)
        {
            _value = 0;

            bool div100 = (rk & 0x01) != 0;
            bool isInteger = (rk & 0x02) != 0;

            if (!isInteger)
            {
                _value = ToDouble(rk & ~1);
                if (div100)
                    _value /= 100.0;
            }
            else
            {
                rk = rk >> 2;
                if (div100)
                    _value = rk / 100.0;
                else
                    _value = rk;
            }
        }

        private double ToDouble(int n)
        {
            byte[] doubleBytes = new byte[8];
            byte[] uintBytes = BitConverter.GetBytes(n);
            Array.Copy(uintBytes, 0, doubleBytes, doubleBytes.Length - uintBytes.Length, uintBytes.Length);

            return BitConverter.ToDouble(doubleBytes, 0);
        }

        /// <summary>
        /// The value of the RK record.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Method for getting the string value of the RK record.
        /// </summary>
        /// <returns>The string value of the RK record.</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

    }
}
