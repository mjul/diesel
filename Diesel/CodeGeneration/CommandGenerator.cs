using System.CodeDom;
using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    public class CommandGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateCommandDeclaration(
            SemanticModel model, NamespaceName namespaceName,
            CommandDeclaration declaration)
        {
            return CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(
                namespaceName, declaration.Name, 
                declaration.Properties.ToArray(), true, false),
                model.KnownTypes);
        } 
    }
}