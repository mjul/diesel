using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class Symbol : ITreeNode
    {
        public string Name { get; set; }
        public Symbol(string name)
        {
            Name = name;
        }
        public IEnumerable<ITreeNode> Children
        {
            get { yield break; }
        }
    }
}