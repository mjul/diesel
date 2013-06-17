using System;
using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    /// <summary>
    /// C# "nullable-type" production
    /// </summary>
    public class NullableType : StructType, IEquatable<NullableType>
    {
        public TypeNode Underlying { get; private set; }

        public NullableType(TypeNode underlying)
        {
            Underlying = underlying;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { yield return Underlying; }
        }

        public bool Equals(NullableType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Underlying, other.Underlying);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NullableType) obj);
        }

        public override int GetHashCode()
        {
            return (Underlying != null ? Underlying.GetHashCode() : 0);
        }

        public static bool operator ==(NullableType left, NullableType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NullableType left, NullableType right)
        {
            return !Equals(left, right);
        }
    }
}