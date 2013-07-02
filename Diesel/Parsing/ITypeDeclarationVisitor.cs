namespace Diesel.Parsing
{
    public interface ITypeDeclarationVisitor
    {
        void Visit(ApplicationServiceDeclaration node);
        void Visit(CommandDeclaration node);
        void Visit(DtoDeclaration node);
        void Visit(DomainEventDeclaration node);
        void Visit(EnumDeclaration node);
        void Visit(ValueTypeDeclaration node);
    }
}