using System;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Enumeration of currently supported/known Excel record id's
	/// </summary>
	public enum RecordType : ushort
	{
		/// <summary>
		/// BOF
		/// </summary>
		Bof = 0x0809,
		/// <summary>
		/// BOUNDSHEET
		/// </summary>
		Boundsheet = 0x0085,
		/// <summary>
		/// INDEX
		/// </summary>
		Index = 0x020B,
		/// <summary>
		/// DBCELL
		/// </summary>
		DbCell = 0x00d7,
		/// <summary>
		/// ROW
		/// </summary>
		Row = 0x0208,
		/// <summary>
		/// CONTINUE
		/// </summary>
		Continue = 0x003c,
		/// <summary>
		/// SST
		/// </summary>
		Sst = 0x00fc,
		/// <summary>
		/// BLANK
		/// </summary>
		Blank = 0x0201,
		/// <summary>
		/// BOOLERR
		/// </summary>
		BoolErr = 0x0205,
		/// <summary>
		/// FORMULA
		/// </summary>
		Formula = 0x006,
		/// <summary>
		/// LABEL
		/// </summary>
		Label = 0x0204,
		/// <summary>
		/// LABELSST
		/// </summary>
		LabelSst = 0x00fd,
		/// <summary>
		/// MULBLANK
		/// </summary>
		MulBlank = 0x00be,
		/// <summary>
		/// MULRK
		/// </summary>
		MulRk = 0x00bd,
		/// <summary>
		/// STRING
		/// </summary>
		String = 0x0207,
		/// <summary>
		/// XF
		/// </summary>
		Xf = 0x00e0,
		/// <summary>
		/// EOF
		/// </summary>
		Eof = 0x000a,
		/// <summary>
		/// RK
		/// </summary>
		Rk = 0x027e,
		/// <summary>
		/// NUMBER
		/// </summary>
		Number = 0x0203,
		/// <summary>
		/// ARRAY
		/// </summary>
		Array = 0x0221,
		/// <summary>
		/// SHRFMLA
		/// </summary>
		ShrFmla = 0x00bc,
		/// <summary>
		/// TABLE
		/// </summary>
		Table = 0x0036,
		/// <summary>
		/// FONT
		/// </summary>
		Font = 0x0031,
		/// <summary>
		/// FORMAT
		/// </summary>
		Format = 0x041e,
		/// <summary>
		/// PALETTE
		/// </summary>
		Palette = 0x0092,
		/// <summary>
		/// HLINK
		/// </summary>
		Hyperlink = 0x01B8,
	}
}
