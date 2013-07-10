using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Test.Diesel.ObjectMothers
{
    public static class PropertyDeclarationObjectMother
    {
        public static PropertyDeclaration[] FirstLastStringPropertyDeclarations()
        {
            return new[]
                    {
                        new PropertyDeclaration("First", new StringReferenceType()),
                        new PropertyDeclaration("Last", new StringReferenceType()),
                    };
        }
    }
}