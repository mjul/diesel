using Diesel.Parsing;

namespace Test.Diesel.ObjectMothers
{
    /// <summary>
    /// Responsible for creating semantically valid <see cref="EnumDeclaration"/> instances for testing.
    /// </summary>
    public static class EnumDeclarationObjectMother
    {
        public static EnumDeclaration CreateRoles()
        {
            return new EnumDeclaration("Roles", new[] {"Tester", "Developer"});
        }
    }
}