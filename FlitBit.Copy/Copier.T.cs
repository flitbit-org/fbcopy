#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using FlitBit.Core;
using FlitBit.Core.Factory;
using FlitBit.Emit;

namespace FlitBit.Copy
{
	/// <summary>
	///   Static copier used with anonymous/closed types.
	/// </summary>
	/// <typeparam name="TTarget"></typeparam>
	public static class Copier<TTarget>
	{
		/// <summary>
		///   Copies source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="source">the source</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <returns>an instance of TTarget, populated with data from the source.</returns>
		public static TTarget CopyConstruct<TSource>(TSource source)
		{
			var cloneable = source as ICloneable;
			if (cloneable != null && typeof(TTarget).IsAssignableFrom(typeof(TSource)))
			{
				return (TTarget)cloneable.Clone();
			}
			return CopyConstruct(source, FactoryProvider.Factory);
		}

		/// <summary>
		///   Copies source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="source">the source</param>
		/// <param name="factory">a factory</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <returns>an instance of TTarget, populated with data from the source.</returns>
		public static TTarget CopyConstruct<TSource>(TSource source, IFactory factory)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var cloneable = source as ICloneable;
			if (cloneable != null && typeof(TTarget).IsAssignableFrom(typeof(TSource)))
			{
				return (TTarget)cloneable.Clone();
			}
			var copier = factory.CreateInstance<ICopier<TSource, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			copier.CopyTo(res, source, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies all source objects into new instances of target type TTarget.
		/// </summary>
		/// <param name="sources">the source</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <returns>instances of TTarget, populated with data from the sources.</returns>
		public static IEnumerable<TTarget> CopyConstructAll<TSource>(IEnumerable<TSource> sources)
		{
			var cloneable = typeof(ICloneable).IsAssignableFrom(typeof(TSource));
			if (cloneable && typeof(TTarget).IsAssignableFrom(typeof(TSource)))
			{
				return (from s in sources
						select ((ICloneable)s).Clone()).Cast<TTarget>();
			}
			return CopyConstructAll(sources, FactoryProvider.Factory);
		}

		/// <summary>
		///   Copies all source objects into new instances of target type TTarget.
		/// </summary>
		/// <param name="sources">the source</param>
		/// <param name="factory">a factory</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <returns>instances of TTarget, populated with data from the sources.</returns>
		public static IEnumerable<TTarget> CopyConstructAll<TSource>(IEnumerable<TSource> sources, IFactory factory)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var cloneable = typeof(ICloneable).IsAssignableFrom(typeof(TSource));
			if (cloneable && typeof(TTarget).IsAssignableFrom(typeof(TSource)))
			{
				return (from s in sources
						select ((ICloneable)s).Clone()).Cast<TTarget>();
			}
			return from s in sources
				   select CopyConstruct(s, factory);
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1>(IFactory factory, TS1 s1)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);

			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2>(IFactory factory, TS1 s1, TS2 s2)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3>(IFactory factory, TS1 s1, TS2 s2, TS3 s3)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <param name="s4">the fourth source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <typeparam name="TS4">fourth source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3, TS4>(IFactory factory, TS1 s1, TS2 s2, TS3 s3, TS4 s4)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			var c4 = factory.CreateInstance<ICopier<TS4, TTarget>>();
			c4.CopyTo(res, s4, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <param name="s4">the fourth source</param>
		/// <param name="s5">the fifth source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <typeparam name="TS4">fourth source's type</typeparam>
		/// <typeparam name="TS5">fifth source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3, TS4, TS5>(IFactory factory, TS1 s1, TS2 s2, TS3 s3, TS4 s4, TS5 s5)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			var c4 = factory.CreateInstance<ICopier<TS4, TTarget>>();
			c4.CopyTo(res, s4, CopyKind.Loose, factory);
			var c5 = factory.CreateInstance<ICopier<TS5, TTarget>>();
			c5.CopyTo(res, s5, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <param name="s4">the fourth source</param>
		/// <param name="s5">the fifth source</param>
		/// <param name="s6">the sixth source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <typeparam name="TS4">fourth source's type</typeparam>
		/// <typeparam name="TS5">fifth source's type</typeparam>
		/// <typeparam name="TS6">sixth source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3, TS4, TS5, TS6>(IFactory factory, TS1 s1, TS2 s2, TS3 s3, TS4 s4,
			TS5 s5, TS6 s6)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			var c4 = factory.CreateInstance<ICopier<TS4, TTarget>>();
			c4.CopyTo(res, s4, CopyKind.Loose, factory);
			var c5 = factory.CreateInstance<ICopier<TS5, TTarget>>();
			c5.CopyTo(res, s5, CopyKind.Loose, factory);
			var c6 = factory.CreateInstance<ICopier<TS6, TTarget>>();
			c6.CopyTo(res, s6, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <param name="s4">the fourth source</param>
		/// <param name="s5">the fifth source</param>
		/// <param name="s6">the sixth source</param>
		/// <param name="s7">the seventh source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <typeparam name="TS4">fourth source's type</typeparam>
		/// <typeparam name="TS5">fifth source's type</typeparam>
		/// <typeparam name="TS6">sixth source's type</typeparam>
		/// <typeparam name="TS7">seventh source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3, TS4, TS5, TS6, TS7>(IFactory factory, TS1 s1, TS2 s2, TS3 s3,
			TS4 s4, TS5 s5, TS6 s6, TS7 s7)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			var c4 = factory.CreateInstance<ICopier<TS4, TTarget>>();
			c4.CopyTo(res, s4, CopyKind.Loose, factory);
			var c5 = factory.CreateInstance<ICopier<TS5, TTarget>>();
			c5.CopyTo(res, s5, CopyKind.Loose, factory);
			var c6 = factory.CreateInstance<ICopier<TS6, TTarget>>();
			c6.CopyTo(res, s6, CopyKind.Loose, factory);
			var c7 = factory.CreateInstance<ICopier<TS7, TTarget>>();
			c7.CopyTo(res, s7, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <param name="s4">the fourth source</param>
		/// <param name="s5">the fifth source</param>
		/// <param name="s6">the sixth source</param>
		/// <param name="s7">the seventh source</param>
		/// <param name="s8">the eigth source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <typeparam name="TS4">fourth source's type</typeparam>
		/// <typeparam name="TS5">fifth source's type</typeparam>
		/// <typeparam name="TS6">sixth source's type</typeparam>
		/// <typeparam name="TS7">seventh source's type</typeparam>
		/// <typeparam name="TS8">eigth source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3, TS4, TS5, TS6, TS7, TS8>(IFactory factory, TS1 s1, TS2 s2, TS3 s3,
			TS4 s4, TS5 s5, TS6 s6, TS7 s7, TS8 s8)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			var c4 = factory.CreateInstance<ICopier<TS4, TTarget>>();
			c4.CopyTo(res, s4, CopyKind.Loose, factory);
			var c5 = factory.CreateInstance<ICopier<TS5, TTarget>>();
			c5.CopyTo(res, s5, CopyKind.Loose, factory);
			var c6 = factory.CreateInstance<ICopier<TS6, TTarget>>();
			c6.CopyTo(res, s6, CopyKind.Loose, factory);
			var c7 = factory.CreateInstance<ICopier<TS7, TTarget>>();
			c7.CopyTo(res, s7, CopyKind.Loose, factory);
			var c8 = factory.CreateInstance<ICopier<TS8, TTarget>>();
			c8.CopyTo(res, s8, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Copies each source's properties to a new instance of target type TTarget.
		/// </summary>
		/// <param name="factory">a factory</param>
		/// <param name="s1">the first source</param>
		/// <param name="s2">the second source</param>
		/// <param name="s3">the third source</param>
		/// <param name="s4">the fourth source</param>
		/// <param name="s5">the fifth source</param>
		/// <param name="s6">the sixth source</param>
		/// <param name="s7">the seventh source</param>
		/// <param name="s8">the eigth source</param>
		/// <param name="s9">the ninth source</param>
		/// <typeparam name="TS1">first source's type</typeparam>
		/// <typeparam name="TS2">second source's type</typeparam>
		/// <typeparam name="TS3">thrid source's type</typeparam>
		/// <typeparam name="TS4">fourth source's type</typeparam>
		/// <typeparam name="TS5">fifth source's type</typeparam>
		/// <typeparam name="TS6">sixth source's type</typeparam>
		/// <typeparam name="TS7">seventh source's type</typeparam>
		/// <typeparam name="TS8">eigth source's type</typeparam>
		/// <typeparam name="TS9">ninth source's type</typeparam>
		/// <returns>An instance of TTarget, populated with data from the sources.</returns>
		public static TTarget CopyEachSource<TS1, TS2, TS3, TS4, TS5, TS6, TS7, TS8, TS9>(IFactory factory, TS1 s1, TS2 s2,
			TS3 s3, TS4 s4, TS5 s5, TS6 s6, TS7 s7, TS8 s8, TS9 s9)
		{
			Contract.Requires<ArgumentNullException>(factory != null);
			var c1 = factory.CreateInstance<ICopier<TS1, TTarget>>();
			var res = factory.CreateInstance<TTarget>();
			c1.CopyTo(res, s1, CopyKind.Loose, factory);
			var c2 = factory.CreateInstance<ICopier<TS2, TTarget>>();
			c2.CopyTo(res, s2, CopyKind.Loose, factory);
			var c3 = factory.CreateInstance<ICopier<TS3, TTarget>>();
			c3.CopyTo(res, s3, CopyKind.Loose, factory);
			var c4 = factory.CreateInstance<ICopier<TS4, TTarget>>();
			c4.CopyTo(res, s4, CopyKind.Loose, factory);
			var c5 = factory.CreateInstance<ICopier<TS5, TTarget>>();
			c5.CopyTo(res, s5, CopyKind.Loose, factory);
			var c6 = factory.CreateInstance<ICopier<TS6, TTarget>>();
			c6.CopyTo(res, s6, CopyKind.Loose, factory);
			var c7 = factory.CreateInstance<ICopier<TS7, TTarget>>();
			c7.CopyTo(res, s7, CopyKind.Loose, factory);
			var c8 = factory.CreateInstance<ICopier<TS8, TTarget>>();
			c8.CopyTo(res, s8, CopyKind.Loose, factory);
			var c9 = factory.CreateInstance<ICopier<TS9, TTarget>>();
			c9.CopyTo(res, s9, CopyKind.Loose, factory);
			return res;
		}

		/// <summary>
		///   Performs a loose copy from source to target.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="factory"></param>
		public static void LooseCopyTo<TSource>(TTarget target, TSource source, IFactory factory)
		{
			Contract.Requires<ArgumentNullException>(factory != null);

			if (factory.CanConstruct<ICopier<TSource, TTarget>>())
			{
				var copier = factory.CreateInstance<ICopier<TSource, TTarget>>();
				copier.CopyTo(target, source, CopyKind.Loose, factory);
			}
			else
			{
				CopyHelper<TTarget>.LooseCopyTo(target, source, factory);
			}
		}

		
	}
}