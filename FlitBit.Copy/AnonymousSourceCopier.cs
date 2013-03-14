#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using System;
using FlitBit.Core.Factory;

namespace FlitBit.Copy
{
	/// <summary>
	///   Copier implementation for anonymous source objects.
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <typeparam name="TTarget"></typeparam>
	public class AnonymousSourceCopier<TSource, TTarget> : ICopier<TSource, TTarget>
	{
		#region ICopier<TSource,TTarget> Members

		/// <summary>
		///   Copies source to target.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="kind"></param>
		/// <param name="factory"></param>
		public void CopyTo(TTarget target, TSource source, CopyKind kind, IFactory factory)
		{
			if (kind == CopyKind.Loose)
			{
				Copier<TTarget>.LooseCopyTo(target, source, factory);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}