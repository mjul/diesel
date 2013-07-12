using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public abstract class ValueTypeNode : ITypeNode
    {
        public abstract IEnumerable<ITreeNode> Children { get; }
        public abstract void Accept(ITypeNodeVisitor visitor);
    }
}