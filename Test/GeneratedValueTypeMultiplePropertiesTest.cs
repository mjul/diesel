using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedValueTypeMultiplePropertiesTest
    {
        // Generate the class to test and place it in the Generated folder:
        // (defvaluetype EmployeeName (string FirstName, string LastName))

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            const string first = "Joe";
            const string last = "User";
            var actual = new Generated.EmployeeName(first, last);
            Assert.That(actual.FirstName, Is.EqualTo(first));
            Assert.That(actual.LastName, Is.EqualTo(last));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            const string first = "Joe";
            const string last = "User";

            var a = new Generated.EmployeeName(first, last);
            var b = new Generated.EmployeeName(first, last);
            var c = new Generated.EmployeeName(first, last);

            var otherFirstName = new Generated.EmployeeName(first + "x", last);
            var otherLastName = new Generated.EmployeeName(first, "von " + last);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherFirstName, otherLastName);
            EqualityTesting.TestEqualityOperators(a, b, c, otherFirstName, otherLastName);
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatter_ShouldBeSerializable()
        {
            const int number = 1;
            var actual = new Generated.EmployeeNumber(number);
            
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(actual);
            
            Assert.That(deserialized, Is.EqualTo(actual));
            Assert.That(actual.Value, Is.EqualTo(number));
        }
    }
}