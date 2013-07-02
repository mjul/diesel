using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Diesel;
using NUnit.Framework;

namespace Test.Diesel
{
    // Verify that GetHashCode includes model value type fields
    // This is not strictly required by its semantics, but
    // it verifies that code is generated that uses these fields
    // for the GetHashCode calculation.
    [TestFixture]
    public class GeneratedGetHashCodeIntegrationTest
    {
        private CompilerResults _compilerResults;

        public object CreateDtoInstance(Guid id, int number, string name, string genderName)
        {
            Assert.That(_compilerResults, Is.Not.Null);
            var genderType = _compilerResults.CompiledAssembly.GetType("Foo.Gender");
            Assert.That(genderType, Is.Not.Null);
            object gender = Enum.Parse(genderType, genderName);
            Assert.That(gender, Is.Not.Null);
            var instance = _compilerResults.CompiledAssembly
                                  .CreateInstance("Foo.ValueObject", false,
                                                  BindingFlags.CreateInstance,
                                                  null,
                                                  new object[] {id, number, name, gender},
                                                  null, null);
            return instance;
        }

        [TestFixtureSetUp]
        public void CompileModel()
        {
            var dieselSource = "(namespace Foo " +
                               "  (defenum Gender [Female Male]) " +
                               "  (defdto ValueObject (Guid Id, int Number, string Name, Gender Gender)))";
            _compilerResults = Compile(dieselSource);
        }

        private static CompilerResults Compile(string dieselSource)
        {
            var csharpSource = DieselCompiler.Compile(dieselSource);
            var csharpCompiler = DieselCompiler.GetCSharpProvider();
            var parameters = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    IncludeDebugInformation = false,
                    ReferencedAssemblies = { "System.Runtime.Serialization.dll" }
                };
            var result = csharpCompiler.CompileAssemblyFromSource(parameters, csharpSource);
            Assert.That(result.Errors, Is.Empty);
            return result;
        }

        [Test]
        public void GetHashCode_OtherValueOfValueTypeField_ShouldChangeHashCode()
        {
            var id = Guid.NewGuid();
            const int number = 1;
            const string name = "Kim Crypto";
            object male = CreateDtoInstance(id, number, name, "Male");
            object female = CreateDtoInstance(id, number, name, "Female");

            Assert.That(male.GetHashCode() != female.GetHashCode());
        }

        [Test]
        public void GetHashCode_OtherValueOfGuidField_ShouldChangeHashCode()
        {
            const int number = 1;
            const string name = "Kim Crypto";
            object a = CreateDtoInstance(Guid.NewGuid(), number, name, "Female");
            object b = CreateDtoInstance(Guid.NewGuid(), number, name, "Female");

            Assert.That(a.GetHashCode() != b.GetHashCode());
        }


        [Test]
        public void GetHashCode_OtherValueOfIntField_ShouldChangeHashCode()
        {
            var id = Guid.NewGuid();
            const int number = 1;
            const string name = "Kim Crypto";
            var a = CreateDtoInstance(id, number, name, "Female");
            var b = CreateDtoInstance(id, number + 1, name, "Female");

            Assert.That(a.GetHashCode() != b.GetHashCode());
        }

    }
}