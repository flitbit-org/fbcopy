#region COPYRIGHT© 2009-2014 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using FlitBit.Core.Factory;

namespace FlitBit.Copy
{
	/// <summary>
	///   Copies properties of the source object to the target object.
	/// </summary>
	/// <typeparam name="TSource">source type S</typeparam>
	/// <typeparam name="TTarget">target type T</typeparam>
	[CopierAutoImplement]
	public interface ICopier<in TSource, in TTarget>
	{
		/// <summary>
		///   Copies properties from a source object to a target object.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="kind">kind of copy (loose or strict)</param>
		/// <param name="factory">a container scope</param>
		void CopyTo(TTarget target, TSource source, CopyKind kind, IFactory factory);
	}
}