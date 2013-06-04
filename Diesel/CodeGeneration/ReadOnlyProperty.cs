using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Diesel.CodeGeneration
{
    public abstract class Member
    {
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public IEnumerable<CodeAttributeDeclaration> Attributes { get; private set; }

        protected Member(string name, Type valueType, IEnumerable<CodeAttributeDeclaration> attributeDeclarations)
        {
            Name = name;
            Type = valueType;
            Attributes = attributeDeclarations;
        }
    }

    public class ReadOnlyProperty : Member
    {
        public BackingField BackingField { get; private set; }

        public ReadOnlyProperty(string name, Type valueType, BackingField backingField, IEnumerable<CodeAttributeDeclaration> attributeDeclarations) 
            : base(name, valueType, attributeDeclarations)
        {
            BackingField = backingField;
        }
    }

    public class BackingField : Member
    {
        public BackingField(string name, Type valueType, IEnumerable<CodeAttributeDeclaration> attributeDeclarations) 
            : base(name, valueType, attributeDeclarations)
        {
        }
    }
}