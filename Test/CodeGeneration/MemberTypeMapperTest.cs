using System;
using Diesel.CodeGeneration;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class MemberTypeMapperTest
    {
        [Test]
        public void MemberType_ForSystemType_ShouldBeSystemTypeMemberType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new SimpleType(typeof(int)), 
                new KnownType[0]);

            Assert.That(actual.FullName, Is.EqualTo(typeof(int).FullName));
            Assert.That(actual.IsValueType, Is.True);
        }

        [Test]
        public void MemberType_ForNamedType_ShouldBeNamedTypeMemberType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new TypeName("Foo.Bar.SomeType"),
                new KnownType[0]);
            Assert.That(actual.FullName, Is.EqualTo("Foo.Bar.SomeType"));
        }


        [Test]
        public void MemberType_ForArrayOfNamedType_ShouldBeArrayOfNamedTypeMemberType()
        {
            var elementType = new TypeName("Foo.Bar.SomeType");
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new ArrayType(elementType, new RankSpecifiers(new[] { new RankSpecifier(1) })),
                new KnownType[0]);
            Assert.That(actual.FullName, Is.EqualTo("Foo.Bar.SomeType[]"));
        }

        [Test]
        public void MemberType_ForNamedType_QualifiedKnownValueType_ShouldSetIsValueType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new TypeName("Foo.SomeType"),
                new[] { new KnownType("Foo.SomeType", true) });
            Assert.That(actual.IsValueType, Is.True);
        }

        [Test]
        public void MemberType_ForNamedType_UnqualifiedKnownValueType_ShouldSetIsValueType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new TypeName("SomeType"),
                new[] { new KnownType("MyNamespace.SomeType", true) });
            Assert.That(actual.IsValueType, Is.True);
        }


        [Test]
        public void MemberType_ForNamedType_KnownNonValueType_ShouldNotSetIsValueType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new TypeName("Foo.SomeType"),
                new[] { new KnownType("Foo.SomeType", false) });
            Assert.That(actual.IsValueType, Is.False);
        }

        [Test]
        public void MemberType_ForNamedType_UnknownNamedType_ShouldAssumeItIsNotAValueType()
        {
            var actual = MemberTypeMapper.MemberTypeFor(
                new NamespaceName("MyNamespace"),
                new TypeName("Foo.SomeType"),
                new KnownType[] {});
        }
    }
}