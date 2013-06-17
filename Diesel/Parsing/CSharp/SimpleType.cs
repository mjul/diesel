using System;
using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    /// <summary>
    /// The C# "simple-type" production (simplified, no subtypes for bool, integral and floating)
    /// </summary>
    public class SimpleType : StructType, IEquatable<SimpleType>
    {
        public Type Type { get; set; }

        public SimpleType(Type type)
        {
            Type = type;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }

        public bool Equals(SimpleType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleType) obj);
        }

        public override int GetHashCode()
        {
            return (Type != null ? Type.GetHashCode() : 0);
        }

        public static bool operator ==(SimpleType left, SimpleType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimpleType left, SimpleType right)
        {
            return !Equals(left, right);
        }
    }
}