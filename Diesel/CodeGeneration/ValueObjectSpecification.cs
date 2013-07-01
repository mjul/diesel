using System.Diagnostics.Contracts;
using Diesel.Parsing;

namespace Diesel.CodeGeneration
{
    public class ValueObjectSpecification
    {
        public bool IsValueType { get; private set; }
        public string Name { get; private set; }
        public PropertyDeclaration[] Properties { get; private set; }
        public bool IsDataContract { get; private set; }
        public bool IsSealed { get; private set; }

        private ValueObjectSpecification(bool isValueType, string name, PropertyDeclaration[] properties, bool isDataContract, bool isSealed)
        {
            IsValueType = isValueType;
            Name = name;
            Properties = properties;
            IsDataContract = isDataContract;
            IsSealed = isSealed;
        }

        [Pure]
        public static ValueObjectSpecification CreateStruct(
            string name, PropertyDeclaration[] properties,
            bool isDataContract)
        {
            return new ValueObjectSpecification(true, name, properties, isDataContract, true);
        }

        [Pure]
        public static ValueObjectSpecification CreateClass(
            string name, PropertyDeclaration[] properties, 
            bool isDataContract, bool isSealed)
        {
            return new ValueObjectSpecification(false, name, properties, isDataContract, isSealed);
        }

    }
}