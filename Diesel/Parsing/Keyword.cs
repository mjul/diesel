namespace Diesel.Parsing
{
    public class Keyword : Terminal
    {
        public string Name { get; private set; }
        public Keyword(string name)
        {
            Name = name;
        }
    }
}