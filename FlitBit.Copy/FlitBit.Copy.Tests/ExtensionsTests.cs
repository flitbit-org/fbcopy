using System;
using System.Linq;
using FlitBit.Copy.Tests.Models;
using FlitBit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlitBit.Copy.Tests
{
	[TestClass]
	public class ExtensionsTests
	{
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
	}
}