using System.CodeDom;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class CommandGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateCommandDeclaration(CommandDeclaration declaration)
        {
            return CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(declaration.Name, declaration.Properties.ToArray(), true, false));
        } 
    }
}