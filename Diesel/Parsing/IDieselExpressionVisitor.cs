namespace Diesel.Parsing
{
    public interface IDieselExpressionVisitor : ITypeDeclarationVisitor
    {
        void Visit(AbstractSyntaxTree node);
        void Visit(Namespace node);
        void Visit(ConventionsDeclaration node);
    }
}