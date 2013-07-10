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
            CommandDeclaration declaration, 
            CommandConventions conventions)
        {
            var type = CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateClass(
                namespaceName, declaration.Name, 
                declaration.Properties.ToArray(), true, false),
                model.KnownTypes);
            ApplyConventions(conventions, type);
            return type;
        }

        private static void ApplyConventions(CommandConventions conventions, CodeTypeDeclaration typeDeclaration)
        {
            foreach (var typeName in conventions.BaseTypes.TypeNames)
            {
                typeDeclaration.BaseTypes.Add(typeName.Name);
            }
        }
    }
}