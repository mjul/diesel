using System;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Test.Diesel.ObjectMothers
{
    /// <summary>
    /// Responsible for creating semantically valid <see cref="CommandDeclaration"/> instances for testing.
    /// </summary>
    public class CommandDeclarationObjectMother
    {
        public static CommandDeclaration CreateImportEmployee()
        {
            return new CommandDeclaration("ImportEmployee", new[]
                {
                    new PropertyDeclaration("EmployeeNumber", new SimpleType(typeof (Int32))),
                    new PropertyDeclaration("FirstName", new StringReferenceType()),
                    new PropertyDeclaration("LastName", new StringReferenceType())
                });
        }
    }
}