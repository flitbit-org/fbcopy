using System;
using System.Diagnostics.Contracts;
using FlitBit.Core;
using FlitBit.Core.Factory;

namespace FlitBit.Copy
{
	/// <summary>
	///   Contains extensions over copiers.
	/// </summary>
	public static class CopierExtensions
	{
		/// <summary>
		///   Copy constructs a target object from data contained in the source.
		/// </summary>
		/// <param name="copier"></param>
		/// <param name="source"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <returns></returns>
		public static TTarget CopyConstruct<TSource, TTarget>(this ICopier<TSource, TTarget> copier, TSource source)
		{
			var factory = FactoryProvider.Factory;
			var target = factory.CreateInstance<TTarget>();
			copier.CopyTo(target, source, CopyKind.Loose, factory);
			return target;
		}

		/// <summary>
		///   Makes a loose copy of source to target.
		/// </summary>
		/// <param name="copier"></param>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <returns></returns>
		public static TTarget LooseCopy<TSource, TTarget>(this ICopier<TSource, TTarget> copier, TTarget target,
			TSource source)
		{
			return LooseCopy(copier, target, source, FactoryProvider.Factory);
		}

		/// <summary>
		///   Makes a strict copy of source to target.
		/// </summary>
		/// <param name="copier"></param>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="factory"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <returns></returns>
		public static TTarget LooseCopy<TSource, TTarget>(this ICopier<TSource, TTarget> copier, TTarget target,
			TSource source, IFactory factory)
		{
			Contract.Requires<ArgumentNullException>(copier != null);
			Contract.Requires<ArgumentNullException>(factory != null);

			copier.CopyTo(target, source, CopyKind.Loose, factory);
			return target;
		}

		/// <summary>
		///   Makes a strict copy of source to target.
		/// </summary>
		/// <param name="copier"></param>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <returns></returns>
		public static TTarget StrictCopy<TSource, TTarget>(this ICopier<TSource, TTarget> copier, TTarget target,
			TSource source)
		{
			return StrictCopy(copier, target, source, FactoryProvider.Factory);
		}

		/// <summary>
		///   Makes a strict copy of source to target.
		/// </summary>
		/// <param name="copier"></param>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="factory"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <returns></returns>
		public static TTarget StrictCopy<TSource, TTarget>(this ICopier<TSource, TTarget> copier, TTarget target,
			TSource source, IFactory factory)
		{
			Contract.Requires<ArgumentNullException>(copier != null);
			Contract.Requires<ArgumentNullException>(factory != null);

			copier.CopyTo(target, source, CopyKind.Strict, factory);
			return target;
		}
	}
}