using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class CommandConventionsTest
    {
        [Test]
        public void EqualityMethods()
        {
            var baseTypes = new BaseTypes(new[] {new TypeName("SomeNamespace.IContract")});
            var anotherContractBaseTypes = new BaseTypes(new[] { new TypeName("SomeNamespace.IAnotherContract") });

            var a = new CommandConventions(baseTypes);
            var b = new CommandConventions(baseTypes);
            var c = new CommandConventions(baseTypes);

            var otherBaseTypes = new CommandConventions(anotherContractBaseTypes);

            EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherBaseTypes);
            EqualityTesting.TestEqualityOperators(a, b, c, otherBaseTypes);
        }
    }
}