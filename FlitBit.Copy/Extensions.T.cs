#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using FlitBit.Core;

namespace FlitBit.Copy
{
	/// <summary>
	///   Copy extensions.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		///   Copies source's properties onto the target.
		/// </summary>
		/// <param name="source">the source</param>
		/// <param name="target">the target</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <typeparam name="TTarget">target type TTarget</typeparam>
		/// <returns>the source object.</returns>
		public static TSource CopyTo<TSource, TTarget>(this TSource source, TTarget target)
		{
			Copier<TTarget>.LooseCopyTo(target, source, FactoryProvider.Factory);
			return source;
		}

		/// <summary>
		///   Copies source's properties onto the target.
		/// </summary>
		/// <param name="source">the source</param>
		/// <param name="target">the target</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <typeparam name="TTarget">target type TTarget</typeparam>
		/// <returns>the target object.</returns>
		public static TTarget CopyFrom<TTarget, TSource>(this TTarget target, TSource source)
		{
			Copier<TTarget>.LooseCopyTo(target, source, FactoryProvider.Factory);
			return target;
		}
	}
}