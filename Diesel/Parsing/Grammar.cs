using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Diesel.Parsing
{
    public static class Grammar
    {
        
        private static Parser<Char> Char(char c, string name) 
        {
            return Parse.Char(c).Named(name);
        }

        private static readonly Parser<Char> LeftParen = Char('(', "LeftParen");
        private static readonly Parser<Char> RightParen = Char(')', "RightParen");
        private static readonly Parser<Char> Letter = Parse.Letter;
        private static readonly Parser<Char> LetterOrDigit = Parse.LetterOrDigit;
        private static readonly Parser<Char> Comma = Char(',', "Comma");
        private static readonly Parser<Char> Period = Char('.', "Period");
        private static readonly Parser<Char> QuestionMark = Char('?', "Question Mark");
        private static readonly Parser<Char> Colon = Char(':', "Colon");

        private static readonly Parser<Char> LeftCurlyBrace = Char('{', "LeftCurlyBrace");
        private static readonly Parser<Char> RightCurlyBrace = Char('}', "RightCurlyBrace");
        private static readonly Parser<Char> LeftSquareBracket = Char('[', "LeftSquareBracket");
        private static readonly Parser<Char> RightSquareBracket = Char(']', "RightSquareBracket");

        /// <summary>
        /// A symbol is a naked string.
        /// </summary>
        private static Parser<Symbol> Symbol(string value)
        {
            return Parse.String(value).Text()
                .Select(s => new Symbol(s))
                .Named(String.Format("Symbol {0}", value));
        }

        /// <summary>
        /// Keywords are identifiers prefixed with colon (':').
        /// The colon is not part of their name.
        /// </summary>
        public static Parser<Keyword> Keyword()
        {
            return
                (from colon in Colon
                 from initial in Letter.Once().Text()
                 from rest in LetterOrDigit.Many().Text()
                 select new Keyword(String.Format("{0}{1}", initial, rest)))
                    .Named("Keyword");
        }

        /// <summary>
        /// Recognize a specific keyword.
        /// </summary>
        public static Parser<Keyword> Keyword(string name)
        {
            return Keyword().Where(k => k.Name == name);
        }

        /// <summary>
        /// Recognize valid .NET identifiers (for now just a subset, letters and digits).
        /// </summary>
        public static Parser<string> Identifier =
            (from first in Letter
             from rest in LetterOrDigit.Many().Text()
             select first + rest)
                .Named("Identifier");

        /// <summary>
        /// Recognize names and their corresponding known type, e.g. "Int32" and typeof(Int32).
        /// </summary>
        private static Parser<Type> KnownTypeName(string name, Type type)
        {
            return (from id in Identifier
                    where id == name
                    select type);
        }

        public static readonly Parser<NamespaceIdentifier> NamespaceIdentifier
            = Identifier.DelimitedBy(Period)
                .Select(names => new NamespaceIdentifier(String.Join(".", names)))
                .Named("NamespaceIdentifier");

        /// <summary>
        /// This parses a useful subset of .NET Type names (qualified names, not nested types).
        /// </summary>
        public static Parser<TypeName> TypeName
            = Identifier
                .DelimitedBy(Period)
                .Select(names => new TypeName(String.Join(".", names)))
                .Named("TypeName");


        // In constrast to the C# grammar, we count string a simple type since it has value semantics
        public static Parser<Type> SimpleType =
            KnownTypeName("Int32", typeof (Int32))
                .Or(KnownTypeName("String", typeof (String)))
                .Or(KnownTypeName("Decimal", typeof (Decimal)))
                .Or(KnownTypeName("Single", typeof (Single)))
                .Or(KnownTypeName("Double", typeof (Double)))
                .Or(KnownTypeName("Int64", typeof (Int64)))
                .Or(KnownTypeName("int", typeof (Int32)))
                .Or(KnownTypeName("string", typeof (String)))
                .Or(KnownTypeName("decimal", typeof (Decimal)))
                .Or(KnownTypeName("float", typeof (Single)))
                .Or(KnownTypeName("double", typeof (Double)))
                .Or(KnownTypeName("long", typeof (Int64)))
                .Named("SimpleType");

        
        public static Parser<Type> NullableOf(Parser<Type> underlying)
        {
            return (from underlyingType in underlying
                    from nullableIndicator in QuestionMark
                    select Type.GetType(String.Format("System.Nullable`1[{0}]", underlyingType.FullName), true));
        }

        public static Parser<Type> SimpleTypeAllowNullable =
            NullableOf(SimpleType)
            .Or(SimpleType);

        private static readonly Parser<Type> SystemValueType =
            KnownTypeName("Guid", typeof (Guid))
                .Or(KnownTypeName("DateTime", typeof (DateTime)))
                .Or(SimpleType);

        public static Parser<Type> SystemValueTypeAllowNullable
            = NullableOf(SystemValueType)
                .Or(SystemValueType);

        public static Parser<PropertyDeclaration> PropertyDeclaration
            = (from type in SystemValueTypeAllowNullable.Named("Property type").Token()
               from name in Identifier.Named("Property name").Token()
               select new PropertyDeclaration(name, type))
                .Named("PropertyDeclartion");

        private static Parser<IEnumerable<TElement>> SequenceOf<TElement,TDelimiter>(Parser<TElement> elementParser, Parser<TDelimiter> delimiterParser)
        {
            return (from element in elementParser
                        .Many()
                        .DelimitedBy(delimiterParser)
                        .Token()
                        .Contained(LeftParen, RightParen)
                    select element.SelectMany(x => x));
        }

        private static readonly Parser<IEnumerable<PropertyDeclaration>> PropertyDeclarations
            = SequenceOf(PropertyDeclaration, Comma)
                .Named("PropertyDeclarations");


        private static readonly Parser<ValueTypeDeclaration> SimpleValueTypeDeclaration
            = (from declaration in Symbol("defvaluetype").Token()
               from name in Identifier.Token()
               from optionalTypeDeclaration in SystemValueTypeAllowNullable.Optional().Token()
               let property = new[] {new PropertyDeclaration(null, optionalTypeDeclaration.GetOrDefault())}
               select new ValueTypeDeclaration(name, property))
                .Contained(LeftParen, RightParen)
                .Named("SimpleValueTypeDeclaration");

        private static readonly Parser<ValueTypeDeclaration> ValueTypeDeclarationWithPropertyList
            = (from declaration in Symbol("defvaluetype").Token()
               from name in Identifier.Token()
               from properties in PropertyDeclarations.Token()
               select new ValueTypeDeclaration(name, properties))
                .Contained(LeftParen, RightParen)
                .Named("ValueTypeDeclarationWithPropertyList");

        public static readonly Parser<ValueTypeDeclaration> ValueTypeDeclaration
            = SimpleValueTypeDeclaration
                .Or(ValueTypeDeclarationWithPropertyList)
                .Named("ValueTypeDeclaration");


        public static readonly Parser<CommandDeclaration> CommandDeclaration
            = (from declaration in Symbol("defcommand").Token()
               from name in Identifier.Token()
               from optionalPropertyDeclarations in PropertyDeclarations.Optional()
               select new CommandDeclaration(name, optionalPropertyDeclarations.GetOrElse(new PropertyDeclaration[] {})))
                .Contained(LeftParen, RightParen)
                .Named("CommandDeclaration");


        public static readonly Parser<DomainEventDeclaration> DomainEventDeclaration
            = (from declaration in Symbol("defdomainevent").Token()
               from name in Identifier.Token()
               from propertyDeclarations in PropertyDeclarations.Token()
               select new DomainEventDeclaration(name, propertyDeclarations))
                .Contained(LeftParen, RightParen)
                .Named("DomainEventDeclaration");


        public static readonly Parser<ApplicationServiceDeclaration> ApplicationServiceDeclaration
            = (from declaration in Symbol("defapplicationservice").Token()
               from name in Identifier.Token()
               from commandDeclarations in CommandDeclaration.Token().AtLeastOnce()
               select new ApplicationServiceDeclaration(name, commandDeclarations))
                .Contained(LeftParen, RightParen);

        public static readonly Parser<TypeDeclaration> TypeDeclaration
            = ValueTypeDeclaration
                .Or<TypeDeclaration>(CommandDeclaration)
                .Or<TypeDeclaration>(DomainEventDeclaration)
                .Or<TypeDeclaration>(ApplicationServiceDeclaration);

        public static readonly Parser<Namespace> Namespace
            = (from declaration in Symbol("namespace").Token()
               from name in NamespaceIdentifier.Named("namespace name").Token()
               from typeDeclarations in TypeDeclaration
                   .Token()
                   .AtLeastOnce()
                   .Optional()
               let declarationList = typeDeclarations.GetOrElse(new List<TypeDeclaration>())
               select new Namespace(name, declarationList))
                .Contained(LeftParen, RightParen);

        // This can be expanded to generalized nested key-value maps later
        public static readonly Parser<ConventionsDeclaration> ConventionsDeclaration
            = (from declaration in Symbol("defconventions").Token()
               from name in Keyword("domainevents").Token()
               from lcurly in LeftCurlyBrace.Token()
               from inherits in Keyword("inherits").Token()
               from lbracket in LeftSquareBracket.Token()
               from baseTypes in TypeName.Token().Many()
               from rbracket in RightSquareBracket.Token()
               from rcurly in RightCurlyBrace.Token()
               let domainEventConventions = new DomainEventConventions(baseTypes)
               select new ConventionsDeclaration(domainEventConventions))
                .Contained(LeftParen, RightParen)
                .Named("ConventionsDeclaration");

        public static readonly Parser<AbstractSyntaxTree> AbstractSyntaxTree
            = (from namespaces in Namespace.Token().Many().Token()
               select new AbstractSyntaxTree(namespaces));

        /// <summary>
        /// Top-level production for parsing everything in the source string.
        /// </summary>
        public static readonly Parser<AbstractSyntaxTree> Everything
            = AbstractSyntaxTree.Token().End();

    }
}