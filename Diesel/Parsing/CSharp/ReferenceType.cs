using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public abstract class ReferenceType : TypeNode
    {
        public abstract override IEnumerable<ITreeNode> Children { get; }
    }
}