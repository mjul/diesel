using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Diesel;
using Diesel.CodeGeneration;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class CodeDomGeneratorTest
    {
        [Test]
        public void ValueType_ValidDeclaration_ShouldCompile()
        {
            var model = CreateAbstractSyntaxTreeWith(
                new ValueTypeDeclaration("EmployeeNumber",
                                         new[] {new PropertyDeclaration("Value", new SimpleType(typeof (int)))}));
            var actual = CodeDomGenerator.Compile(model);
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("public partial struct EmployeeNumber"));
            Console.WriteLine(source);
        }

        [Test]
        public void ValueType_ValidDeclarationMultipleProperties_ShouldCompile()
        {
            var model = CreateAbstractSyntaxTreeWith(
                new ValueTypeDeclaration("Point",
                                         new[]
                                             {
                                                 new PropertyDeclaration("X", new SimpleType(typeof (int))),
                                                 new PropertyDeclaration("Y", new SimpleType(typeof (int)))
                                             }));
            var actual = CodeDomGenerator.Compile(model);
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("struct Point"));
            Console.WriteLine(source);
        }


        [Test]
        public void ValueType_ValidDeclaration_ShouldNotHaveDataContractAttributes()
        {
            var model = CreateAbstractSyntaxTreeWith(
                new ValueTypeDeclaration("EmployeeNumber",
                                         new[] { new PropertyDeclaration("Value", new SimpleType(typeof (int))) }));
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
        public void CommandDeclaration_WithNestedDto_Enum_ShouldCompile()
        {
            var enumDeclaration = new EnumDeclaration("Role", new[] {"Tester", "Developer"});
            var commandDeclaration = new CommandDeclaration("ImportEmployeeCommand",
                                                new[]
                                                                {
                                                                    new PropertyDeclaration("EmployeeNumber", new SimpleType(typeof (int))),
                                                                    new PropertyDeclaration("Role", new TypeNameTypeNode(new TypeName("Role"))),
                                                                });
            var model = CreateAbstractSyntaxTreeWith(enumDeclaration, commandDeclaration);
            var actual = CodeDomGenerator.Compile(model);

            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("class ImportEmployeeCommand"));
            Assert.That(source, Is.StringMatching(@"public.*Role\s+Role"));
        }


        [Test]
        public void CommandDeclaration_WithNestedDto_Dto_ShouldCompile()
        {
            var dtoDeclaration = new DtoDeclaration("AmountDto", new[] { new PropertyDeclaration("Amount", new SimpleType(typeof(decimal))) });
            var commandDeclaration = new CommandDeclaration("ImportEmployeeCommand",
                                                new[]
                                                                {
                                                                    new PropertyDeclaration("EmployeeNumber", new SimpleType(typeof (int))),
                                                                    new PropertyDeclaration("Salary", new TypeNameTypeNode(new TypeName("AmountDto")))
                                                                });
            var model = CreateAbstractSyntaxTreeWith(dtoDeclaration, commandDeclaration);
            var actual = CodeDomGenerator.Compile(model);

            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("class ImportEmployeeCommand"));
            Assert.That(source, Is.StringMatching(@"public.*AmountDto\s+Salary"));
        }


        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldAddDataMemberAttributes()
        {
            var source = CompileToSource(CompileImportEmployeeCommand());
            
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Name=""EmployeeNumber"", Order=1\)]\s+private\s+int\s+_employeeNumber"));
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Name=""FirstName"", Order=2\)]\s+private\s+string\s+_firstName"));
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Name=""LastName"", Order=3\)]\s+private\s+string\s+_lastName"));
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldAddDataContractAttribute()
        {
            var source = CompileToSource(CompileImportEmployeeCommand());
            Assert.That(source, Is.StringContaining(@"DataContractAttribute(Name=""ImportEmployeeCommand"")"));
        }

        private CodeCompileUnit CompileImportEmployeeCommand()
        {
            var commandDeclaration = new CommandDeclaration("ImportEmployeeCommand",
                                                            new[]
                                                                {
                                                                    new PropertyDeclaration("EmployeeNumber", new SimpleType(typeof (int))),
                                                                    new PropertyDeclaration("FirstName", new StringReferenceType()),
                                                                    new PropertyDeclaration("LastName", new StringReferenceType())
                                                                });
            var model = CreateAbstractSyntaxTreeWith(commandDeclaration);
            return CodeDomGenerator.Compile(model);
        }

        [Test]
        public void DomainEventDeclaration_ValidDeclaration_ShouldParse()
        {
            var source = CompileToSource(CompileEmployeeImportedEvent());
            Assert.That(source, Is.StringContaining(@"class EmployeeImported"));
        }

        [Test]
        public void DomainEventDeclaration_ValidDeclaration_ShouldProduceSealedClass()
        {
            var source = CompileToSource(CompileEmployeeImportedEvent());
            Assert.That(source, Is.StringContaining(@"sealed partial class EmployeeImported"));
        }

        [Test]
        public void DomainEventDeclaration_WithConventionInheritIDomainEvent_ShouldProduceClassInheritingIDomainEvent()
        {
            var declaration = CreateEmployeeImportedEventDeclaration();
            var conventions = new ConventionsDeclaration(
                new DomainEventConventions(new[] {new TypeName("Test.Diesel.IDomainEvent")}));
            var model = CreateAbstractSyntaxTreeWith(conventions, declaration);
            var source = CompileToSource(CodeDomGenerator.Compile(model));
            Assert.That(source, Is.StringMatching(@"class EmployeeImported :.* Test.Diesel.IDomainEvent \{"));
        }

        private CodeCompileUnit CompileEmployeeImportedEvent()
        {
            var declaration = CreateEmployeeImportedEventDeclaration();
            var model = CreateAbstractSyntaxTreeWith(declaration);
            return CodeDomGenerator.Compile(model);
        }

        private static DomainEventDeclaration CreateEmployeeImportedEventDeclaration()
        {
            var declaration = new DomainEventDeclaration("EmployeeImported",
                                                         new[]
                                                             {
                                                                 new PropertyDeclaration("Id", new TypeNameTypeNode(new TypeName(typeof (Guid).FullName))),
                                                                 new PropertyDeclaration("EmployeeNumber", new SimpleType(typeof (Int32))),
                                                                 new PropertyDeclaration("FirstName", new StringReferenceType()),
                                                                 new PropertyDeclaration("LastName", new StringReferenceType())
                                                             });
            return declaration;
        }



        [Test]
        public void DtoDeclaration_ValidDeclaration_ShouldProduceSealedClass()
        {
            var source = CompileToSource(CompileNameDto());
            Assert.That(source, Is.StringContaining(@"sealed partial class Name"));
        }

        [Test]
        public void DtoDeclaration_ValidDeclaration_ShouldAddDataMemberAttributes()
        {
            var source = CompileToSource(CompileNameDto());
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Name=""First"", Order=1\)]\s+private\s+string\s+_first"));
            Assert.That(source, Is.StringMatching(@"DataMemberAttribute\(Name=""Last"", Order=2\)]\s+private\s+string\s+_last"));
        }

        [Test]
        public void DtoDeclaration_ValidDeclaration_ShouldAddDataContractAttribute()
        {
            var source = CompileToSource(CompileNameDto());
            Assert.That(source, Is.StringContaining(@"DataContractAttribute(Name=""Name"")"));
        }


        private CodeCompileUnit CompileNameDto()
        {
            var declaration = new DtoDeclaration("Name",
                                                 new[]
                                                     {
                                                         new PropertyDeclaration("First", new StringReferenceType()),
                                                         new PropertyDeclaration("Last", new StringReferenceType())
                                                     });
            var model = CreateAbstractSyntaxTreeWith(declaration);
            return CodeDomGenerator.Compile(model);
        }


        [Test]
        public void EnumDeclaration_ValidDeclaration_ShouldProduceSealedClass()
        {
            var source = CompileToSource(CompileOnOffStateEnum());
            Assert.That(source, Is.StringContaining(@"enum State"));
        }

        [Test]
        public void EnumDeclaration_ValidDeclaration_ShouldAddDataMemberAttributes()
        {
            var source = CompileToSource(CompileOnOffStateEnum());
            Assert.That(source, Is.StringMatching(@"EnumMemberAttribute\(Value\s*=\s*""On""\)]\s+On"));
            Assert.That(source, Is.StringMatching(@"EnumMemberAttribute\(Value\s*=\s*""Off""\)]\s+Off"));
        }

        [Test]
        public void EnumDeclaration_ValidDeclaration_ShouldAddDataContractAttribute()
        {
            var source = CompileToSource(CompileOnOffStateEnum());
            Assert.That(source, Is.StringContaining(@"DataContractAttribute(Name=""State"")"));
        }

        private CodeCompileUnit CompileOnOffStateEnum()
        {
            var declaration = new EnumDeclaration("State", new[] {"On", "Off"});
            var model = CreateAbstractSyntaxTreeWith(declaration);
            return CodeDomGenerator.Compile(model);
        }


        private AbstractSyntaxTree CreateAbstractSyntaxTreeWith(params TypeDeclaration[] typeDeclaration)
        {
            return CreateAbstractSyntaxTreeWith(null, typeDeclaration);
        }

        private AbstractSyntaxTree CreateAbstractSyntaxTreeWith(ConventionsDeclaration conventions, params TypeDeclaration[] typeDeclarations)
        {
            var ns = new NamespaceName(typeof(CodeDomGeneratorTest).Namespace + ".Generated");
            return new AbstractSyntaxTree(conventions, new[] { new Namespace(ns, typeDeclarations) });
        }


        [Test]
        public void Namespace_ValidDeclarationWithValueTypeDeclaration_ShouldCompile()
        {
            var declarations = new[]
                {
                    new ValueTypeDeclaration("EmployeeNumber",
                                             new[] {new PropertyDeclaration("Value", new SimpleType(typeof (Int32)))})
                };
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

        private ApplicationServiceDeclaration CreateImportApplicationServiceDeclaration(CommandDeclaration commandDeclaration)
        {
            return new ApplicationServiceDeclaration("ImportApplicationService", new[]
                {
                    commandDeclaration
                });
        }

        [Test]
        public void Namespace_ValidDeclarationWithMultipleDeclarations_ShouldCompile()
        {
            var declarations = new TypeDeclaration[]
                {
                    CreateImportEmployeeCommandDeclaration(),
                    CreateEmployeeNumberValueTypeDeclaration()
                };

            AssertNamespaceCompiledCodeShouldContain(declarations, 
                "class ImportEmployeeCommand", "struct EmployeeNumber");
        }

        private static ValueTypeDeclaration CreateEmployeeNumberValueTypeDeclaration()
        {
            return new ValueTypeDeclaration("EmployeeNumber", new[]
                {
                    new PropertyDeclaration("Value", new SimpleType(typeof (Int32)))
                });
        }

        private static CommandDeclaration CreateImportEmployeeCommandDeclaration()
        {
            return new CommandDeclaration("ImportEmployeeCommand", new[]
                {
                    new PropertyDeclaration("EmployeeNumber", new SimpleType(typeof (Int32))),
                    new PropertyDeclaration("FirstName", new StringReferenceType()),
                    new PropertyDeclaration("LastName", new StringReferenceType())
                });
        }


        [Test]
        public void AbstractSyntaxTree_ValidDeclarationWithMultipleDeclarations_ShouldCompile()
        {
            var ast = new AbstractSyntaxTree(null, new[]
                {
                    new Namespace(new NamespaceName("Employees.Commands"), new[] {CreateImportEmployeeCommandDeclaration()}),
                    new Namespace(new NamespaceName("Employees.Model"), new[] {CreateEmployeeNumberValueTypeDeclaration()})
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


        private static void AssertNamespaceCompiledCodeShouldContain(IEnumerable<TypeDeclaration> declarations, 
            params string[] expectedTypeDeclarations)
        {
            var ns = new NamespaceName(typeof (CodeDomGeneratorTest).Namespace + ".Generated");
            var model = new AbstractSyntaxTree(null, new[] { new Namespace(ns, declarations) });
            var actual = CodeDomGenerator.Compile(model);
            Assert.That(actual, Is.Not.Null);
            
            var source = CompileToSource(actual);
            
            var expectedNamespaceDeclaration = String.Format("namespace {0}", ns.Name);
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
