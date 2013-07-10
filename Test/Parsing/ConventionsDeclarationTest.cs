using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class ConventionsDeclarationTest
    {
        [Test]
        public void ApplyOverrides_WithNullConventions_ShouldReturnEqualInstance()
        {
            var domainEventsConventions = new DomainEventConventions(new BaseTypes(new TypeName[] {}));
            var commandConventions = new CommandConventions(new BaseTypes(new TypeName[] {}));
            var instance = new ConventionsDeclaration(domainEventsConventions, commandConventions);

            var actual = instance.ApplyOverridesFrom(new ConventionsDeclaration(null, null));

            Assert.That(actual, Is.EqualTo(instance));
        }

        [Test]
        public void ApplyOverrides_WithNewDomainEventConventions_ShouldReturnNewInstanceWithThese()
        {
            var domainEventsConventions = new DomainEventConventions(new BaseTypes(new TypeName[] { }));
            var commandConventions = new CommandConventions(new BaseTypes(new TypeName[] { }));
            var instance = new ConventionsDeclaration(domainEventsConventions, commandConventions);

            var newDomainEventsConventions = new DomainEventConventions(
                new BaseTypes(new[] {new TypeName("SomeNamespace.IDomainEvent")}));

            var actual = instance.ApplyOverridesFrom(new ConventionsDeclaration(newDomainEventsConventions, null));

            Assert.That(actual, Is.Not.SameAs(instance));
            Assert.That(actual.DomainEventConventions, Is.EqualTo(newDomainEventsConventions));
            Assert.That(actual.CommandConventions, Is.EqualTo(commandConventions));
        }


        [Test]
        public void ApplyOverrides_WithNewCommandConventions_ShouldReturnNewInstanceWithThese()
        {
            var domainEventsConventions = new DomainEventConventions(new BaseTypes(new TypeName[] { }));
            var commandConventions = new CommandConventions(new BaseTypes(new TypeName[] { }));
            var instance = new ConventionsDeclaration(domainEventsConventions, commandConventions);

            var newCommandConventions = new CommandConventions(
                new BaseTypes(new[] {new TypeName("SomeNamespace.ICommand")}));

            var actual = instance.ApplyOverridesFrom(new ConventionsDeclaration(null, newCommandConventions));

            Assert.That(actual, Is.Not.SameAs(instance));
            Assert.That(actual.DomainEventConventions, Is.EqualTo(domainEventsConventions));
            Assert.That(actual.CommandConventions, Is.EqualTo(newCommandConventions));
        }

        [Test]
        public void EqualityOperations()
        {
            var domainEventsConventions = new DomainEventConventions(new BaseTypes(new TypeName[] { }));
            var commandConventions = new CommandConventions(new BaseTypes(new TypeName[] { }));

            var a = new ConventionsDeclaration(domainEventsConventions, commandConventions);
            var b = new ConventionsDeclaration(domainEventsConventions, commandConventions);
            var c = new ConventionsDeclaration(domainEventsConventions, commandConventions);

            var otherDomainEvents = new ConventionsDeclaration(
                new DomainEventConventions(new BaseTypes(new[] { new TypeName("Some.BaseType") })), 
                commandConventions);
            var otherCommands = new ConventionsDeclaration(
                domainEventsConventions, 
                new CommandConventions(new BaseTypes(new[] { new TypeName("Some.BaseType") })));

            EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherDomainEvents, otherCommands);
            EqualityTesting.TestEqualityOperators(a,b,c, otherDomainEvents, otherCommands);
        }

    }
}