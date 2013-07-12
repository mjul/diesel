using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.Parsing.CSharp
{
    [TestFixture]
    public class ArrayTypeTest
    {
         [Test, Ignore("Move Name to Code-gen phase")]
         public void Constructor_UnidimensionalQualifiedType_ShouldSetType()
         {
             var typeName = new TypeName("System.Guid");
             var actual = new ArrayType(typeName, 
                 new RankSpecifiers(new[] { new RankSpecifier(1) }));
             Assert.That(actual.Type, Is.EqualTo(typeName));
         }

         [Test, Ignore("Move Name to Code-gen phase")]
         public void Constructor_UnidimensionalSimpleType_ShouldSetName()
         {
             var actual = new ArrayType(new SimpleType(typeof (int)), new RankSpecifiers(new[] {new RankSpecifier(1)}));
             // TODO: Assert.That(actual.Name, Is.EqualTo("System.Int32[]"));
         }

         [Test, Ignore("Move Name to Code-gen phase")]
         public void Constructor_Bidimensional_ShouldSetName()
         {
             var actual = new ArrayType(new TypeName("System.Guid"), 
                 new RankSpecifiers(new[] {new RankSpecifier(2)}));
             // TODO: Assert.That(actual.Name, Is.EqualTo("System.Guid[,]"));
         }

         [Test, Ignore("Move Name to Code-gen phase")]
         public void Constructor_HigherOrder_ShouldSetName()
         {
             var actual = new ArrayType(new TypeName("System.Guid"),
                                        new RankSpecifiers(new[]
                                            {
                                                new RankSpecifier(1),
                                                new RankSpecifier(2), 
                                                new RankSpecifier(3)
                                            }));
             // TODO: Assert.That(actual.Name, Is.EqualTo("System.Int32[][,][,,]"));
         }


        [Test]
        public void EqualsAndGetHashCode()
        {
            var a = new ArrayType(new SimpleType(typeof(int)), new RankSpecifiers(new[] { new RankSpecifier(1) }));
            var b = new ArrayType(new SimpleType(typeof(int)), new RankSpecifiers(new[] { new RankSpecifier(1) }));
            var c = new ArrayType(new SimpleType(typeof(int)), new RankSpecifiers(new[] { new RankSpecifier(1) }));

            var otherType = new ArrayType(new SimpleType(typeof(decimal)), new RankSpecifiers(new[] { new RankSpecifier(1) }));
            var otherDimensions = new ArrayType(new SimpleType(typeof(int)), new RankSpecifiers(new[] { new RankSpecifier(2) }));
            var otherRanks = new ArrayType(new SimpleType(typeof(int)), new RankSpecifiers(new[] { new RankSpecifier(1), new RankSpecifier(2) }));

            TestHelpers.EqualityTesting.TestEqualsAndGetHashCode(a,b,c, otherType, otherDimensions, otherRanks);
            TestHelpers.EqualityTesting.TestEqualityOperators(a,b,c, otherType, otherDimensions, otherRanks);
        }
    }
}