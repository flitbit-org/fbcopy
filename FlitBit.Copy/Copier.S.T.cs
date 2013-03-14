#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using System.Collections.Generic;
using FlitBit.Core.Factory;

namespace FlitBit.Copy
{
	/// <summary>
	///   Copies properties of the source object to the target object.
	/// </summary>
	/// <typeparam name="TSource">source type S</typeparam>
	/// <typeparam name="TTarget">target type T</typeparam>
	public abstract class Copier<TSource, TTarget> : ICopier<TSource, TTarget>
	{
		readonly EqualityComparer<TSource> _comparer = EqualityComparer<TSource>.Default;

		/// <summary>
		///   Allows subclasses to perform a loose copy.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="factory">a container scope</param>
		protected abstract void PerformLooseCopy(TTarget target, TSource source, IFactory factory);

		/// <summary>
		///   Allows subclasses to perform a strict copy.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="factory">a container scope</param>
		protected abstract void PerformStrictCopy(TTarget target, TSource source, IFactory factory);

		#region ICopier<TSource,TTarget> Members

		/// <summary>
		///   Copies properties from a source object to a target object.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="kind">kind of copy (loose or strict)</param>
		/// <param name="factory">a container scope</param>
		public void CopyTo(TTarget target, TSource source, CopyKind kind, IFactory factory)
		{
			if (!_comparer.Equals(default(TSource), source))
			{
				if (kind == CopyKind.Strict)
				{
					PerformStrictCopy(target, source, factory);
				}
				else
				{
					PerformLooseCopy(target, source, factory);
				}
			}
		}

		#endregion
	}
}