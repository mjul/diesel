using System;
using NUnit.Framework;
using Test.Diesel.Generated;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedDomainEventWithArrayTest
    {
        [Test]
        public void EqualsGetHashCodeAndEqualityOperators()
        {
            var id = Guid.NewGuid();
            var aliceAndBob = new[]
                {
                    new Name("Alice", "Crypto"),
                    new Name("Bob", "Crypto")
                };
            var coryAndDan = new[]
                {
                    new Name("Cory", "Crypto"),
                    new Name("Dan", "Crypto")
                };
            
            var a = new DepartmentImported(id, aliceAndBob);
            var b = new DepartmentImported(id, aliceAndBob);
            var c = new DepartmentImported(id, aliceAndBob);

            var otherNamesSameCount = new DepartmentImported(id, coryAndDan);
            var otherNamesDifferentCount = new DepartmentImported(id, new[] {aliceAndBob[0]});
            var otherNamesNull= new DepartmentImported(id, null);
            var otherNamesSameCountNullElements = new DepartmentImported(id, new Name[] { null,null });

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c,
                                                     otherNamesSameCount,
                                                     otherNamesDifferentCount,
                                                     otherNamesNull, 
                                                     otherNamesSameCountNullElements);

            EqualityTesting.TestEqualityOperators(a, b, c,
                                                  otherNamesSameCount,
                                                  otherNamesDifferentCount,
                                                  otherNamesNull,
                                                  otherNamesSameCountNullElements);
        }
 
    }
}