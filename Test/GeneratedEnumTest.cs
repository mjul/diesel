using System.Linq;
using NUnit.Framework;
using Test.Diesel.Generated;
using Test.Diesel.TestHelpers;

namespace Test.Diesel
{
    [TestFixture]
    public class GeneratedEnumTest
    {
         [Test]
         public void Instance_WhenSerializedWithDataContractFormatter_ShouldBeSerializable()
         {
             foreach (var instance in System.Enum.GetValues(typeof(Gender)).Cast<Gender>())
             {
                 var deserialized = SerializationTesting.SerializeDeserializeWithDataContractSerializer(instance);
                 Assert.That(deserialized, Is.EqualTo(instance));
             }
         }

         [Test]
         public void Instance_Values_ShouldBeDistinct()
         {
             Assert.That(Gender.Female != Gender.Male);
         }
    }
}