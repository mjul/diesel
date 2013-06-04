namespace Diesel.Parsing
{
    public class TypeName : Terminal
    {
        public string Name { get; private set; }
        public TypeName(string name)
        {
            Name = name;
        }
    }
}