using System.Linq;
using Sprache;

namespace Diesel
{
    public static class ModelTransformations
    {
         public static AbstractSyntaxTree ApplyDefaults(AbstractSyntaxTree ast)
         {
             return new AbstractSyntaxTree(
                 ast.Namespaces.Select(ApplyDefaults));
         }

         private static Namespace ApplyDefaults(Namespace ns)
         {
             return new Namespace(ns.Name, ns.Declarations
                 .Select<ITypeDeclaration, ITypeDeclaration>(d => ApplyDefaults((dynamic)d)));
         }

         private static ValueTypeDeclaration ApplyDefaults(ValueTypeDeclaration declaration)
         {
             if (null == declaration.ValueType)
             {
                 return new ValueTypeDeclaration(declaration.Name, typeof (int));
             }
             return declaration;
         }

        private static CommandDeclaration ApplyDefaults(CommandDeclaration declaration)
        {
            return declaration;
        }

    }
}