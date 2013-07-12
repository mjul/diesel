namespace Diesel.Parsing.CSharp
{
    /// <summary>
    /// Visitor interface for TypeNode hierarchy.
    /// </summary>
    public interface ITypeNodeVisitor
    {
        void Visit(TypeName typeName);
        void Visit(StringReferenceType stringReferenceType);
        void Visit(ArrayType arrayType);
        void Visit(SimpleType simpleType);
        void Visit(NullableType nullableType);
    }
}