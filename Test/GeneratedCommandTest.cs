using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Diesel.Generated;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedCommandTest
    {
        // Generate the class to test and place it in the Generated folder:
        // (defcommand ImportEmployeeCommand (int EmployeeNumber, string FirstName, string LastName))

        private const int EmployeeNumber = 1;
        private const string FirstName = "Alice";
        private const string Lastname = "von Lastname";

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            var actual = new ImportEmployeeCommand(EmployeeNumber, FirstName, Lastname);

            Assert.That(actual.EmployeeNumber, Is.EqualTo(EmployeeNumber));
            Assert.That(actual.FirstName, Is.EqualTo(FirstName));
            Assert.That(actual.LastName, Is.EqualTo(Lastname));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var a = new ImportEmployeeCommand(EmployeeNumber, FirstName, Lastname);
            var b = new ImportEmployeeCommand(EmployeeNumber, FirstName, Lastname);
            var c = new ImportEmployeeCommand(EmployeeNumber, FirstName, Lastname);

            var otherEmployeeNumber = new ImportEmployeeCommand(EmployeeNumber + 1, FirstName, Lastname);
            var otherFirstName = new ImportEmployeeCommand(EmployeeNumber, FirstName + "x", Lastname);
            var otherFirstNameNull = new ImportEmployeeCommand(EmployeeNumber, null, Lastname);
            var otherLastName = new ImportEmployeeCommand(EmployeeNumber, FirstName, Lastname + "x");
            var otherLastNameNull = new ImportEmployeeCommand(EmployeeNumber, FirstName, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                otherEmployeeNumber, otherFirstName, otherFirstNameNull,
                otherLastName, otherLastNameNull);

            EqualityTesting.TestEqualityOperators(a, b, c,
                otherEmployeeNumber, otherFirstName, otherFirstNameNull,
                otherLastName, otherLastNameNull);
        }

        [Test]
        public void Instance_WhenSerialized_ShouldBeSerializable()
        {
            var instance = new ImportEmployeeCommand(EmployeeNumber, FirstName, Lastname);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }
    }
}
