using System;
using Diesel.CodeGeneration;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class SystemTypeMapperTest
    {
        [Test]
        public void SystemTypeFor_ArrayTypeOfSimpleTypeUnidimensional_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new ArrayType(new SimpleType(typeof (double)),
                                                         new RankSpecifiers(new[] {new RankSpecifier(1)})));
            Assert.That(actual, Is.EqualTo(typeof (double[])));
        }

        [Test]
        public void SystemTypeFor_ArrayTypeOfSimpleTypeRankTwo_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new ArrayType(new SimpleType(typeof (double)),
                                                         new RankSpecifiers(new[] {new RankSpecifier(2)})));
            Assert.That(actual, Is.EqualTo(typeof (double[,])));
        }


        [Test]
        public void SystemTypeFor_ArrayTypeOfSimpleTypeRankOneTwo_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new ArrayType(new SimpleType(typeof(float)),
                                                         new RankSpecifiers(new[]
                                                             {
                                                                 new RankSpecifier(1),
                                                                 new RankSpecifier(2)
                                                             })));
            Assert.That(actual, Is.EqualTo(typeof(float[][,])));
        }

        
        [Test]
        public void SystemTypeFor_ArrayTypeOfSimpleTypeRankOneTwoThree_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new ArrayType(new SimpleType(typeof (float)),
                                                         new RankSpecifiers(new[]
                                                             {
                                                                 new RankSpecifier(1),
                                                                 new RankSpecifier(2), 
                                                                 new RankSpecifier(3)
                                                             })));
            Assert.That(actual, Is.EqualTo(typeof(float[][,][,,])));
        }


        [Test]
        public void SystemTypeFor_NullableTypeOfSimpleType_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new NullableType(new SimpleType(typeof (decimal))));
            Assert.That(actual, Is.EqualTo(typeof (decimal?)));
        }

        [Test]
        public void SystemTypeFor_SimpleType_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new SimpleType(typeof(float)));
            Assert.That(actual, Is.EqualTo(typeof(float)));
        }

        [Test]
        public void SystemTypeFor_ReferenceTypeString_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new StringReferenceType());
            Assert.That(actual, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void SystemTypeFor_NamedTypeFromSystemNamespaceDateTime_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new TypeNameTypeNode(new TypeName("DateTime")));
            Assert.That(actual, Is.EqualTo(typeof(DateTime)));
        }

        [Test]
        public void SystemTypeFor_NamedTypeFromSystemNamespaceGuid_ShouldMapToType()
        {
            var sut = new SystemTypeMapper();
            var actual = sut.SystemTypeFor(new TypeNameTypeNode(new TypeName("Guid")));
            Assert.That(actual, Is.EqualTo(typeof(Guid)));
        }

    }
}