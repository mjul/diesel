using System;
using System.Collections.Generic;
using System.Linq;

namespace Diesel.Parsing
{
    public class ValueTypeDeclaration : TypeDeclaration
    {
        public ValueTypeDeclaration(string name, IEnumerable<PropertyDeclaration> propertyDeclarations)
            : base(name)
        {
            Properties = propertyDeclarations;
        }

        public IEnumerable<PropertyDeclaration> Properties { get; private set; }

        public override IEnumerable<ITreeNode> Children
        {
            get { return Properties; }
        }

        public override void Accept(ITypeDeclarationVisitor visitor)
        {
            visitor.Visit(this);
        }

        [Obsolete("Move to semantic model")]
        public ValueTypeDeclaration ReplaceProperties(Func<PropertyDeclaration, PropertyDeclaration> function)
        {
            return new ValueTypeDeclaration(Name, Properties.Select(function));
        }
    }
}