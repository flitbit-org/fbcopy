#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.
// For licensing information see License.txt (MIT style licensing).
#endregion

using System;
using FlitBit.Core.Factory;

namespace FlitBit.Copy
{
	/// <summary>
	/// Copier implementation for anonymous source objects.
	/// </summary>
	/// <typeparam name="S"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class AnonymousSourceCopier<S, T> : ICopier<S, T>
	{
		/// <summary>
		/// Copies source to target.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="kind"></param>
		/// <param name="factory"></param>
		public void CopyTo(T target, S source, CopyKind kind, IFactory factory)
		{
			if (kind == CopyKind.Loose)
			{
				Copier<T>.LooseCopyTo(target, source, factory);
			}
			else throw new NotImplementedException();
		}
	}
}
