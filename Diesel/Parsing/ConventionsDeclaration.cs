using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class ConventionsDeclaration : ITreeNode
    {
        public DomainEventConventions DomainEventConventions { get; private set; }

        public ConventionsDeclaration(DomainEventConventions domainEventConventions)
        {
            DomainEventConventions = domainEventConventions;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { yield return DomainEventConventions; }
        }
    }
}