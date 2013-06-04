using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class AbstractSyntaxTree : ITreeNode
    {
        public ConventionsDeclaration Conventions { get; private set; }
        public IEnumerable<Namespace> Namespaces { get; private set; }

        public AbstractSyntaxTree(ConventionsDeclaration conventions, IEnumerable<Namespace> namespaces)
        {
            Conventions = conventions;
            Namespaces = namespaces;
        }

        public IEnumerable<ITreeNode> Children
        {
            get
            {
                yield return Conventions;
                foreach (var ns in Namespaces)
                    yield return ns;
            }
        }
    }
}