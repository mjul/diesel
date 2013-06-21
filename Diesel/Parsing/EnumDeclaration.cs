using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class EnumDeclaration : TypeDeclaration
    {
        public IEnumerable<string> Values { get; private set; }

        public EnumDeclaration(string name, IEnumerable<string> values)
            :base(name)
        {
            Values = values;
        }

        public override IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }
    }
}