using FlitBit.Copy.Tests.Models;
using FlitBit.Core;
using FlitBit.Wireup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlitBit.Copy.Tests
{
	[TestClass]
	public class ExtensionsTests
	{
		[TestInitialize]
		public void Init()
		{
			WireupCoordinator.SelfConfigure();
			FactoryProvider.Factory.RegisterImplementationType<IPerson, Person>();
			FactoryProvider.Factory.RegisterImplementationType<ISneakyPerson, SneakyPerson>();
			FactoryProvider.Factory.RegisterImplementationType<INinja, Ninja>();
		}

		[TestMethod]
		public void Extensions_CanCopyFrom_InterfaceFromInterface()
		{
			var factory = FactoryProvider.Factory;

			var person = factory.CreateInstance<IPerson>();
			person.Name = "Bob";
			person.Age = 3;

			//System.ArgumentException: Invalid type owner for DynamicMethod.
			var otherBob = person.CopyFrom(person);
			Assert.AreEqual(person.Name, otherBob.Name);
			Assert.AreEqual(person.Age, otherBob.Age);
		}

		[TestMethod]
		public void Extensions_CanCopyTo_InterfaceToInterface()
		{
			var factory = FactoryProvider.Factory;

			var person = factory.CreateInstance<IPerson>();
			person.Name = "Bob";
			person.Age = 31;

			//System.ArgumentException: Invalid type owner for DynamicMethod.
			var otherBob = factory.CreateInstance<IPerson>();
			person.CopyTo(otherBob);

			Assert.AreEqual(person.Name, otherBob.Name);
			Assert.AreEqual(person.Age, otherBob.Age);
		}

		[TestMethod]
		public void Extensions_CanCopyTo_ConcreteToInterface()
		{
			var factory = FactoryProvider.Factory;

			var person = new Person()
			{
				Name = "Bob",
				Age = 31
			};

			//System.ArgumentException: Invalid type owner for DynamicMethod.
			var otherBob = factory.CreateInstance<IPerson>();
			person.CopyTo(otherBob);

			Assert.AreEqual(person.Name, otherBob.Name);
			Assert.AreEqual(person.Age, otherBob.Age);
		}

		[TestMethod]
		public void Extensions_CanCopyFrom()
		{
			var sources = new[]
			{
				new
				{
					Name = "Spartacus",
					Description = "Body was never found.",
					YearLastSeen = "71 BC"
				},
				new
				{
					Name = "Miguel Corte-Real",
					Description = "Disappeared while searching for his brother Gaspar.",
					YearLastSeen = "1502"
				},
				new
				{
					Name = "Susan Powell",
					Description = "Utah mother of two who disappeared from her home under suspicious circumstances.",
					YearLastSeen = "2009"
				}
			};
			var missing = Copier<MissingPerson>.CopyConstruct(sources[0]);
			Assert.AreEqual(sources[0].Name, missing.Name);
			Assert.AreEqual(sources[0].Description, missing.Description);
			Assert.AreEqual(sources[0].YearLastSeen, missing.YearLastSeen);

			Assert.IsNotNull(missing);
			for (var i = sources.Length - 1; i >= 0; i--)
			{
				missing.CopyFrom(sources[i]);

				Assert.AreEqual(sources[i].Name, missing.Name);
				Assert.AreEqual(sources[i].Description, missing.Description);
				Assert.AreEqual(sources[i].YearLastSeen, missing.YearLastSeen);
			}
		}

		[TestMethod]
		public void Extensions_CanCopyTo()
		{
			var sources = new[]
			{
				new MissingPerson
				{
					Name = "Antonio Villas Boas",
					Description = "Brazilian farmer abducted while whorking at night near Sao Francisco de Sales.",
					YearLastSeen = "1957"
				},
				new MissingPerson
				{
					Name = "Barney Hill",
					Description = "Abducted by extraterrestrials in rural New Hampshire.",
					YearLastSeen = "1961"
				},
				new MissingPerson
				{
					Name = "George Adamski",
					Description = "Ok, not an abductee, but witnessed a large cigar-shaped `mother ship`.",
					YearLastSeen = "1947"
				}
			};
			var missing = Copier<AbductedPerson>.CopyConstruct(sources[0]);
			Assert.IsNotNull(missing);
			Assert.AreEqual(sources[0].Name, missing.Name);
			Assert.AreEqual(sources[0].Description, missing.Description);
			Assert.AreNotEqual(sources[0].YearLastSeen, missing.YearAbducted);
			
			for (var i = sources.Length - 1; i >= 0; i--)
			{
				sources[i].CopyTo(missing);

				Assert.AreEqual(sources[i].Name, missing.Name);
				Assert.AreEqual(sources[i].Description, missing.Description);
				Assert.AreNotEqual(sources[i].YearLastSeen, missing.YearAbducted);
			}
		}

		[TestMethod]
		public void Extensions_CanCopyConstruct_InterfaceToInterface()
		{
			var factory = FactoryProvider.Factory;
			var copier = factory.CreateInstance<ICopier<INinja, IPerson>>();
			var ninja = new Ninja {Age = 5, IsSneaky = true, Name = "Sneeks", Rank = 5, Weight = 154};
			var copy = copier.CopyConstruct(ninja);
			Assert.IsInstanceOfType(copy, typeof (IPerson));
			Assert.IsNotInstanceOfType(copy, typeof (INinja));
			Assert.AreEqual(ninja.Name, copy.Name);
			Assert.AreEqual(ninja.Age, copy.Age);
			Assert.AreEqual(ninja.Weight, copy.Weight);
		}
	}
}