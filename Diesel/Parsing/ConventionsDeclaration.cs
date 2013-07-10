using System;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class ConventionsDeclaration : IDieselExpression, IEquatable<ConventionsDeclaration>
    {
        public DomainEventConventions DomainEventConventions { get; private set; }
        public CommandConventions CommandConventions { get; private set; }

        public ConventionsDeclaration(DomainEventConventions domainEventConventions, 
                                        CommandConventions commandConventions)
        {
            DomainEventConventions = domainEventConventions;
            CommandConventions = commandConventions;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { 
                if (null != DomainEventConventions)
                {
                    foreach (var d in DomainEventConventions.Children)
                        yield return d;
                }
                if (null != CommandConventions)
                {
                    foreach (var d in CommandConventions.Children)
                        yield return d;
                }
            }
        }

        public void Accept(IDieselExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ConventionsDeclaration ApplyOverridesFrom(ConventionsDeclaration other)
        {
            if (other != null)
            {
                var newDomainEventConventions = 
                    (DomainEventConventions ?? new DomainEventConventions(new BaseTypes(new TypeName[] {})))
                    .ApplyOverridesFrom(other.DomainEventConventions);
                var newCommandConventions = 
                    (CommandConventions ?? new CommandConventions(new BaseTypes(new TypeName[] {})))
                    .ApplyOverridesFrom(other.CommandConventions);
                return new ConventionsDeclaration(newDomainEventConventions, newCommandConventions);
            }
            else
            {
                return this;
            }
        }

        public bool Equals(ConventionsDeclaration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(DomainEventConventions, other.DomainEventConventions) && Equals(CommandConventions, other.CommandConventions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConventionsDeclaration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DomainEventConventions != null ? DomainEventConventions.GetHashCode() : 0)*397) ^ (CommandConventions != null ? CommandConventions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ConventionsDeclaration left, ConventionsDeclaration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConventionsDeclaration left, ConventionsDeclaration right)
        {
            return !Equals(left, right);
        }
    }
}