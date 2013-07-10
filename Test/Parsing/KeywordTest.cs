using Diesel.Parsing;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class KeywordTest
    {
        [Test]
        public void EqualsAndGetHashCodeAndEquality()
        {
            var a = new Keyword("a");
            var b = new Keyword("a");
            var c = new Keyword("a");

            var otherName = new Keyword("b");

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherName);
            EqualityTesting.TestEqualityOperators(a, b, c, otherName);
        }
    }
}