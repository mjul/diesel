﻿using System;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using NUnit.Framework;
using Sprache;

namespace Test.Diesel.Parsing.CSharp
{
    [TestFixture]
    public class CSharpGrammarTest
    {

        private static readonly CSharpGrammar SystemUnderTest = new CSharpGrammar();

        [Test]
        public void Identifier_ValidSingleLetter_ShouldParse()
        {
            var actual = SystemUnderTest.Identifier().Parse("x");
            Assert.That(actual.Name, Is.EqualTo("x"));
        }

        [Test]
        public void Identifier_ValidString_ShouldParse()
        {
            var actual = SystemUnderTest.Identifier().Parse("name");
            Assert.That(actual.Name, Is.EqualTo("name"));
        }

        [Test]
        public void Identifier_StartsWithUnderscore_ShouldParse()
        {
            var actual = SystemUnderTest.Identifier().Parse("_name");
            Assert.That(actual.Name, Is.EqualTo("_name"));
        }

        [Test]
        public void Identifier_IncludesUnderscore_ShouldParse()
        {
            var actual = SystemUnderTest.Identifier().Parse("first_name");
            Assert.That(actual.Name, Is.EqualTo("first_name"));
        }


        [Test]
        public void Identifier_Blank_ShouldNotParse()
        {
            var actual = SystemUnderTest.Identifier().TryParse("");
            Assert.That(actual.WasSuccessful, Is.False);
        }

        [Test]
        public void Identifier_ValidStringAndNumber_ShouldParse()
        {
            var actual = SystemUnderTest.Identifier().Parse("name1");
            Assert.That(actual.Name, Is.EqualTo("name1"));
        }


        [Test]
        public void NamespaceIdentifier_BlankName_ShouldNotParse()
        {
            var actual = SystemUnderTest.NamespaceName().TryParse("");
            Assert.False(actual.WasSuccessful);
        }

        [Test]
        public void NamespaceIdentifier_SinglePartName_ShouldParse()
        {
            var actual = SystemUnderTest.NamespaceName().Parse("System");
            Assert.That(actual.Name, Is.EqualTo("System"));
        }

        [Test]
        public void NamespaceIdentifier_MultiPartName_ShouldParse()
        {
            var actual = SystemUnderTest.NamespaceName().Parse("System.Runtime.Serialization");
            Assert.That(actual.Name, Is.EqualTo("System.Runtime.Serialization"));
        }

        [Test]
        public void TypeName_ValidUnqualifiedName_ShouldParse()
        {
            var actual = SystemUnderTest.TypeName().Parse("Guid");
            Assert.That(actual.Name, Is.EqualTo("Guid"));
        }

        [Test]
        public void TypeName_ValidQualifiedName_ShouldParse()
        {
            var actual = SystemUnderTest.TypeName().Parse("System.Guid");
            Assert.That(actual.Name, Is.EqualTo("System.Guid"));
        }

        [Test]
        public  void ArrayType_UnidimensionalQualifiedName_ShouldParse()
        {
            var actual = SystemUnderTest.ArrayType().Parse("System.Int32[]");
            Assert.That(actual.Type, Is.EqualTo(new TypeNameTypeNode(new TypeName("System.Int32"))));
            Assert.That(actual.RankSpecifiers, Is.EqualTo(new RankSpecifiers(new[] { new RankSpecifier(1)})));
        }

        [Test]
        public void ArrayType_UnidimensionalMultiPartQualifiedName_ShouldParse()
        {
            var actual = SystemUnderTest.ArrayType().Parse("Test.Namespaces.Number[]");
            Assert.That(actual.Type, Is.EqualTo(new TypeNameTypeNode(new TypeName("Test.Namespaces.Number"))));
            Assert.That(actual.RankSpecifiers, Is.EqualTo(new RankSpecifiers(new[] { new RankSpecifier(1)})));
        }

        [Test]
        public void ArrayType_UnidimensionalUnqualifiedName_ShouldParse()
        {
            var actual = SystemUnderTest.ArrayType().Parse("Guid[]");
            Assert.That(actual.Type, Is.EqualTo(new TypeNameTypeNode(new TypeName("Guid"))));
            Assert.That(actual.RankSpecifiers, Is.EqualTo(new RankSpecifiers(new[] { new RankSpecifier(1) })));
        }

        [Test]
        public void SimpleType_Bool_ShouldParse()
        {
            AssertSimpleTypeParsesAs("bool", typeof (bool));
        }

        [Test]
        public void SimpleType_Decimal_ShouldParse()
        {
            AssertSimpleTypeParsesAs("decimal", typeof(decimal));
        }

        [Test]
        public void SimpleType_SByte_ShouldParse()
        {
            AssertSimpleTypeParsesAs("sbyte", typeof(sbyte));
        }

        [Test]
        public void SimpleType_Byte_ShouldParse()
        {
            AssertSimpleTypeParsesAs("byte", typeof(byte));
        }

        [Test]
        public void SimpleType_Short_ShouldParse()
        {
            AssertSimpleTypeParsesAs("short", typeof(short));
        }

        [Test]
        public void SimpleType_UShort_ShouldParse()
        {
            AssertSimpleTypeParsesAs("ushort", typeof(ushort));
        }

        [Test]
        public void SimpleType_Int_ShouldParse()
        {
            AssertSimpleTypeParsesAs("int", typeof(int));
        }

        [Test]
        public void SimpleType_UInt_ShouldParse()
        {
            AssertSimpleTypeParsesAs("uint", typeof(uint));
        }

        [Test]
        public void SimpleType_Long_ShouldParse()
        {
            AssertSimpleTypeParsesAs("long", typeof(long));
        }

        [Test]
        public void SimpleType_ULong_ShouldParse()
        {
            AssertSimpleTypeParsesAs("ulong", typeof(ulong));
        }

        [Test]
        public void SimpleType_Char_ShouldParse()
        {
            AssertSimpleTypeParsesAs("char", typeof(char));
        }

        [Test]
        public void SimpleType_Float_ShouldParse()
        {
            AssertSimpleTypeParsesAs("float", typeof(float));
        }

        [Test]
        public void SimpleType_Double_ShouldParse()
        {
            AssertSimpleTypeParsesAs("double", typeof(double));
        }

        private void AssertSimpleTypeParsesAs(string input, Type expectedType)
        {
            var actual = SystemUnderTest.SimpleType().Parse(input);
            Assert.That(actual.Type, Is.EqualTo(expectedType));
        }


        [Test]
        public void NullableOf_ForSimpleType_ShouldParse()
        {
            AssertNullableOfSimpleTypeParsesAs<Int32?>("Int32?");
            AssertNullableOfSimpleTypeParsesAs<Int32?>("int?");
            AssertNullableOfSimpleTypeParsesAs<Int64?>("Int64?");
            AssertNullableOfSimpleTypeParsesAs<Int64?>("long?");
            AssertNullableOfSimpleTypeParsesAs<Decimal?>("decimal?");
            AssertNullableOfSimpleTypeParsesAs<Double?>("double?");
        }

        private static void AssertNullableOfSimpleTypeParsesAs<TExpected>(string input)
        {
            var actual = SystemUnderTest.NullableOf(Grammar.SimpleType).Parse(input);
            Assert.That(actual, Is.EqualTo(typeof(TExpected)));
        }

        [Test]
        public void NullableType_Int_ShouldParse()
        {
            AssertNullableTypeParsesAs("int?", new SimpleType(typeof(int)));
        }

        private void AssertNullableTypeParsesAs(string input, TypeNode expectedUnderlying)
        {
            var actual = SystemUnderTest.NullableType().Parse(input);
            Assert.That(actual.Underlying, Is.EqualTo(expectedUnderlying));
        }

        [Test]
        public void StructType_TypeName_ShouldParse()
        {
            Assert.Inconclusive();            
        }

        [Test]
        public void StructType_SimpleType_ShouldParse()
        {
            var actual = SystemUnderTest.StructType().Parse("int");
            Assert.That(actual, Is.EqualTo(new SimpleType(typeof(int))));
        }

        [Test]
        public void StructType_NullableType_ShouldParse()
        {
            var actual = SystemUnderTest.StructType().Parse("int?");
            Assert.That(actual, Is.EqualTo(new NullableType(new SimpleType(typeof (int)))));
        }


        [Test]
        public void StringType_String_ShouldParse()
        {
            var actual = SystemUnderTest.StringType().Parse("string");
            Assert.That(actual, Is.EqualTo(new StringReferenceType()));
        }


        [Test]
        public void ClassType_TypeName_ShouldParse()
        {
            var actual = SystemUnderTest.ClassType().Parse("Test.Diesel.Parsing.CSharp.CSharpGrammarTest");
            Assert.That(actual, Is.EqualTo(new TypeNameTypeNode(new TypeName("Test.Diesel.Parsing.CSharp.CSharpGrammarTest"))));
        }

        [Test, Ignore("Not implemented")]
        public void ClassType_Object_ShouldParse()
        {
        }

        [Test, Ignore("Not implemented")]
        public void ClassType_Dynamic_ShouldParse()
        {
        }

        [Test]
        public void ClassType_String_ShouldParse()
        {
            var actual = SystemUnderTest.ClassType().Parse("string");
            Assert.That(actual, Is.EqualTo(new StringReferenceType()));
        }


        [Test]
        public void ReferenceType_ClassTypeString_ShouldParse()
        {
            var actual = SystemUnderTest.ReferenceType().Parse("string");
            Assert.That(actual, Is.EqualTo(new StringReferenceType()));
        }

        [Test]
        public void ReferenceType_ClassTypeTypeName_ShouldParse()
        {
            var actual = SystemUnderTest.ReferenceType().Parse("Diesel.Test.Parsing.CSharpGrammarTest");
            Assert.That(actual, Is.EqualTo(new TypeNameTypeNode(new TypeName("Diesel.Test.Parsing.CSharpGrammarTest"))));
        }

        [Test]
        public void ReferenceType_ArrayType_ShouldParse()
        {
            var actual = SystemUnderTest.ReferenceType().Parse("int[]");
            Assert.That(actual,
                        Is.EqualTo(new ArrayType(new SimpleType(typeof (int)),
                                                 new RankSpecifiers(new[] {new RankSpecifier(1)}))));
        }



        [Test]
        public void TypeNode_ValueType_ShouldParse()
        {
            var actual = SystemUnderTest.TypeNode().Parse("int");
            Assert.That(actual, Is.EqualTo(new SimpleType(typeof(int))));
        }

        [Test]
        public void TypeNode_ReferenceType_ShouldParse()
        {
            var actual = SystemUnderTest.TypeNode().Parse("string");
            Assert.That(actual, Is.EqualTo(new StringReferenceType()));
        }

        [Test]
        public void TypeNode_TypeName_ShouldParse()
        {
            var actual = SystemUnderTest.TypeNode().Parse("Diesel.Test.Parsing.CSharpGrammarTest");
            Assert.That(actual, Is.EqualTo(new TypeNameTypeNode(new TypeName("Diesel.Test.Parsing.CSharpGrammarTest"))));
        }
    }

}