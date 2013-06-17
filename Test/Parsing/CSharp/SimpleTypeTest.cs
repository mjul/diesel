using System;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.Parsing.CSharp
{
    [TestFixture]
    public class SimpleTypeTest
    {
        [Test]
        public void Constructor_WithType_ShouldSetName()
        {
            var actual = new SimpleType(typeof(Int32));
            Assert.That(actual.Type, Is.EqualTo(typeof(Int32)));
        }
    }
}