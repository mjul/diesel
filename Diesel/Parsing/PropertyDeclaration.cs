using System;
using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class PropertyDeclaration : ITreeNode
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public PropertyDeclaration(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }

        [Obsolete("Move to semantic model")]
        public PropertyDeclaration OverrideType(Type type)
        {
            return new PropertyDeclaration(Name, type);
        }

        [Obsolete("Move to semantic model")]
        public PropertyDeclaration OverrideName(string name)
        {
            return new PropertyDeclaration(name, Type);
        }

    }
}