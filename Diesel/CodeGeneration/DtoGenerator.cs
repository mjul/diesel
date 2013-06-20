using System.CodeDom;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class DtoGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateCommandDeclaration(DtoDeclaration declaration)
        {
            const bool isValueType = false;
            return CreateTypeWithValueSemantics(isValueType, declaration.Name, declaration.Properties.ToArray(), true, true);
        } 
    }
}