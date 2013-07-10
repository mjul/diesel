using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel.Parsing
{
    [TestFixture]
    public class DomainEventConventionsTest
    {
        [Test]
        public void Override_WithNull_ShouldReturnSameInstance()
        {
            var instance = new DomainEventConventions(new BaseTypes(new TypeName[] { }));
            var actual = instance.ApplyOverridesFrom(null);
            Assert.That(actual, Is.SameAs(instance));
        }

        [Test]
        public void Override_WithEmptyBaseTypes_ShouldReturnInstanceWithEmptyBaseTypes()
        {
            var instance = new DomainEventConventions(
                new BaseTypes(new[] { new TypeName("Foo.EventBase") }));
            var actual = instance.ApplyOverridesFrom(
                new DomainEventConventions(new BaseTypes(new TypeName[] { })));
            Assert.That(actual.BaseTypes, Is.EqualTo(new BaseTypes(new TypeName[0])));
        }

        [Test]
        public void Override_WithNewBaseTypes_ShouldReturnInstanceWithNewBaseTypes()
        {
            var instance = new DomainEventConventions(
                new BaseTypes(new[] { new TypeName("Foo.EventBase") }));

            var actual = instance.ApplyOverridesFrom(
                new DomainEventConventions(
                    new BaseTypes(
                        new[]
                            {
                                new TypeName("Bar.EventBase"),
                                new TypeName("Bar.IDomainEvent")
                            })));
            Assert.That(actual.BaseTypes.TypeNames.Count(), Is.EqualTo(2));
            Assert.That(actual.BaseTypes.TypeNames.SingleOrDefault(x => x.Name == "Bar.EventBase"), Is.Not.Null);
            Assert.That(actual.BaseTypes.TypeNames.SingleOrDefault(x => x.Name == "Bar.IDomainEvent"), Is.Not.Null);
        }

        [Test]
         public void EqualityMethods()
         {
             var baseTypes = new BaseTypes(new[] {new TypeName("SomeNamespace.IContract")});
             var anotherContractBaseTypes = new BaseTypes(new[] { new TypeName("SomeNamespace.IAnotherContract") });

             var a = new DomainEventConventions(baseTypes);
             var b = new DomainEventConventions(baseTypes);
             var c = new DomainEventConventions(baseTypes);

             var otherBaseTypes = new DomainEventConventions(anotherContractBaseTypes);

             EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherBaseTypes);
             EqualityTesting.TestEqualityOperators(a, b, c, otherBaseTypes);
         }
    }
}