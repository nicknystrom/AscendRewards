using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Font boldness structure.
	/// </summary>
	public struct FontBoldness
	{
		/// <summary>
		/// The minimum boldness possible for a font.
		/// </summary>
		public static readonly FontBoldness Min = new FontBoldness(100);
		/// <summary>
		/// The maximum boldness possible for a font.
		/// </summary>
		public static readonly FontBoldness Max = new FontBoldness(1000);
		/// <summary>
		/// The normal (no boldness) for a font.
		/// </summary>
		public static readonly FontBoldness Normal = new FontBoldness(400);
		/// <summary>
		/// Normal bold weight for a font.
		/// </summary>
		public static readonly FontBoldness Bold = new FontBoldness(700);

		private ushort _boldness;

		private static void CheckValue(ushort boldness)
		{
			if(boldness < 100 || boldness > 1000)
				throw new ArgumentOutOfRangeException("boldness", boldness, "Boldness should be a value between 100-1000");
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="boldness">The boldness of the font.</param>
		/// <exception cref="ArgumentOutOfRangeException">Exception thrown if boldness is an invalid value.</exception>
		public FontBoldness(ushort boldness)
		{
			CheckValue(boldness);
			_boldness = boldness;
		}

		/// <summary>
		/// The boldness.
		/// </summary>
		public ushort Boldness
		{
			get
			{
				return _boldness;
			}
		}
	}
}
