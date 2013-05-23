using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Diesel
{
    public class Compiler
    {
        public static CodeCompileUnit Compile(ValueTypeDeclaration declaration)
        {
            var unit = new CodeCompileUnit();
            var ns = new CodeNamespace("Generated");
            unit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            AddValueType(ns, declaration);
            return unit;
        }

        public static CodeCompileUnit Compile(CommandDeclaration declaration)
        {
            var unit = new CodeCompileUnit();
            var ns = new CodeNamespace("Generated");
            unit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            AddCommand(ns, declaration);
            return unit;
        }

        public static CodeCompileUnit Compile(Namespace declaration)
        {
            var unit = new CodeCompileUnit();
            var ns = new CodeNamespace(declaration.Name);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            unit.Namespaces.Add(ns);
            return unit;
        }


        private static void AddCommand(CodeNamespace ns, CommandDeclaration declaration)
        {
            ns.Types.Add(CreateCommandDeclaration(declaration));
        }

        private static CodeTypeDeclaration CreateCommandDeclaration(CommandDeclaration declaration)
        {
            const bool isValueType = false;
            var result = new CodeTypeDeclaration(declaration.Name) { IsStruct = isValueType, IsPartial = true, IsClass = !isValueType };
            result.BaseTypes.AddRange(CreateImplementsIEquatableOf(declaration.Name));
            result.Members.AddRange(CreateConstructorAssigningBackingFieldsFor(declaration.Properties));
            result.Members.AddRange(CreateReadOnlyProperties(declaration.Properties));
            result.Members.AddRange(CreateEqualityOperatorOverloading(declaration.Name, isValueType));
            result.Members.AddRange(CreateEqualsOverloadingUsingEqualityOperator(declaration.Name, isValueType, declaration.Properties));
            result.Members.AddRange(CreateGetHashCode(declaration.Properties));
            return result;
        }


        private static void AddValueType(CodeNamespace ns, ValueTypeDeclaration declaration)
        {
            ns.Types.Add(CreateValueTypeDeclaration(declaration));
        }

        private static CodeTypeDeclaration CreateValueTypeDeclaration(ValueTypeDeclaration declaration)
        {
            const string valuePropertyName = "Value";
            var valueProperty = new PropertyDeclaration(valuePropertyName, declaration.ValueType);
            var properties = new[] { valueProperty };
            const bool isValueType = true;

            var result = new CodeTypeDeclaration(declaration.Name) { IsStruct = isValueType, IsPartial = true, IsClass = !isValueType};
            result.CustomAttributes.Add(CreateDebuggerDisplayAttribute(String.Format("{{{0}}}", valuePropertyName)));
            result.BaseTypes.AddRange(CreateImplementsIEquatableOf(declaration.Name));
            result.Members.AddRange(CreateConstructorAssigningBackingFieldsFor(new[] {valueProperty}));
            result.Members.AddRange(CreateReadOnlyProperties(properties));
            result.Members.AddRange(CreateEqualityOperatorOverloading(declaration.Name, isValueType));
            result.Members.AddRange(CreateGetHashCode(properties));
            result.Members.AddRange(CreateEqualsOverloadingUsingEqualityOperator(declaration.Name, isValueType, properties));
            return result;
        }

        private static CodeTypeMember[] CreateConstructorAssigningBackingFieldsFor(
            IEnumerable<PropertyDeclaration> properties)
        {
            var constructor = new CodeConstructor()
                {
                    Attributes = MemberAttributes.Public,
                };
            foreach (var property in properties)
            {
                var backingField = BackingFieldName(property.Name);
                var parameterName = CamelCase(property.Name);
                constructor.Parameters.Add(new CodeParameterDeclarationExpression(property.Type, parameterName));
                constructor.Statements.Add(CreateFieldAssignment(backingField, parameterName));
            }
            return new CodeTypeMember[] {constructor};
        }


        private static CodeAssignStatement CreateFieldAssignment(string fieldName, string variableWithValue)
        {
            return new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),
                new CodeVariableReferenceExpression(variableWithValue));
        }

        private static CodeTypeReference[] CreateImplementsIEquatableOf(string type)
        {
            return new[] { new CodeTypeReference("System.IEquatable", new CodeTypeReference(type)) };
        }


        private static CodeTypeMember[] CreateEqualsOverloadingUsingEqualityOperator(string typeName, bool isValueType, IEnumerable<PropertyDeclaration> properties)
        {
            // if (ReferenceEquals(null, obj)) return false;
            // return obj is EmployeeId && Equals((EmployeeId) obj);
            var ifReferenceEqualsNullObjReturnFalse =
                new CodeConditionStatement(
                    CreateObjectReferenceEqualsNullPredicateExpression(new CodeArgumentReferenceExpression("obj")),
                    new CodeStatement[]
                        {
                            new CodeMethodReturnStatement(new CodePrimitiveExpression(false))
                        },
                    new CodeStatement[] { }
                    );

            var returnObjIsTypeNameAndThisEqualsObj =
                new CodeMethodReturnStatement(
                    new CodeBinaryOperatorExpression(
                        CreateTypeIsAssignableFrom(typeName, new CodeArgumentReferenceExpression("obj")),
                        CodeBinaryOperatorType.BooleanAnd,
                        new CodeMethodInvokeExpression(
                            new CodeThisReferenceExpression(),
                            "Equals",
                            new CodeCastExpression(typeName, new CodeArgumentReferenceExpression("obj")))
                        ));

            var equalsObject = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Override,
                Name = "Equals",
                Parameters = { new CodeParameterDeclarationExpression(typeof(object), "obj") },
                ReturnType = new CodeTypeReference(typeof(bool)),
                Statements = 
                    { 
                        ifReferenceEqualsNullObjReturnFalse, 
                        returnObjIsTypeNameAndThisEqualsObj 
                    }
            };

            var compareExpressions = from property in properties
                                     select new CodeBinaryOperatorExpression(
                                         new CodePropertyReferenceExpression(
                                             new CodeThisReferenceExpression(), property.Name),
                                         CodeBinaryOperatorType.ValueEquality,
                                         new CodePropertyReferenceExpression(
                                             new CodeVariableReferenceExpression("other"), property.Name));

            var nullGuardSeed = isValueType
                                    ? (CodeExpression) new CodePrimitiveExpression(true)
                                    : new CodeBinaryOperatorExpression(
                                          new CodePrimitiveExpression(false),
                                          CodeBinaryOperatorType.ValueEquality,
                                          CreateObjectReferenceEqualsNullPredicateExpression(
                                              new CodeVariableReferenceExpression("other")));

            var comparerExpressionJoinedWithAnd = compareExpressions.Aggregate(
                nullGuardSeed,
                (CodeExpression a, CodeExpression b) =>
                    new CodeBinaryOperatorExpression(a, CodeBinaryOperatorType.BooleanAnd, b));

            var equalsTyped = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Equals",
                Parameters = { new CodeParameterDeclarationExpression(new CodeTypeReference(typeName), "other") },
                ReturnType = new CodeTypeReference(typeof(bool)),
                Statements =
                        {
                            // return this == other
                            new CodeMethodReturnStatement(comparerExpressionJoinedWithAnd)
                        }
            };

            return new CodeTypeMember[] { equalsTyped, equalsObject };
        }

        private static CodeMethodInvokeExpression CreateTypeIsAssignableFrom(string typeName, CodeExpression instanceExpression)
        {
            // typeof(typeName).IsAssignableFrom(instanceExpression.GetType())
            return new CodeMethodInvokeExpression(
                new CodeTypeOfExpression(typeName),
                "IsAssignableFrom",
                new CodeMethodInvokeExpression(
                    instanceExpression,
                    "GetType"));
        }

        private static CodeMethodInvokeExpression CreateObjectReferenceEqualsNullPredicateExpression(CodeExpression codeVariableReferenceExpression)
        {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(object)),
                "ReferenceEquals",
                new CodeExpression[]
                    {
                        new CodePrimitiveExpression(null),
                        codeVariableReferenceExpression
                    });
        }

        private static CodeAttributeDeclaration CreateDebuggerDisplayAttribute(string formatString)
        {
            return new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(System.Diagnostics.DebuggerDisplayAttribute)),
                new[] { new CodeAttributeArgument(new CodePrimitiveExpression(formatString)) });
        }


        private static CodeTypeMember[] CreateGetHashCode(IEnumerable<PropertyDeclaration> properties)
        {
            // Use value types for GetHashCode only to save writing null guards when accessing
            // Hash code just needs to be the same if the objects are Equal, 
            // not different if they are not Equal
            var hashCodeExpressions = properties
                .Where(p => p.Type.IsValueType)
                .Select(p =>
                    new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(),
                            p.Name),
                        "GetHashCode",
                        new CodeExpression[] {}));
            
            var hashCodeCalculationExpression =
                hashCodeExpressions.Aggregate(
                    new CodePrimitiveExpression(0),
                    (CodeExpression a, CodeExpression b) =>
                    new CodeBinaryOperatorExpression(a, CodeBinaryOperatorType.Add, b));

            return CreateGetHashCode(hashCodeCalculationExpression);
        }


        private static CodeTypeMember[] CreateGetHashCode(CodeExpression hashCodeCalculationExpression)
        {
            var getHashCode = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Override,
                Name = "GetHashCode",
                Parameters = { },
                ReturnType = new CodeTypeReference(typeof(int)),
                Statements = { new CodeMethodReturnStatement(hashCodeCalculationExpression) }
            };
            return new CodeTypeMember[] { getHashCode };
        }

        /// <summary>
        /// Create a read-only property using a backing field with standard name.
        /// </summary>
        private static CodeTypeMember[] CreateReadOnlyProperty(string name, Type valueType)
        {
            var backingFieldName = BackingFieldName(name);
            var backingField = new CodeMemberField(valueType, backingFieldName);
            var propertyField = new CodeMemberProperty()
            {
                Name = name,
                Type = new CodeTypeReference(valueType),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                GetStatements = 
                        { 
                            new CodeMethodReturnStatement( 
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(), backingFieldName)) 
                        },
            };
            return new CodeTypeMember[] { backingField, propertyField };
        }

        private static CodeTypeMember[] CreateReadOnlyProperties(IEnumerable<PropertyDeclaration> propertyDeclarations)
        {
            return propertyDeclarations.SelectMany(p => CreateReadOnlyProperty(p.Name, p.Type)).ToArray();
        }

        private static CodeMemberMethod[] CreateSetMethodForProperty(string name, Type valueType)
        {
            var parameterName = CamelCase(name);
            return new[] { 
                new CodeMemberMethod()
                    {
                        Attributes = MemberAttributes.Private | MemberAttributes.Final,
                        Name = PropertySetMethodName(name),
                        Parameters =
                            {
                                new CodeParameterDeclarationExpression(valueType, parameterName)
                            },
                        Statements =
                            {
                                new CodeAssignStatement(
                                    new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(), BackingFieldName(name)),
                                    new CodeVariableReferenceExpression(parameterName))
                            }
                    }
            };
        }

        private static string PropertySetMethodName(string propertyName)
        {
            return String.Format("Set{0}", propertyName);
        }

        private static CodeTypeMember[] CreateEqualityOperatorOverloading(string eigenType, bool isValueType)
        {
            var nullGuardNecessary = !isValueType;

            var equality = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Name = "operator ==",
                Parameters = 
                        {
                            new CodeParameterDeclarationExpression(eigenType, "left"),
                            new CodeParameterDeclarationExpression(eigenType, "right"),
                        },
                ReturnType = new CodeTypeReference(typeof(bool)),
            };

            if (nullGuardNecessary)
            {
                equality.Statements.Add(
                    // if (Object.ReferenceEquals(null, left)) return Object.ReferenceEquals(null, right);
                    new CodeConditionStatement(
                        CreateObjectReferenceEqualsNullPredicateExpression(new CodeArgumentReferenceExpression("left")),
                        new CodeMethodReturnStatement(
                            CreateObjectReferenceEqualsNullPredicateExpression(
                                new CodeArgumentReferenceExpression("right")))));
            }
            equality.Statements.Add(
                // return left.Equals(right);
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodeArgumentReferenceExpression("left"),
                        "Equals",
                        new CodeExpression[] {new CodeArgumentReferenceExpression("right")})));


            var disequality = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Name = "operator !=",
                Parameters = 
                    {
                        new CodeParameterDeclarationExpression(eigenType, "left"),
                        new CodeParameterDeclarationExpression(eigenType, "right"),
                    },
                ReturnType = new CodeTypeReference(typeof(bool)),
            };

            if (nullGuardNecessary)
            {
                disequality.Statements.Add(
                    // if (Object.ReferenceEquals(null, left)) return !Object.ReferenceEquals(null, right);
                    new CodeConditionStatement(
                        CreateObjectReferenceEqualsNullPredicateExpression(new CodeArgumentReferenceExpression("left")),
                        new CodeMethodReturnStatement(
                            CreateUnaryNegation(
                                CreateObjectReferenceEqualsNullPredicateExpression(
                                    new CodeArgumentReferenceExpression("right"))))));
            }

            disequality.Statements.Add(
                // return (false == left.Equals(right));
                new CodeMethodReturnStatement(
                    new CodeBinaryOperatorExpression(
                        new CodePrimitiveExpression(false),
                        CodeBinaryOperatorType.ValueEquality,
                        new CodeMethodInvokeExpression(
                            new CodeArgumentReferenceExpression("left"),
                            "Equals",
                            new CodeExpression[] {new CodeArgumentReferenceExpression("right")}))));
            
            return new CodeTypeMember[] { equality, disequality };
        }

        private static CodeExpression CreateUnaryNegation(CodeExpression predicateExpression)
        {
            return CreateBinaryValueEquality(new CodePrimitiveExpression(false), predicateExpression);
        }

        private static CodeExpression CreateBinaryValueEquality(CodeExpression a, CodeExpression b)
        {
            return new CodeBinaryOperatorExpression(a, CodeBinaryOperatorType.ValueEquality, b);
        }



        private static string CamelCase(string name)
        {
            var first = name[0].ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            var rest = name.Substring(1);
            return String.Format("{0}{1}", first, rest);
        }

        private static string BackingFieldName(string name)
        {
            return String.Format("_{0}", CamelCase(name));
        }

    }
}