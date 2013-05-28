using System.Linq;

namespace Diesel.Transformations
{
    public static class ModelTransformations
    {
        public static AbstractSyntaxTree Transform(AbstractSyntaxTree ast)
        {
            var transformations = new[] {new ApplyDefaults()};
            return transformations.Aggregate(ast, (a, t) => t.Transform(a));
        }         
    }
}