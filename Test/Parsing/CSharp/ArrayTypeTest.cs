using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.Parsing.CSharp
{
    [TestFixture]
    public class ArrayTypeTest
    {
         [Test]
         public void Constructor_Unidimensional_ShouldSetName()
         {
             var actual = new ArrayType(new TypeName("System.Int32"), new [] {1});
             Assert.That(actual.Name, Is.EqualTo("System.Int32[]"));
         }

         [Test]
         public void Constructor_Bidimensional_ShouldSetName()
         {
             var actual = new ArrayType(new TypeName("System.Int32"), new[] { 2 });
             Assert.That(actual.Name, Is.EqualTo("System.Int32[,]"));
         }

         [Test]
         public void Constructor_HigherOrder_ShouldSetName()
         {
             var actual = new ArrayType(new TypeName("System.Int32"), new[] { 1, 2, 3 });
             Assert.That(actual.Name, Is.EqualTo("System.Int32[][,][,,]"));
         }

    }
}