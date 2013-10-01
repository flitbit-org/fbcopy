using FlitBit.Core;
using FlitBit.Core.Factory;
using FlitBit.Emit;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace FlitBit.Copy
{
	internal abstract class CopyHelper
	{
		internal abstract void UntypedLooseCopyTo(object target, object source, IFactory factory);
	}

	internal static class CopyHelper<TTarget>
	{
		// ReSharper disable once StaticFieldInGenericType
		private static readonly ConcurrentDictionary<object, CopyHelper> Helpers = new ConcurrentDictionary<object, CopyHelper>();

		public static void LooseCopyTo<TSource>(TTarget target, TSource source, IFactory factory)
		{
			Contract.Requires<ArgumentNullException>(factory != null);

			var sourceType = source.GetType();
			var sourceKey = sourceType.GetKeyForType();
			var helper = Helpers.GetOrAdd(sourceKey, k =>
			{
				var helperType = typeof(LooseCopyHelper<>).MakeGenericType(typeof(TTarget), sourceType);
				return (CopyHelper)Activator.CreateInstance(helperType);
			});
			helper.UntypedLooseCopyTo(target, source, factory);
		}

		class LooseCopyHelper<TSource> : CopyHelper
		{
			readonly Lazy<Action<TTarget, TSource, IFactory>> _looseCopy = new Lazy<Action<TTarget, TSource, IFactory>>(
				GeneratePerformLooseCopy, LazyThreadSafetyMode.ExecutionAndPublication);

			internal override void UntypedLooseCopyTo(object target, object source, IFactory factory)
			{
				_looseCopy.Value((TTarget)target, (TSource)source, factory);
			}

			static Action<TTarget, TSource, IFactory> GeneratePerformLooseCopy()
			{
				var targetType = typeof(TTarget);
				var method = new DynamicMethod(String.Concat("LooseCopyTo", targetType.Name),
					MethodAttributes.Public | MethodAttributes.Static,
					CallingConventions.Standard,
					null,
					new[] { typeof(TTarget), typeof(TSource), typeof(IFactory) },
					typeof(TSource),
					false);
				var il = method.GetILGenerator();
				il.DeclareLocal(typeof(TSource));

				il.Nop();

				var props =
					(from src in typeof(TSource).GetReadablePropertiesFromHierarchy(BindingFlags.Instance | BindingFlags.Public)
					 join dest in typeof(TTarget).GetWritablePropertiesFromHierarchy(BindingFlags.Instance | BindingFlags.Public)
						 on src.Name equals dest.Name
					 select new
					 {
						 Source = src,
						 Destination = dest
					 }).ToArray();
				foreach (var prop in props)
				{
					if (!prop.Destination.PropertyType.IsAssignableFrom(prop.Source.PropertyType)) continue;
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
					//else if (prop.Destination.PropertyType.IsDefined(typeof(ModelAttribute), false))
					//{

					//}
				}
				il.Return();

				// Create the delegate
				return (Action<TTarget, TSource, IFactory>)method.CreateDelegate(typeof(Action<TTarget, TSource, IFactory>));
			}			
		}
	}
}
