#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

namespace FlitBit.Copy
{
	/// <summary>
	///   Enum of copy kinds.
	/// </summary>
	public enum CopyKind
	{
		/// <summary>
		///   Default. Indicates a loose copy; all properties of source do not have to
		///   be present on target.
		/// </summary>
		Loose = 0,

		/// <summary>
		///   Indicates a strict copy; all properties of source must be present on target.
		/// </summary>
		Strict = 1
	}
}