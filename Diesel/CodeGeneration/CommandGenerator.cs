using System.CodeDom;
using System.Linq;
using Diesel.Parsing;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    public class CommandGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateCommandDeclaration(
            SemanticModel model,
            CommandDeclaration declaration)
        {
            return CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(declaration.Name, declaration.Properties.ToArray(), 
                true, false),
                model.KnownTypes);
        } 
    }
}