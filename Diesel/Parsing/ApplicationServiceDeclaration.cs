using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class ApplicationServiceDeclaration : ITypeDeclaration
    {
        public string Name { get; private set; }
        public IEnumerable<CommandDeclaration> Commands { get; private set; }

        public ApplicationServiceDeclaration(string name, IEnumerable<CommandDeclaration> commands)
        {
            Name = name;
            Commands = commands;
        }
    }
}