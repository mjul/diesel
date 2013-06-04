namespace Diesel.Parsing
{
    public class NamespaceIdentifier : Terminal
    {
        public string Name { get; set; }
        public NamespaceIdentifier(string name)
        {
            Name = name;
        }
    }
}