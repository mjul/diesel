using System.CodeDom;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    internal class DomainEventGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateDomainEventDeclaration(DomainEventDeclaration declaration)
        {
            const bool isValueType = false;
            return CreateTypeWithValueSemantics(isValueType, declaration.Name, declaration.Properties.ToArray(), true, true);
        } 
    }
}