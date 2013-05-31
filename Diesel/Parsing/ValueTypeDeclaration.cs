using System;
using System.Collections.Generic;
using System.Linq;

namespace Diesel.Parsing
{
    public class ValueTypeDeclaration : ITypeDeclaration
    {
        public ValueTypeDeclaration(string name, IEnumerable<PropertyDeclaration> propertyDeclarations)
        {
            Name = name;
            Properties = propertyDeclarations;
        }

        public String Name { get; private set; }
        public IEnumerable<PropertyDeclaration> Properties { get; private set; }

        public ValueTypeDeclaration ReplaceProperties(Func<PropertyDeclaration, PropertyDeclaration> function)
        {
            return new ValueTypeDeclaration(Name, Properties.Select(function));
        }
    }
}