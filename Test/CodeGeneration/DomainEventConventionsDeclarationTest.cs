using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class DomainEventConventionsTest
    {
        [Test]
        public void Override_WithNull_ShouldReturnSameInstance()
        {
            var instance = new DomainEventConventions(new TypeName[] {});
            var actual = instance.ApplyOverridesFrom(null);
            Assert.That(actual, Is.SameAs(instance));
        }

        [Test]
        public void Override_WithEmptyBaseTypes_ShouldReturnInstanceWithEmptyBaseTypes()
        {
            var instance = new DomainEventConventions(new[] { new TypeName("Foo.EventBase")  });
            var actual = instance.ApplyOverridesFrom(new DomainEventConventions(new TypeName[] {}));
            Assert.That(actual.BaseTypes, Is.Empty);
        }

        [Test]
        public void Override_WithNewBaseTypes_ShouldReturnInstanceWithNewBaseTypes()
        {
            var instance = new DomainEventConventions(new[] { new TypeName("Foo.EventBase") });
            var actual = instance.ApplyOverridesFrom(new DomainEventConventions(new[]
                {
                    new TypeName("Bar.EventBase"),
                    new TypeName("Bar.IDomainEvent")
                }));
            Assert.That(actual.BaseTypes.Count(), Is.EqualTo(2));
            Assert.That(actual.BaseTypes.SingleOrDefault(x => x.Name == "Bar.EventBase"), Is.Not.Null);
            Assert.That(actual.BaseTypes.SingleOrDefault(x => x.Name == "Bar.IDomainEvent"), Is.Not.Null);
        }

    }
}