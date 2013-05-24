using System;
using System.Linq;
using Diesel;
using NUnit.Framework;

namespace Test.Diesel
{
    [TestFixture]
    public class ModelTransformationsTest
    {
        [Test]
        public void ApplyDefaults_ValueTypeDeclarationWithNullType_ShouldSetTypeToInt32()
        {
            var valueTypeDeclaration = new ValueTypeDeclaration("EmployeeNumber", null);
            var actualDeclaration =
                (ValueTypeDeclaration) ApplyDefaultsOnSingleDeclarationNamespace(valueTypeDeclaration);
            Assert.That(actualDeclaration.Name, Is.EqualTo(valueTypeDeclaration.Name));
            Assert.That(actualDeclaration.ValueType, Is.EqualTo(typeof (Int32)));
        }

        [Test]
        public void ApplyDefaults_ValueTypeDeclarationWithType_ShouldNotModifyType()
        {
            var valueTypeDeclaration = new ValueTypeDeclaration("Name", typeof (string));
            var actualDeclaration =
                (ValueTypeDeclaration) ApplyDefaultsOnSingleDeclarationNamespace(valueTypeDeclaration);
            Assert.That(actualDeclaration, Is.SameAs(valueTypeDeclaration));
        }

        private static ITypeDeclaration ApplyDefaultsOnSingleDeclarationNamespace(ITypeDeclaration valueTypeDeclaration)
        {
            var input = new AbstractSyntaxTree(
                new[]
                    {
                        new Namespace("Test",
                                      new[] {valueTypeDeclaration})
                    });

            var actual = ModelTransformations.ApplyDefaults(input);
            var actualDeclaration = (ValueTypeDeclaration) actual.Namespaces.Single().Declarations.Single();
            return actualDeclaration;
        }
    }
}