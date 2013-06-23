using System;
using Diesel.Parsing;
using NUnit.Framework;
using Sprache;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class TokenGrammarTest
    {
        [Test]
        public void RestOfLine_EmptyString_ShouldReturnEmptyString()
        {
            var actual = TokenGrammar.RestOfLine.Parse("");
            Assert.That(actual, Is.EqualTo(""));
        }

        [Test]
        public void RestOfLine_JustNewLine_ShouldReturnEmptyString()
        {
            var actual = TokenGrammar.RestOfLine.Parse(Environment.NewLine);
            Assert.That(actual, Is.EqualTo(""));
        }

        [Test]
        public void RestOfLine_TextAndNewLine_ShouldReturnText()
        {
            var actual = TokenGrammar.RestOfLine.Parse("Text" + Environment.NewLine);
            Assert.That(actual, Is.EqualTo("Text"));
        }

        [Test]
        public void RestOfLine_TextAndEndOfFile_ShouldReturnText()
        {
            var actual = TokenGrammar.RestOfLine.Parse("Text");
            Assert.That(actual, Is.EqualTo("Text"));
        }
    }
}