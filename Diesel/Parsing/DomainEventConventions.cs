using System;
using System.Collections.Generic;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class DomainEventConventions : IConventionsNode, IEquatable<DomainEventConventions>
    {
        public BaseTypes BaseTypes { get; private set; }

        public DomainEventConventions(BaseTypes baseTypes)
        {
            BaseTypes = baseTypes;
        }

        public IEnumerable<ITreeNode> Children
        {
            get 
            { 
                if (BaseTypes != null)
                {
                    foreach (var treeNode in BaseTypes.Children)
                    {
                        yield return treeNode;
                    }
                }
            }
        }

        public DomainEventConventions ApplyOverridesFrom(DomainEventConventions other)
        {
            if (null == other) return this;
            if (null == BaseTypes) return other;
            var newBaseTypes = BaseTypes.ApplyOverridesFrom(other.BaseTypes);
            return new DomainEventConventions(newBaseTypes);
        }


        public bool Equals(DomainEventConventions other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(BaseTypes, other.BaseTypes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventConventions) obj);
        }

        public override int GetHashCode()
        {
            return (BaseTypes != null ? BaseTypes.GetHashCode() : 0);
        }

        public static bool operator ==(DomainEventConventions left, DomainEventConventions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DomainEventConventions left, DomainEventConventions right)
        {
            return !Equals(left, right);
        }
    }
}