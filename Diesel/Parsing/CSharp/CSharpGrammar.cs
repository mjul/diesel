using System;
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
        /// <summary>
        /// Recognize valid .NET identifiers (for now just a subset, letters and digits).
        /// </summary>
        public static Parser<Identifier> Identifier =
            (from first in TokenGrammar.Letter
             from rest in TokenGrammar.LetterOrDigit.Many().Text()
             select new Identifier(first + rest))
                .Named("Identifier");

        public static readonly Parser<NamespaceName> NamespaceName
            = Identifier.DelimitedBy(TokenGrammar.Period)
                        .Select(identifiers => new NamespaceName(String.Join(".", identifiers.Select(i => i.Name))))
                        .Named("NamespaceName");


        // TODO: rewrite to C# namespace + identifier production

        /// <summary>
        /// This parses a useful subset of .NET Type names (qualified names, not nested types).
        /// </summary>
        public static Parser<TypeName> TypeName
            = Identifier
                .DelimitedBy(TokenGrammar.Period)
                .Select(identifiers => new TypeName(String.Join(".", identifiers.Select(i => i.Name))))
                .Named("TypeName");


        public static Parser<Type> NullableOf(Parser<Type> underlying)
        {
            return (from underlyingType in underlying
                    from nullableIndicator in TokenGrammar.QuestionMark
                    select Type.GetType(String.Format("System.Nullable`1[{0}]", underlyingType.FullName), true));
        }

        // Just uni-dimensional arrays for now, not full C# syntax rank specifiers
        public static Parser<ArrayType> ArrayType(Parser<TypeName> nonArrayTypeParser)
        {
            return (from type in nonArrayTypeParser
                    from rankSpecificer in
                        (from open in TokenGrammar.LeftSquareBracket.Token()
                         from close in TokenGrammar.RightSquareBracket.Token()
                         select "[]")
                    select new ArrayType(type, new[] {1}));
        }
    }
}