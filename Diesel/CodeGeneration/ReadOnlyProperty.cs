using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public abstract class Member
    {
        public String Name { get; private set; }
        public MemberType Type { get; private set; }
        public IEnumerable<CodeAttributeDeclaration> Attributes { get; private set; }

        protected Member(string name, MemberType type, IEnumerable<CodeAttributeDeclaration> attributeDeclarations)
        {
            Name = name;
            Type = type;
            Attributes = attributeDeclarations;
        }
    }

    public class ReadOnlyProperty : Member
    {
        public BackingField BackingField { get; private set; }

        public ReadOnlyProperty(string name, MemberType type, BackingField backingField, IEnumerable<CodeAttributeDeclaration> attributeDeclarations)
            : base(name, type, attributeDeclarations)
        {
            BackingField = backingField;
        }
    }

    public class BackingField : Member
    {
        public BackingField(string name, MemberType type, IEnumerable<CodeAttributeDeclaration> attributeDeclarations) 
            : base(name, type, attributeDeclarations)
        {
        }
    }

    public class MemberType
    {
        public string FullName { get; private set; }
        public bool IsValueType { get; private set; }

        private MemberType(string fullName, bool isValueType)
        {
            FullName = fullName;
            IsValueType = isValueType;
        }

        public static MemberType CreateForSystemType(Type type)
        {
            return new MemberType(type.FullName, type.IsValueType);
        }

        public static MemberType CreateForTypeName(TypeName name, bool isValueType)
        {
            return new MemberType(name.Name, isValueType);
        }

        public static MemberType CreateForArray(MemberType elementType, RankSpecifiers rankSpecifiers)
        {
            var fullName = TypeNameMapper.TypeNameForArray(elementType.FullName, rankSpecifiers);
            return new MemberType(fullName, false);
        }
    }
}