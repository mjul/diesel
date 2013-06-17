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
    }
}