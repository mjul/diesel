using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class ConventionsDeclarationTest
    {
        [Test]
        public void Override_WithNull_ShouldReturnSameInstance()
        {
            var instance = new ConventionsDeclaration(
                new DomainEventConventions(new BaseTypes(new TypeName[] {})),
                new CommandConventions(new BaseTypes(new TypeName[] {})));
            var actual = instance.ApplyOverridesFrom(null);
            Assert.That(actual, Is.SameAs(instance));
        }

        [Test]
        public void Override_WithOtherDomainEventConventions_ShouldReturnNewInstance()
        {
            var instance = new ConventionsDeclaration(
                new DomainEventConventions(new BaseTypes(new TypeName[] { })),
                new CommandConventions(new BaseTypes(new TypeName[] { })));
            var other = new ConventionsDeclaration(
                new DomainEventConventions(
                    new BaseTypes(new[] {new TypeName("Foo.EventBase")})),
                null);
            var actual = instance.ApplyOverridesFrom(other);
            Assert.That(actual.DomainEventConventions.BaseTypes.TypeNames.Single().Name == "Foo.EventBase");
        }

        [Test]
        public void Override_WithOtherCommandConventions_ShouldReturnNewInstance()
        {
            var instance = new ConventionsDeclaration(
                new DomainEventConventions(new BaseTypes(new TypeName[] { })),
                new CommandConventions(new BaseTypes(new TypeName[] { })));
            var other = new ConventionsDeclaration(
                new DomainEventConventions(
                    new BaseTypes(new[] {new TypeName("Foo.EventBase")})),
                null);
            var actual = instance.ApplyOverridesFrom(other);
            Assert.That(actual.DomainEventConventions.BaseTypes.TypeNames.Single().Name == "Foo.EventBase");
        }

    }
}