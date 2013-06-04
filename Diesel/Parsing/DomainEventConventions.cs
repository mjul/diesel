using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class DomainEventConventions
    {
        public IEnumerable<TypeName> BaseTypes { get; private set; }

        public DomainEventConventions(IEnumerable<TypeName> baseTypes)
        {
            BaseTypes = baseTypes;
        }
    }
}