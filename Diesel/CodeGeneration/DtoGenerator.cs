using System.CodeDom;
using System.Linq;
using Diesel.Parsing;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    public class DtoGenerator : CodeDomGenerator
    {
        // TODO: Rename
        public static CodeTypeDeclaration CreateCommandDeclaration(SemanticModel model, DtoDeclaration declaration)
        {
            return CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(declaration.Name, declaration.Properties.ToArray(), true, true),
                model.KnownTypes);
        } 
    }
}