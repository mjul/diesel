using System.CodeDom;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class CommandGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateCommandDeclaration(CommandDeclaration declaration)
        {
            const bool isValueType = false;
            return CreateTypeWithValueSemantics(new ValueObjectSpecification(isValueType, declaration.Name, declaration.Properties.ToArray(), true, false));
        } 
    }
}