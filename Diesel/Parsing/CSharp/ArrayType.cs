using System;
using System.Collections.Generic;
using System.Linq;

namespace Diesel.Parsing.CSharp
{
    public class ArrayType : Terminal
    {
        public string Name { get; private set; }

        public ArrayType(TypeName typeName, IEnumerable<int> dimensionalities)
        {
            Name = String.Format("{0}{1}",
                                 typeName.Name,
                                 String.Join("", dimensionalities.Select(DimSeparators)));
        }

        private static string DimSeparators(int i)
        {
            return String.Format("[{0}]", String.Join("", Enumerable.Repeat(",", i-1)));
        }
    }
}