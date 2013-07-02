using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    /// <summary>
    /// Responsible for mapping AST <see cref="TypeNode"/> instances to <see cref="MemberType"/>.
    /// </summary>
    [Pure]
    public class MemberTypeMapper
    {
        /// <summary>
        /// Map a <see cref="TypeNode"/> instance to the
        /// corresponding <see cref="MemberType"/>.
        /// </summary>
        [Pure]
        public static MemberType MemberTypeFor(TypeNode type, IEnumerable<KnownType> knownTypes)
        {
            var visitor = new MemberTypeMapperTypeNodeVisitor(knownTypes);
            type.Accept(visitor);
            return visitor.MemberType;
        }

        private class MemberTypeMapperTypeNodeVisitor : ITypeNodeVisitor
        {
            private readonly List<KnownType> _knownTypes;
            public MemberTypeMapperTypeNodeVisitor(IEnumerable<KnownType> knownTypes)
            {
                _knownTypes = knownTypes.ToList();
            }

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
                var typeName = typeNameTypeNode.TypeName;
                var knownType = _knownTypes.SingleOrDefault(x => x.FullName == typeName.Name);
                var isValueType = knownType != null && knownType.IsValueType;
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
                var elementMemberType = MemberTypeFor(arrayType.Type, _knownTypes);
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