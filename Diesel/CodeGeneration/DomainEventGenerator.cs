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
                    declaration.Properties.ToArray(),
                    conventions.BaseTypes,
                    true, true),
                model.KnownTypes);
            return type;
        }
    }
}