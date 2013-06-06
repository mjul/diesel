using System.Collections.Generic;
using Diesel.Parsing.CSharp;

namespace Diesel.Parsing
{
    public class Namespace : ITreeNode
    {
        public NamespaceName Name { get; private set; }
        public IEnumerable<TypeDeclaration> Declarations;

        public Namespace(NamespaceName name, IEnumerable<TypeDeclaration> declarations)
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