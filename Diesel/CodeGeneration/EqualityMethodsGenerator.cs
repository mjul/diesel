using System;
using System.CodeDom;
using System.Diagnostics.Contracts;
using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    /// <summary>
    /// Functions to generate equality methods.
    /// </summary>
    public static class EqualityMethodsGenerator
    {
        /// <summary>
        /// Produce an expression that compares two properties by value.
        /// </summary>
        [Pure]
        public static CodeExpression ComparePropertyValueEqualityExpression(PropertyDeclaration property, string otherVariableName)
        {
            return ComparePropertyValueEqualityExpression((dynamic)property.Type, property.Name, otherVariableName);
        }

        [Pure]
        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(SimpleType propertyType, String propertyName, String otherVariableName)
        {
            return CompareValueEquality(propertyName, otherVariableName);
        }

        [Pure]
        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(StringReferenceType propertyType, String propertyName, String otherVariableName)
        {
            return CompareValueEquality(propertyName, otherVariableName);
        }

        /// <summary>
        /// this.PropertyName == other.PropertyName
        /// </summary>
        [Pure]
        private static CodeBinaryOperatorExpression CompareValueEquality(string propertyName, string otherVariableName)
        {
            return new CodeBinaryOperatorExpression(
                ExpressionBuilder.ThisPropertyReference(propertyName),
                CodeBinaryOperatorType.ValueEquality,
                OtherPropertyReference(propertyName, otherVariableName));
        }

        /// <summary>
        /// Object.Equals(a.Property, b.Property)
        /// </summary>
        [Pure]
        private static CodeExpression CompareObjectEquality(string propertyName, string otherVariableName)
        {
            return ExpressionBuilder.ObjectEquals(ExpressionBuilder.ThisPropertyReference(propertyName),
                                                  OtherPropertyReference(propertyName, otherVariableName));
        }

        [Pure]
        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(NullableType propertyType, String propertyName, String otherVariableName)
        {
            return CompareValueEquality(propertyName, otherVariableName);
        }

        /// <summary>
        /// Compare ArraType member: both null or same length and values
        /// (((this.Property == null) && (other.Property == null)) 
        /// || ((this.Property.Length == other.Property.Length) 
        ///      && Enumerable.Zip(a, b, (a, b) => Object.Equals(a, b)).All(areEqual => areEqual)));
        /// </summary>
        [Pure]
        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(ArrayType propertyType,
                                                                                           String propertyName,
                                                                                           String otherVariableName)
        {
            // TODO: this should probably be a warning in the model
            if (propertyType.RankSpecifiers.Ranks.Count() > 1)
                throw new InvalidOperationException(
                    "Cannot generate equality for Array Types with more than one rank-specifier.");
            var rankSpecifier = propertyType.RankSpecifiers.Ranks.Single();
            if (rankSpecifier.Dimensions > 1)
                throw new InvalidOperationException(
                    "Cannot generate equality for Array Type with more than one dimension");

            // (this.Property.Length == other.Property.Length) 
            // && Enumerable.Zip(a, b, (a, b) => Object.Equals(a, b)).All(areEqual => areEqual);

            var thisPropertyReference = ExpressionBuilder.ThisPropertyReference(propertyName);
            var otherPropertyReference = OtherPropertyReference(propertyName, otherVariableName);

            var bothNull = new CodeBinaryOperatorExpression(CompareToNull(thisPropertyReference),
                                                            CodeBinaryOperatorType.BooleanAnd,
                                                            CompareToNull(otherPropertyReference));

            var bothNotNull = new CodeBinaryOperatorExpression(
                ExpressionBuilder.Negate(CompareToNull(thisPropertyReference)),
                CodeBinaryOperatorType.BooleanAnd,
                ExpressionBuilder.Negate(CompareToNull(otherPropertyReference)));

            var sameArrayLength = new CodeBinaryOperatorExpression(
                new CodePropertyReferenceExpression(thisPropertyReference, "Length"),
                CodeBinaryOperatorType.ValueEquality,
                new CodePropertyReferenceExpression(otherPropertyReference, "Length"));

            var zipExpression = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(typeof (Enumerable)),
                    "Zip"),
                thisPropertyReference,
                otherPropertyReference,
                new CodeSnippetExpression("(a, b) => Object.Equals(a,b)"));

            var zipPairwiseEquality =
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof (Enumerable)),
                        "All"),
                    zipExpression,
                    new CodeSnippetExpression("areEqual => areEqual")
                    );

            return new CodeBinaryOperatorExpression(
                bothNull,
                CodeBinaryOperatorType.BooleanOr,
                new CodeBinaryOperatorExpression(
                    bothNotNull,
                    CodeBinaryOperatorType.BooleanAnd,
                    new CodeBinaryOperatorExpression(sameArrayLength,
                                                     CodeBinaryOperatorType.BooleanAnd, zipPairwiseEquality)));
        }

        [Pure]
        private static CodeExpression CompareToNull(CodePropertyReferenceExpression propertyReference)
        {
            return ExpressionBuilder.ObjectReferenceEqualsNull(propertyReference);
        }

        [Pure]
        private static CodePropertyReferenceExpression OtherPropertyReference(string propertyName, string otherVariableName)
        {
            return ExpressionBuilder.VariablePropertyReference(otherVariableName, propertyName);
        }

        [Pure]
        private static CodeExpression ComparePropertyValueEqualityExpression(TypeName propertyType, String propertyName, String otherVariableName)
        {
            return CompareObjectEquality(propertyName, otherVariableName);
        }
    }
}