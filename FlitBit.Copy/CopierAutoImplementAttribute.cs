#region COPYRIGHT© 2009-2014 Phillip Clark. All rights reserved.

// For licensing information see License.txt (MIT style licensing).

#endregion

using System;
using FlitBit.Core.Factory;
using FlitBit.Core.Meta;
using FlitBit.Emit;

namespace FlitBit.Copy
{
	/// <summary>
	///   Used by the framework too wireup copier implementations.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class CopierAutoImplementAttribute : AutoImplementedAttribute
	{
		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public CopierAutoImplementAttribute()
		{
			RecommemdedScope = InstanceScopeKind.ContainerScope;
		}

		/// <summary>
		///   Generates an instance of ICopier&lt;,>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="factory">the requesting factory</param>
		/// <param name="complete">callback upon completion</param>
		/// <returns></returns>
		public override bool GetImplementation<T>(IFactory factory, Action<Type, Func<T>> complete)
		{
			return GetImplementation(factory, typeof(T), (type, functor) =>
			{
				if (type != null) complete(type, null);
				else complete(null, () => (T) functor());
			});
		}

		/// <summary>
		/// Gets the implementation for type
		/// </summary>
		/// <param name="factory">the factory from which the type was requested.</param><param name="type">the target types</param><param name="complete">callback invoked when the implementation is available</param>
		/// <returns>
		/// <em>true</em> if implemented; otherwise <em>false</em>.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">thrown if <paramref name="type"/> is not eligible for implementation</exception>
		/// <remarks>
		/// If the <paramref name="complete"/> callback is invoked, it must be given either an implementation type
		///               assignable to type T, or a factory function that creates implementations of type T.
		/// </remarks>
		public override bool GetImplementation(IFactory factory, Type type, Action<Type, Func<object>> complete)
		{
			var args = type.GetGenericArguments();
			var source = args[0];
			var target = args[1];

			if (source.IsAnonymousType())
			{
				var anon = typeof(AnonymousSourceCopier<,>).MakeGenericType(source, target);
				complete(null, () => Activator.CreateInstance(anon));
			}
			else
			{
				complete(CopierTypeFactory.ConcreteType(source, target), null);
			}
			return true;
		}
	}
}