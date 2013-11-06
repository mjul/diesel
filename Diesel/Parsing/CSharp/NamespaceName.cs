using System.Diagnostics;

namespace Diesel.Parsing.CSharp
{
    [DebuggerDisplay("{Name}")]
    public class NamespaceName : Terminal
    {
        public string Name { get; private set; }
        public NamespaceName(string name)
        {
            Name = name;
        }
    }
}