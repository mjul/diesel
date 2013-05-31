using System.Collections.Generic;

namespace Diesel
{
    public class Namespace
    {
        public string Name { get; private set; }
        public IEnumerable<ITypeDeclaration> Declarations;

        public Namespace(string name, IEnumerable<ITypeDeclaration> declarations)
        {
            Name = name;
            Declarations = declarations;
        }
    }
}