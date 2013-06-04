using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class Symbol : Terminal
    {
        public string Name { get; set; }
        public Symbol(string name)
        {
            Name = name;
        }
    }
}