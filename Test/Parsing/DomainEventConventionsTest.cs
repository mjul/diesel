using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class DomainEventConventionsTest
    {
         [Test]
         public void EqualityMethods()
         {
             var baseTypes = new BaseTypes(new[] {new TypeName("SomeNamespace.IContract")});
             var anotherContractBaseTypes = new BaseTypes(new[] { new TypeName("SomeNamespace.IAnotherContract") });

             var a = new DomainEventConventions(baseTypes);
             var b = new DomainEventConventions(baseTypes);
             var c = new DomainEventConventions(baseTypes);

             var otherBaseTypes = new DomainEventConventions(anotherContractBaseTypes);

             EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherBaseTypes);
             EqualityTesting.TestEqualityOperators(a, b, c, otherBaseTypes);
         }
    }
}