namespace Diesel.Parsing
{
    /// <summary>
    /// Interface for the high-level model nodes.
    /// </summary>
    public interface IDieselExpression : ITreeNode
    {
        void Accept(IDieselExpressionVisitor visitor);
    }
}