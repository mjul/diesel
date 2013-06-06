using System.Collections.Generic;
using Diesel.Parsing.CSharp;

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

        public DomainEventConventions ApplyOverridesFrom(DomainEventConventions other)
        {
            if (null == other) return this;
            var newBaseTypes = new List<TypeName>();
            if (other.BaseTypes != null)
            {
                newBaseTypes.AddRange(other.BaseTypes);
            }
            else
            {
                if (BaseTypes != null)
                {
                    newBaseTypes.AddRange(BaseTypes);
                }
            }
            return new DomainEventConventions(newBaseTypes);
        }
    }
}