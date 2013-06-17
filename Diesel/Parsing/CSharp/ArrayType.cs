using System;
using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public class ArrayType : ReferenceType, IEquatable<ArrayType>
    {
        public TypeNode Type { get; private set; }
        public RankSpecifiers RankSpecifiers { get; set; }

        public ArrayType(TypeNode nonArrayType, RankSpecifiers rankSpecifiers)
        {
            Type = nonArrayType;
            RankSpecifiers = rankSpecifiers;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get 
            { 
                yield return Type;
                yield return RankSpecifiers;
            }
        }

        public bool Equals(ArrayType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Type, other.Type) && Equals(RankSpecifiers, other.RankSpecifiers);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ArrayType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0)*397) ^ (RankSpecifiers != null ? RankSpecifiers.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ArrayType left, ArrayType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ArrayType left, ArrayType right)
        {
            return !Equals(left, right);
        }
    }
}