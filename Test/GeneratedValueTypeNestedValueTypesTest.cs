using NUnit.Framework;
using Test.Diesel.Generated;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedValueTypeNestedValueTypesTest
    {
        [Test]
        public void EqualsAndGetHashCodeAndEqualityOperators()
         {
             var alice = new EmployeeName("Alice", "Crypto");
             var bob = new EmployeeName("Bob", "Crypto");

             var aliceNumber = new EmployeeNumber(1);
             var bobNumber = new EmployeeNumber(2);

             var aliceEmail = new EmailAddress("alice@crypto.org");
             var bobEmail = new EmailAddress("bob@crypto.org");

             var a = new EmployeeInfo(aliceNumber, alice, aliceEmail);
             var b = new EmployeeInfo(aliceNumber, alice, aliceEmail);
             var c = new EmployeeInfo(aliceNumber, alice, aliceEmail);

             var otherNumber = new EmployeeInfo(bobNumber, alice, aliceEmail);
             var otherName = new EmployeeInfo(aliceNumber, bob, aliceEmail);
             var otherEmail = new EmployeeInfo(aliceNumber, alice, bobEmail);

             TestHelpers.EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherNumber, otherName, otherEmail);
             TestHelpers.EqualityTesting.TestEqualityOperators(a, b, c, otherNumber, otherName, otherEmail);
         }
    }
}