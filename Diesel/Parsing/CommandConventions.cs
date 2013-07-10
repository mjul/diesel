using System;
using System.Collections.Generic;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class CommandConventions : IConventionsNode, IEquatable<CommandConventions>
    {
        public BaseTypes BaseTypes { get; private set; }

        public CommandConventions(BaseTypes baseTypes)
        {
            BaseTypes = baseTypes;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { yield return BaseTypes; }
        }

        public CommandConventions ApplyOverridesFrom(CommandConventions other)
        {
            if (other == null) 
                return this;
            var newBaseTypeConventions =
                (BaseTypes ?? new BaseTypes(new TypeName[0]))
                    .ApplyOverridesFrom(other.BaseTypes);
            return new CommandConventions(newBaseTypeConventions);
        }

        public bool Equals(CommandConventions other)
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
            return Equals((CommandConventions) obj);
        }

        public override int GetHashCode()
        {
            return (BaseTypes != null ? BaseTypes.GetHashCode() : 0);
        }

        public static bool operator ==(CommandConventions left, CommandConventions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CommandConventions left, CommandConventions right)
        {
            return !Equals(left, right);
        }
    }
}