using System;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public class MemberTypeMapper
    {
        public static MemberType MemberTypeFor(TypeNode type)
        {
            var visitor = new MemberTypeMapperTypeNodeVisitor();
            type.Accept(visitor);
            return visitor.MemberType;
        }

        private class MemberTypeMapperTypeNodeVisitor : ITypeNodeVisitor
        {
            public MemberType MemberType { get; private set; }

            public void Visit(TypeNameTypeNode typeNameTypeNode)
            {
                if (SystemTypeMapper.IsSystemType(typeNameTypeNode))
                {
                    ReturnSystemMemberType(typeNameTypeNode);
                }
                else
                {
                    ReturnNamedMember(typeNameTypeNode);
                }
            }

            private void ReturnNamedMember(TypeNameTypeNode typeNameTypeNode)
            {
                // TODO : look up the actual value type info 
                var isValueType = false;
                MemberType = MemberType.CreateForTypeName(typeNameTypeNode.TypeName, isValueType);
            }

            private void ReturnSystemMemberType(TypeNode node)
            {
                MemberType = MemberType.CreateForSystemType(SystemTypeMapper.SystemTypeFor(node));
            }

            public void Visit(StringReferenceType stringReferenceType)
            {
                ReturnSystemMemberType(stringReferenceType);
            }

            public void Visit(ArrayType arrayType)
            {
                var elementMemberType = MemberTypeFor(arrayType.Type);
                MemberType = MemberType.CreateForArray(elementMemberType, arrayType.RankSpecifiers);
            }

            public void Visit(SimpleType simpleType)
            {
                ReturnSystemMemberType(simpleType);
            }

            public void Visit(NullableType nullableType)
            {
                if (SystemTypeMapper.IsSystemType(nullableType.Underlying))
                {
                    ReturnSystemMemberType(nullableType);
                }
                else
                {
                    throw new NotImplementedException("Nullable Type members not implemented for non-system types.");
                }
            }
        }
    }
}