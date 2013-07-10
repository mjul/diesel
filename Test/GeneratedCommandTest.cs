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
        // (defcommand ImportEmployee (Guid CommandId, int EmployeeNumber, string FirstName, string LastName, int? SourceId))

        private static readonly Guid CommandId = Guid.NewGuid();
        private const int EmployeeNumber = 1;
        private const string FirstName = "Alice";
        private const string Lastname = "von Lastname";
        private const int SourceId = 100;

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            var actual = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId);

            Assert.That(actual.CommandId, Is.EqualTo(CommandId));
            Assert.That(actual.EmployeeNumber, Is.EqualTo(EmployeeNumber));
            Assert.That(actual.FirstName, Is.EqualTo(FirstName));
            Assert.That(actual.LastName, Is.EqualTo(Lastname));
            Assert.That(actual.SourceId, Is.EqualTo(SourceId));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var a = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId);
            var b = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId);
            var c = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId);

            var otherCommandId = new ImportEmployee(Guid.NewGuid(), EmployeeNumber + 1, FirstName, Lastname, SourceId);
            var otherEmployeeNumber = new ImportEmployee(CommandId, EmployeeNumber + 1, FirstName, Lastname, SourceId);
            var otherFirstName = new ImportEmployee(CommandId, EmployeeNumber, FirstName + "x", Lastname, SourceId);
            var otherFirstNameNull = new ImportEmployee(CommandId, EmployeeNumber, null, Lastname, SourceId);
            var otherLastName = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname + "x", SourceId);
            var otherLastNameNull = new ImportEmployee(CommandId, EmployeeNumber, FirstName, null, SourceId);
            var otherSourceId = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId + 1);
            var otherSourceIdNull = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                otherCommandId,
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
            var instance = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatterNullField_ShouldBeSerializable()
        {
            var instance = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, null);
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void Instance_WhenSerializedWithDataContractFormatter_ShouldBeSerializable()
        {
            var instance = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, SourceId);
            var deserialized = SerializationTesting.SerializeDeserializeWithDataContractSerializer(instance);
            Assert.That(deserialized, Is.EqualTo(instance));
        }

        [Test]
        public void BackingField_Attributes_ShouldHaveOrderedDataMemberAttributes()
        {
            var backingFields = typeof (Generated.ImportEmployee).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_commandId"), Is.EqualTo(1));
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_employeeNumber"), Is.EqualTo(2));
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_firstName"), Is.EqualTo(3));
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_lastName"), Is.EqualTo(4));
            Assert.That(GetDataMemberAttributeOrderValue(backingFields, "_sourceId"), Is.EqualTo(5));
        }

        private static object GetDataMemberAttributeOrderValue(FieldInfo[] fields, string fieldName)
        {
            var employeeNumber = fields.Single(p => p.Name == fieldName);
            var dataMemberAttribute = employeeNumber.CustomAttributes
                                                    .Single(a => a.AttributeType == typeof (DataMemberAttribute));
            return dataMemberAttribute.NamedArguments.Single(a => a.MemberName == "Order").TypedValue.Value;
        }

        [Test]
        public void Instance_WhenConventionsDeclareABaseInterface_MustImplementTheInterface()
        {
            var instance = new ImportEmployee(CommandId, EmployeeNumber, FirstName, Lastname, null);
            Assert.That(instance, Is.AssignableTo<ICommand>());
        }
    }
}
