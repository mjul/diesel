using System;
using System.Collections.Generic;
using Diesel.CodeGeneration;
using Diesel.Parsing;

namespace Diesel.Transformations
{
    /// <summary>
    /// Get all declared types from the <see cref="AbstractSyntaxTree"/>.
    /// </summary>
    public class KnownTypesHarvester : IDieselExpressionVisitor
    {
        private readonly List<KnownType> _knownTypes = new List<KnownType>();
        private readonly Stack<Namespace> _namespaceStack = new Stack<Namespace>();
        
        /// <summary>
        /// Get the known types from the <see cref="AbstractSyntaxTree"/>.
        /// </summary>
        /// <param name="abstractSyntaxTree"></param>
        /// <returns></returns>
        public static List<KnownType> GetKnownTypes(AbstractSyntaxTree abstractSyntaxTree)
        {
            var harvester = new KnownTypesHarvester();
            harvester.Visit(abstractSyntaxTree);
            return harvester._knownTypes;
        }

        public void Visit(AbstractSyntaxTree node)
        {
            Visit(node.Conventions);
            foreach (var ns in node.Namespaces)
            {
                _namespaceStack.Push(ns);
                ns.Accept(this);
                _namespaceStack.Pop();
            }
        }

        public void Visit(Namespace node)
        {
            foreach (var declaration in node.Declarations)
            {
                declaration.Accept(this);
            }
        }

        public void Visit(ConventionsDeclaration node)
        {
        }

        public void Visit(ApplicationServiceDeclaration node)
        {
            var fullName = FullName(node.Name);
            _knownTypes.Add(new KnownType(fullName, false));
        }

        private string FullName(string name)
        {
            String result;
            var isQualifiedName = name.Contains(".");
            if (isQualifiedName)
            {
                result = name;
            }
            else
            {
                var ns = _namespaceStack.Peek();
                var fullName = String.Format("{0}.{1}", ns.Name.Name, name);
                result = fullName;
            }
            return result;
        }

        public void Visit(CommandDeclaration node)
        {
            var fullName = FullName(node.Name);
            _knownTypes.Add(new KnownType(fullName, false));
        }

        public void Visit(DtoDeclaration node)
        {
            var fullName = FullName(node.Name);
            _knownTypes.Add(new KnownType(fullName, false));
        }

        public void Visit(DomainEventDeclaration node)
        {
            var fullName = FullName(node.Name);
            _knownTypes.Add(new KnownType(fullName, false));
        }

        public void Visit(EnumDeclaration node)
        {
            var fullName = FullName(node.Name);
            _knownTypes.Add(new KnownType(fullName, true));
        }

        public void Visit(ValueTypeDeclaration node)
        {
            var fullName = FullName(node.Name);
            _knownTypes.Add(new KnownType(fullName, true));
        }
    }
}