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
            var modelSource = CodeExamples.DieselCompilerIntegrationTestCase;
            var sourceCode = DieselCompiler.Compile(modelSource);

            Assert.That(sourceCode, Is.Not.Empty);
            Assert.That(sourceCode, Is.StringContaining("namespace Employees"));
            Assert.That(sourceCode, Is.StringContaining("struct EmployeeNumber"));
            Assert.That(sourceCode, Is.StringContaining("class ChangeTelephoneNumber"));
            Assert.That(sourceCode, Is.StringContaining("namespace Clients"));
            Assert.That(sourceCode, Is.StringContaining("interface IImportService"));
            Assert.That(sourceCode, Is.StringContaining("class ImportClient"));
        }
    }
}
