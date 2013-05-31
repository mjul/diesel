using System.Collections.Generic;

namespace Diesel.Parsing
{
    public interface ITreeNode
    {
        IEnumerable<ITreeNode> Children { get; }
    }
}