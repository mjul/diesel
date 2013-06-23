using System;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing.CSharp;
using Sprache;

namespace Diesel.Parsing
{
    public static class Grammar
    {
        private static readonly CSharpGrammar CSharpGrammar = new CSharpGrammar();

        public static readonly Parser<string> Comment =
            (from semicolon in TokenGrammar.Semicolon
             from comment in TokenGrammar.RestOfLine
             select comment);

        public static readonly Parser<string> CommentOrWhiteSpace =
            Comment.Token().Optional().Token().Select(c => c.GetOrDefault());

        public static Parser<T> TokenAllowingComments<T>(this Parser<T> innerParser)
        {
            return (from leadingTrivia in CommentOrWhiteSpace.Many().Optional()
                    from inner in innerParser
                    from followingTrivia in CommentOrWhiteSpace.Many().Optional()
                    select inner);
        }

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
                (from colon in TokenGrammar.Colon
                 from initial in TokenGrammar.Letter.Once().Text()
                 from rest in TokenGrammar.LetterOrDigit.Many().Text()
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

        public static Parser<PropertyDeclaration> PropertyDeclaration
            = (from type in CSharpGrammar.TypeNode().Token().Named("Property type")
               from name in CSharpGrammar.Identifier().Token().Named("Property name")
               select new PropertyDeclaration(name.Name, type))
                .Named("PropertyDeclartion");



        private static Parser<IEnumerable<TElement>> DelimitedSequenceParser<TElement, TDelimiter, 
            TSequenceOpen, TSequenceClose>(
            Parser<TElement> elementParser, 
            Parser<TDelimiter> delimiterParser,
            Parser<TSequenceOpen> sequenceOpenParser, 
            Parser<TSequenceClose> sequenceCloseParser
            )
        {
            return (from element in elementParser
                        .Many()
                        .DelimitedBy(delimiterParser)
                        .Token()
                        .Contained(sequenceOpenParser, sequenceCloseParser)
                    select element.SelectMany(x => x));
        }

        private static Parser<IEnumerable<TElement>> SequenceOf<TElement,TDelimiter>(Parser<TElement> elementParser, Parser<TDelimiter> delimiterParser)
        {
            return DelimitedSequenceParser(elementParser, delimiterParser, 
                TokenGrammar.LeftParen, TokenGrammar.RightParen);
        }

        private static Parser<IEnumerable<TElement>> VectorOf<TElement>(Parser<TElement> elementParser)
        {
            return DelimitedSequenceParser(elementParser, 
                TokenGrammar.Comma.Optional().Token(),
                TokenGrammar.LeftSquareBracket, TokenGrammar.RightSquareBracket);
        }


        private static readonly Parser<IEnumerable<PropertyDeclaration>> PropertyDeclarations
            = SequenceOf(PropertyDeclaration, TokenGrammar.Comma)
                .Named("PropertyDeclarations");


        private static Parser<TResult> DefIdentifierPropertyDeclarations<TResult>(
            string symbol, Func<Identifier, IEnumerable<PropertyDeclaration>, TResult> selectFunction)
        {
            return (from declaration in Symbol(symbol).Token()
                    from name in CSharpGrammar.Identifier().Token()
                    from propertyDeclarations in PropertyDeclarations
                    select selectFunction(name, propertyDeclarations))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named(symbol);
        }


        private static readonly Parser<ValueTypeDeclaration> SimpleValueTypeDeclaration
            = (from declaration in Symbol("defvaluetype").Token()
               from name in CSharpGrammar.Identifier().Token()
               from optionalTypeDeclaration in CSharpGrammar.TypeNode().Optional().Token()
               let property = new[] {new PropertyDeclaration(null, optionalTypeDeclaration.GetOrDefault())}
               select new ValueTypeDeclaration(name.Name, property))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("SimpleValueTypeDeclaration");


        private static readonly Parser<ValueTypeDeclaration> ValueTypeDeclarationWithPropertyList
            = DefIdentifierPropertyDeclarations("defvaluetype",
                                                (identifier, properties) =>
                                                new ValueTypeDeclaration(identifier.Name, properties));

        public static readonly Parser<ValueTypeDeclaration> ValueTypeDeclaration
            = SimpleValueTypeDeclaration
                .Or(ValueTypeDeclarationWithPropertyList)
                .Named("ValueTypeDeclaration");

        
        public static readonly Parser<CommandDeclaration> CommandDeclaration
            = DefIdentifierPropertyDeclarations("defcommand",
                                                (identifier, properties) =>
                                                new CommandDeclaration(identifier.Name, properties));

        
        public static readonly Parser<DomainEventDeclaration> DomainEventDeclaration
            = DefIdentifierPropertyDeclarations("defdomainevent",
                                                (identifier, properties) =>
                                                new DomainEventDeclaration(identifier.Name, properties));


        public static readonly Parser<DtoDeclaration> DtoDeclaration =
            DefIdentifierPropertyDeclarations("defdto",
                                              (identifier, properties) =>
                                              new DtoDeclaration(identifier.Name, properties));


        public static readonly Parser<EnumDeclaration> EnumDeclaration =
            (from declaration in Symbol("defenum").Token()
             from name in CSharpGrammar.Identifier().Token()
             from values in VectorOf(CSharpGrammar.Identifier().Token())
             select new EnumDeclaration(name.Name, values.Select(v => v.Name)))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("EnumDeclaration");



        public static readonly Parser<ApplicationServiceDeclaration> ApplicationServiceDeclaration
            = (from declaration in Symbol("defapplicationservice").Token()
               from name in CSharpGrammar.Identifier().Token()
               from commandDeclarations in CommandDeclaration.TokenAllowingComments().AtLeastOnce()
               select new ApplicationServiceDeclaration(name.Name, commandDeclarations))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("ApplicationServiceDeclaration");


        public static readonly Parser<TypeDeclaration> TypeDeclaration
            = ValueTypeDeclaration
                .Or<TypeDeclaration>(CommandDeclaration)
                .Or<TypeDeclaration>(DomainEventDeclaration)
                .Or<TypeDeclaration>(DtoDeclaration)
                .Or<TypeDeclaration>(EnumDeclaration)
                .Or<TypeDeclaration>(ApplicationServiceDeclaration);


        public static readonly Parser<Namespace> Namespace
            = (from declaration in Symbol("namespace").Token()
               from name in CSharpGrammar.NamespaceName().Named("namespace name").Token()
               from typeDeclarations in TypeDeclaration
                   .TokenAllowingComments()
                   .AtLeastOnce()
                   .Optional()
               let declarationList = typeDeclarations.GetOrElse(new List<TypeDeclaration>())
               select new Namespace(name, declarationList))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen);



        // This can be expanded to generalized nested key-value maps later
        public static readonly Parser<ConventionsDeclaration> ConventionsDeclaration
            = (from declaration in Symbol("defconventions").Token()
               from name in Keyword("domainevents").Token()
               from lcurly in TokenGrammar.LeftCurlyBrace.Token()
               from inherits in Keyword("inherit").Token()
               from lbracket in TokenGrammar.LeftSquareBracket.Token()
               from baseTypes in CSharpGrammar.TypeName().Token().Many()
               from rbracket in TokenGrammar.RightSquareBracket.Token()
               from rcurly in TokenGrammar.RightCurlyBrace.Token()
               let domainEventConventions = new DomainEventConventions(baseTypes)
               select new ConventionsDeclaration(domainEventConventions))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("ConventionsDeclaration");

        public static readonly Parser<AbstractSyntaxTree> AbstractSyntaxTree
            = (from conventions in ConventionsDeclaration.Optional().TokenAllowingComments()
               from namespaces in Namespace.TokenAllowingComments().Many()
               select new AbstractSyntaxTree(conventions.GetOrDefault(), namespaces));

        /// <summary>
        /// Top-level production for parsing everything in the source string.
        /// </summary>
        public static readonly Parser<AbstractSyntaxTree> Everything
            = AbstractSyntaxTree.Token().End();
    }
}