using System.CodeDom;
using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    public class DtoGenerator : CodeDomGenerator
    {
        // TODO: Rename
        public static CodeTypeDeclaration CreateCommandDeclaration(
            SemanticModel model, NamespaceName namespaceName,
            DtoDeclaration declaration)
        {
            return CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(
                    namespaceName, declaration.Name,
                    declaration.Properties.ToArray(),
                    new BaseTypes(new TypeName[0]),
                    true, true),
                model.KnownTypes);
        } 
    }
}