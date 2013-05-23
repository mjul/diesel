using System;
using System.Linq;
using Diesel;
using NUnit.Framework;
using Sprache;

namespace Test.Diesel
{
    [TestFixture]
    public class GrammarTest
    {
        [Test]
        public void Identifier_ValidSingleLetter_ShouldParse()
        {
            var actual = Grammar.Identifier.Parse("x");
            Assert.That(actual, Is.EqualTo("x"));
        }

        [Test]
        public void Identifier_ValidString_ShouldParse()
        {
            var actual = Grammar.Identifier.Parse("name");
            Assert.That(actual, Is.EqualTo("name"));
        }

        [Test]
        public void Identifier_ValidStringAndNumber_ShouldParse()
        {
            var actual = Grammar.Identifier.Parse("name1");
            Assert.That(actual, Is.EqualTo("name1"));
        }

        [Test]
        public void PrimitiveType_PrimitiveType_ShouldParse()
        {
            AssertPrimitiveTypeParsesAs<decimal>("Decimal");
        }

        [Test]
        public void PrimitiveType_SyntacticSugarNames_ShouldParse()
        {
            AssertPrimitiveTypeParsesAs<Int32>("int");
            AssertPrimitiveTypeParsesAs<Int64>("long");
            AssertPrimitiveTypeParsesAs<Decimal>("decimal");
            AssertPrimitiveTypeParsesAs<Double>("double");
        }

        private static void AssertPrimitiveTypeParsesAs<T>(string input)
        {
            var actual = Grammar.PrimitiveType.Parse(input);
            Assert.That(actual, Is.EqualTo(typeof(T)));
        }


        [Test]
        public void PrimitiveType_ComplexType_ShouldNotParse()
        {
            Assert.Throws<ParseException>(() => Grammar.PrimitiveType.Parse("ArgumentException"));
        }

        [Test]
        public void ValueTypeDeclaration_ValidDeclaration_ShouldParseName()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype EmployeeNumber)");
            Assert.That(actual.Name, Is.EqualTo("EmployeeNumber"));
        }

        [Test]
        public void ValueTypeDeclaration_ValidDeclarationNoType_ShouldSetNoType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype EmployeeNumber)");
            Assert.That(actual.ValueType, Is.Null);
        }

        [Test]
        public void ValueTypeDeclaration_ValidDeclarationWithExplicitType_ShouldSetType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype GradePointAverage Decimal)");
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.ValueType, Is.EqualTo(typeof(decimal)));
        }

        [Test]
        public void Namespace_SinglePartName_ShouldParse()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration)");
            Assert.That(actual.Name, Is.EqualTo("Administration"));
        }

        [Test]
        public void Namespace_MultiPartName_ShouldParse()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration.Client)");
            Assert.That(actual.Name, Is.EqualTo("Administration.Client"));
        }

        [Test]
        public void Namespace_NoDeclarations_ShouldHaveEmptyList()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration)");
            Assert.That(actual.Declarations, Is.Not.Null);
            Assert.That(actual.Declarations, Is.Empty);
        }

        [Test]
        public void Namespace_WithDeclarations_ShouldParseDeclarations()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration.Client" +
                                                 "  (defvaluetype ClientId))");
            Assert.That(actual.Name, Is.EqualTo("Administration.Client"));
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldParseName()
        {
            var actual = Grammar.CommandDeclaration.Parse("(defcommand ImportEmployee)");
            Assert.That(actual.Name, Is.EqualTo("ImportEmployee"));
        }

        [Test]
        public void CommandDeclaration_ValidDeclarationNoProperties_ShouldHaveNoProperties()
        {
            var actual = Grammar.CommandDeclaration.Parse("(defcommand ImportEmployee)");
            Assert.That(actual.Properties, Is.Empty);
        }


        [Test]
        public void PropertyDeclaration_SingleProperty_ShouldSetNameAndType()
        {
            var actual = Grammar.PropertyDeclaration.Parse("int EmployeeNumber");
            Assert.That(actual.Name, Is.EqualTo("EmployeeNumber"));
            Assert.That(actual.Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void CommandDeclaration_SingleProperty_ShouldParseProperty()
        {
            var actual = Grammar.CommandDeclaration.Parse("(defcommand ImportEmployee (int EmployeeNumber))");
            Assert.That(actual.Properties, Is.Not.Null);
            var property = actual.Properties.Single();
            Assert.That(property.Name, Is.EqualTo("EmployeeNumber"));
            Assert.That(property.Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void CommandDeclaration_MultipleProperties_ShouldParseProperties()
        {
            var actual = Grammar.CommandDeclaration.Parse("(defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))");
            var properties = actual.Properties.ToList();
            AssertPropertyEquals(properties[0], "EmployeeNumber", typeof (int));
            AssertPropertyEquals(properties[1], "FirstName", typeof(string));
            AssertPropertyEquals(properties[2], "LastName", typeof(string));
        }

        private void AssertPropertyEquals(PropertyDeclaration actual, string expectedName, Type expectedType)
        {
            Assert.That(actual.Name, Is.EqualTo(expectedName));
            Assert.That(actual.Type, Is.EqualTo(expectedType));
        }
    }
}