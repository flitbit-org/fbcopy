#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.
// For licensing information see License.txt (MIT style licensing).
#endregion

using System.Collections.Generic;
using FlitBit.IoC;

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
		/// <param name="container">a container scope</param>		
		public void CopyTo(T target, S source, CopyKind kind, IContainer container)
		{
			if (!_comparer.Equals(default(S), source))
			{
				if (kind == CopyKind.Strict)
				{
					PerformStrictCopy(target, source, container);
				}
				else
				{
					PerformLooseCopy(target, source, container);
				}
			}
		}

		/// <summary>
		/// Allows subclasses to perform a strict copy.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="container">a container scope</param>
		protected abstract void PerformStrictCopy(T target, S source, IContainer container);
		/// <summary>
		/// Allows subclasses to perform a loose copy.
		/// </summary>
		/// <param name="target">the target object</param>
		/// <param name="source">the source object</param>
		/// <param name="container">a container scope</param>
		protected abstract void PerformLooseCopy(T target, S source, IContainer container);
	}
}
