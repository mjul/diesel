using System;
using System.Linq;
using Diesel;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Sprache;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class GrammarTest
    {

        [Test]
        public void Keyword_ValidName_ShouldParse()
        {
            var actual = Grammar.Keyword().Parse(":foo");
            Assert.That(actual.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void Keyword_SpecificNameOverloadSameName_ShouldParse()
        {
            var actual = Grammar.Keyword("foo").Parse(":foo");
            Assert.That(actual.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void Keyword_SpecificNameOverloadOtherKeyword_ShouldNotParse()
        {
            var actual = Grammar.Keyword("foo").TryParse(":bar");
            Assert.That(actual.WasSuccessful, Is.False);
        }


        [Test]
        public void SimpleType_SimpleType_ShouldParse()
        {
            AssertSimpleTypeParsesAs<decimal>("Decimal");
        }

        [Test]
        public void SimpleType_SyntacticSugarNames_ShouldParse()
        {
            AssertSimpleTypeParsesAs<Int32>("int");
            AssertSimpleTypeParsesAs<Int64>("long");
            AssertSimpleTypeParsesAs<Decimal>("decimal");
            AssertSimpleTypeParsesAs<Double>("double");
        }

        private static void AssertSimpleTypeParsesAs<T>(string input)
        {
            var actual = Grammar.SimpleType.Parse(input);
            Assert.That(actual, Is.EqualTo(typeof(T)));
        }


        [Test]
        public void SimpleType_ComplexType_ShouldNotParse()
        {
            Assert.Throws<ParseException>(() => Grammar.SimpleType.Parse("ArgumentException"));
        }


        [Test]
        public void SimpleOrNullableType_ForSimpleType_ShouldParse()
        {
            AssertSimpleOrNullableTypeParsesAs<Int32>("Int32");
        }

        [Test]
        public void SimpleOrNullableType_ForNullableType_ShouldParse()
        {
            AssertSimpleOrNullableTypeParsesAs<Int32?>("Int32?");
            AssertSimpleOrNullableTypeParsesAs<Int32?>("int?");
            AssertSimpleOrNullableTypeParsesAs<Int64?>("Int64?");
            AssertSimpleOrNullableTypeParsesAs<Int64?>("long?");
            AssertSimpleOrNullableTypeParsesAs<Decimal?>("decimal?");
            AssertSimpleOrNullableTypeParsesAs<Double?>("double?");
        }

        private static void AssertSimpleOrNullableTypeParsesAs<T>(string input)
        {
            var actual = Grammar.SimpleTypeAllowNullable.Parse(input);
            Assert.That(actual, Is.EqualTo(typeof(T)));
        }


        [Test]
        public void ValueTypeDeclaration_JustNameNoType_ShouldParseName()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype EmployeeNumber)");
            Assert.That(actual.Name, Is.EqualTo("EmployeeNumber"));
        }

        [Test]
        public void ValueTypeDeclaration_JustNameNoType_ShouldNotSetPropertyName()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype EmployeeNumber)");
            Assert.That(actual.Properties.Single().Name, Is.Null);
        }

        [Test]
        public void ValueTypeDeclaration_JustNameNoType_ShouldSetNoPropertyType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype EmployeeNumber)");
            var actualProperty = actual.Properties.Single();
            Assert.That(actualProperty.Type, Is.Null);
        }

        [Test]
        public void ValueTypeDeclaration_NameAndExplicitType_ShouldSetType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype GradePointAverage Decimal)");
            var actualProperty = actual.Properties.Single();
            Assert.That(actualProperty.Type, Is.EqualTo(typeof(decimal)));
        }

        [Test]
        public void ValueTypeDeclaration_NameAndExplicitNullableType_ShouldSetType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype Optional int?)");
            var actualProperty = actual.Properties.Single();
            Assert.That(actualProperty.Type, Is.EqualTo(typeof(int?)));
        }

        [Test]
        public void ValueTypeDeclaration_NameAndExplicitSystemValueType_ShouldSetType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype AggregateRootId Guid)");
            var actualProperty = actual.Properties.Single();
            Assert.That(actualProperty.Type, Is.EqualTo(typeof(Guid)));
        }

        [Test]
        public void ValueTypeDeclaration_NameAndExplicitType_ShouldNotSetPropertyName()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype GradePointAverage Decimal)");
            var actualProperty = actual.Properties.Single();
            Assert.That(actualProperty.Name, Is.Null);
        }

        [Test]
        public void ValueTypeDeclaration_ComplexWithSingleProperty_ShouldSetNameAndType()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype BookSize (int Pages))");
            var actualProperty = actual.Properties.Single();
            Assert.That(actualProperty.Name, Is.EqualTo("Pages"));
            Assert.That(actualProperty.Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void ValueTypeDeclaration_ComplexWithMultipleProperties_ShouldSetNamesAndTypes()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype BookSize (int Pages, int Chapters))");
            var actualProperties = actual.Properties.ToList();
            Assert.That(actualProperties[0].Name, Is.EqualTo("Pages"));
            Assert.That(actualProperties[0].Type, Is.EqualTo(typeof(int)));
            Assert.That(actualProperties[1].Name, Is.EqualTo("Chapters"));
            Assert.That(actualProperties[1].Type, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void ValueTypeDeclaration_ComplexWithNullableProperty_ShouldSetTypes()
        {
            var actual = Grammar.ValueTypeDeclaration.Parse("(defvaluetype BookSize (int Pages, int? Chapters))");
            var actualProperties = actual.Properties.ToList();
            Assert.That(actualProperties[1].Type, Is.EqualTo(typeof(int?)));
        }

        [Test]
        public void Namespace_SinglePartName_ShouldParse()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration)");
            Assert.That(actual.Name.Name, Is.EqualTo("Administration"));
        }

        [Test]
        public void Namespace_MultiPartName_ShouldParse()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration.Client)");
            Assert.That(actual.Name.Name, Is.EqualTo("Administration.Client"));
        }

        [Test]
        public void Namespace_NoDeclarations_ShouldHaveEmptyList()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration)");
            Assert.That(actual.Declarations, Is.Not.Null);
            Assert.That(actual.Declarations, Is.Empty);
        }

        [Test]
        public void Namespace_WithSingleDeclaration_ShouldParseDeclaration()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration.Client" +
                                                 "  (defvaluetype ClientId))");
            Assert.That(actual.Name.Name, Is.EqualTo("Administration.Client"));
            var singleDeclaration = actual.Declarations.Single();
            Assert.That(singleDeclaration.Name, Is.EqualTo("ClientId"));
            Assert.That(singleDeclaration, Is.TypeOf<ValueTypeDeclaration>());
        }

        [Test]
        public void Namespace_WithMultipleDeclarations_ShouldParseDeclarations()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration.Client" +
                                                 "  (defvaluetype ClientId)" +
                                                 "  (defvaluetype UserId)" +
                                                 ")");

            
            var clientIdDeclaration = actual.Declarations.SingleOrDefault(x => x.Name == "ClientId");
            var userIdDeclaration = actual.Declarations.SingleOrDefault(x => x.Name == "UserId");
            Assert.That(clientIdDeclaration, Is.Not.Null);
            Assert.That(userIdDeclaration, Is.Not.Null);
        }

        [Test]
        public void Namespace_DeclarationsOfMultipleTypes_ShouldParseDeclarations()
        {
            var actual = Grammar.Namespace.Parse("(namespace Administration.Client" +
                                                 "  (defvaluetype ClientId)" +
                                                 "  (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))" +
                                                 "  (defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, string FirstName, string LastName))" +
                                                 ")");
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Declarations.Single(x => x.Name == "ClientId"), Is.Not.Null);
            Assert.That(actual.Declarations.Single(x => x.Name == "ImportEmployee"), Is.Not.Null);
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
        public void PropertyDeclaration_NullableType_ShouldSetType()
        {
            var actual = Grammar.PropertyDeclaration.Parse("int? Optional");
            Assert.That(actual.Type, Is.EqualTo(typeof(int?)));
        }

        [Test]
        public void PropertyDeclaration_PrimitiveArrayType_ShouldSetType()
        {
            var actual = Grammar.PropertyDeclaration.Parse("int[] Roles");
            Assert.That(actual.Type, Is.EqualTo(typeof(int[])));
        }

        [Test]
        public void PropertyDeclaration_UnqualifiedSystemTypeDateTime_ShouldSetType()
        {
            var actual = Grammar.PropertyDeclaration.Parse("DateTime OccurredOn");
            Assert.That(actual.Type, Is.EqualTo(typeof(DateTime)));
        }

        [Test]
        public void PropertyDeclaration_UnqualifiedSystemTypeGuid_ShouldSetType()
        {
            var actual = Grammar.PropertyDeclaration.Parse("Guid CommandId");
            Assert.That(actual.Type, Is.EqualTo(typeof(Guid)));
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

        [Test]
        public void CommandDeclaration_PropertyWithArrayType_ShouldParseProperties()
        {
            var actual = Grammar.CommandDeclaration.Parse("(defcommand ImportEmployeeRoles (int EmployeeNumber, int[] RoleIds))");
            var properties = actual.Properties.ToList();
            AssertPropertyEquals(properties[0], "EmployeeNumber", typeof(int));
            AssertPropertyEquals(properties[1], "RoleIds", typeof(int[]));
        }


        private void AssertPropertyEquals(PropertyDeclaration actual, string expectedName, Type expectedType)
        {
            Assert.That(actual.Name, Is.EqualTo(expectedName));
            Assert.That(actual.Type, Is.EqualTo(expectedType));
        }

        [Test]
        public void ApplicationServiceDeclaration_NoCommands_ShouldNotParse()
        {
            var result = Grammar.ApplicationServiceDeclaration.TryParse("(defapplicationservice ImportService)");
            Assert.That(result.WasSuccessful, Is.False);
        }

        [Test]
        public void ApplicationServiceDeclaration_SingleCommand_ShouldParseName()
        {
            var actual = Grammar.ApplicationServiceDeclaration.Parse("(defapplicationservice ImportService" +
                                                                     "  (defcommand ImportEmployee (int EmployeeNumber, string Name)))");
            Assert.That(actual.Name, Is.EqualTo("ImportService"));
        }


        [Test]
        public void ApplicationServiceDeclaration_MultipleCommands_ShouldParseCommands()
        {
            var actual = Grammar.ApplicationServiceDeclaration.Parse("(defapplicationservice ImportService" +
                                                                     "  (defcommand ImportEmployee (int EmployeeNumber, string Name))" +
                                                                     "  (defcommand ImportClient (int ClientNumber, string Name))" +
                                                                     ")");
            var commands = actual.Commands.ToList();
            Assert.That(commands.Count, Is.EqualTo(2));
            Assert.That(commands[0].Name, Is.EqualTo("ImportEmployee"));
            Assert.That(commands[1].Name, Is.EqualTo("ImportClient"));
        }

        [Test]
        public void DomainEventDeclaration_MultipleProperties_ShouldParseProperties()
        {
            var actual = Grammar.DomainEventDeclaration.Parse("(defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, string FirstName, string LastName))");
            var properties = actual.Properties.ToList();
            AssertPropertyEquals(properties[0], "Id", typeof(Guid));
            AssertPropertyEquals(properties[1], "EmployeeNumber", typeof(int));
            AssertPropertyEquals(properties[2], "FirstName", typeof(string));
            AssertPropertyEquals(properties[3], "LastName", typeof(string));
        }


        [Test]
        public void TypeDeclaration_ValueTypeDeclaration_ShouldBeAccepted()
        {
            var actual = Grammar.TypeDeclaration.TryParse("(defvaluetype EmployeeNumber)");
            Assert.True(actual.WasSuccessful);
        }

        [Test]
        public void TypeDeclaration_CommandDeclaration_ShouldBeAccepted()
        {
            var actual = Grammar.TypeDeclaration.TryParse("(defcommand ImportEmployee (int EmployeeNumber, String Name))");
            Assert.True(actual.WasSuccessful);
        }

        [Test]
        public void TypeDeclaration_DomainEventDeclaration_ShouldBeAccepted()
        {
            var actual = Grammar.TypeDeclaration.TryParse("(defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, String Name))");
            Assert.True(actual.WasSuccessful);
        }

        [Test]
        public void ConventionDeclarations_Valid_ShouldParse()
        {
            var actual = Grammar.ConventionsDeclaration.Parse(
                    "(defconventions :domainevents {:inherit [SomeNamespace.IDomainEvent]})");
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.DomainEventConventions, Is.Not.Null);
            Assert.That(actual.DomainEventConventions, Is.Not.Null);
        }


        [Test]
        public void AbstractSyntaxTree_MultipleNamespaces_ShouldParseNamespaces()
        {
            var actual =
                Grammar.AbstractSyntaxTree.Parse(
                    "(namespace Foo (defvaluetype FooId)) " +
                    "(namespace Bar (defcommand Say (string message)))");
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Namespaces.Count(), Is.EqualTo(2));
        }

        [Test]
        public void AbstractSyntaxTree_WithConventions_ShouldParseConventions()
        {
            var actual =
                Grammar.AbstractSyntaxTree.Parse(
                    "(defconventions :domainevents {:inherit [SomeName.IDomainEvent]})" +
                    "(namespace Bar (defcommand Say (string message)))");
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Conventions, Is.Not.Null);
            Assert.That(actual.Namespaces.Count(), Is.EqualTo(1));
        }

        
        [Test]
        public void Everything_ValidAstFollowedByInvalidSource_ShouldNotParse()
        {
            var actual = Grammar.Everything.TryParse("(namespace Foo (defvaluetype FooId)) )");
            Assert.False(actual.WasSuccessful);
        }


        [Test]
        public void Everything_ValidAst_ShouldParse()
        {
            var actual = Grammar.Everything.TryParse("(namespace Foo (defvaluetype FooId))");
            Assert.True(actual.WasSuccessful);
        }

    }
}