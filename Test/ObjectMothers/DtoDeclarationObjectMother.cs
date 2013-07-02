using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Test.Diesel.ObjectMothers
{
    /// <summary>
    /// Responsible for creating semantically valid <see cref="DtoDeclaration"/> instances for testing.
    /// </summary>
    public static class DtoDeclarationObjectMother
    {
         public static DtoDeclaration CreateName()
         {
             return new DtoDeclaration("Name", 
                 new[]
                     {
                         new PropertyDeclaration("First", new StringReferenceType()), 
                         new PropertyDeclaration("Last", new StringReferenceType()), 
                     }
                 );
         }
    }
}