using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public static class TypeNameMapper
    {
        [Pure]
        public static string TypeNameForArray(string elementTypeFullName, RankSpecifiers ranks)
        {
            var rankSpecifiers = (from rank in ranks.Ranks.Reverse()
                                  from rankSpec in
                                      String.Format("[{0}]",
                                                    String.Join("", Enumerable.Repeat(',', rank.Dimensions - 1)))
                                  select rankSpec);
            return String.Concat(elementTypeFullName, String.Concat(rankSpecifiers));
        }
    }
}