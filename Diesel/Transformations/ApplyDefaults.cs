using System;
using System.Linq;

namespace Diesel.Transformations
{
    public class ApplyDefaults : ModelTransformation
    {
        private static readonly Type DefaultPropertyType = typeof (Int32);
        private const string DefaultPropertyName = "Value";

        public override ValueTypeDeclaration Transform(ValueTypeDeclaration valueTypeDeclaration)
        {
            var result = base.Transform(valueTypeDeclaration);
            var isSimpleDeclaration = (valueTypeDeclaration.Properties.Count() == 1);
            if (isSimpleDeclaration)
            {
                result = result
                    .ReplaceProperties(p => (null == p.Type) ? p.OverrideType(DefaultPropertyType) : p)
                    .ReplaceProperties(p => (null == p.Name) ? p.OverrideName(DefaultPropertyName) : p);
            }
            return result;
        }
    }
}