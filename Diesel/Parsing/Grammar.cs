﻿using System;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing.CSharp;
using Sprache;

namespace Diesel.Parsing
{
    public static class Grammar
    {
        private static CSharpGrammar _cSharpGrammar = new CSharpGrammar();

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
            = (from type in _cSharpGrammar.TypeNode().Token().Named("Property type")
               from name in _cSharpGrammar.Identifier().Token().Named("Property name")
               select new PropertyDeclaration(name.Name, type))
                .Named("PropertyDeclartion");

        private static Parser<IEnumerable<TElement>> SequenceOf<TElement,TDelimiter>(Parser<TElement> elementParser, Parser<TDelimiter> delimiterParser)
        {
            return (from element in elementParser
                        .Many()
                        .DelimitedBy(delimiterParser)
                        .Token()
                        .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                    select element.SelectMany(x => x));
        }

        private static readonly Parser<IEnumerable<PropertyDeclaration>> PropertyDeclarations
            = SequenceOf(PropertyDeclaration, TokenGrammar.Comma)
                .Named("PropertyDeclarations");


        private static readonly Parser<ValueTypeDeclaration> SimpleValueTypeDeclaration
            = (from declaration in Symbol("defvaluetype").Token()
               from name in _cSharpGrammar.Identifier().Token()
               from optionalTypeDeclaration in _cSharpGrammar.TypeNode().Optional().Token()
               let property = new[] {new PropertyDeclaration(null, optionalTypeDeclaration.GetOrDefault())}
               select new ValueTypeDeclaration(name.Name, property))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("SimpleValueTypeDeclaration");

        private static readonly Parser<ValueTypeDeclaration> ValueTypeDeclarationWithPropertyList
            = (from declaration in Symbol("defvaluetype").Token()
               from name in _cSharpGrammar.Identifier().Token()
               from properties in PropertyDeclarations.Token()
               select new ValueTypeDeclaration(name.Name, properties))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("ValueTypeDeclarationWithPropertyList");

        public static readonly Parser<ValueTypeDeclaration> ValueTypeDeclaration
            = SimpleValueTypeDeclaration
                .Or(ValueTypeDeclarationWithPropertyList)
                .Named("ValueTypeDeclaration");


        public static readonly Parser<CommandDeclaration> CommandDeclaration
            = (from declaration in Symbol("defcommand").Token()
               from name in _cSharpGrammar.Identifier().Token()
               from optionalPropertyDeclarations in PropertyDeclarations.Optional()
               select new CommandDeclaration(name.Name, optionalPropertyDeclarations.GetOrElse(new PropertyDeclaration[] { })))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("CommandDeclaration");


        public static readonly Parser<DomainEventDeclaration> DomainEventDeclaration
            = (from declaration in Symbol("defdomainevent").Token()
               from name in _cSharpGrammar.Identifier().Token()
               from propertyDeclarations in PropertyDeclarations.Token()
               select new DomainEventDeclaration(name.Name, propertyDeclarations))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("DomainEventDeclaration");


        public static readonly Parser<ApplicationServiceDeclaration> ApplicationServiceDeclaration
            = (from declaration in Symbol("defapplicationservice").Token()
               from name in _cSharpGrammar.Identifier().Token()
               from commandDeclarations in CommandDeclaration.Token().AtLeastOnce()
               select new ApplicationServiceDeclaration(name.Name, commandDeclarations))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("ApplicationServiceDeclaration");

        public static readonly Parser<TypeDeclaration> TypeDeclaration
            = ValueTypeDeclaration
                .Or<TypeDeclaration>(CommandDeclaration)
                .Or<TypeDeclaration>(DomainEventDeclaration)
                .Or<TypeDeclaration>(ApplicationServiceDeclaration);

        public static readonly Parser<Namespace> Namespace
            = (from declaration in Symbol("namespace").Token()
               from name in _cSharpGrammar.NamespaceName().Named("namespace name").Token()
               from typeDeclarations in TypeDeclaration
                   .Token()
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
               from baseTypes in _cSharpGrammar.TypeName().Token().Many()
               from rbracket in TokenGrammar.RightSquareBracket.Token()
               from rcurly in TokenGrammar.RightCurlyBrace.Token()
               let domainEventConventions = new DomainEventConventions(baseTypes)
               select new ConventionsDeclaration(domainEventConventions))
                .Contained(TokenGrammar.LeftParen, TokenGrammar.RightParen)
                .Named("ConventionsDeclaration");

        public static readonly Parser<AbstractSyntaxTree> AbstractSyntaxTree
            = (from conventions in ConventionsDeclaration.Optional()
               from namespaces in Namespace.Token().Many().Token() 
               select new AbstractSyntaxTree(conventions.GetOrDefault(), namespaces));

        /// <summary>
        /// Top-level production for parsing everything in the source string.
        /// </summary>
        public static readonly Parser<AbstractSyntaxTree> Everything
            = AbstractSyntaxTree.Token().End();
    }
}