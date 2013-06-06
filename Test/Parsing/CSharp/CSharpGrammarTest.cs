using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Sprache;

namespace Test.Diesel.Parsing.CSharp
{
    [TestFixture]
    public class CSharpGrammarTest
    {

        [Test]
        public void Identifier_ValidSingleLetter_ShouldParse()
        {
            var actual = CSharpGrammar.Identifier.Parse("x");
            Assert.That(actual.Name, Is.EqualTo("x"));
        }

        [Test]
        public void Identifier_ValidString_ShouldParse()
        {
            var actual = CSharpGrammar.Identifier.Parse("name");
            Assert.That(actual.Name, Is.EqualTo("name"));
        }

        [Test]
        public void Identifier_StartsWithUnderscore_ShouldParse()
        {
            var actual = CSharpGrammar.Identifier.Parse("_name");
            Assert.That(actual.Name, Is.EqualTo("_name"));
        }

        [Test]
        public void Identifier_IncludesUnderscore_ShouldParse()
        {
            var actual = CSharpGrammar.Identifier.Parse("first_name");
            Assert.That(actual.Name, Is.EqualTo("first_name"));
        }


        [Test]
        public void Identifier_Blank_ShouldNotParse()
        {
            var actual = CSharpGrammar.Identifier.TryParse("");
            Assert.That(actual.WasSuccessful, Is.False);
        }

        [Test]
        public void Identifier_ValidStringAndNumber_ShouldParse()
        {
            var actual = CSharpGrammar.Identifier.Parse("name1");
            Assert.That(actual.Name, Is.EqualTo("name1"));
        }


        [Test]
        public void NamespaceIdentifier_BlankName_ShouldNotParse()
        {
            var actual = CSharpGrammar.NamespaceName.TryParse("");
            Assert.False(actual.WasSuccessful);
        }

        [Test]
        public void NamespaceIdentifier_SinglePartName_ShouldParse()
        {
            var actual = CSharpGrammar.NamespaceName.Parse("System");
            Assert.That(actual.Name, Is.EqualTo("System"));
        }

        [Test]
        public void NamespaceIdentifier_MultiPartName_ShouldParse()
        {
            var actual = CSharpGrammar.NamespaceName.Parse("System.Runtime.Serialization");
            Assert.That(actual.Name, Is.EqualTo("System.Runtime.Serialization"));
        }

        [Test]
        public void TypeName_ValidUnqualifiedName_ShouldParse()
        {
            var actual = CSharpGrammar.TypeName.Parse("Guid");
            Assert.That(actual.Name, Is.EqualTo("Guid"));
        }

        [Test]
        public void TypeName_ValidQualifiedName_ShouldParse()
        {
            var actual = CSharpGrammar.TypeName.Parse("System.Guid");
            Assert.That(actual.Name, Is.EqualTo("System.Guid"));
        }

        [Test]
        public  void ArrayType_UnidimensionalQualifiedName_ShouldParse()
        {
            var actual = CSharpGrammar.ArrayType(CSharpGrammar.TypeName).Parse("System.Int32[]");
            Assert.That(actual.Name, Is.EqualTo("System.Int32[]"));
        }

        [Test]
        public void ArrayType_UnidimensionalSimpleName_ShouldParse()
        {
            var actual = CSharpGrammar.ArrayType(CSharpGrammar.TypeName).Parse("Guid[]");
            Assert.That(actual.Name, Is.EqualTo("Guid[]"));
        }
    }
}