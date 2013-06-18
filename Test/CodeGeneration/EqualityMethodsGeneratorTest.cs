using Diesel.CodeGeneration;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class EqualityMethodsGeneratorTest
    {

        [Test]
        public void ComparePropertyValueEqualityExpression_StringReferenceType_ShouldGenerateExpression()
        {
            var actual = EqualityMethodsGenerator.ComparePropertyValueEqualityExpression(
                new PropertyDeclaration("Text", new StringReferenceType()), "other");
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void ComparePropertyValueEqualityExpression_SimpleType_ShouldGenerateExpression()
        {
            var actual = EqualityMethodsGenerator.ComparePropertyValueEqualityExpression(
                new PropertyDeclaration("Value", new SimpleType(typeof(decimal))), "other");
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void ComparePropertyValueEqualityExpression_NullableSimpleType_ShouldGenerateExpression()
        {
            var actual = EqualityMethodsGenerator.ComparePropertyValueEqualityExpression(
                new PropertyDeclaration("Value", new NullableType(new SimpleType(typeof(decimal)))), "other");
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void ComparePropertyValueEqualityExpression_NamedDateTime_ShouldGenerateExpression()
        {
            var actual = EqualityMethodsGenerator.ComparePropertyValueEqualityExpression(
                new PropertyDeclaration("Value", new TypeNameTypeNode(new TypeName("System.DateTime"))), "other");
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void ComparePropertyValueEqualityExpression_ArrayType1D_ShouldGenerateExpression()
        {
            var actual = EqualityMethodsGenerator.ComparePropertyValueEqualityExpression(
                new PropertyDeclaration("Value", 
                    new ArrayType(new SimpleType(typeof(decimal)), 
                        new RankSpecifiers(new[] {new RankSpecifier(1)}))), 
                        "other");
            Assert.That(actual, Is.Not.Null);
        }


    }
}