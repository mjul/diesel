using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diesel;
using NUnit.Framework;

namespace Test.Diesel
{
    [TestFixture]
    public class DieselCompilerIntegrationTest
    {
        [Test]
        public void Compile_ValidModelSource_ShouldGenerateSourceCode()
        {
            var sourceCode = CompileIngegrationTestCases();

            Assert.That(sourceCode, Is.Not.Empty);
            Assert.That(sourceCode, Is.StringContaining("namespace Employees"));
            Assert.That(sourceCode, Is.StringContaining("struct EmployeeNumber"));
            Assert.That(sourceCode, Is.StringContaining("class ChangeTelephoneNumber"));

            Assert.That(sourceCode, Is.StringContaining("namespace Clients"));
            Assert.That(sourceCode, Is.StringContaining("interface IImportService"));
            Assert.That(sourceCode, Is.StringContaining("class ImportClient"));
        }

        private static string CompileIngegrationTestCases()
        {
            var modelSource = CodeExamples.DieselCompilerIntegrationTestCase;
            var sourceCode = DieselCompiler.Compile(modelSource);
            return sourceCode;
        }

        [Test]
        public void Compile_ValidModelSource_ShouldIncludePropertyTypeDeclarations()
        {
            var sourceCode = CompileIngegrationTestCases();
            Assert.That(sourceCode, Is.StringContaining("class PrintInt"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNullable"));
            Assert.That(sourceCode, Is.StringContaining("class PrintArraySimple"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNullableSimple"));
            Assert.That(sourceCode, Is.StringContaining("class PrintString"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNamedTypeQualifiedDateTime"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNamedTypeQualifiedGuid"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNamedTypeUnqualifiedGuid"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNamedTypeUnqualifiedDecimal"));
            Assert.That(sourceCode, Is.StringContaining("class PrintMulti"));
        }

        [Test]
        private void Compile_ValidModelSource_ShouldIncludeDefapplicationserviceExamples()
        {
            var sourceCode = CompileIngegrationTestCases();
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Defapplicationservice"));
            Assert.That(sourceCode, Is.StringContaining("interface IImportApplicationService"));
            Assert.That(sourceCode, Is.StringContaining("class ImportApplicationServiceCommand"));
        }


        [Test]
        private void Compile_ValidModelSource_ShouldIncludeDefdomaineventExamples()
        {
            var sourceCode = CompileIngegrationTestCases();
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Defdomainevent"));
            Assert.That(sourceCode, Is.StringContaining("sealed partial class PaymentReceived"));
            Assert.That(sourceCode, Is.StringMatching("class PaymentReceived :.*Test.Diesel.IDomainEvent"));
        }

        [Test]
        private void Compile_ValidModelSource_ShouldIncludeDefvaluetypeExamples()
        {
            var sourceCode = CompileIngegrationTestCases();
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Defvaluetype"));
            Assert.That(sourceCode, Is.StringContaining("struct InvoiceNumber"));
            Assert.That(sourceCode, Is.StringContaining("struct Amount"));
            Assert.That(sourceCode, Is.StringContaining("struct LineItemNumber"));
            Assert.That(sourceCode, Is.StringContaining("struct Name"));
            Assert.That(sourceCode, Is.StringContaining("struct SourceMetadata"));
        }

        [Test]
        private void Compile_ValidModelSource_ShouldIncludeDefcommandExamples()
        {
            var sourceCode = CompileIngegrationTestCases();
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Defcommand"));
            Assert.That(sourceCode, Is.StringContaining("class PrintString"));
            Assert.That(sourceCode, Is.StringContaining("class PrintNullable"));
            Assert.That(sourceCode, Is.StringContaining("class PrintMultiple"));
            Assert.That(sourceCode, Is.StringContaining("class PrintDateTime"));
            Assert.That(sourceCode, Is.StringContaining("class PrintGuid"));
        }

        [Test]
        public void Compile_ValidModelSource_ShouldIncludeDefdtoExamples()
        {
            var sourceCode = CompileIngegrationTestCases(); 
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Defdto"));
            Assert.That(sourceCode, Is.StringContaining("class EmployeeName"));
        }

        [Test]
        public void Compile_ValidModelSource_ShouldIncludeDefenumExamples()
        {
            var sourceCode = CompileIngegrationTestCases(); 
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Defenum"));
            Assert.That(sourceCode, Is.StringContaining("enum State"));
        }

        [Test]
        public void Compile_ValidModelSource_ShouldIncludeCommentExamples()
        {
            var sourceCode = CompileIngegrationTestCases(); 
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.Comments"));
            Assert.That(sourceCode, Is.StringContaining("struct CommentId"));
            Assert.That(sourceCode, Is.StringContaining("interface ICommentService"));
            Assert.That(sourceCode, Is.StringContaining("class ImportComment"));
        }


        [Test]
        public void Compile_ValidModelSource_ShouldIncludeNestedDtoExamples()
        {
            var sourceCode = CompileIngegrationTestCases();
            Assert.That(sourceCode, Is.StringContaining("namespace TestCases.NestedDtos"));
            Assert.That(sourceCode, Is.StringContaining("class DtoNestedEnum"));
            Assert.That(sourceCode, Is.StringContaining("class DtoNestedDto"));
            Assert.That(sourceCode, Is.StringContaining("class ImportPersonNestedEnum"));
            Assert.That(sourceCode, Is.StringContaining("class PersonImportedNestedEnum"));
            Assert.That(sourceCode, Is.StringContaining("class ImportPersonNestedDto"));
            Assert.That(sourceCode, Is.StringContaining("class PersonImportedNestedDto"));
        }


        [Test]
        public void Compile_InvalidModelSource_ShouldFail()
        {
            const string modelSource = "(namespace Foo (defvaluetype SpuriousParen)) (";
            Assert.Throws<Sprache.ParseException>(() => DieselCompiler.Compile(modelSource));
        }
    }
}
