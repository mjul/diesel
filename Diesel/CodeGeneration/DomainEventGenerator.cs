using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    internal class DomainEventGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateDomainEventDeclaration(
            SemanticModel model, NamespaceName namespaceName,
            DomainEventDeclaration declaration, 
            DomainEventConventions conventions)
        {
            var type = CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(
                    namespaceName, declaration.Name, 
                    declaration.Properties.ToArray(), true, true),
                model.KnownTypes);
            ApplyConventions(conventions, type);
            return type;
        }

        private static void ApplyConventions(DomainEventConventions conventions, CodeTypeDeclaration typeDeclaration)
        {
            foreach (var typeName in conventions.BaseTypes)
            {
                typeDeclaration.BaseTypes.Add(typeName.Name);
            }
        }
    }
}