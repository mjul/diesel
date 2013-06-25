using Diesel.CodeGeneration;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class MemberTypeMapperTest
    {
        [Test]
        public void MemberType_ForSystemType_ShouldBeSystemTypeMemberType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(new SimpleType(typeof (int)));

            Assert.That(actual.FullName, Is.EqualTo(typeof(int).FullName));
            Assert.That(actual.IsValueType, Is.True);
        }

        [Test]
        public void MemberType_ForNamedType_ShouldBeNamedTypeMemberType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(new TypeNameTypeNode(new TypeName("Foo.Bar.SomeType")));
            Assert.That(actual.FullName, Is.EqualTo("Foo.Bar.SomeType"));
        }


        [Test]
        public void MemberType_ForArrayOfNamedType_ShouldBeArrayOfNamedTypeMemberType()
        {
            var elementType = new TypeNameTypeNode(new TypeName("Foo.Bar.SomeType"));
            var actual = MemberTypeMapper.MemberTypeFor(
                new ArrayType(elementType, new RankSpecifiers(new[] {new RankSpecifier(1)})));
            Assert.That(actual.FullName, Is.EqualTo("Foo.Bar.SomeType[]"));
        }

        [Test]
        public void MemberType_ForNamedType_ShouldSetIsValueType()
        {
            Assert.Inconclusive();
        }

    }
}