using System.Collections.Generic;

namespace Diesel
{
    public class CommandDeclaration
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