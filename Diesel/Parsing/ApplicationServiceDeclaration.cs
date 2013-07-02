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

        public override IEnumerable<ITreeNode> Children
        {
            get { return Commands; }
        }

        public override void Accept(ITypeDeclarationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public abstract class TypeDeclaration : ITypeDeclaration
    {
        public string Name { get; private set; }
        protected TypeDeclaration(string name)
        {
            Name = name;
        }
        public abstract IEnumerable<ITreeNode> Children { get; }
        
        public void Accept(IDieselExpressionVisitor visitor)
        {
            Accept((ITypeDeclarationVisitor) visitor);
        }

        public abstract void Accept(ITypeDeclarationVisitor visitor);
    }
}