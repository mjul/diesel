using System;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class BaseTypes : ITreeNode, IEquatable<BaseTypes>
    {
        public IEnumerable<TypeName> TypeNames { get; private set; }

        public BaseTypes(IEnumerable<TypeName> baseTypes)
        {
            TypeNames = baseTypes;
        }

        public BaseTypes ApplyOverridesFrom(BaseTypes other)
        {
            if (other == null) return this;

            var newBaseTypes = new List<TypeName>();
            if (other.TypeNames != null)
            {
                newBaseTypes.AddRange(other.TypeNames);
            }
            else
            {
                if (TypeNames != null)
                {
                    newBaseTypes.AddRange(TypeNames);
                }
            }
            return new BaseTypes(newBaseTypes);
        }

        public IEnumerable<ITreeNode> Children
        {
            get { return TypeNames; }
        }

        public bool Equals(BaseTypes other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.TypeNames == null && other.TypeNames == null) return true;
            if (this.TypeNames == null || other.TypeNames == null) return false;
            return Enumerable.SequenceEqual(TypeNames, other.TypeNames);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseTypes) obj);
        }

        public override int GetHashCode()
        {
            return (TypeNames != null ? TypeNames.GetHashCode() : 0);
        }

        public static bool operator ==(BaseTypes left, BaseTypes right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseTypes left, BaseTypes right)
        {
            return !Equals(left, right);
        }
    }
}