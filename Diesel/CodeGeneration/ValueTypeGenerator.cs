using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;
using Diesel.Transformations;

namespace Diesel.CodeGeneration
{
    public class ValueTypeGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateValueTypeDeclaration(
            SemanticModel model, NamespaceName namespaceName,
            ValueTypeDeclaration declaration)
        {
            var result = CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateStruct(
                    namespaceName, declaration.Name,
                    declaration.Properties.ToArray(),
                    new BaseTypes(new TypeName[0]), 
                    false),
                model.KnownTypes);
            if (declaration.Properties.Count() == 1)
            {
                AddDebuggerDisplayAttribute(declaration, result);
                AddToString(declaration, result);
            }
            return result;
        }

        private static void AddToString(ValueTypeDeclaration declaration, CodeTypeDeclaration result)
        {
            result.Members.Add(CreateToString(declaration.Properties));
        }

        private static void AddDebuggerDisplayAttribute(ValueTypeDeclaration declaration, CodeTypeDeclaration result)
        {
            var displayTemplate = String.Format("{{{0}}}", declaration.Properties.Single().Name);
            result.CustomAttributes.Add(CreateDebuggerDisplayAttribute(displayTemplate));
        }
    }
}
