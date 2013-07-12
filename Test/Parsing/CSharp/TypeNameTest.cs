using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing.CSharp
{
    [TestFixture]
    public class TypeNameTest
    {
         [Test]
         public void EqualityOperations()
         {
             var a = new TypeName("name");
             var b = new TypeName("name");
             var c = new TypeName("name");
             
             var otherName = new TypeName("other name");

             EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherName);
             EqualityTesting.TestEqualityOperators(a,b,c, otherName);
         }
    }
}