using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class CommandDeclaration : ITypeDeclaration
    {
        public string Name { get; private set; }
        public IEnumerable<PropertyDeclaration> Properties { get; private set; }

        public CommandDeclaration(string name, IEnumerable<PropertyDeclaration> properties)
        {
            Name = name;
            Properties = properties;
        }
    }
}