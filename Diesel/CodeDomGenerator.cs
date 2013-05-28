using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Diesel
{
    public class CodeDomGenerator
    {
        public static CodeCompileUnit Compile(AbstractSyntaxTree ast)
        {
            var unit = new CodeCompileUnit();
            Add(unit, ast);
            return unit;
        }

        private static void Add(CodeCompileUnit codeCompileUnit, AbstractSyntaxTree ast)
        {
            foreach (var ns in ast.Namespaces)
            {
                Add(codeCompileUnit, ns);                
            }
        }

        private static void Add(CodeCompileUnit codeCompileUnit, Namespace declaration)
        {
            var ns = new CodeNamespace(declaration.Name);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            codeCompileUnit.Namespaces.Add(ns);
            foreach (var typeDeclaration in declaration.Declarations)
            {
                Add(ns, (dynamic)typeDeclaration);
            }
        }

        private static void Add(CodeNamespace ns, CommandDeclaration declaration)
        {
            ns.Types.Add(CreateCommandDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ValueTypeDeclaration declaration)
        {
            ns.Types.Add(CreateValueTypeDeclaration(declaration));
        }

        private static void Add(CodeNamespace ns, ApplicationServiceDeclaration declaration)
        {
            ns.Types.Add(CreateApplicationServiceInterface(declaration));
            foreach (var command in declaration.Commands)
            {
                Add(ns, command);
            }
        }

        private static CodeTypeDeclaration CreateApplicationServiceInterface(ApplicationServiceDeclaration declaration)
        {
            var interfaceName = InterfaceNameFor(declaration.Name);
            var result = new CodeTypeDeclaration(interfaceName)
            {
                IsPartial = true,
                IsInterface = true,
            };
            // Define an "Execute" overload for each command
            var commandHandlerMembers =
                (from c in declaration.Commands
                 select (CodeTypeMember) new CodeMemberMethod()
                     {
                         Attributes = MemberAttributes.Public,
                         Name = "Execute",
                         Parameters = {new CodeParameterDeclarationExpression(c.Name, "command")},
                     }).ToArray();

            result.Members.AddRange(commandHandlerMembers);
            return result;
        }

        private static string InterfaceNameFor(string name)
        {
            return String.Format("I{0}", name);
        }

        private static CodeTypeDeclaration CreateCommandDeclaration(CommandDeclaration declaration)
        {
            const bool isValueType = false;
            return CreateTypeWithValueSemantics(isValueType, declaration.Name, declaration.Properties.ToArray(), true);
        }

        private static CodeTypeDeclaration CreateValueTypeDeclaration(ValueTypeDeclaration declaration)
        {
            const string valuePropertyName = "Value";
            var valueProperty = new PropertyDeclaration(valuePropertyName, declaration.ValueType);
            var properties = new[] { valueProperty };
            const bool isValueType = true;
            var result = CreateTypeWithValueSemantics(isValueType, declaration.Name, properties, false);
            result.CustomAttributes.Add(CreateDebuggerDisplayAttribute(String.Format("{{{0}}}", valuePropertyName)));
            return result;
        }

        private static CodeTypeDeclaration CreateTypeWithValueSemantics(bool isValueType, string name, 
            PropertyDeclaration[] properties, bool isDataContract)
        {
            var result = new CodeTypeDeclaration(name)
                {
                    IsStruct = isValueType,
                    IsPartial = true,
                    IsClass = !isValueType
                };

            var attributesByProperty = properties.ToDictionary(pd => pd, pd => new List<CodeAttributeDeclaration>());
            if (isDataContract)
            {
                result.CustomAttributes.Add(CreateAttribute(typeof(DataContractAttribute)));
                var dataMemberAttributeByProperty = Enumerable.Zip(
                        properties,
                        Enumerable.Range(1, properties.Length),
                        (p, i) => new
                            {
                                Property = p,
                                Attribute = new [] {CreateDataMemberAttribute(i)}.ToList()
                            })
                                                              .ToDictionary(pi => pi.Property, pi => pi.Attribute);
                attributesByProperty = dataMemberAttributeByProperty;
            }

            result.CustomAttributes.Add(CreateAttribute(typeof (SerializableAttribute)));
            result.BaseTypes.AddRange(CreateImplementsIEquatableOf(name));
            result.Members.AddRange(CreateConstructorAssigningBackingFieldsFor(properties));
            result.Members.AddRange(CreateReadOnlyProperties(properties, p => attributesByProperty[p]));
            result.Members.AddRange(CreateEqualityOperatorOverloading(name, isValueType));
            result.Members.AddRange(CreateGetHashCode(properties));
            result.Members.AddRange(CreateEqualsOverloadingUsingEqualityOperator(name, isValueType, properties));
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


        private static CodeAttributeDeclaration CreateAttribute(Type type)
        {
            return new CodeAttributeDeclaration(new CodeTypeReference(type));
        }

        private static CodeAttributeDeclaration CreateDebuggerDisplayAttribute(string formatString)
        {
            return new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(System.Diagnostics.DebuggerDisplayAttribute)),
                new[] { new CodeAttributeArgument(new CodePrimitiveExpression(formatString)) });
        }

        
        private static CodeAttributeDeclaration CreateDataMemberAttribute(int order)
        {
            return new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(DataMemberAttribute)),
                new[] { new CodeAttributeArgument("Order", new CodePrimitiveExpression(order)) });
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
        private static CodeTypeMember[] CreateReadOnlyProperty(string name, Type valueType, IEnumerable<CodeAttributeDeclaration> customAttributes)
        {
            var backingFieldName = BackingFieldName(name);
            var backingField = new CodeMemberField(valueType, backingFieldName);
            var propertyField = new CodeMemberProperty()
            {
                Name = name,
                Type = new CodeTypeReference(valueType),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                CustomAttributes = new CodeAttributeDeclarationCollection(customAttributes.ToArray()),
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
            return CreateReadOnlyProperties(propertyDeclarations, p => new List<CodeAttributeDeclaration>());
        }

        private static CodeTypeMember[] CreateReadOnlyProperties(IEnumerable<PropertyDeclaration> propertyDeclarations, Func<PropertyDeclaration, IEnumerable<CodeAttributeDeclaration>> generateAttributesFunction)
        {
            return propertyDeclarations.SelectMany(p => CreateReadOnlyProperty(p.Name, p.Type, generateAttributesFunction(p))).ToArray();
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