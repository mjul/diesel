using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public abstract class ReferenceType : ITypeNode
    {
        public abstract IEnumerable<ITreeNode> Children { get; }
        public abstract void Accept(ITypeNodeVisitor visitor);
    }
}