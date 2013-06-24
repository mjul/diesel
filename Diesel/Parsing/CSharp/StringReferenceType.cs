using System;
using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public class StringReferenceType : ReferenceType, IEquatable<StringReferenceType>
    {
        public StringReferenceType()
        {
        }

        public override void Accept(ITypeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }

        public bool Equals(StringReferenceType other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringReferenceType) obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public static bool operator ==(StringReferenceType left, StringReferenceType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringReferenceType left, StringReferenceType right)
        {
            return !Equals(left, right);
        }
    }
}