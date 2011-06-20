using System;

namespace Net.SourceForge.Koogra.Excel.ValueTypes
{
	/// <summary>
	/// Character set enumeration.
	/// </summary>
	public enum FontCharacterSet : byte
	{
		/// <summary>
		/// ANSI Latin
		/// </summary>
		ANSILatin = 0x00,
		/// <summary>
		/// Symbol
		/// </summary>
		Symbol = 0x02,
		/// <summary>
		/// Apple Roman
		/// </summary>
		AppleRoman = 0x4D,
		/// <summary>
		/// ANSI Japanese
		/// </summary>
		ANSIJapanese = 0x80,
		/// <summary>
		/// ANSI Korean Hangul
		/// </summary>
		ANSIKoreanHangul = 0x81,
		/// <summary>
		/// ANSI Korean Johab
		/// </summary>
		ANSIKoreanJohab = 0x82,
		/// <summary>
		/// ANSI Chinese Simplified GBK
		/// </summary>
		ANSIChineseSimplifiedGBK = 0x86,
		/// <summary>
		/// ANSI Chinese Traditional BIG5
		/// </summary>
		ANSIChineseTraditionalBIG5 = 0x88,
		/// <summary>
		/// ANSI Greek
		/// </summary>
		ANSIGreek = 0xA1,
		/// <summary>
		/// ANSI Turkish
		/// </summary>
		ANSITurkish = 0xA2,
		/// <summary>
		/// ANSI Vietnamese
		/// </summary>
		ANSIVietnamese = 0xA3,
		/// <summary>
		/// ANSI Hebrew
		/// </summary>
		ANSIHebrew = 0xB1,
		/// <summary>
		/// ANSI Arabic
		/// </summary>
		ANSIArabic = 0xB2,
		/// <summary>
		/// ANSI Baltic
		/// </summary>
		ANSIBaltic = 0xBA,
		/// <summary>
		/// ANSI Cyrillic
		/// </summary>
		ANSICyrillic = 0xCC,
		/// <summary>
		/// ANSI Thai
		/// </summary>
		ANSIThai = 0xDE,
		/// <summary>
		/// ANSI Latin 2
		/// </summary>
		ANSILatin2 = 0xEE,
		/// <summary>
		/// ANSI OEM Latin 1
		/// </summary>
		ANSIOEMLatin1 = 0xFF,
	}
}
