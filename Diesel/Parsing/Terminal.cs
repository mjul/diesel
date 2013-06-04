using System.Collections.Generic;

namespace Diesel.Parsing
{
    public abstract class Terminal : ITreeNode
    {
        public IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }
    }
}