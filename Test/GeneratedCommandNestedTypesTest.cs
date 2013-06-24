using System;
using NUnit.Framework;
using Test.Diesel.Generated;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedCommandNestedDtosTest
    {
        [Test]
        public void EqualsAndGetHashCode()
        {
            var id = Guid.NewGuid();
            const int number = 1;
            var name = new Name("Alice", "Crypto");
            const Gender gender = Gender.Female;

            var a = new ImportEmployeeNestedTypes(id, number, name, gender);
            var b = new ImportEmployeeNestedTypes(id, number, name, gender);
            var c = new ImportEmployeeNestedTypes(id, number, name, gender);

            var otherId = new ImportEmployeeNestedTypes(Guid.NewGuid(), number, name, gender);
            var otherNumber = new ImportEmployeeNestedTypes(id, number + 1, name, gender);
            var otherNameNull = new ImportEmployeeNestedTypes(id, number, null, gender);
            var otherNameFirst = new ImportEmployeeNestedTypes(id, number, new Name("Bob", "Crypto"), gender);
            var otherNameLast = new ImportEmployeeNestedTypes(id, number, new Name("Alice", "Newlywed"), gender);
            var otherGender = new ImportEmployeeNestedTypes(id, number, name, Gender.Male);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherId, otherNumber, otherNameNull, otherNameFirst, otherNameLast, otherGender);
            EqualityTesting.TestEqualityOperators(a, b, c, otherId, otherNumber, otherNameNull, otherNameFirst, otherNameLast, otherGender);
        }

        [Test]
        public void Instance_WhenSerializedWithDataContractFormatter_ShouldBeSerializable()
        {
            var instance = CreateInstance();
            var deserialized = SerializationTesting.SerializeDeserializeWithDataContractSerializer(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void Instance_WhenSerializedWithBinarySerializer_ShouldBeSerializable()
        {
            var instance = CreateInstance();
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }


        private static ImportEmployeeNestedTypes CreateInstance()
        {
            var id = Guid.NewGuid();
            const int number = 1;
            var name = new Name("Alice", "Crypto");
            const Gender gender = Gender.Female;
            var instance = new ImportEmployeeNestedTypes(id, number, name, gender);
            return instance;
        }
    }
}