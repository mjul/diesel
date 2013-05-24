using System.Collections.Generic;

namespace Diesel
{
    public class AbstractSyntaxTree
    {
        public IEnumerable<Namespace> Namespaces { get; private set; }

        public AbstractSyntaxTree(IEnumerable<Namespace> namespaces)
        {
            Namespaces = namespaces;
        }
    }
}