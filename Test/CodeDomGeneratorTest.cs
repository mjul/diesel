using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Diesel;
using NUnit.Framework;

namespace Test.Diesel
{
    [TestFixture]
    public class CodeDomGeneratorTest
    {
        [Test]
        public void ValueType_ValidDeclaration_ShouldCompile()
        {
            var actual = CodeDomGenerator.Compile(new ValueTypeDeclaration("EmployeeNumber", typeof(int)));
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Console.WriteLine(source);
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldCompile()
        {
            var actual = CodeDomGenerator.Compile(new CommandDeclaration("ImportEmployeeCommand", new[]
                {
                    new PropertyDeclaration("EmployeeNumber", typeof(int)), 
                    new PropertyDeclaration("FirstName", typeof(String)), 
                    new PropertyDeclaration("LastName", typeof(String))
                }));
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("class ImportEmployeeCommand"));
            Console.WriteLine(source);
        }

        [Test]
        public void Namespace_ValidDeclarationWithValueTypeDeclaration_ShouldCompile()
        {
            var declarations = new[] { new ValueTypeDeclaration("EmployeeNumber", typeof(int)) };
            AssertNamespaceCompiledCodeShouldContain(declarations, "struct EmployeeNumber");
        }

        [Test]
        public void Namespace_ValidDeclarationWithCommandDeclaration_ShouldCompile()
        {
            var declarations = new[] { CreateImportEmployeeCommandDeclaration() };
            AssertNamespaceCompiledCodeShouldContain(declarations, "class ImportEmployeeCommand");
        }

        [Test]
        public void Namespace_ValidDeclarationWithMultipleDeclarations_ShouldCompile()
        {
            var declarations = new ITypeDeclaration[]
                {
                    CreateImportEmployeeCommandDeclaration(),
                    CreateEmployeeNumberValueTypeDeclaration()
                };

            AssertNamespaceCompiledCodeShouldContain(declarations, 
                "class ImportEmployeeCommand", "struct EmployeeNumber");
        }

        private static ValueTypeDeclaration CreateEmployeeNumberValueTypeDeclaration()
        {
            return new ValueTypeDeclaration("EmployeeNumber", typeof(int));
        }

        private static CommandDeclaration CreateImportEmployeeCommandDeclaration()
        {
            return new CommandDeclaration("ImportEmployeeCommand", new[]
                {
                    new PropertyDeclaration("EmployeeNumber", typeof (int)),
                    new PropertyDeclaration("FirstName", typeof (String)),
                    new PropertyDeclaration("LastName", typeof (String))
                });
        }


        [Test]
        public void AbstractSyntaxTree_ValidDeclarationWithMultipleDeclarations_ShouldCompile()
        {
            var ast = new AbstractSyntaxTree(new Namespace[]
                {
                    new Namespace("Employees.Commands", new[] {CreateImportEmployeeCommandDeclaration()}),
                    new Namespace("Employees.Model", new[] {CreateEmployeeNumberValueTypeDeclaration()}),
                });

            var dom = CodeDomGenerator.Compile(ast);
            var source = CompileToSource(dom);

            var expectedSnippets = new[]
                {
                    "namespace Employees.Commands",
                    "namespace Employees.Model",
                    "class ImportEmployeeCommand", 
                    "struct EmployeeNumber"
                };

            Array.ForEach(expectedSnippets, (x=> Assert.That(source, Is.StringContaining(x))));
        }


        private static void AssertNamespaceCompiledCodeShouldContain(IEnumerable<ITypeDeclaration> declarations, 
            params string[] expectedTypeDeclarations)
        {
            var ns = typeof (CodeDomGeneratorTest).Namespace + ".Generated";
            var actual = CodeDomGenerator.Compile(new Namespace(ns, declarations));
            Assert.That(actual, Is.Not.Null);
            
            var source = CompileToSource(actual);
            
            var expectedNamespaceDeclaration = String.Format("namespace {0}", ns);
            Assert.That(source, Is.StringContaining(expectedNamespaceDeclaration));
            
            foreach (var expected in expectedTypeDeclarations)
            {
                Assert.That(source, Is.StringContaining(expected));
            }
            Console.WriteLine(source);
        }

        private static string CompileToSource(CodeCompileUnit codeCompileUnit)
        {
            return DieselCompiler.CompileToSource(codeCompileUnit, DieselCompiler.GetCSharpProvider());
        }
    }
}
