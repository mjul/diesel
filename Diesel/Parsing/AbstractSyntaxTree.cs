using System.Collections.Generic;

namespace Diesel.Parsing
{
    public abstract class TreeNode
    {
        public abstract IEnumerable<TreeNode> Children { get; }
    }

    public class AbstractSyntaxTree : TreeNode
    {
        public IEnumerable<Namespace> Namespaces { get; private set; }

        public AbstractSyntaxTree(IEnumerable<Namespace> namespaces)
        {
            Namespaces = namespaces;
        }

        public override IEnumerable<TreeNode> Children
        {
            get { return Namespaces; }
        }
    }
}