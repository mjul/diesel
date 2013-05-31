using System;

namespace Diesel.Parsing
{
    public class PropertyDeclaration
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public PropertyDeclaration(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public PropertyDeclaration OverrideType(Type type)
        {
            return new PropertyDeclaration(this.Name, type);
        }

        public PropertyDeclaration OverrideName(string name)
        {
            return new PropertyDeclaration(name, this.Type);
        }
    }
}