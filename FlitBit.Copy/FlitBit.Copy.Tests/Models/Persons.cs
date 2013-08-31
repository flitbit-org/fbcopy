using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlitBit.Copy.Tests.Models
{
	public interface IPerson
	{
		int Age { get; set; }
		string Name { get; set; }
		float Weight { get; set; }
	}

	public class Person : IPerson
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public float Weight { get; set; }
	}

	public interface ISneakyPerson : IPerson
	{
		bool IsSneaky { get; set; }
	}

	public class SneakyPerson : Person, ISneakyPerson
	{
		public bool IsSneaky { get; set; }
	}

	public interface INinja : ISneakyPerson
	{
		int Rank { get; set; }
	}

	public class Ninja : SneakyPerson, INinja
	{
		public int Rank { get; set; }
	}

}
