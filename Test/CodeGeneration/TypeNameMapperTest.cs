using System;
using Diesel.CodeGeneration;
using Diesel.Parsing.CSharp;
using NUnit.Framework;

namespace Test.Diesel.CodeGeneration
{
    [TestFixture]
    public class TypeNameMapperTest
    {
        [Test]
        public void TypeNameForArray_ArrayTypeOfSimpleTypeUnidimensional_ShouldMapToType()
        {
            var actual = TypeNameMapper.TypeNameForArray(typeof (double).FullName,
                                                               new RankSpecifiers(new[] {new RankSpecifier(1)}));
            var correspondingType = Type.GetType(actual);
            Assert.That(correspondingType, Is.EqualTo(typeof(double[])));
        }

        [Test]
        public void TypeNameForArray_ArrayTypeOfSimpleTypeRankTwo_ShouldMapToType()
        {
            var actual = TypeNameMapper.TypeNameForArray(typeof (double).FullName,
                                                               new RankSpecifiers(new[] {new RankSpecifier(2)}));
            var correspondingType = Type.GetType(actual);
            Assert.That(correspondingType, Is.EqualTo(typeof(double[,])));
        }


        [Test]
        public void TypeNameForArray_ArrayTypeOfSimpleTypeRankOneTwo_ShouldMapToType()
        {
            var actual = TypeNameMapper.TypeNameForArray(typeof (float).FullName,
                                                               new RankSpecifiers(new[]
                                                                   {
                                                                       new RankSpecifier(1),
                                                                       new RankSpecifier(2)
                                                                   }));
            var correspondingType = Type.GetType(actual);
            Assert.That(correspondingType, Is.EqualTo(typeof(float[][,])));
        }


        [Test]
        public void TypeNameForArray_ArrayTypeOfSimpleTypeRankOneTwoThree_ShouldMapToType()
        {
            var actual = TypeNameMapper.TypeNameForArray(typeof(float).FullName,
                                                                      new RankSpecifiers(new[]
                                                                          {
                                                                              new RankSpecifier(1),
                                                                              new RankSpecifier(2),
                                                                              new RankSpecifier(3)
                                                                          }));
            var correspondingType = Type.GetType(actual);
            Assert.That(correspondingType, Is.EqualTo(typeof (float[][,][,,])));
        }

        [Test]
        public void TypeNameForNullable_NamedType_ShouldMapToType()
        {
            var actual = TypeNameMapper.TypeNameForNullableType(typeof(int).FullName);
            var actualCorrespondingType = Type.GetType(actual);
            var expectedCorrespondingType = typeof (Nullable<int>);
            Assert.That(actualCorrespondingType, Is.EqualTo(expectedCorrespondingType));
        }
    }
}