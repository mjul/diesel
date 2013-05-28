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
            var model = CreateAbstractSyntaxTreeWith(new ValueTypeDeclaration("EmployeeNumber", typeof (int)));
            var actual = CodeDomGenerator.Compile(model);
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("struct EmployeeNumber"));
            Console.WriteLine(source);
        }

        [Test]
        public void ValueType_ValidDeclaration_ShouldNotHaveDataContractAttributes()
        {
            var model = CreateAbstractSyntaxTreeWith(new ValueTypeDeclaration("EmployeeNumber", typeof(int)));
            var actual = CodeDomGenerator.Compile(model);
            var source = CompileToSource(actual);
            Assert.That(source, Is.Not.StringContaining("DataContractAttribute"));
            Assert.That(source, Is.Not.StringContaining("DataMemberAttribute"));
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldCompile()
        {
            var actual = CompileImportEmployeeCommand();
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("class ImportEmployeeCommand"));
            Console.WriteLine(source);
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldAddDataMemberAttributes()
        {
            var source = CompileToSource(CompileImportEmployeeCommand());
            
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Order=1\)]\s+public\s+int\s+EmployeeNumber"));
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Order=2\)]\s+public\s+string\s+FirstName"));
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Order=3\)]\s+public\s+string\s+LastName"));
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldAddDataContractAttribute()
        {
            var source = CompileToSource(CompileImportEmployeeCommand());
            Assert.That(source, Is.StringMatching(@"DataContractAttribute"));
        }

        private CodeCompileUnit CompileImportEmployeeCommand()
        {
            var commandDeclaration = new CommandDeclaration("ImportEmployeeCommand",
                                                            new[]
                                                                {
                                                                    new PropertyDeclaration("EmployeeNumber", typeof (int)),
                                                                    new PropertyDeclaration("FirstName", typeof (String)),
                                                                    new PropertyDeclaration("LastName", typeof (String))
                                                                });
            var model = CreateAbstractSyntaxTreeWith(commandDeclaration);
            return CodeDomGenerator.Compile(model);
        }


        private AbstractSyntaxTree CreateAbstractSyntaxTreeWith(ITypeDeclaration typeDeclaration)
        {
            var ns = typeof (CodeDomGeneratorTest).Namespace + ".Generated";
            return new AbstractSyntaxTree(new[] { new Namespace(ns, new[] { typeDeclaration}) });
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
        public void Namespace_ValidDeclarationWithApplicationServiceDeclaration_ShouldCompile()
        {
            var declarations = new[] { CreateImportApplicationServiceDeclaration(CreateImportEmployeeCommandDeclaration()) };
            AssertNamespaceCompiledCodeShouldContain(declarations, "interface IImportApplicationService");
            AssertNamespaceCompiledCodeShouldContain(declarations, "class ImportEmployeeCommand");
        }

        private ITypeDeclaration CreateImportApplicationServiceDeclaration(CommandDeclaration commandDeclaration)
        {
            return new ApplicationServiceDeclaration("ImportApplicationService", new[]
                {
                    commandDeclaration
                });
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
            var ast = new AbstractSyntaxTree(new[]
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
            var model = new AbstractSyntaxTree(new[] {new Namespace(ns, declarations)});
            var actual = CodeDomGenerator.Compile(model);
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
