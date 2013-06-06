namespace Diesel.Parsing.CSharp
{
    public class Identifier : Terminal
    {
        public string Name { get; private set; }
        public Identifier(string name)
        {
            Name = name;
        }
    }
}