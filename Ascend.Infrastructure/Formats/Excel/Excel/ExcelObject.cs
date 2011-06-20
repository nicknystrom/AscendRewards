using System;

namespace Net.SourceForge.Koogra.Excel
{
	/// <summary>
	/// Base class implemenation of IExcelObject
	/// </summary>
	public abstract class ExcelObject : IExcelObject
	{
		private Workbook _wb;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="wb">The workbook.</param>
		/// <exception cref="ArgumentNullException">Exception is thrown if wb is null.</exception>
		public ExcelObject(Workbook wb)
		{
			if(wb == null)
				throw new ArgumentNullException("wb");

			_wb = wb;
		}

		/// <summary>
		/// The workbook.
		/// </summary>
		public Workbook Workbook
		{
			get
			{
				return _wb;
			}
		}
	}
}
