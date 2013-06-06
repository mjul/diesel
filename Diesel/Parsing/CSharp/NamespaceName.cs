namespace Diesel.Parsing.CSharp
{
    public class NamespaceName : Terminal
    {
        public string Name { get; set; }
        public NamespaceName(string name)
        {
            Name = name;
        }
    }
}