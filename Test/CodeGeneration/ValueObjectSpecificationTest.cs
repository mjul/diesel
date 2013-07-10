using Diesel.CodeGeneration;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.ObjectMothers;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class ValueObjectSpecificationTest
    {

        [Test]
        public void CreateClass_WithBaseTypes_ShouldSetBaseTypes()
        {
            var ns = new NamespaceName("Foo");
            var properties = PropertyDeclarationObjectMother.FirstLastStringPropertyDeclarations();
            var baseTypes = BaseTypesObjectMother.CreateDieselTestingCommand();
            
            var actual = ValueObjectSpecification.CreateClass(ns, "SomeClass", properties, baseTypes, false, false);

            Assert.That(actual.BaseTypes, Is.EqualTo(baseTypes));
        }
         
    }
}