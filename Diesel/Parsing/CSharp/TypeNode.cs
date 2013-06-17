using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public abstract class TypeNode : ITreeNode
    {
        public abstract IEnumerable<ITreeNode> Children { get; }
    }
}