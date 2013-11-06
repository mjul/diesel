using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Test.Diesel.Generated;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    /// <summary>
    /// Here, we are just interested in checking that user-defined (non-system) value types can be 
    /// used as members on the generated contracts.
    /// </summary>
    [TestFixture]
    public class GeneratedDtoWithNullableNonSystemMemberTypeTest
    {
        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var alice = new Name("Alice", "Crypto");
            var bob = new Name("Bob", "Crypto");

            var a = new NamedPerson(alice, Gender.Female);
            var b = new NamedPerson(alice, Gender.Female);
            var c = new NamedPerson(alice, Gender.Female);

            var otherName = new NamedPerson(bob, Gender.Female);
            var otherGenderNull = new NamedPerson(alice, null);
            var otherGenderMale = new NamedPerson(alice, Gender.Male);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                                                     otherName, otherGenderNull, otherGenderMale);
            EqualityTesting.TestEqualityOperators(a, b, c,
                                                  otherName, otherGenderNull, otherGenderMale);
        }
    }
}