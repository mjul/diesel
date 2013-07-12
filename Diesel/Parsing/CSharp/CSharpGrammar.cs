using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sprache;

namespace Diesel.Parsing.CSharp
{
    /// <summary>
    /// This represents some grammar productions from C#
    /// for use in the DSL.
    /// </summary>
    public class CSharpGrammar
    {
        public readonly string[] CSharpKeywords =
            {
                "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
                "char", "checked", "class", "const", "continue", "decimal", "default",
                "delegate", "do", "double", "else", "enum", "event", "explicit",
                "extern", "false", "finally", "fixed", "float", "for", "foreach",
                "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
                "lock", "long", "namespace", "new", "null", "object", "operator",
                "out", "override", "params", "private", "protected", "public",
                "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
                "stackalloc", "static", "string", "struct", "switch", "this", "throw",
                "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
                "ushort", "using", "virtual", "void", "volatile", "while"
            };


        /// <summary>
        /// Recognize valid .NET identifiers.
        /// Not the full C# syntax with @keywords.
        /// </summary>
        public Parser<Identifier> Identifier()
        {
            return (from first in TokenGrammar.Letter.Or(TokenGrammar.Underscore)
                    from rest in TokenGrammar.LetterOrDigit.Or(TokenGrammar.Underscore).Many().Text()
                    let identifier = first + rest
                    where !CSharpKeywords.Contains(identifier)
                    select new Identifier(identifier))
                .Named("Identifier");
        }

        public Parser<NamespaceName> NamespaceName()
        {
            return Identifier()
                .DelimitedBy(TokenGrammar.Period)
                .Select(identifiers => new NamespaceName(String.Join(".", identifiers.Select(i => i.Name))))
                .Named("NamespaceName");
        }

        /// <summary>
        /// This parses a useful subset of .NET Type names (qualified names, not nested types).
        /// </summary>
        public Parser<TypeName> TypeName()
        {
            return Identifier()
                .DelimitedBy(TokenGrammar.Period)
                .Select(identifiers => new TypeName(String.Join(".", identifiers.Select(i => i.Name))))
                .Named("TypeName");
        }

        private Parser<SimpleType> KnownSimpleType(string name, Type type)
        {
            return (from t in TokenGrammar.String(name)
                    select new SimpleType(type));
        }

        public Parser<SimpleType> SimpleType()
        {
            return KnownSimpleType("bool", typeof (bool))
                .Or(KnownSimpleType("decimal", typeof (decimal)))
                .Or(KnownSimpleType("sbyte", typeof (sbyte)))
                .Or(KnownSimpleType("byte", typeof (byte)))
                .Or(KnownSimpleType("short", typeof (short)))
                .Or(KnownSimpleType("ushort", typeof (ushort)))
                .Or(KnownSimpleType("int", typeof (int)))
                .Or(KnownSimpleType("uint", typeof (uint)))
                .Or(KnownSimpleType("long", typeof (long)))
                .Or(KnownSimpleType("ulong", typeof (ulong)))
                .Or(KnownSimpleType("char", typeof (char)))
                .Or(KnownSimpleType("float", typeof (float)))
                .Or(KnownSimpleType("double", typeof (double)));
        }

        /// <summary>
        /// Parse the C# array-type production.
        /// </summary>
        public Parser<ArrayType> ArrayType()
        {
            return from type in NonArrayType()
                   from rankSpecificers in RankSpecifiers()
                   select new ArrayType(type, rankSpecificers);
        }

        public Parser<RankSpecifiers> RankSpecifiers()
        {
            return from ranks in RankSpecifier().Token().AtLeastOnce()
                   select new RankSpecifiers(ranks);
        }

        public Parser<RankSpecifier> RankSpecifier()
        {
            return from open in TokenGrammar.LeftSquareBracket.Token()
                   from commas in TokenGrammar.Comma.Token().Many()
                   from close in TokenGrammar.RightSquareBracket.Token()
                   select new RankSpecifier(commas.Count() + 1);
        } 

        public Parser<StructType> StructType()
        {
            return StructType(true);
        }

        public Parser<StructType> StructType(bool includeNullableTypes)
        {
            var parser = SimpleType();
                // TODO: .Or<StructType>(TypeNameStructType());

            return includeNullableTypes
                       ? NullableType().Or<StructType>(parser)
                       : parser;
        }

        public Parser<ValueTypeNode> ValueTypeNode()
        {
            return ValueTypeNode(true);
        }

        private Parser<ValueTypeNode> ValueTypeNode(bool includeNullableTypes)
        {
            return StructType(includeNullableTypes)
                // TODO: .Or(EnumType)
                ;
        }


        /// <summary>
        /// A bit more restricted than the C# version we allow nullables only of explicit value types.
        /// C# would also allow generic types with the right type constraints to fit.
        /// </summary>
        private Parser<ValueTypeNode> NonNullableValueType()
        {
            return ValueTypeNode(false);
        }

        public Parser<ITypeNode> TypeNode(bool includeNullableTypes, bool includeArrayTypes)
        {
            return ReferenceType(includeArrayTypes)
                .Or(ValueTypeNode(includeNullableTypes))
                // TODO: .Or(TypeParameter)
                ;
        }

        public Parser<ITypeNode> TypeNode()
        {
            return TypeNode(true, true);
        }


        private Parser<ITypeNode> NonArrayType()
        {
            return TypeNode(true, false);
        }

        public Parser<NullableType> NullableType()
        {
            return (from underlyingType in NonNullableValueType()
                    from nullableIndicator in TokenGrammar.QuestionMark
                    where !(underlyingType is NullableType)
                    select new NullableType(underlyingType));
        }


        public Parser<TypeNameTypeNode> TypeNameTypeNode()
        {
            return (from t in TypeName()
                    select new TypeNameTypeNode(t));
        }

        public Parser<ITypeNode> ReferenceType()
        {
            return ReferenceType(true);
        }

        private Parser<ITypeNode> ReferenceType(bool includeArrayTypes)
        {
            var parser = 
                // interface-type not implemented
                // delegate-type not implemented 
                ClassType()
                .Or(TypeNameTypeNode());
            return includeArrayTypes
                       ? ArrayType().Or(parser)
                       : parser;
        }


        public Parser<ITypeNode> ClassType()
        {
            return StringType()
                .Or<ITypeNode>(TypeNameTypeNode()
                );
        }

        public Parser<StringReferenceType> StringType()
        {
            return (from keyword in TokenGrammar.String("string")
                    select new StringReferenceType());
        }

    }
}