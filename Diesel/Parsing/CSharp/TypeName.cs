using System;
using System.Diagnostics;

namespace Diesel.Parsing.CSharp
{
    [DebuggerDisplay("{Name}")]
    public class TypeName : Terminal, IEquatable<TypeName>, ITypeNode, IStructType
    {
        public string Name { get; private set; }
        public TypeName(string name)
        {
            Name = name;
        }

        public bool Equals(TypeName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeName) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public void Accept(ITypeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public static bool operator ==(TypeName left, TypeName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeName left, TypeName right)
        {
            return !Equals(left, right);
        }
    }
}