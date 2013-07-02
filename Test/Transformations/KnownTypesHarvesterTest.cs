using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;
using NUnit.Framework;
using Test.Diesel.ObjectMothers;

namespace Test.Diesel.Transformations
{
    [TestFixture]
    public class KnownTypesHarvesterTest
    {
        [Test]
        public void Harvester_WithEnum_ShouldReturnEnumAsValueType()
        {
            AssertCorrectlyGetsKnownTypes("MyNamespace.Declarations", EnumDeclarationObjectMother.CreateRoles(),
                                          "MyNamespace.Declarations.Roles", true);
        }

        [Test]
        public void Harvester_WithDto_ShouldReturnDtoAsNonValueType()
        {
            AssertCorrectlyGetsKnownTypes("MyNamespace.Declarations", DtoDeclarationObjectMother.CreateName(),
                                          "MyNamespace.Declarations.Name", false);
        }

        [Test]
        public void Harvester_WithCommand_ShouldReturnCommandAsNonValueType()
        {
            AssertCorrectlyGetsKnownTypes("MyNamespace.Declarations", CommandDeclarationObjectMother.CreateImportEmployee(),
                                          "MyNamespace.Declarations.ImportEmployee", false);
        }

        [Test]
        public void Harvester_WithDomainEvent_ShouldReturnDomainEventAsNonValueType()
        {
            AssertCorrectlyGetsKnownTypes("MyNamespace.Declarations", DomainEventDeclarationObjectMother.CreateEmployeeImported(),
                                          "MyNamespace.Declarations.EmployeeImported", false);
        }

        [Test]
        public void Harvester_WithValueType_ShouldReturnValueTypeAsValueType()
        {
            AssertCorrectlyGetsKnownTypes("MyNamespace.Declarations", ValueTypeDeclarationObjectMother.CreateEmployeeNumber(),
                                          "MyNamespace.Declarations.EmployeeNumber", true);
        }


        private AbstractSyntaxTree CreateAbstractSyntaxTreeWith(string ns, params TypeDeclaration[] declarations)
        {
            return new AbstractSyntaxTree(null, new[]
                {
                    new Namespace(new NamespaceName(ns), declarations)
                });
        }

        private void AssertCorrectlyGetsKnownTypes(string namespaceName, TypeDeclaration typeDeclaration, 
                                                   string expectedFullName, bool expectedIsValueType)
        {
            var ast = CreateAbstractSyntaxTreeWith(namespaceName, typeDeclaration);
            var actual = KnownTypesHarvester.GetKnownTypes(ast);

            var actualKnownType = actual.Single();
            Assert.That(actualKnownType.FullName, Is.EqualTo(expectedFullName));
            Assert.That(actualKnownType.IsValueType, Is.EqualTo(expectedIsValueType));
        }
    }
}
