using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Diesel;
using Diesel.CodeGeneration;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class CodeDomCompilerTest
    {
        [Test]
        public void ValueType_ValidDeclaration_ShouldCompile()
        {
            var model = CreateSemanticModelWith(
                new ValueTypeDeclaration("EmployeeNumber",
                                         new[] {new PropertyDeclaration("Value", new SimpleType(typeof (int)))}));
            var actual = CodeDomCompiler.Compile(model);
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("public partial struct EmployeeNumber"));
            Console.WriteLine(source);
        }


        [Test]
        public void ValueType_WithNestedValueTypes_ShouldCompile()
        {
            var employeeNumber = new ValueTypeDeclaration(
                "EmployeeNumber",
                new[]
                    {
                        new PropertyDeclaration("Value",
                                                new SimpleType(typeof (int)))
                    });
            var employeeName = new ValueTypeDeclaration(
                "EmployeeName",
                new[]
                    {
                        new PropertyDeclaration("First",
                                                new StringReferenceType()),
                        new PropertyDeclaration("Last",
                                                new StringReferenceType()),
                    }
                );
            var employeeInfo = new ValueTypeDeclaration(
                "EmployeeInfo",
                new[]
                    {
                        new PropertyDeclaration("Number",
                                                new TypeNameTypeNode(new TypeName("EmployeeNumber"))),
                        new PropertyDeclaration("Name",
                                                new TypeNameTypeNode(new TypeName("EmployeeName"))),
                    }
                );
            var model = CreateSemanticModelWith(employeeNumber, employeeName, employeeInfo);
            var actual = CodeDomCompiler.Compile(model);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringMatching(@"public\s+EmployeeNumber\s+Number"));
            Assert.That(source, Is.StringMatching(@"public\s+EmployeeName\s+Name"));
            Console.WriteLine(source);
        }



        [Test]
        public void ValueType_ValidDeclarationMultipleProperties_ShouldCompile()
        {
            var model = CreateSemanticModelWith(
                new ValueTypeDeclaration("Point",
                                         new[]
                                             {
                                                 new PropertyDeclaration("X", new SimpleType(typeof (int))),
                                                 new PropertyDeclaration("Y", new SimpleType(typeof (int)))
                                             }));
            var actual = CodeDomCompiler.Compile(model);
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Assert.That(source, Is.StringContaining("struct Point"));
            Console.WriteLine(source);
        }


        [Test]
        public void ValueType_ValidDeclaration_ShouldNotHaveDataContractAttributes()
        {
            var model = CreateSemanticModelWith(
                new ValueTypeDeclaration("EmployeeNumber",
                                         new[] { new PropertyDeclaration("Value", new SimpleType(typeof (int))) }));
            var actual = CodeDomCompiler.Compile(model);
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
            var model = CreateSemanticModelWith(enumDeclaration, commandDeclaration);
            var actual = CodeDomCompiler.Compile(model);

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
            var model = CreateSemanticModelWith(dtoDeclaration, commandDeclaration);
            var actual = CodeDomCompiler.Compile(model);

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

        [Test]
        public void CommandDeclaration_WithConventionInheritICommand_ShouldProduceClassInheritingICommand()
        {
            var declaration = CreateImportEmployeeCommandDeclaration();
            var conventions = new ConventionsDeclaration(
                new DomainEventConventions(new BaseTypes(new TypeName[0])),
                new CommandConventions(new BaseTypes(new[] { new TypeName("Test.Diesel.ICommand") })));
            var model = CreateSemanticModelWith(conventions, declaration);
            var source = CompileToSource(CodeDomCompiler.Compile(model));
            Assert.That(source, Is.StringMatching(@"class ImportEmployeeCommand :.* Test.Diesel.ICommand \{"));
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
            var model = CreateSemanticModelWith(commandDeclaration);
            return CodeDomCompiler.Compile(model);
        }

        [Test]
        public void DomainEventDeclaration_ValidDeclaration_ShouldParse()
        {
            var source = CompileToSource(CompileEmployeeImportedEvent());
            Assert.That(source, Is.StringContaining(@"class EmployeeImported"));
        }


        [Test]
        public void DomainEventDeclaration_WithNestedDto_Enum_ShouldCompile()
        {
            var enumDeclaration = new EnumDeclaration("Role", new[] {"Tester", "Developer"});
            var eventDeclaration = new DomainEventDeclaration(
                "EmployeeImported",
                new[]
                    {
                        new PropertyDeclaration(
                            "EmployeeNumber", new SimpleType(typeof (int))),
                        new PropertyDeclaration(
                            "EmployeeRole", new TypeNameTypeNode(new TypeName("Role")))
                    });
            var model = CreateSemanticModelWith(enumDeclaration, eventDeclaration);
            var actual = CodeDomCompiler.Compile(model);
            var source = CompileToSource(actual);

            Assert.That(source, Is.StringMatching(@"Role\s+EmployeeRole"));
        }

        [Test]
        public void DomainEventDeclaration_WithNestedDto_Dto_ShouldCompile()
        {
            var dtoDeclaration = new DtoDeclaration(
                "EmployeeName",
                new[]
                    {
                        new PropertyDeclaration("First", new StringReferenceType()),
                        new PropertyDeclaration("Last", new StringReferenceType()),
                    });
            var eventDeclaration = new DomainEventDeclaration(
                "EmployeeImported",
                new[]
                    {
                        new PropertyDeclaration(
                            "EmployeeNumber", new SimpleType(typeof (int))),
                        new PropertyDeclaration(
                            "Name", new TypeNameTypeNode(new TypeName("EmployeeName")))
                    });
            var model = CreateSemanticModelWith(dtoDeclaration, eventDeclaration);
            var actual = CodeDomCompiler.Compile(model);
            var source = CompileToSource(actual);

            Assert.That(source, Is.StringMatching(@"EmployeeName\s+Name"));
        }

        [Test]
        public void DomainEventDeclaration_WithNestedDto_DtoArray_ShouldCompile()
        {
            var dtoDeclaration = new DtoDeclaration(
                "EmployeeName",
                new[]
                    {
                        new PropertyDeclaration("First", new StringReferenceType()),
                        new PropertyDeclaration("Last", new StringReferenceType()),
                    });
            var eventDeclaration = new DomainEventDeclaration(
                "DepartmentImported",
                new[]
                    {
                        new PropertyDeclaration(
                            "DepartmentNumber", new SimpleType(typeof (int))),
                        new PropertyDeclaration(
                            "Employees", 
                            new ArrayType(new TypeNameTypeNode(new TypeName("EmployeeName")),
                                new RankSpecifiers(new [] {new RankSpecifier(1)}))
                            )
                    });
            var model = CreateSemanticModelWith(dtoDeclaration, eventDeclaration);
            var actual = CodeDomCompiler.Compile(model);
            var source = CompileToSource(actual);

            Assert.That(source, Is.StringMatching(@"EmployeeName\[\]\s+Employees"));
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
                new DomainEventConventions(
                    new BaseTypes(new[] {new TypeName("Test.Diesel.IDomainEvent")})),
                new CommandConventions(new BaseTypes(new TypeName[0])));
            var model = CreateSemanticModelWith(conventions, declaration);
            var source = CompileToSource(CodeDomCompiler.Compile(model));
            Assert.That(source, Is.StringMatching(@"class EmployeeImported :.* Test.Diesel.IDomainEvent \{"));
        }

        private CodeCompileUnit CompileEmployeeImportedEvent()
        {
            var declaration = CreateEmployeeImportedEventDeclaration();
            var model = CreateSemanticModelWith(declaration);
            return CodeDomCompiler.Compile(model);
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
        public void DtoDeclaration_WithNestedDto_Dto_ShouldCompile()
        {
            var nameDto = CreateNameDtoDeclaration();
            var employeeInfoDto = new DtoDeclaration(
                "EmployeeInfo",
                new[]
                    {
                        new PropertyDeclaration("EmployeeName", new TypeNameTypeNode(new TypeName("Name")))
                    }
                );
            var source = CompileToSource(CodeDomCompiler
                                             .Compile(CreateSemanticModelWith(nameDto, employeeInfoDto)));
            Assert.That(source, Is.StringMatching(@"public\s+Name\s+EmployeeName"));
        }

        [Test]
        public void DtoDeclaration_WithNestedDto_Enum_ShouldCompile()
        {
            var enumDeclaration = new EnumDeclaration("Role", new[] { "Tester", "Developer" });
            var employeeInfoDto = new DtoDeclaration(
                "EmployeeInfo",
                new[]
                    {
                        new PropertyDeclaration("EmployeeRole", new TypeNameTypeNode(new TypeName("Role")))
                    }
                );
            var source = CompileToSource(CodeDomCompiler
                                             .Compile(CreateSemanticModelWith(enumDeclaration, employeeInfoDto)));
            Assert.That(source, Is.StringMatching(@"public\s+Role\s+EmployeeRole"));
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
            var declaration = CreateNameDtoDeclaration();
            var model = CreateSemanticModelWith(declaration);
            return CodeDomCompiler.Compile(model);
        }

        private static DtoDeclaration CreateNameDtoDeclaration()
        {
            return new DtoDeclaration("Name",
                                      new[]
                                          {
                                              new PropertyDeclaration("First", new StringReferenceType()),
                                              new PropertyDeclaration("Last", new StringReferenceType())
                                          });
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
            var model = CreateSemanticModelWith(declaration);
            return CodeDomCompiler.Compile(model);
        }


        private SemanticModel CreateSemanticModelWith(params TypeDeclaration[] typeDeclaration)
        {
            return CreateSemanticModelWith(null, typeDeclaration);
        }

        private SemanticModel CreateSemanticModelWith(ConventionsDeclaration conventions, params TypeDeclaration[] typeDeclarations)
        {
            var ns = new NamespaceName(typeof(CodeDomCompilerTest).Namespace + ".Generated");
            return new SemanticModel(new AbstractSyntaxTree(conventions, new[] {new Namespace(ns, typeDeclarations)}));
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

            var dom = CodeDomCompiler.Compile(new SemanticModel(ast));
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
            var ns = new NamespaceName(typeof(CodeDomCompilerTest).Namespace + ".Generated");
            var model = new SemanticModel(new AbstractSyntaxTree(null, new[] {new Namespace(ns, declarations)}));
            var actual = CodeDomCompiler.Compile(model);
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
