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
	}
}