using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class ValueTypeGenerator : CodeDomGenerator
    {
        public static CodeTypeDeclaration CreateValueTypeDeclaration(ValueTypeDeclaration declaration)
        {
            var result = CreateTypeWithValueSemantics(
                ValueObjectSpecification.CreateStruct(declaration.Name, declaration.Properties.ToArray(), false));
            if (declaration.Properties.Count() == 1)
            {
                var displayTemplate = String.Format("{{{0}}}", declaration.Properties.Single().Name);
                result.CustomAttributes.Add(CreateDebuggerDisplayAttribute(displayTemplate));
            }
            return result;
        }
    }
}
