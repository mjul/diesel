using System.Linq;
using Diesel.Parsing;

namespace Diesel.Transformations
{
    public static class ModelTransformations
    {
        public static SemanticModel Transform(AbstractSyntaxTree ast)
        {
            var transformations = new[] {new ApplyDefaults()};
            var transformedAst = transformations.Aggregate(ast, (a, t) => t.Transform(a));
            return new SemanticModel(transformedAst);
        }         
    }
}