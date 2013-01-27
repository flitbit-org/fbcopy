#region COPYRIGHT© 2009-2013 Phillip Clark. All rights reserved.
// For licensing information see License.txt (MIT style licensing).
#endregion

using System;
using System.Reflection;
using FlitBit.Emit;
using FlitBit.IoC;
using FlitBit.IoC.Stereotype;

namespace FlitBit.Copy
{
	/// <summary>
	/// Used by the framework too wireup copier implementations.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
	public class CopierAutoImplementAttribute : StereotypeAttribute
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public CopierAutoImplementAttribute()
			: base(StereotypeBehaviors.AutoImplementedBehavior)
		{
		}
		
		/// <summary>
		/// Creates and registers a copier implementation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		/// <returns></returns>	
		public override bool RegisterStereotypeImplementation<T>(IoC.IContainer container)
		{
			RequireTypeIsInterface<T>();
			Type source = typeof(T).GetGenericArguments()[0];
			Type target = typeof(T).GetGenericArguments()[1];

			if (source.IsAnonymousType())
			{
				Type copierT = typeof(Copier<>).MakeGenericType(target);
				MethodInfo registerAnonymousSourceCopierForTarget = copierT.GetGenericMethod("RegisterAnonymousSourceCopierForTarget", BindingFlags.Static | BindingFlags.NonPublic, 0, 1).MakeGenericMethod(source);
				registerAnonymousSourceCopierForTarget.Invoke(null, Type.EmptyTypes);
			}
			else
			{
				Type concreteType = CopierTypeFactory.ConcreteType(source, target);

				Container.Root.ForType<T>()
					.Register(concreteType)
					.ResolveAnInstancePerRequest()
					.End();
			}
			return true;
		}
	}
}
