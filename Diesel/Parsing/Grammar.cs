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
        private static readonly Parser<Char> QuestionMark = Char('?', "Question Mark");

        private static Parser<Symbol> Symbol(string value)
        {
            return Parse.String(value).Text()
                .Select(s => new Symbol(s))
                .Named(String.Format("Symbol {0}", value));
        }

        public static Parser<string> Identifier =
            (from first in Letter.Once().Text()
             from rest in LetterOrDigit.Many().Text()
             select first + rest)
                .Named("Identifier");

        private static Parser<Type> TypeName(string name, Type type)
        {
            return (from id in Identifier
                    where id == name
                    select type);
        }

        // In constrast to the C# grammar, we count string a simple type since it has value semantics
        public static Parser<Type> SimpleType =
            TypeName("Int32", typeof (Int32))
                .Or(TypeName("String", typeof (String)))
                .Or(TypeName("Decimal", typeof (Decimal)))
                .Or(TypeName("Single", typeof (Single)))
                .Or(TypeName("Double", typeof (Double)))
                .Or(TypeName("Int64", typeof (Int64)))
                .Or(TypeName("int", typeof (Int32)))
                .Or(TypeName("string", typeof (String)))
                .Or(TypeName("decimal", typeof (Decimal)))
                .Or(TypeName("float", typeof (Single)))
                .Or(TypeName("double", typeof (Double)))
                .Or(TypeName("long", typeof (Int64)))
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
            TypeName("Guid", typeof (Guid))
                .Or(TypeName("DateTime", typeof (DateTime)))
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

        private static readonly Parser<string> NamespaceIdentifier
            = Identifier.Named("Namespace part")
                        .DelimitedBy(Parse.Char('.'))
                        .Select(parts => String.Join(".", parts))
                        .Named("NamespaceIdentifier");


        public static readonly Parser<CommandDeclaration> CommandDeclaration
            = (from declaration in Symbol("defcommand").Token()
               from name in Identifier.Token()
               from optionalPropertyDeclarations in PropertyDeclarations.Optional()
               select new CommandDeclaration(name, optionalPropertyDeclarations.GetOrElse(new PropertyDeclaration[] {})))
                .Contained(LeftParen, RightParen)
                .Named("CommandDeclaration");

        public static readonly Parser<ApplicationServiceDeclaration> ApplicationServiceDeclaration
            = (from declaration in Symbol("defapplicationservice").Token()
               from name in Identifier.Token()
               from commandDeclarations in CommandDeclaration.Token().AtLeastOnce()
               select new ApplicationServiceDeclaration(name, commandDeclarations))
                .Contained(LeftParen, RightParen);

        private static readonly Parser<TypeDeclaration> TypeDeclaration
            = ValueTypeDeclaration
                .Or<TypeDeclaration>(CommandDeclaration)
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

        public static readonly Parser<AbstractSyntaxTree> AbstractSyntaxTree
            = (from namespaces in Namespace.Token().Many().Token()
               select new AbstractSyntaxTree(namespaces));

    }
}