using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public abstract class TypeNode : ITreeNode
    {
        public abstract void Accept(ITypeNodeVisitor visitor);
        public abstract IEnumerable<ITreeNode> Children { get; }
    }
}