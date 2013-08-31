using FlitBit.Copy.Tests.Models;
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

			IPerson person = new Person();
			Copier<IPerson>.LooseCopyTo(person, new
			{
				Name = name,
				Age = age,
				Weight = weight
			}, FactoryProvider.Factory);

			Assert.AreEqual(name, person.Name);
			Assert.AreEqual(age, person.Age);
			Assert.AreEqual(weight, person.Weight);
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

			ISneakyPerson sneakyPerson = new SneakyPerson();
			Copier<ISneakyPerson>.LooseCopyTo(sneakyPerson, new
			{
				Name = name,
				Age = age,
				Weight = weight,
				IsSneaky = isSneaky
			}, FactoryProvider.Factory);

			Assert.AreEqual(name, sneakyPerson.Name);
			Assert.AreEqual(age, sneakyPerson.Age);
			Assert.AreEqual(weight, sneakyPerson.Weight);
			Assert.IsTrue(sneakyPerson.IsSneaky);
		}

		[TestInitialize]
		public void Init()
		{
			FactoryProvider.Factory.RegisterImplementationType<IPerson, Person>();
			FactoryProvider.Factory.RegisterImplementationType<ISneakyPerson, SneakyPerson>();
			FactoryProvider.Factory.RegisterImplementationType<INinja, Ninja>();
		}
	}

	
}