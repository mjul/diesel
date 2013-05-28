using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Diesel
{
    public static class Grammar
    {
        public static Parser<string> Identifier =
            (from first in Parse.Letter.Once().Text()
             from rest in Parse.LetterOrDigit.Many().Text()
             select first + rest)
            .Named("Identifier");

        private static Parser<Type> TypeName(string name, Type type)
        {
            return (from id in Identifier
                    where id == name
                    select type);
        }

        public static Parser<Type> PrimitiveType =
            (from type in TypeName("Int32", typeof (Int32))
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
             select type)
            .Named("PrimitiveType");

        public static Parser<ValueTypeDeclaration> ValueTypeDeclaration
            = (from open in Parse.Char('(')
               from declaration in Parse.String("defvaluetype").Token()
               from name in Identifier.Token()
               from optionalTypeDeclaration in PrimitiveType.Optional().Token()
               from close in Parse.Char(')')
               let valueType = optionalTypeDeclaration.GetOrDefault()
               select new ValueTypeDeclaration(name, valueType))
               .Named("ValueTypeDeclaration");

        public static Parser<string> NamespaceIdentifier
            = Identifier.Named("Namespace part")
                        .DelimitedBy(Parse.Char('.'))
                        .Select(parts => String.Join(".", parts))
                        .Named("Namespace Identifier");

        public static Parser<PropertyDeclaration> PropertyDeclaration
            = (from type in PrimitiveType.Named("Property type").Token()
               from name in Identifier.Named("Property name").Token()
               select new PropertyDeclaration(name, type))
               .Named("PropertyDeclartion ::= PrimitiveType NAME");

        public static Parser<IEnumerable<PropertyDeclaration>> PropertyDeclarations
            = (from open in Parse.Char('(')
               from declarations in PropertyDeclaration
                   .Many()
                   .DelimitedBy(Parse.Char(','))
                   .Token()
               from close in Parse.Char(')')
               select declarations.SelectMany(x => x));

        public static Parser<CommandDeclaration> CommandDeclaration
            = (from open in Parse.Char('(')
               from declaration in Parse.String("defcommand").Token()
               from name in Identifier.Token()
               from optionalPropertyDeclarations in PropertyDeclarations.Optional()
               from close in Parse.Char(')').Named("closing parenthesis for defcommand")
               select new CommandDeclaration(name, optionalPropertyDeclarations.GetOrElse(new List<PropertyDeclaration>())));


        public static Parser<ApplicationServiceDeclaration> ApplicationServiceDeclaration
            = (from open in Parse.Char('(')
               from declaration in Parse.String("defapplicationservice").Token()
               from name in Identifier.Token()
               from commandDeclarations in CommandDeclaration.Token().AtLeastOnce()
               from close in Parse.Char(')')
               select new ApplicationServiceDeclaration(name, commandDeclarations));

        private static Parser<ITypeDeclaration> TypeDeclaration
            = ValueTypeDeclaration
                .Or<ITypeDeclaration>(CommandDeclaration)
                .Or<ITypeDeclaration>(ApplicationServiceDeclaration);

        public static Parser<Namespace> Namespace
            = (from open in Parse.Char('(')
               from declaration in Parse.String("namespace").Token()
               from name in NamespaceIdentifier.Named("namespace name").Token()
               from typeDeclarations in TypeDeclaration
                   .Token()
                   .AtLeastOnce()
                   .Optional()
               from close in Parse.Char(')')
               let declarationList = typeDeclarations.GetOrElse(new List<ITypeDeclaration>())
               select new Namespace(name, declarationList));

        public static Parser<AbstractSyntaxTree> AbstractSyntaxTree
            = (from namespaces in Namespace.Token().Many().Token()
               select new AbstractSyntaxTree(namespaces));

    }
}