using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class AbstractSyntaxTree : ITreeNode
    {
        public IEnumerable<Namespace> Namespaces { get; private set; }

        public AbstractSyntaxTree(IEnumerable<Namespace> namespaces)
        {
            Namespaces = namespaces;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { return Namespaces; }
        }
    }
}