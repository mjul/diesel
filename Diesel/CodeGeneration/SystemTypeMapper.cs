using System;
using System.Linq;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public class SystemTypeMapper 
    {
        public static System.Type SystemTypeFor(TypeNode node)
        {
            var visitor = Visit(node);
            if (!visitor.FoundSystemType) throw new ArgumentOutOfRangeException("node", node, "No System type found for TypeNode.");
            return visitor.Result;
        }

        public static bool IsSystemType(TypeNameTypeNode typeNameTypeNode)
        {
            return Visit(typeNameTypeNode).FoundSystemType;
        }

        private static SystemTypeMapperVisitor Visit(TypeNode node)
        {
            var visitor = new SystemTypeMapperVisitor();
            node.Accept(visitor);
            return visitor;
        }

        private class SystemTypeMapperVisitor : ITypeNodeVisitor
        {
            public Type Result { get; private set; }
            public bool FoundSystemType
            {
                get { return (null != Result); }
            }

            public void Visit(StringReferenceType typeNode)
            {
                Result = typeof (String);
            }

            public void Visit(ArrayType typeNode)
            {
                Type underlying = SystemTypeFor(typeNode.Type);
                var rankSpecifiers = (from rank in typeNode.RankSpecifiers.Ranks.Reverse()
                                      from rankSpec in String.Format("[{0}]",
                                                                     String.Join("", Enumerable.Repeat(',', rank.Dimensions - 1)))
                                      select rankSpec);
                Result = Type.GetType(String.Format("{0}{1}",
                                                    underlying.FullName,
                                                    String.Concat(rankSpecifiers)));
            }

            public void Visit(SimpleType typeNode)
            {
                Result = typeNode.Type;
            }

            public void Visit(NullableType typeNode)
            {
                Type underlying = SystemTypeFor(typeNode.Underlying);
                Result = Type.GetType(String.Format("System.Nullable`1[{0}]", underlying.FullName));
            }

            public void Visit(TypeNameTypeNode typeNode)
            {
                var knownSystemType = TryGetSystemTypeFor(typeNode);
                Result = knownSystemType;
            }

            private Type TryGetSystemTypeFor(TypeNameTypeNode node)
            {
                var knownType = Type.GetType(node.TypeName.Name, false);
                if (knownType != null) return knownType;
                var asQualifiedSystemType = String.Format("System.{0}", node.TypeName.Name);
                var knownSystemType = Type.GetType(asQualifiedSystemType, false);
                return knownSystemType;
            }

        }
    }
}