using System.CodeDom;
using System.Linq;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class DtoGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateCommandDeclaration(DtoDeclaration declaration)
        {
            return CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(declaration.Name, declaration.Properties.ToArray(), true, true));
        } 
    }
}