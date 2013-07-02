using System.CodeDom;
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
            get { return new ConventionsDeclaration(new DomainEventConventions(new TypeName[] { })); }
        }

        /// <summary>
        /// Compile the model into a CodeDom <see cref="CodeCompileUnit"/>.
        /// </summary>
        public static CodeCompileUnit Compile(SemanticModel model)
        {
            var unit = new CodeCompileUnit();
            var conventions = DefaultConventions;
            Add(unit, conventions, model.AbstractSyntaxTree);
            return unit;
        }

        private static void Add(CodeCompileUnit codeCompileUnit, ConventionsDeclaration conventions, AbstractSyntaxTree ast)
        {
            var userConventions = conventions;
            if (ast.Conventions != null)
            {
                userConventions = conventions.ApplyOverridesFrom(ast.Conventions);
            }
            foreach (var ns in ast.Namespaces)
            {
                Add(codeCompileUnit, userConventions, ns);
            }
        }


        private static void Add(CodeCompileUnit codeCompileUnit, ConventionsDeclaration conventions, Namespace declaration)
        {
            var ns = new CodeNamespace(declaration.Name.Name);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            codeCompileUnit.Namespaces.Add(ns);
            foreach (var typeDeclaration in declaration.Declarations)
            {
                Add(ns, conventions, (dynamic)typeDeclaration);
            }
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, CommandDeclaration declaration)
        {
            ns.Types.Add(CommandGenerator.CreateCommandDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, DomainEventDeclaration declaration)
        {
            ns.Types.Add(DomainEventGenerator.CreateDomainEventDeclaration(declaration, conventions.DomainEventConventions));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, ValueTypeDeclaration declaration)
        {
            ns.Types.Add(ValueTypeGenerator.CreateValueTypeDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, DtoDeclaration declaration)
        {
            ns.Types.Add(DtoGenerator.CreateCommandDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, EnumDeclaration declaration)
        {
            ns.Types.Add(EnumGenerator.CreateEnumDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ConventionsDeclaration conventions, ApplicationServiceDeclaration declaration)
        {
            ns.Types.Add(ApplicationServiceGenerator.CreateApplicationServiceInterface(declaration));
            foreach (var command in declaration.Commands)
            {
                Add(ns, conventions, command);
            }
        }
 
    }
}