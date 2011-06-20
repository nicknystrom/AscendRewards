using System;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// This class decodes the hyperlink options field
	/// Thanks to openoffice.org as the format was taken from their documentation of the excel file format http://www.openoffice.org
	/// </summary>
	public struct HyperLinkOptions
	{
		private uint _options;

		/// <summary>
		/// The constructor
		/// </summary>
		/// <param name="options">The options flag as taken from the HLINK record</param>
		public HyperLinkOptions(uint options)
		{
			_options = options;
		}

		private bool HasNoLink
		{
			get
			{
				return !HasMasked(0x0001);
			}
		}

		private bool HasMasked(int mask)
		{
			return (_options & mask) != 0;
		}

		/// <summary>
		/// Returns true if the HLINK record contains a file or url link
		/// </summary>
		public bool HasFileLinkOrUrl
		{
			get
			{
				return HasMasked(0x0001) || !HasMasked(0x0100);
			}
		}

		/// <summary>
		/// Returns true if the link is a relative file path
		/// </summary>
		public bool HasRelativeFilePath
		{
			get
			{
				return HasMasked(0x0002);
			}
		}

		/// <summary>
		/// Returns true if the HLINK record contains a description
		/// </summary>
		public bool HasDescription
		{
			get
			{
				return HasMasked(0x0014);
			}
		}

		/// <summary>
		/// Returns true if the HLINK record contains a text mark
		/// </summary>
		public bool HasTextMark
		{
			get
			{
				return HasMasked(0x0008);
			}
		}

		/// <summary>
		/// Returns true if the HLINK record contains a target frame
		/// </summary>
		public bool HasTargetFrame
		{
			get
			{
				return HasMasked(0x0080);
			}
		}

		/// <summary>
		/// Returns true if the HLINK record contains a UNC path
		/// </summary>
		public bool HasUNCPath
		{
			get
			{
				return HasMasked(0x0100);
			}
		}
	}
}
