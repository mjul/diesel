using System.Linq;
using Diesel.Parsing;

namespace Diesel.Transformations
{
    public abstract class ModelTransformation 
    {
        public virtual AbstractSyntaxTree Transform(AbstractSyntaxTree ast)
        {
            return new AbstractSyntaxTree(Transform(ast.Conventions), ast.Namespaces.Select(Transform));
        }

        private ConventionsDeclaration Transform(ConventionsDeclaration conventions)
        {
            return conventions;
        }

        public virtual Namespace Transform(Namespace ns)
        {
            return new Namespace(ns.Name, ns.Declarations
                                            .Select<TypeDeclaration, TypeDeclaration>(d => Transform((dynamic)d)));
        }

        public virtual ValueTypeDeclaration Transform(ValueTypeDeclaration declaration)
        {
            return declaration;
        }

        public virtual CommandDeclaration Transform(CommandDeclaration declaration)
        {
            return declaration;
        }

        public virtual DomainEventDeclaration Transform(DomainEventDeclaration declaration)
        {
            return declaration;
        }

        public virtual DtoDeclaration Transform(DtoDeclaration declaration)
        {
            return declaration;
        }

        public virtual ApplicationServiceDeclaration Transform(ApplicationServiceDeclaration declaration)
        {
            return declaration;
        }

    }
}