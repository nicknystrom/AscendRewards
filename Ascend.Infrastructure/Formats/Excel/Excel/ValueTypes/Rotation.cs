using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Rotation type enumeration.
	/// </summary>
	public enum RotationType
	{
		/// <summary>
		/// None 
		/// </summary>
		None = 0,
		/// <summary>
		/// Counter Clockwise
		/// </summary>
		CounterClockwise,
		/// <summary>
		/// Clock Wise
		/// </summary>
		ClockWise,
		/// <summary>
		/// Top To Bottom
		/// </summary>
		TopToBottom
	}

	/// <summary>
	/// Rotation enumeration.
	/// </summary>
	public struct Rotation 
	{
		private byte _rotationAngle;

		/// <summary>
		/// Constructor for the rotation
		/// </summary>
		/// <param name="rotationAngle">The rotation angle.</param>
		/// <remarks>The rotation angle must be a value less than 180 or equal to 255</remarks>
		/// <exception cref="ArgumentException">Exception is thrown if value is invalid.</exception>
		public Rotation(byte rotationAngle)
		{
			if(rotationAngle <= 180 || rotationAngle == 255)
				_rotationAngle = rotationAngle;
			else
				throw new ArgumentException("Invalid rotation angle", "rotationAngle");
		}

		/// <summary>
		/// The rotation angle.
		/// </summary>
		public byte RotationAngle
		{
			get
			{
				return _rotationAngle;
			}
		}

		/// <summary>
		/// The rotation type.
		/// </summary>
		public RotationType RotationType
		{
			get
			{
				if(_rotationAngle == 0)
					return RotationType.None;
				else if(_rotationAngle >= 1 && _rotationAngle <= 90)
					return RotationType.CounterClockwise;
				else if(_rotationAngle >= 91 && _rotationAngle <= 180)
					return RotationType.ClockWise;
				else 
					return RotationType.TopToBottom;
			}
		}
	}
}
