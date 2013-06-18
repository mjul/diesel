using System;
using System.Linq;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public class SystemTypeMapper
    {
         public System.Type SystemTypeFor(TypeNode node)
         {
             var instance = new SystemTypeMapper();
             return instance.MapToSystemType((dynamic)node);
         }

        private Type MapToSystemType(SimpleType node)
        {
            return node.Type;
        }

        private Type MapToSystemType(StringReferenceType node)
        {
            return typeof (string);
        }

        private Type MapToSystemType(NullableType node)
        {
            Type underlying = MapToSystemType((dynamic) node.Underlying);
            return Type.GetType(String.Format("System.Nullable`1[{0}]", underlying.FullName));
        }


        private Type MapToSystemType(ArrayType node)
        {
            Type underlying = MapToSystemType((dynamic)node.Type);
            var rankSpecifiers = (from rank in node.RankSpecifiers.Ranks.Reverse()
                                  from rankSpec in String.Format("[{0}]",
                                                                 String.Join("", Enumerable.Repeat(',', rank.Dimensions - 1)))
                                  select rankSpec);
            return Type.GetType(String.Format("{0}{1}", 
                                    underlying.FullName, 
                                    String.Concat(rankSpecifiers)));
        }


        // TODO: we should probably not support these, rather fix the code gen to emit the name directly
        private Type MapToSystemType(TypeNameTypeNode node)
        {
            var knownType = Type.GetType(node.TypeName.Name, false);
            if (knownType != null) return knownType;
            var asQualifiedSystemType = String.Format("System.{0}", node.TypeName.Name);
            var knownSystemType = Type.GetType(asQualifiedSystemType, false);
            if (knownSystemType != null) return knownSystemType;
            throw new ArgumentOutOfRangeException("node", node.TypeName.Name, "Unknown type name.");
        }
    }
}