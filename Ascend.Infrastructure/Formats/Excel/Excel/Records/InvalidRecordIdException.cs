using System;

namespace Net.SourceForge.Koogra.Excel.Records
{
	/// <summary>
	/// Exception thrown when encountering invalid record types when reading GenericBiff records.
	/// </summary>
	public class InvalidRecordIdException : Exception
	{
		/// <summary>
		/// Exception constructor.
		/// </summary>
		/// <param name="actual">The actual id encountered.</param>
		/// <param name="expected">The expected id.</param>
		public InvalidRecordIdException(object actual, object expected) : base(string.Format("Invalid record id {0}. Expected {1}.", actual, expected))
		{
		}
	}
}
