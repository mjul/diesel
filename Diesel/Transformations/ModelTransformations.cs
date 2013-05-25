namespace Diesel.Transformations
{
    public static class ModelTransformations
    {
        public static AbstractSyntaxTree Transform(AbstractSyntaxTree ast)
        {
            return (new ApplyDefaults()).Transform(ast);
        }         
    }
}