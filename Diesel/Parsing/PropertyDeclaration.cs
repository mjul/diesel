﻿using System;
using System.Collections.Generic;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class PropertyDeclaration : ITreeNode
    {
        public string Name { get; private set; }
        public ITypeNode Type { get; private set; }

        public PropertyDeclaration(string name, ITypeNode type)
        {
            Name = name;
            Type = type;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }

        [Obsolete("Move to semantic model")]
        public PropertyDeclaration OverrideType(ITypeNode type)
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