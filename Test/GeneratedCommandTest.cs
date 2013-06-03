using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
        // (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))

        private const int EmployeeNumber = 1;
        private const string FirstName = "Alice";
        private const string Lastname = "von Lastname";
        private const int SourceId = 100;

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            var actual = new ImportEmployee(EmployeeNumber, FirstName, Lastname, SourceId);

            Assert.That(actual.EmployeeNumber, Is.EqualTo(EmployeeNumber));
            Assert.That(actual.FirstName, Is.EqualTo(FirstName));
            Assert.That(actual.LastName, Is.EqualTo(Lastname));
            Assert.That(actual.SourceId, Is.EqualTo(SourceId));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var a = new ImportEmployee(EmployeeNumber, FirstName, Lastname, SourceId);
            var b = new ImportEmployee(EmployeeNumber, FirstName, Lastname, SourceId);
            var c = new ImportEmployee(EmployeeNumber, FirstName, Lastname, SourceId);

            var otherEmployeeNumber = new ImportEmployee(EmployeeNumber + 1, FirstName, Lastname, SourceId);
            var otherFirstName = new ImportEmployee(EmployeeNumber, FirstName + "x", Lastname, SourceId);
            var otherFirstNameNull = new ImportEmployee(EmployeeNumber, null, Lastname, SourceId);
            var otherLastName = new ImportEmployee(EmployeeNumber, FirstName, Lastname + "x", SourceId);
            var otherLastNameNull = new ImportEmployee(EmployeeNumber, FirstName, null, SourceId);
            var otherSourceId = new ImportEmployee(EmployeeNumber, FirstName, Lastname, SourceId + 1);
            var otherSourceIdNull = new ImportEmployee(EmployeeNumber, FirstName, Lastname, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                otherEmployeeNumber, otherFirstName, otherFirstNameNull,
                otherLastName, otherLastNameNull,
                otherSourceId, otherSourceIdNull);

            EqualityTesting.TestEqualityOperators(a, b, c,
                otherEmployeeNumber, otherFirstName, otherFirstNameNull,
                otherLastName, otherLastNameNull,
                otherSourceId, otherSourceIdNull);
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatter_ShouldBeSerializable()
        {
            var instance = new ImportEmployee(EmployeeNumber, FirstName, Lastname, SourceId);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatterNullField_ShouldBeSerializable()
        {
            var instance = new ImportEmployee(EmployeeNumber, FirstName, Lastname, null);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }


        [Test]
        public void Properties_Attributes_ShouldHaveOrderedDataMemberAttributes()
        {
            var getterProperties = typeof (Generated.ImportEmployee).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            Assert.That(GetDataMemberAttributeOrderValue(getterProperties, "EmployeeNumber"), Is.EqualTo(1));
            Assert.That(GetDataMemberAttributeOrderValue(getterProperties, "FirstName"), Is.EqualTo(2));
            Assert.That(GetDataMemberAttributeOrderValue(getterProperties, "LastName"), Is.EqualTo(3));
            Assert.That(GetDataMemberAttributeOrderValue(getterProperties, "SourceId"), Is.EqualTo(4));
        }

        private static object GetDataMemberAttributeOrderValue(PropertyInfo[] getProperties, string employeenumber)
        {
            var employeeNumber = getProperties.Single(p => p.Name == employeenumber);
            var dataMemberAttribute = employeeNumber.CustomAttributes
                                                    .Single(a => a.AttributeType == typeof (DataMemberAttribute));
            return dataMemberAttribute.NamedArguments.Single(a => a.MemberName == "Order").TypedValue.Value;
        }
    }
}
