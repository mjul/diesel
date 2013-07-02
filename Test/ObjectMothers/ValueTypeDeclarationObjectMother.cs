using System;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Test.Diesel.ObjectMothers
{
    /// <summary>
    /// Responsible for creating semantically valid <see cref="ValueTypeDeclaration"/> instances for testing.
    /// </summary>
    public static class ValueTypeDeclarationObjectMother
    {
        public static ValueTypeDeclaration CreateEmployeeNumber()
        {
            return new ValueTypeDeclaration(
                "EmployeeNumber",
                new[]
                    {
                        new PropertyDeclaration("Value", new SimpleType(typeof (Int32))),
                    });
        }
    }
}