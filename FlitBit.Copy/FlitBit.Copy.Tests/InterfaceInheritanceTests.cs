using FlitBit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlitBit.Copy.Tests
{
	[TestClass]
	public class InterfaceInheritanceTests
	{
		[TestMethod]
		public void CopyTo_Human_From_Anonymous_Should_Copy_Properly()
		{
			const string name = "Billy Maddison";
			const int age = 30;
			const float weight = 170.5f;

			IHuman human = new Human();
			Copier<IHuman>.LooseCopyTo(human, new
			{
				Name = name,
				Age = age,
				Weight = weight
			}, FactoryProvider.Factory);

			Assert.AreEqual(name, human.Name);
			Assert.AreEqual(age, human.Age);
			Assert.AreEqual(weight, human.Weight);
		}

		[TestMethod]
		public void CopyTo_Ninja_From_Anonymous_Should_Copy_Properly()
		{
			const string name = "Billy Maddison";
			const int age = 30;
			const float weight = 170.5f;
			const bool isSneaky = true;
			const int rank = 1;

			INinja ninja = new Ninja();
			Copier<INinja>.LooseCopyTo(ninja, new
			{
				Name = name,
				Age = age,
				Weight = weight,
				IsSneaky = isSneaky,
				Rank = rank
			}, FactoryProvider.Factory);

			Assert.AreEqual(name, ninja.Name);
			Assert.AreEqual(age, ninja.Age);
			Assert.AreEqual(weight, ninja.Weight);
			Assert.IsTrue(ninja.IsSneaky);
			Assert.AreEqual(rank, ninja.Rank);
		}

		[TestMethod]
		public void CopyTo_SneakyHuman_From_Anonymous_Should_Copy_Properly()
		{
			const string name = "Billy Maddison";
			const int age = 30;
			const float weight = 170.5f;
			const bool isSneaky = true;

			ISneakyHuman sneakyHuman = new SneakyHuman();
			Copier<ISneakyHuman>.LooseCopyTo(sneakyHuman, new
			{
				Name = name,
				Age = age,
				Weight = weight,
				IsSneaky = isSneaky
			}, FactoryProvider.Factory);

			Assert.AreEqual(name, sneakyHuman.Name);
			Assert.AreEqual(age, sneakyHuman.Age);
			Assert.AreEqual(weight, sneakyHuman.Weight);
			Assert.IsTrue(sneakyHuman.IsSneaky);
		}

		[TestInitialize]
		public void Init()
		{
			FactoryProvider.Factory.RegisterImplementationType<IHuman, Human>();
			FactoryProvider.Factory.RegisterImplementationType<ISneakyHuman, SneakyHuman>();
			FactoryProvider.Factory.RegisterImplementationType<INinja, Ninja>();
		}
	}

	public interface IHuman
	{
		int Age { get; set; }
		string Name { get; set; }
		float Weight { get; set; }
	}

	public class Human : IHuman
	{
		#region IHuman Members

		public string Name { get; set; }
		public int Age { get; set; }
		public float Weight { get; set; }

		#endregion
	}

	public interface ISneakyHuman : IHuman
	{
		bool IsSneaky { get; set; }
	}

	public class SneakyHuman : Human, ISneakyHuman
	{
		#region ISneakyHuman Members

		public bool IsSneaky { get; set; }

		#endregion
	}

	public interface INinja : ISneakyHuman
	{
		int Rank { get; set; }
	}

	public class Ninja : SneakyHuman, INinja
	{
		#region INinja Members

		public int Rank { get; set; }

		#endregion
	}
}