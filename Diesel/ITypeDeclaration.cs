using System;

namespace Diesel
{
    /// <summary>
    /// Common interface for DSL named type declarations.
    /// </summary>
    public interface ITypeDeclaration
    {
        String Name { get; }
    }
}