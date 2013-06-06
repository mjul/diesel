using Diesel.Parsing;
using NUnit.Framework;
using Sprache;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class CSharpGrammarTest
    {
        [Test]
        public  void ArrayType_UnidimensionalQualifiedName_ShouldParse()
        {
            var actual = Grammar.ArrayType.Parse("System.Int32[]");
            Assert.That(actual.Name, Is.EqualTo("System.Int32[]"));
        }

        [Test]
        public void ArrayType_UnidimensionalSimpleName_ShouldParse()
        {
            var actual = Grammar.ArrayType.Parse("Guid[]");
            Assert.That(actual.Name, Is.EqualTo("Guid[]"));
        }

    }
}