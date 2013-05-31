using System.Collections.Generic;

namespace Diesel.Parsing
{
    public class ApplicationServiceDeclaration : TypeDeclaration
    {
        public IEnumerable<CommandDeclaration> Commands { get; private set; }

        public ApplicationServiceDeclaration(string name, IEnumerable<CommandDeclaration> commands)
            : base(name)
        {
            Commands = commands;
        }

        public override IEnumerable<TreeNode> Children
        {
            get { return Commands; }
        }
    }

    public abstract class TypeDeclaration : TreeNode, ITypeDeclaration
    {
        public string Name { get; private set; }
        protected TypeDeclaration(string name)
        {
            Name = name;
        }
    }
}