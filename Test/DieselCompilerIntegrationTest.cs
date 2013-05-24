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
            const string modelSource = 
                @"(namespace Employees (defvaluetype EmployeeNumber int))" +
                @"(namespace Clients (defvaluetype ClientId))";
            var sourceCode = DieselCompiler.Compile(modelSource);
            Assert.That(sourceCode, Is.Not.Empty);
            Assert.That(sourceCode, Is.StringContaining("namespace Employees"));
            Assert.That(sourceCode, Is.StringContaining("namespace Clients"));
        }
    }
}
