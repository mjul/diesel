using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Test.Diesel.ObjectMothers
{
    public static class BaseTypesObjectMother
    {
         public static BaseTypes CreateDieselTestingCommand()
         {
             return new BaseTypes(new[] {new TypeName("Diesel.Testing.Command") });
         }
    }
}