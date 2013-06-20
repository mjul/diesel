using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Test.Diesel.Generated;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedDtoTest
    {
        // Generate the class to test and place it in the Generated folder (or use the T4 template)
        // (defdto Name (string First, string Last))

        private const string FirstName = "Alice";
        private const string Lastname = "von Lastname";

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            var actual = new Name(FirstName, Lastname);

            Assert.That(actual.First, Is.EqualTo(FirstName));
            Assert.That(actual.Last, Is.EqualTo(Lastname));
        }

        [Test]
        public void Type_ShouldImplementIEquatableInterface()
        {
            Assert.That(typeof(IEquatable<EmployeeImported>).IsAssignableFrom(typeof(EmployeeImported)));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var a = new Name(FirstName, Lastname);
            var b = new Name(FirstName, Lastname);
            var c = new Name(FirstName, Lastname);

            var otherFirstName = new Name(FirstName + "x", Lastname);
            var otherFirstNameNull = new Name(null, Lastname);
            var otherLastName = new Name(FirstName, Lastname + "x");
            var otherLastNameNull = new Name(FirstName, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                                                     otherFirstName, otherFirstNameNull,
                                                     otherLastName, otherLastNameNull);

            EqualityTesting.TestEqualityOperators(a, b, c,
                                                  otherFirstName, otherFirstNameNull,
                                                  otherLastName, otherLastNameNull);
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatter_ShouldBeSerializable()
        {
            var instance = new Name(FirstName, Lastname);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatterNullField_ShouldBeSerializable()
        {
            var instance = new Name(FirstName, Lastname);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void Instance_WhenSerializedWithDataContractFormatter_ShouldBeSerializable()
        {
            var instance = new Name(FirstName, Lastname);
            var deserialized = SerializationTesting.SerializeDeserializeWithDataContractSerializer(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void BackingField_Attributes_ShouldHaveOrderedDataMemberAttributes()
        {
            var backingFields = typeof(Generated.Name).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_first"), Is.EqualTo(1));
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_last"), Is.EqualTo(2));
        }

        private static object GetDataMemberAttributeOrderValue(FieldInfo[] fields, string fieldName)
        {
            var employeeNumber = fields.Single(p => p.Name == fieldName);
            var dataMemberAttribute = employeeNumber.CustomAttributes
                                                    .Single(a => a.AttributeType == typeof (DataMemberAttribute));
            return dataMemberAttribute.NamedArguments.Single(a => a.MemberName == "Order").TypedValue.Value;
        }
    }
}