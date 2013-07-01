using System;
using System.CodeDom;
using System.Diagnostics.Contracts;

namespace Diesel.CodeGeneration
{
    /// <summary>
    /// Functions to build CodeDom expressions.
    /// </summary>
    [Pure]
    public static class ExpressionBuilder
    {
        /// <summary>
        /// For an expression A, return a negation expression, non-A.
        /// </summary>
        [Pure]
        public static CodeExpression Negate(CodeExpression expression)
        {
            return new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),
                                                    CodeBinaryOperatorType.ValueEquality, expression);
        }

        /// <summary>
        /// Return a reference to an instance field on "this".
        /// </summary>
        [Pure]
        public static CodeFieldReferenceExpression ThisFieldReference(string fieldName)
        {
            return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName);
        }

        /// <summary>
        /// Return a reference to an instance property on "this".
        /// </summary>
        [Pure]
        public static CodePropertyReferenceExpression ThisPropertyReference(string propertyName)
        {
            return PropertyReference(new CodeThisReferenceExpression(), propertyName);
        }

        /// <summary>
        /// Return a reference to an instance property on a variable.
        /// </summary>
        [Pure]
        public static CodePropertyReferenceExpression VariablePropertyReference(string variableName, string propertyName)
        {
            return PropertyReference(new CodeVariableReferenceExpression(variableName), propertyName);
        }

        /// <summary>
        /// Return an reference to a named property.
        /// </summary>
        [Pure]
        public static CodePropertyReferenceExpression PropertyReference(CodeExpression instance, string propertyName)
        {
            return new CodePropertyReferenceExpression(instance, propertyName);
        }


        /// <summary>
        /// Return an expression comparing an object reference to null.
        /// </summary>
        [Pure]
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

        /// <summary>
        /// Return an expression comparing two expressions a and be with Object.Equals(a,b).
        /// </summary>
        [Pure]        
        public static CodeExpression ObjectEquals(CodeExpression a, CodeExpression b)
        {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof (Object)),
                "Equals",
                a, b);
        }
    }
}