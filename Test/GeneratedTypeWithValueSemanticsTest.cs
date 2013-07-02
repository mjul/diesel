using System;
using Diesel;
using NUnit.Framework;
using Test.Diesel.Generated;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedTypeWithValueSemanticsTest
    {
        // Verify that GetHashCode includes model value type fields
        // This is not strictly required by its semantics, but
        // it verifies that code is generated that uses these fields
        // for the GetHashCode calculation.
        
        [Test]
        public void Debug()
        {
            DieselCompiler.Compile("(namespace Foo " +
                                   "  (defenum Gender [Female Male]) " +
                                   "  (defdto Foo (int Number, Gender Gender)))");
        }

        [Test]
        public void GetHashCode_OtherValueOfValueTypeField_ShouldChangeHashCode()
        {
            var id = Guid.NewGuid();
            const int number = 1;
            var name = new Name("Kim", "Crypto");
            var male = new ImportEmployeeNestedTypes(id, number, name, Gender.Male);
            var female = new ImportEmployeeNestedTypes(id, number, name, Gender.Female);

            Assert.That(male.GetHashCode() != female.GetHashCode());
        }

        [Test]
        public void GetHashCode_OtherValueOfGuidField_ShouldChangeHashCode()
        {
            var name = new Name("Kim", "Crypto");
            var a = new ImportEmployeeNestedTypes(Guid.NewGuid(), 1, name, Gender.Male);
            var b = new ImportEmployeeNestedTypes(Guid.NewGuid(), 1, name, Gender.Male);

            Assert.That(a.GetHashCode() != b.GetHashCode());
        }


        [Test]
        public void GetHashCode_OtherValueOfIntField_ShouldChangeHashCode()
        {
            var id = Guid.NewGuid();
            var name = new Name("Kim", "Crypto");
            var a = new ImportEmployeeNestedTypes(id, 1, name, Gender.Male);
            var b = new ImportEmployeeNestedTypes(id, 2, name, Gender.Male);

            Assert.That(a.GetHashCode() != b.GetHashCode());
        }

    }
}