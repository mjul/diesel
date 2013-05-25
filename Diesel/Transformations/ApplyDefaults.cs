using System;

namespace Diesel.Transformations
{
    public class ApplyDefaults : ModelTransformation
    {
        public override ValueTypeDeclaration Transform(ValueTypeDeclaration valueTypeDeclaration)
        {
            var result = base.Transform(valueTypeDeclaration);
            bool addDefaultType = (null == result.ValueType);
            return addDefaultType ? result.OverrideValueType(typeof(Int32)) : result;
        }
    }
}