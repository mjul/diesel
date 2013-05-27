using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedValueTypeTest
    {
        // Generate the class to test and place it in the Generated folder:
        // (defvaluetype EmployeeNumber int)

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            const int number = 1;
            var actual = new Generated.EmployeeNumber(number);
            Assert.That(actual.Value, Is.EqualTo(number));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            const int number = 1;

            var a = new Generated.EmployeeNumber(number);
            var b = new Generated.EmployeeNumber(number);
            var c = new Generated.EmployeeNumber(number);

            var otherEmployeeNumber = new Generated.EmployeeNumber(number + 1);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherEmployeeNumber);

            EqualityTesting.TestEqualityOperators(a, b, c, otherEmployeeNumber);
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