using System;
using Diesel.Parsing.CSharp;

namespace Diesel.Transformations
{
    internal class FullyQualifiedNameRule
    {
        public static string For(NamespaceName namespaceName, TypeName typeName)
        {
            return For(namespaceName, typeName.Name);
        }

        public static string For(NamespaceName namespaceName, String typeName)
        {
            var isQualified = typeName.Contains(".");
            return isQualified
                       ? typeName
                       : String.Format("{0}.{1}", namespaceName.Name, typeName);
        }
    }
}