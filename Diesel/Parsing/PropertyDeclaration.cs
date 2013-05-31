using System;
using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class PropertyDeclaration : TypeDeclaration
    {
        public Type Type { get; private set; }

        public PropertyDeclaration(string name, Type type) : base(name)
        {
            Type = type;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }

        [Obsolete("Move to semantic model")]
        public PropertyDeclaration OverrideType(Type type)
        {
            return new PropertyDeclaration(this.Name, type);
        }

        [Obsolete("Move to semantic model")]
        public PropertyDeclaration OverrideName(string name)
        {
            return new PropertyDeclaration(name, this.Type);
        }

    }
}