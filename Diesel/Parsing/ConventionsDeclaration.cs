namespace Diesel.Parsing
{
    public class ConventionsDeclaration
    {
        public DomainEventConventions DomainEventConventions { get; private set; }

        public ConventionsDeclaration(DomainEventConventions domainEventConventions)
        {
            DomainEventConventions = domainEventConventions;
        }
    }
}