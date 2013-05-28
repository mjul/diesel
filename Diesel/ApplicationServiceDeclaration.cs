using System;
using System.Collections.Generic;

namespace Diesel
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