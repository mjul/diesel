using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class DtoDeclaration : TypeDeclaration
    {
        public IEnumerable<PropertyDeclaration> Properties { get; private set; }

        public DtoDeclaration(string name, IEnumerable<PropertyDeclaration> properties) 
            : base(name)
        {
            Properties = properties;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { return Properties; }
        }

        public override void Accept(ITypeDeclarationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}