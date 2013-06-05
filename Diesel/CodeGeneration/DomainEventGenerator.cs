using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    internal class DomainEventGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateDomainEventDeclaration(DomainEventDeclaration declaration, DomainEventConventions conventions)
        {
            const bool isValueType = false;
            var type = CreateTypeWithValueSemantics(isValueType, declaration.Name, declaration.Properties.ToArray(), true, true);
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