using System.Collections.Generic;

namespace Diesel
{
    public class Namespace
    {
        public string Name { get; private set; }
        public IEnumerable<ValueTypeDeclaration> Declarations;

        public Namespace(string name, IEnumerable<ValueTypeDeclaration> declarations)
        {
            Name = name;
            Declarations = declarations;
        }
    }
}