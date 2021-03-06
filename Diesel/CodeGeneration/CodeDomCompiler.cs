﻿using System.CodeDom;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    /// <summary>
    /// Responsible for compiling a <see cref="SemanticModel"/> instance to CodeDom.
    /// </summary>
    public class CodeDomCompiler
    {
        private static ConventionsDeclaration DefaultConventions
        {
            get
            {
                return new ConventionsDeclaration(
                    new DomainEventConventions(new BaseTypes(new TypeName[] {})),
                    new CommandConventions(new BaseTypes(new TypeName[] {})));
            }
        }

        /// <summary>
        /// Compile the model into a CodeDom <see cref="CodeCompileUnit"/>.
        /// </summary>
        public static CodeCompileUnit Compile(SemanticModel model)
        {
            var unit = new CodeCompileUnit();
            var conventions = DefaultConventions;
            Add(unit, conventions, model);
            return unit;
        }

        private static void Add(CodeCompileUnit codeCompileUnit, ConventionsDeclaration conventions, SemanticModel model)
        {
            Add(codeCompileUnit, conventions, model, model.AbstractSyntaxTree);
        }

        private static void Add(CodeCompileUnit codeCompileUnit, ConventionsDeclaration conventions, SemanticModel model, AbstractSyntaxTree ast)
        {
            var userConventions = conventions;
            if (ast.Conventions != null)
            {
                userConventions = conventions.ApplyOverridesFrom(ast.Conventions);
            }
            foreach (var ns in ast.Namespaces)
            {
                Add(codeCompileUnit, userConventions, model, ns);
            }
        }


        private static void Add(CodeCompileUnit codeCompileUnit, ConventionsDeclaration conventions, SemanticModel model, Namespace declaration)
        {
            var ns = new CodeNamespace(declaration.Name.Name);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            codeCompileUnit.Namespaces.Add(ns);
            foreach (var typeDeclaration in declaration.Declarations)
            {
                Add(ns, conventions, model, declaration.Name, (dynamic)typeDeclaration);
            }
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, SemanticModel model, NamespaceName namespaceName, CommandDeclaration declaration)
        {
            ns.Types.Add(CommandGenerator.CreateCommandDeclaration(model, namespaceName, declaration, conventions.CommandConventions));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, SemanticModel model, NamespaceName namespaceName, DomainEventDeclaration declaration)
        {
            ns.Types.Add(DomainEventGenerator.CreateDomainEventDeclaration(model, namespaceName, declaration, conventions.DomainEventConventions));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, SemanticModel model, NamespaceName namespaceName, ValueTypeDeclaration declaration)
        {
            ns.Types.Add(ValueTypeGenerator.CreateValueTypeDeclaration(model, namespaceName, declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, SemanticModel model, NamespaceName namespaceName, DtoDeclaration declaration)
        {
            ns.Types.Add(DtoGenerator.CreateDtoDeclaration(model, namespaceName, declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, SemanticModel model, NamespaceName namespaceName, EnumDeclaration declaration)
        {
            ns.Types.Add(EnumGenerator.CreateEnumDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, SemanticModel model, NamespaceName namespaceName, ApplicationServiceDeclaration declaration)
        {
            ns.Types.Add(ApplicationServiceGenerator.CreateApplicationServiceInterface(declaration));
            foreach (var command in declaration.Commands)
            {
                Add(ns, conventions, model, namespaceName, command);
            }
        }
 
    }
}