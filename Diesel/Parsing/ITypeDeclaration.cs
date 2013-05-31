using System;

namespace Diesel.Parsing
{
    /// <summary>
    /// Common interface for DSL named type declarations.
    /// </summary>
    public interface ITypeDeclaration
    {
        String Name { get; }
    }
}