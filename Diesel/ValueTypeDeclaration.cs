using System;

namespace Diesel
{
    public class ValueTypeDeclaration 
    {
        public ValueTypeDeclaration(string name, Type type)
        {
            Name = name;
            ValueType = type;
        }

        public String Name { get; private set; }
        public Type ValueType { get; private set; }
    }
}