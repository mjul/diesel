using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public interface ITypeNode : ITreeNode
    {
        void Accept(ITypeNodeVisitor visitor);
    }
}