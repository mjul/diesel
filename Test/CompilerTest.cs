using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Diesel;
using NUnit.Framework;

namespace Test.Diesel
{
    [TestFixture]
    public class CompilerTest
    {
        [Test]
        public void ValueType_ValidDeclaration_ShouldParse()
        {
            var actual = Compiler.Compile(new ValueTypeDeclaration("EmployeeNumber", typeof(int)));
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Console.WriteLine(source);
        }

        [Test]
        public void CommandDeclaration_ValidDeclaration_ShouldParse()
        {
            var actual = Compiler.Compile(new CommandDeclaration("ImportEmployeeCommand", new[]
                {
                    new PropertyDeclaration("EmployeeNumber", typeof(int)), 
                    new PropertyDeclaration("FirstName", typeof(String)), 
                    new PropertyDeclaration("LastName", typeof(String))
                }));
            Assert.That(actual, Is.Not.Null);
            var source = CompileToSource(actual);
            Console.WriteLine(source);
        }


        private static string CompileToSource(CodeCompileUnit actual)
        {
            var output = new StringWriter();
            var provider = CSharpProvider();
            provider.GenerateCodeFromCompileUnit(actual, output,
                                                 new CodeGeneratorOptions() { BlankLinesBetweenMembers = true });
            var source = output.GetStringBuilder().ToString();
            return source;
        }

        private static CodeDomProvider CSharpProvider()
        {
            return CodeDomProvider.CreateProvider("CSharp");
        }
    }
}
