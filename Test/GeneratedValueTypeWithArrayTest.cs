using NUnit.Framework;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedValueTypeWithArrayTest
    {
        // Generate the class to test and place it in the Generated folder:
        // (defvaluetype EmployeeRatings (int EmployeeNumber, int[] Ratings))

        [Test]
        public void Constructor_WithAllValues_ShouldSetProperties()
        {
            const int number = 1;
            var ratings = new [] {1, 2};
            var actual = new Generated.EmployeeRatings(number, ratings);
            Assert.That(actual.EmployeeNumber, Is.EqualTo(number));
            Assert.That(actual.Ratings, Is.EqualTo(ratings));
        }

        [Test]
        public void EqualsGetHashCodeAndEqualityOperators_TypeWithValueTypeArray_ShouldWork()
        {
            const int number = 1;
            var ratings = new[] { 1, 2 };

            var a = new Generated.EmployeeRatings(number, ratings);
            var b = new Generated.EmployeeRatings(number, new[] {1, 2});
            var c = new Generated.EmployeeRatings(number, new[] {1, 2});

            var otherEmployeeNumber = new Generated.EmployeeRatings(number + 1, ratings);
            var otherRatingsLength = new Generated.EmployeeRatings(number, new[] {1});
            var otherRatingsValues = new Generated.EmployeeRatings(number, new[] {1,3});
            var otherRatingsOrder = new Generated.EmployeeRatings(number, new[] { 2, 1 });
            var otherRatingsNull = new Generated.EmployeeRatings(number, null);

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherEmployeeNumber, otherRatingsLength, otherRatingsValues, otherRatingsOrder, otherRatingsNull);
            EqualityTesting.TestEqualityOperators(a, b, c, otherEmployeeNumber, otherRatingsLength, otherRatingsValues, otherRatingsOrder, otherRatingsNull);
        }


        [Test]
        public void EqualsGetHashCodeAndEqualityOperators_TypeWithStringReferenceTypeArray_ShouldWork()
        {
            const int number = 1;
            var roles = new[] { "tester", "developer" };

            var a = new Generated.EmployeeRoles(number, roles);
            var b = new Generated.EmployeeRoles(number, new[] { "tester", "developer" });
            var c = new Generated.EmployeeRoles(number, roles);

            var otherEmployeeNumber = new Generated.EmployeeRoles(number + 1, roles);
            var otherRolesLength = new Generated.EmployeeRoles(number, new[] { "tester" });
            var otherRolesValues = new Generated.EmployeeRoles(number, new[] { "tester", "analyst" });

            EqualityTesting.TestEqualsAndGetHashCode(a, b, c, otherEmployeeNumber, otherRolesLength, otherRolesValues);
            EqualityTesting.TestEqualityOperators(a, b, c, otherEmployeeNumber, otherRolesLength, otherRolesValues);
        }


        [Test]
        public void Instance_WhenSerializedWithBinaryFormatter_ShouldBeSerializable()
        {
            const int number = 1;
            var roles = new[] { "tester", "developer" };
            var actual = new Generated.EmployeeRoles(number, roles);
            
            var deserialized = SerializationTesting.SerializeDeserializeWithBinaryFormatter(actual);
            
            Assert.That(deserialized, Is.EqualTo(actual));
            Assert.That(actual.EmployeeNumber, Is.EqualTo(number));
            Assert.That(actual.Roles, Is.EquivalentTo(roles));
        }

    }
}