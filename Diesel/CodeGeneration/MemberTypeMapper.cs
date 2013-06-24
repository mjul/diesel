using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public class MemberTypeMapper
    {
        public static MemberType MemberTypeFor(TypeNode type)
        {
            if (SystemTypeMapper.IsSystemType(type))
            {
                return MemberType.CreateForSystemType(SystemTypeMapper.SystemTypeFor(type));
            }
            var visitor = new MemberTypeMapperForNonSystemType();
            type.Accept(visitor);
            // TODO: fix this
            bool isValueType = false;
            return MemberType.CreateForTypeName(visitor.Result, isValueType);
        }

        private class MemberTypeMapperForNonSystemType : ITypeNodeVisitor
        {
            public TypeName Result { get; private set; }

            public void Visit(TypeNameTypeNode typeNameTypeNode)
            {
                Result = typeNameTypeNode.TypeName;
            }

            public void Visit(StringReferenceType stringReferenceType)
            {
                throw new System.NotImplementedException();
            }

            public void Visit(ArrayType arrayType)
            {
                throw new System.NotImplementedException();
            }

            public void Visit(SimpleType simpleType)
            {
                throw new System.NotImplementedException();
            }

            public void Visit(NullableType nullableType)
            {
                throw new System.NotImplementedException();
            }

        }
    }
}