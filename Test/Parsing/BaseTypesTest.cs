using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class BaseTypesTest
    {
        [Test]
        public void ApplyOverrides_Null_ShouldReturnSameInstance()
        {
            var instance = new BaseTypes(new TypeName[0]);
            var actual = instance.ApplyOverridesFrom(null);
            Assert.That(actual, Is.SameAs(instance));
        }

        [Test]
        public void ApplyOverrides_DifferentBaseTypes_ShouldReturnInstanceWithTheOtherBaseTypes()
        {
            var typeName = new TypeName("SomeNamespace.IBaseType");
            var otherTypeName = new TypeName("SomeNamespace.IOtherBaseType");
            var instance = new BaseTypes(new[] { typeName });
            var other = new BaseTypes(new[] { otherTypeName });
            var actual = instance.ApplyOverridesFrom(other);

            Assert.That(actual.TypeNames,
                        Is.EquivalentTo(new[] { otherTypeName }));
        }

        [Test]
        public void EqualityOperations()
        {
            var typeNameBaseType = new TypeName("SomeNamespace.IBaseType");
            var typeNameOtherBaseType = new TypeName("SomeNamespace.IOtherBaseType");

            var a = new BaseTypes(new[] { typeNameBaseType });
            var b = new BaseTypes(new[] { typeNameBaseType });
            var c = new BaseTypes(new[] { typeNameBaseType });
            
            var otherTypeName = new BaseTypes(new[] { typeNameOtherBaseType });
            var otherTypeNameLength = new BaseTypes(new[] { typeNameBaseType, typeNameOtherBaseType });
            var otherTypeNameNull = new BaseTypes(null);

            EqualityTesting.TestEqualityOperators(a,b,c, otherTypeName, otherTypeNameLength, otherTypeNameNull);
            EqualityTesting.TestEqualityOperators(a,b,c, otherTypeName, otherTypeNameLength, otherTypeNameNull);
        }
    }
}