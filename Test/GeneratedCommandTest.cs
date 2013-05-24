using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generated;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedCommandTest
    {
        // Generate the class to test and place it in the Generated folder:
        // (defcommand ImportEmployeeCommand (int EmployeeNumber, string FirstName, string LastName))

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            const int number = 1;
            const string first = "Alice";
            const string last = "von Lastname";

            var actual = new ImportEmployeeCommand(number, first, last);

            Assert.That(actual.EmployeeNumber, Is.EqualTo(number));
            Assert.That(actual.FirstName, Is.EqualTo(first));
            Assert.That(actual.LastName, Is.EqualTo(last));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            const int number = 1;
            const string first = "Alice";
            const string last = "von Lastname";

            var a = new ImportEmployeeCommand(number, first, last);
            var b = new ImportEmployeeCommand(number, first, last);
            var c = new ImportEmployeeCommand(number, first, last);

            var otherEmployeeNumber = new ImportEmployeeCommand(number + 1, first, last);
            var otherFirstName = new ImportEmployeeCommand(number, first + "x", last);
            var otherFirstNameNull = new ImportEmployeeCommand(number, null, last);
            var otherLastName = new ImportEmployeeCommand(number, first, last + "x");
            var otherLastNameNull = new ImportEmployeeCommand(number, first, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                otherEmployeeNumber, otherFirstName, otherFirstNameNull,
                otherLastName, otherLastNameNull);

            EqualityTesting.TestEqualityOperators(a, b, c,
                otherEmployeeNumber, otherFirstName, otherFirstNameNull,
                otherLastName, otherLastNameNull);
        }
    }
}
