using System.Collections.Generic;

namespace Diesel.Parsing
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