using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedValueTypeWithNullableProperty
    {
        // Generate the class to test and place it in the Generated folder:
        // (defvaluetype EmployeeMetadata (string Source, int? SourceId))

        const string Source = "Legacy system";
        const int SourceId = 1;

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            var actual = new Generated.EmployeeMetadata(Source, SourceId);
            Assert.That(actual.Source, Is.EqualTo(Source));
            Assert.That(actual.SourceId, Is.EqualTo(SourceId));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var a = new Generated.EmployeeMetadata(Source, SourceId);
            var b = new Generated.EmployeeMetadata(Source, SourceId);
            var c = new Generated.EmployeeMetadata(Source, SourceId);

            var otherSource = new Generated.EmployeeMetadata(Source + "x", SourceId);
            var otherSourceId = new Generated.EmployeeMetadata(Source, SourceId + 1);
            var otherSourceIdNull = new Generated.EmployeeMetadata(Source, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherSource, otherSourceId, otherSourceIdNull);
            EqualityTesting.TestEqualityOperators(a, b, c, otherSource, otherSourceId, otherSourceIdNull);
        }

        [Test]
        public void Instance_WhenSerializedWithBinaryFormatter_ShouldBeSerializable()
        {
            var actual = new Generated.EmployeeMetadata(Source, SourceId);
            
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(actual);
            
            Assert.That(deserialized, Is.EqualTo(actual));
        }

        [Test]
        public void ToString_NotNull_ShouldFormatValue()
        {
            var actual = new Generated.EmployeeMetadata("Source", 1).ToString();
            Assert.That(actual, Is.EqualTo("Source 1"));
        }

        [Test]
        public void ToString_NullableFieldIsNull_ShouldFormatNullValueAsEmpty()
        {
            var actual = new Generated.EmployeeMetadata("Source", null).ToString();
            Assert.That(actual, Is.EqualTo("Source "));
        }   

    }
}