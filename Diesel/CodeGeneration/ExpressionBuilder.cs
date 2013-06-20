using System;
using System.CodeDom;

namespace Diesel.CodeGeneration
{
    public static class ExpressionBuilder
    {
        public static CodeExpression Negate(CodeExpression expression)
        {
            return new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),
                                                    CodeBinaryOperatorType.ValueEquality, expression);
        }

        public static CodeFieldReferenceExpression ThisFieldReference(string fieldName)
        {
            return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName);
        }

        public static CodePropertyReferenceExpression ThisPropertyReference(string propertyName)
        {
            return PropertyReference(new CodeThisReferenceExpression(), propertyName);
        }

        public static CodePropertyReferenceExpression VariablePropertyReference(string variableName, string propertyName)
        {
            return PropertyReference(new CodeVariableReferenceExpression(variableName), propertyName);
        }

        public static CodePropertyReferenceExpression PropertyReference(CodeExpression instance, string propertyName)
        {
            return new CodePropertyReferenceExpression(instance, propertyName);
        }


        public static CodeMethodInvokeExpression ObjectReferenceEqualsNull(CodeExpression instanceToCompareToNull)
        {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(object)),
                "ReferenceEquals",
                new[]
                    {
                        new CodePrimitiveExpression(null),
                        instanceToCompareToNull
                    });
        }

        public static CodeExpression ObjectEquals(CodeExpression a, CodeExpression b)
        {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof (Object)),
                "Equals",
                a, b);
        }
    }
}