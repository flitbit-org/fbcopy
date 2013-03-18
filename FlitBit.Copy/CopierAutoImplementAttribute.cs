#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.

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
			var args = typeof(T).GetGenericArguments();
			var source = args[0];
			var target = args[1];

			if (source.IsAnonymousType())
			{
				var anon = typeof(AnonymousSourceCopier<,>).MakeGenericType(source, target);
				complete(null, () => (T) Activator.CreateInstance(anon));
			}
			else
			{
				complete(CopierTypeFactory.ConcreteType(source, target), null);
			}
			return true;
		}
	}
}