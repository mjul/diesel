using System;
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

        private Type MapToSystemType(TypeNameTypeNode node)
        {
            var typeName = node.TypeName;
            if (typeName.Name == "Guid") return typeof (Guid);
            if (typeName.Name == "DateTime") return typeof (DateTime);
            var knownType = Type.GetType(node.TypeName.Name, false);
            if (knownType != null) return knownType;
            throw new ArgumentOutOfRangeException("node", node.TypeName.Name, "Unknown type name.");
        }

        // TODO: add all TypeNode subtypes

    }
}