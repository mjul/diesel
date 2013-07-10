using System;
using System.Diagnostics;

namespace Diesel.Parsing
{
    [DebuggerDisplay(":{Name}")]
    public class Keyword : Terminal, IEquatable<Keyword>
    {
        public string Name { get; private set; }

        public Keyword(string name)
        {
            Name = name;
        }

        public bool Equals(Keyword other)
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
            return Equals((Keyword) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(Keyword left, Keyword right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Keyword left, Keyword right)
        {
            return !Equals(left, right);
        }
    }
}