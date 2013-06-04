using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class DomainEventConventions : ITreeNode
    {
        public IEnumerable<TypeName> BaseTypes { get; private set; }

        public DomainEventConventions(IEnumerable<TypeName> baseTypes)
        {
            BaseTypes = baseTypes;
        }

        public IEnumerable<ITreeNode> Children { get { return BaseTypes; } }
    }
}