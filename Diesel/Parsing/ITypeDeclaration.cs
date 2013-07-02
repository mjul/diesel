using System;

namespace Diesel.Parsing
{
    /// <summary>
    /// Common interface for DSL named type declarations.
    /// </summary>
    public interface ITypeDeclaration : IDieselExpression
    {
        String Name { get; }
        void Accept(ITypeDeclarationVisitor visitor);
    }
}