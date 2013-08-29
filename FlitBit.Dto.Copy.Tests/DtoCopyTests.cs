using System.Dynamic;
using FlitBit.Copy;
using FlitBit.IoC;
using FlitBit.Wireup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlitBit.Dto.Copy.Tests
{
    [DTO]
    public interface IPerson
    {
        string Name { get; set; }
        int Age { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [DTO]
    public interface INinja : IPerson
    {
        int AttackPower { get; set; }
    }

    [TestClass]
    public class DtoCopyTests
    {
        [TestInitialize]
        public void Init()
        {
            WireupCoordinator.SelfConfigure();
        }


        [TestMethod]
        public void It_Can_Copy_From_Dto_To_Dto()
        {
            using (var container = Create.SharedOrNewContainer())
            {
                var person = Create.NewInit<IPerson>().Init(new
                {
                    Name = "Bob",
                    Age = 3
                });

                //System.ArgumentException: Invalid type owner for DynamicMethod.
                var otherBob = person.CopyFrom(person);
                Assert.AreEqual(person.Name, otherBob.Name);
                Assert.AreEqual(person.Age, otherBob.Age);
            }
        }


        [TestMethod]
        public void It_Can_Copy_To_Dto_From_Dto()
        {
            using (var container = Create.SharedOrNewContainer())
            {
                var person = Create.NewInit<IPerson>().Init(new
                {
                    Name = "Bob",
                    Age = 31
                });

                //System.ArgumentException: Invalid type owner for DynamicMethod.
                var otherBob = Create.New<IPerson>();
                person.CopyTo(otherBob);

                Assert.AreEqual(person.Name, otherBob.Name);
                Assert.AreEqual(person.Age, otherBob.Age);
            }
        }

        [TestMethod]
        public void It_Can_Copy_From_Concrete_To_Dto()
        {
            using (var container = Create.SharedOrNewContainer())
            {
                var person = new Person()
                {
                    Name = "Bob",
                    Age = 31
                };

                //System.ArgumentException: Invalid type owner for DynamicMethod.
                var otherBob = Create.New<IPerson>();
                person.CopyTo(otherBob);

                Assert.AreEqual(person.Name, otherBob.Name);
                Assert.AreEqual(person.Age, otherBob.Age);
            }
        }
    }
}
