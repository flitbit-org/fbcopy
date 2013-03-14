#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using System;
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
		public static TTarget CopyConstruct<TSource>(TSource source) { return CopyConstruct(source, FactoryProvider.Factory); }

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
			var res = factory.CreateInstance<TTarget>();
			LooseCopyHelper<TSource>.LooseCopyTo(res, source, factory);
			return res;
		}

		/// <summary>
		///   Copies all source objects into new instances of target type TTarget.
		/// </summary>
		/// <param name="sources">the source</param>
		/// <typeparam name="TSource">source type TSource</typeparam>
		/// <returns>instances of TTarget, populated with data from the sources.</returns>
		public static IEnumerable<TTarget> CopyConstructAll<TSource>(IEnumerable<TSource> sources) { return CopyConstructAll(sources, FactoryProvider.Factory); }

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
			return from s in sources
						select CopyConstruct(s, factory);
		}

		/// <summary>
		///   Performs a loose copy from source to target.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="factory"></param>
		public static void LooseCopyTo<TSource>(TTarget target, TSource source, IFactory factory) { LooseCopyHelper<TSource>.LooseCopyTo(target, source, factory); }

		static class LooseCopyHelper<TSource>
		{
			static readonly Lazy<Action<TTarget, TSource, IFactory>> LooseCopy = new Lazy<Action<TTarget, TSource, IFactory>>(
				GeneratePerformLooseCopy, LazyThreadSafetyMode.ExecutionAndPublication);

			internal static void LooseCopyTo(TTarget target, TSource source, IFactory factory)
			{
				Contract.Requires<ArgumentNullException>(factory != null);

				LooseCopy.Value(target, source, factory);
			}

			static Action<TTarget, TSource, IFactory> GeneratePerformLooseCopy()
			{
				var targetType = typeof(TTarget);
				var method = new DynamicMethod(String.Concat("LooseCopyTo", targetType.Name)
																			, MethodAttributes.Public | MethodAttributes.Static
																			, CallingConventions.Standard
																			, null
																			, new[] {typeof(TTarget), typeof(TSource), typeof(IFactory)}
																			, typeof(TSource)
																			, false
					);
				var il = method.GetILGenerator();
				il.DeclareLocal(typeof(TSource));

				il.Nop();

				var props =
					(from src in typeof(TSource).GetReadablePropertiesFromHierarchy(BindingFlags.Instance | BindingFlags.Public)
					join dest in typeof(TTarget).GetWritableProperties(BindingFlags.Instance | BindingFlags.Public)
						on src.Name equals dest.Name
					select new
					{
						Source = src,
						Destination = dest
					}).ToArray();
				foreach (var prop in props)
				{
					if (prop.Destination.PropertyType.IsAssignableFrom(prop.Source.PropertyType))
					{
						//
						// target.<property-name> = src.<property-name>;
						//
						il.LoadArg_0();
						il.LoadArg_1();
						var getter = prop.Source.GetGetMethod();
						var declaringType = getter.DeclaringType;
						if (getter.IsVirtual || (declaringType != null && declaringType.IsInterface))
						{
							il.CallVirtual(getter);
						}
						else
						{
							il.Call(getter);
						}

						var setter = prop.Destination.GetSetMethod();
						declaringType = setter.DeclaringType;
						if (setter.IsVirtual || (declaringType != null && declaringType.IsInterface))
						{
							il.CallVirtual(setter);
						}
						else
						{
							il.Call(setter);
						}

						il.Nop();
					}
					//else if (prop.Destination.PropertyType.IsDefined(typeof(ModelAttribute), false))
					//{

					//}
				}
				il.Return();

				// Create the delegate
				return (Action<TTarget, TSource, IFactory>) method.CreateDelegate(typeof(Action<TTarget, TSource, IFactory>));
			}
		}
	}
}