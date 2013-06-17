using System;

namespace Diesel.Parsing.CSharp
{
    public class RankSpecifier : Terminal, IEquatable<RankSpecifier>
    {
        public int Dimensions { get; private set; }

        public RankSpecifier(int dimensions)
        {
            Dimensions = dimensions;
        }

        public bool Equals(RankSpecifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Dimensions == other.Dimensions;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RankSpecifier) obj);
        }

        public override int GetHashCode()
        {
            return Dimensions;
        }

        public static bool operator ==(RankSpecifier left, RankSpecifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RankSpecifier left, RankSpecifier right)
        {
            return !Equals(left, right);
        }
    }
}