using System;

namespace Net.SourceForge.Koogra.Storage.Sectors
{
	/// <summary>
	/// Stream type enumeration.
	/// </summary>
	public enum Stgty : byte
	{
		/// <summary>
		/// Invalid
		/// </summary>
		Invalid = 0,
		/// <summary>
		/// Storage
		/// </summary>
		Storage,
		/// <summary>
		/// Stream
		/// </summary>
		Stream,
		/// <summary>
		/// LockBytes
		/// </summary>
		LockBytes,
		/// <summary>
		/// Propery
		/// </summary>
		Property,
		/// <summary>
		///Root
		/// </summary>
		Root
	}
}
