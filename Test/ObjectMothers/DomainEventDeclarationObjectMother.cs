using System;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Test.Diesel.ObjectMothers
{
    /// <summary>
    /// Responsible for creating semantically valid <see cref="DomainEventDeclaration"/> instances for testing.
    /// </summary>
    public static class DomainEventDeclarationObjectMother
    {
         public static DomainEventDeclaration CreateEmployeeImported()
         {
             return new DomainEventDeclaration(
                 "EmployeeImported",
                 new[]
                     {
                         new PropertyDeclaration("Id",
                                                 new TypeNameTypeNode(
                                                     new TypeName("System.Guid"))),
                         new PropertyDeclaration("EmployeeNumber",
                                                 new SimpleType(typeof (Int32))),
                         new PropertyDeclaration("FirstName", new StringReferenceType()),
                         new PropertyDeclaration("LastName", new StringReferenceType())
                     });
         }
    }
}