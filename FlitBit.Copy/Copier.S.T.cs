#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.
// For licensing information see License.txt (MIT style licensing).
#endregion

using System.Collections.Generic;
using FlitBit.Core.Factory;

namespace FlitBit.Copy
{
	/// <summary>
	/// Copies properties of the source object to the target object.
	/// </summary>
	/// <typeparam name="S">source type S</typeparam>
	/// <typeparam name="T">target type T</typeparam>
	public abstract class Copier<S, T> : ICopier<S, T>
	{
		EqualityComparer<S> _comparer = EqualityComparer<S>.Default;

		/// <summary>
		/// Copies properties from a source object to a target object.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="kind">kind of copy (loose or strict)</param>
		/// <param name="factory">a container scope</param>		
		public void CopyTo(T target, S source, CopyKind kind, IFactory factory)
		{
			if (!_comparer.Equals(default(S), source))
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

		/// <summary>
		/// Allows subclasses to perform a strict copy.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="factory">a container scope</param>
		protected abstract void PerformStrictCopy(T target, S source, IFactory factory);
		/// <summary>
		/// Allows subclasses to perform a loose copy.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="factory">a container scope</param>
		protected abstract void PerformLooseCopy(T target, S source, IFactory factory);
	}
}
