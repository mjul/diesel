using System;
using System.Collections.Generic;

namespace Diesel.Parsing.CSharp
{
    public class TypeNameTypeNode : TypeNode, IEquatable<TypeNameTypeNode>
    {
        public TypeName TypeName { get; private set; }

        public TypeNameTypeNode(TypeName typeName)
        {
            TypeName = typeName;
        }

        public override void Accept(ITypeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { yield return TypeName; }
        }

        public bool Equals(TypeNameTypeNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(TypeName, other.TypeName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeNameTypeNode) obj);
        }

        public override int GetHashCode()
        {
            return (TypeName != null ? TypeName.GetHashCode() : 0);
        }

        public static bool operator ==(TypeNameTypeNode left, TypeNameTypeNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeNameTypeNode left, TypeNameTypeNode right)
        {
            return !Equals(left, right);
        }
    }
}