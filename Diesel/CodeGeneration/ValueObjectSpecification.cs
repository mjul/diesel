﻿using System.Diagnostics.Contracts;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public class ValueObjectSpecification
    {
        public bool IsValueType { get; private set; }
        public string Name { get; private set; }
        public NamespaceName Namespace { get; private set; }
        public PropertyDeclaration[] Properties { get; private set; }
        public BaseTypes BaseTypes { get; private set; }
        public bool IsDataContract { get; private set; }
        public bool IsSealed { get; private set; }

        private ValueObjectSpecification(bool isValueType, 
            NamespaceName namespaceName, string name, 
            PropertyDeclaration[] properties, 
            BaseTypes baseTypes,
            bool isDataContract, bool isSealed)
        {
            IsValueType = isValueType;
            Namespace = namespaceName;
            Name = name;
            Properties = properties;
            BaseTypes = baseTypes;
            IsDataContract = isDataContract;
            IsSealed = isSealed;
        }

        [Pure]
        public static ValueObjectSpecification CreateStruct(
            NamespaceName namespaceName, string name, 
            PropertyDeclaration[] properties,
            BaseTypes baseTypes,
            bool isDataContract)
        {
            return new ValueObjectSpecification(true, namespaceName, name, properties, baseTypes, isDataContract, true);
        }

        [Pure]
        public static ValueObjectSpecification CreateClass(
            NamespaceName namespaceName, string name,
            PropertyDeclaration[] properties, 
            BaseTypes baseTypes,
            bool isDataContract, bool isSealed)
        {
            return new ValueObjectSpecification(false, namespaceName, name, properties, baseTypes, isDataContract, isSealed);
        }

    }
}