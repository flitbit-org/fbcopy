using System;
using System.Linq;
using FlitBit.Copy.Tests.Models;
using FlitBit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlitBit.Copy.Tests
{
	[TestClass]
	public class CopierTests
	{
		[TestMethod]
		public void CopierOfT_CanCopyConstructManyFromSource()
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
			var missing = Copier<MissingPerson>.CopyConstructAll(sources).ToArray();
			Assert.IsNotNull(missing);
			Assert.AreEqual(sources.Length, missing.Length);
			for (var i = 0; i < sources.Length; i++)
			{
				Assert.AreEqual(sources[i].Name, missing[i].Name);
				Assert.AreEqual(sources[i].Description, missing[i].Description);
				Assert.AreEqual(sources[i].YearLastSeen, missing[i].YearLastSeen);
			}
		}

		[TestMethod]
		public void CopierOfT_CanCopySource()
		{
			var sources = new[]
			{
				new MissingPerson
				{
					Name = "Antonio Villas Boas",
					Description = "Brazilian farmer abducted while working at night near Sao Francisco de Sales.",
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

			var copier = FactoryProvider.Factory.CreateInstance<ICopier<MissingPerson, AbductedPerson>>();

			for (var i = sources.Length - 1; i >= 0; i--)
			{
				copier.LooseCopy(missing, sources[i]);
				Assert.AreEqual(sources[i].Name, missing.Name);
				Assert.AreEqual(sources[i].Description, missing.Description);
				Assert.AreNotEqual(sources[i].YearLastSeen, missing.YearAbducted);
			}
		}

		[TestMethod]
		public void CopierOfT_CanStrictCopyWhenTargetHasAllTargetProperties()
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
			var missing = Copier<OtherPerson>.CopyConstruct(sources[0]);
			Assert.IsNotNull(missing);
			Assert.AreEqual(sources[0].Name, missing.Name);
			Assert.AreEqual(sources[0].Description, missing.Description);
			Assert.AreEqual(sources[0].YearLastSeen, missing.YearLastSeen);
			Assert.IsNull(missing.YearAbducted);

			var copier = FactoryProvider.Factory.CreateInstance<ICopier<MissingPerson, OtherPerson>>();

			for (var i = sources.Length - 1; i >= 0; i--)
			{
				copier.StrictCopy(missing, sources[i]);
				Assert.AreEqual(sources[i].Name, missing.Name);
				Assert.AreEqual(sources[i].Description, missing.Description);
				Assert.AreEqual(sources[i].YearLastSeen, missing.YearLastSeen);
				Assert.IsNull(missing.YearAbducted);
			}
		}

		[TestMethod, ExpectedException(typeof(InvalidOperationException))] 
		public void CopierOfT_ExceptionOnStrictCopyWhenTypesAreDissimilar()
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

			var copier = FactoryProvider.Factory.CreateInstance<ICopier<MissingPerson, AbductedPerson>>();

			for (var i = sources.Length - 1; i >= 0; i--)
			{
				copier.StrictCopy(missing, sources[i]);
				Assert.Fail("Should have blown up with an InvalidOperationException");
			}
		}

		[TestMethod]
		public void Copier_CanCopySourcesOne2Nine()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
				Five = new NumberedThings5 { Five = gen.GetDouble() },
				Six = new { Six = DateTime.Now },
				Seven = new NumberedThings7 { Seven = gen.GetGuid() },
				Eight = new NumberedThings8 { Eight = gen.GetInt64() },
				Nine = new NumberedThings9 { Nine = gen.GetInt16() }
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four, sources.Five, sources.Six, sources.Seven, sources.Eight, sources.Nine);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(sources.Six.Six, num.Six);
			Assert.AreEqual(sources.Seven.Seven, num.Seven);
			Assert.AreEqual(sources.Eight.Eight, num.Eight);
			Assert.AreEqual(sources.Nine.Nine, num.Nine);
		}

		[TestMethod]
		public void Copier_CanCopySourcesOne2Eight()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
				Five = new NumberedThings5 { Five = gen.GetDouble() },
				Six = new NumberedThings6 { Six = DateTime.Now },
				Seven = new NumberedThings7 { Seven = gen.GetGuid() },
				Eight = new NumberedThings8 { Eight = gen.GetInt64() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four, sources.Five, sources.Six, sources.Seven, sources.Eight);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(sources.Six.Six, num.Six);
			Assert.AreEqual(sources.Seven.Seven, num.Seven);
			Assert.AreEqual(sources.Eight.Eight, num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}

		[TestMethod]
		public void Copier_CanCopySourcesOne2Seven()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
				Five = new NumberedThings5 { Five = gen.GetDouble() },
				Six = new NumberedThings6 { Six = DateTime.Now },
				Seven = new NumberedThings7 { Seven = gen.GetGuid() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four, sources.Five, sources.Six, sources.Seven);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(sources.Six.Six, num.Six);
			Assert.AreEqual(sources.Seven.Seven, num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanCopySourcesOne2Six()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
				Five = new { Five = gen.GetDouble() },
				Six = new NumberedThings6 { Six = DateTime.Now },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four, sources.Five, sources.Six);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(sources.Six.Six, num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanCopySourcesOne2Five()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
				Five = new NumberedThings5 { Five = gen.GetDouble() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four, sources.Five);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanCopySourcesOne2Four()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(default(double), num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanCopySourcesOne2Three()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(default(decimal), num.Four);
			Assert.AreEqual(default(double), num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanCopySourcesOne2Two()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(default(int), num.Three);
			Assert.AreEqual(default(decimal), num.Four);
			Assert.AreEqual(default(double), num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanCopySourcesOne()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) }
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(default(byte), num.Two);
			Assert.AreEqual(default(int), num.Three);
			Assert.AreEqual(default(decimal), num.Four);
			Assert.AreEqual(default(double), num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(default(long), num.Eight);
			Assert.AreEqual(default(short), num.Nine);
		}
		[TestMethod]
		public void Copier_CanPartialFillFromDifferentSources()
		{
			var gen = new DataGenerator();
			var sources = new
			{
				Four = new NumberedThings4 { Four = gen.GetDecimal() },
				Five = new NumberedThings5 { Five = gen.GetDouble() },
				Eight = new NumberedThings8 { Eight = gen.GetInt64() },
				Nine = new NumberedThings9 { Nine = gen.GetInt16() }
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.Four, sources.Five, sources.Eight, sources.Nine);
			Assert.IsNotNull(num);
			// Four, Five, Eight and Nine should have been copied from the different objects...
			Assert.AreEqual(default(string), num.One);
			Assert.AreEqual(default(byte), num.Two);
			Assert.AreEqual(default(int), num.Three);
			Assert.AreEqual(sources.Four.Four, num.Four);
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(default(Guid), num.Seven);
			Assert.AreEqual(sources.Eight.Eight, num.Eight);
			Assert.AreEqual(sources.Nine.Nine, num.Nine);
		}						

		[TestMethod]
		public void Copier_CopyRespectsLastSourceWritten()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Seven = new NumberedThings7 { Seven = gen.GetGuid() },
				Eight = new NumberedThings8 { Eight = gen.GetInt64() },
				Nine = new NumberedThings9 { Nine = gen.GetInt16() },
				Nine1 = new NumberedThings9 { Nine = gen.GetInt16() },
				Nine2 = new NumberedThings9 { Nine = gen.GetInt16() },
				Nine3 = new NumberedThings9 { Nine = gen.GetInt16() },
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings123456789>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Seven, sources.Eight, sources.Nine, sources.Nine1, sources.Nine2, sources.Nine3);
			Assert.IsNotNull(num);

			// The Nines overwrite eachother, the last one given will set the value...
			Assert.AreEqual(sources.One.One, num.One);
			Assert.AreEqual(sources.Two.Two, num.Two);
			Assert.AreEqual(sources.Three.Three, num.Three);
			Assert.AreEqual(default(Decimal), num.Four);
			Assert.AreEqual(default(double), num.Five);
			Assert.AreEqual(default(DateTime), num.Six);
			Assert.AreEqual(sources.Seven.Seven, num.Seven);
			Assert.AreEqual(sources.Eight.Eight, num.Eight);
			Assert.AreEqual(sources.Nine3.Nine, num.Nine);
		}

		[TestMethod]
		public void Copier_CanOverfillSmallerObject()
		{
			var ran = new Random(Environment.TickCount);
			var gen = new DataGenerator();
			var sources = new
			{
				One = new NumberedThings1 { One = gen.GetString(ran.Next(20, 200)) },
				Two = new NumberedThings2 { Two = gen.GetByte() },
				Three = new NumberedThings3 { Three = gen.GetInt32() },
				Four = new { Four = gen.GetDecimal() },
				Five = new NumberedThings5 { Five = gen.GetDouble() },
				Six = new NumberedThings6 { Six = DateTime.Now },
				Seven = new NumberedThings7 { Seven = gen.GetGuid() },
				Eight = new NumberedThings8 { Eight = gen.GetInt64() },
				Nine = new NumberedThings9 { Nine = gen.GetInt16() }
			};
			var factory = FactoryProvider.Factory;
			var num = Copier<NumberedThings567>.CopyEachSource(factory, sources.One, sources.Two, sources.Three,
																																	sources.Four, sources.Five, sources.Six, sources.Seven, sources.Eight, sources.Nine);
			Assert.IsNotNull(num);
			// Each of the properties should have been copied from the different objects...
			Assert.AreEqual(sources.Five.Five, num.Five);
			Assert.AreEqual(sources.Six.Six, num.Six);
			Assert.AreEqual(sources.Seven.Seven, num.Seven);
		}
	}
}