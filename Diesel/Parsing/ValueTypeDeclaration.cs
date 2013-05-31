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

        public override IEnumerable<TreeNode> Children
        {
            get { return Properties; }
        }

        [Obsolete("Move to semantic model")]
        public ValueTypeDeclaration ReplaceProperties(Func<PropertyDeclaration, PropertyDeclaration> function)
        {
            return new ValueTypeDeclaration(Name, Properties.Select(function));
        }
    }
}