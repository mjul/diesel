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
            const bool isValueType = true;
            var result = CreateTypeWithValueSemantics(new ValueObjectSpecification(isValueType, declaration.Name, declaration.Properties.ToArray(), false, isValueType));
            if (declaration.Properties.Count() == 1)
            {
                var displayTemplate = String.Format("{{{0}}}", declaration.Properties.Single().Name);
                result.CustomAttributes.Add(CreateDebuggerDisplayAttribute(displayTemplate));
            }
            return result;
        }
    }
}
