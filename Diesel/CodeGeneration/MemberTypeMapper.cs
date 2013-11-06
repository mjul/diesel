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
        /// Map a <see cref="ITypeNode"/> instance to the
        /// corresponding <see cref="MemberType"/>.
        /// </summary>
        [Pure]
        public static MemberType MemberTypeFor(NamespaceName namespaceName, ITypeNode type, IEnumerable<KnownType> knownTypes)
        {
            var visitor = new MemberTypeMapperTypeNodeVisitor(namespaceName, knownTypes);
            type.Accept(visitor);
            return visitor.MemberType;
        }

        private class MemberTypeMapperTypeNodeVisitor : ITypeNodeVisitor
        {
            private readonly NamespaceName _namespaceName;
            private readonly List<KnownType> _knownTypes;
            public MemberTypeMapperTypeNodeVisitor(NamespaceName namespaceName, IEnumerable<KnownType> knownTypes)
            {
                _namespaceName = namespaceName;
                _knownTypes = knownTypes.ToList();
            }

            public MemberType MemberType { get; private set; }

            public void Visit(TypeName typeName)
            {
                if (SystemTypeMapper.IsSystemType(typeName))
                {
                    ReturnSystemMemberType(typeName);
                }
                else
                {
                    ReturnNamedMember(typeName);
                }
            }

            private void ReturnNamedMember(TypeName typeName)
            {
                var fullName = FullyQualifiedNameRule.For(_namespaceName, typeName);
                var knownType = _knownTypes.SingleOrDefault(x => x.FullName == fullName);
                var isValueType = knownType != null && knownType.IsValueType;
                MemberType = MemberType.CreateForTypeName(typeName, isValueType);
            }

            private void ReturnNullableMember(TypeName underlyingTypeName)
            {
                var fullName = FullyQualifiedNameRule.For(_namespaceName, underlyingTypeName);
                var nullableTypeName = new TypeName(TypeNameMapper.TypeNameForNullableType(fullName));
                MemberType = MemberType.CreateForTypeName(nullableTypeName, true);
            }
            
            private void ReturnSystemMemberType(ITypeNode node)
            {
                MemberType = MemberType.CreateForSystemType(SystemTypeMapper.SystemTypeFor(node));
            }

            public void Visit(StringReferenceType stringReferenceType)
            {
                ReturnSystemMemberType(stringReferenceType);
            }

            public void Visit(ArrayType arrayType)
            {
                var elementMemberType = MemberTypeFor(_namespaceName, arrayType.Type, _knownTypes);
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
                    var hasResult = false;
                    var underlyingTypeName = GetUnderlyingTypeName(nullableType);
                    if (underlyingTypeName != null)
                    {
                       if (IsKnownValueType(underlyingTypeName))
                       {
                           ReturnNullableMember(underlyingTypeName);
                           hasResult = true;
                       }
                    }
                    if (!hasResult)
                    {
                        throw new NotImplementedException("Nullable Type members not implemented for unknown, non-system types.");
                    }

                }
            }

            private TypeName GetUnderlyingTypeName(NullableType nullableType)
            {
                var visitor = new NullableTypeUnderlyingTypeNameVisitor();
                nullableType.Underlying.Accept(visitor);
                var underlyingName = visitor.UnderlyingTypeName;
                return underlyingName;
            }

            private bool IsKnownValueType(TypeName typeName)
            {
                var fullyQualifiedTypeName = FullyQualifiedNameRule.For(_namespaceName, typeName);
                return _knownTypes.Where(x => x.IsValueType).Any(x => x.FullName == fullyQualifiedTypeName);
            }

            private class NullableTypeUnderlyingTypeNameVisitor : ITypeNodeVisitor
            {
                public TypeName UnderlyingTypeName;

                public void Visit(TypeName typeName)
                {
                    UnderlyingTypeName = typeName;
                }

                public void Visit(StringReferenceType stringReferenceType)
                {
                }

                public void Visit(ArrayType arrayType)
                {
                }

                public void Visit(SimpleType simpleType)
                {
                }

                public void Visit(NullableType nullableType)
                {
                }
            }
        }
    }
}