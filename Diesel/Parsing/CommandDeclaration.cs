using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class CommandDeclaration : TypeDeclaration
    {
        public IEnumerable<PropertyDeclaration> Properties { get; private set; }

        public CommandDeclaration(string name, IEnumerable<PropertyDeclaration> properties) 
            : base(name)
        {
            Properties = properties;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { return Properties; }
        }
    }
}