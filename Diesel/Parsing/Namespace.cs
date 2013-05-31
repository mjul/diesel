using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class Namespace : ITreeNode
    {
        public string Name { get; private set; }
        public IEnumerable<TypeDeclaration> Declarations;

        public Namespace(string name, IEnumerable<TypeDeclaration> declarations)
        {
            Name = name;
            Declarations = declarations;
        }

        public IEnumerable<ITreeNode> Children
        {
            get { return Declarations; }
        }
    }
}