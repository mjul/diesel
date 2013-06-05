using System.Linq;
using Diesel.Parsing;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class ConventionsDeclarationTest
    {
        [Test]
        public void Override_WithNull_ShouldReturnSameInstance()
        {
            var instance = new ConventionsDeclaration(new DomainEventConventions(new TypeName[] {}));
            var actual = instance.ApplyOverridesFrom(null);
            Assert.That(actual, Is.SameAs(instance));
        }

        [Test]
        public void Override_WithOther_ShouldReturnNewInstance()
        {
            var instance = new ConventionsDeclaration(new DomainEventConventions(new TypeName[] { }));
            var other = new ConventionsDeclaration(new DomainEventConventions(new[] {new TypeName("Foo.EventBase")}));
            var actual = instance.ApplyOverridesFrom(other);
            Assert.That(actual.DomainEventConventions.BaseTypes.Single().Name == "Foo.EventBase");
        }
    }
}