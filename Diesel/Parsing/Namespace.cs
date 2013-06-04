using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class Namespace : ITreeNode
    {
        public NamespaceIdentifier Name { get; private set; }
        public IEnumerable<TypeDeclaration> Declarations;

        public Namespace(NamespaceIdentifier name, IEnumerable<TypeDeclaration> declarations)
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