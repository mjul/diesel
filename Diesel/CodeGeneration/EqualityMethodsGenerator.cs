using System;
using System.CodeDom;
using System.Linq;
using Diesel.Parsing;
using Diesel.Parsing.CSharp;

namespace Diesel.CodeGeneration
{
    public class EqualityMethodsGenerator
    {
        /// <summary>
        /// Produce an expression that compares two properties by value.
        /// </summary>
        public static CodeExpression ComparePropertyValueEqualityExpression(PropertyDeclaration property, string otherVariableName)
        {
            return ComparePropertyValueEqualityExpression((dynamic)property.Type, property.Name, otherVariableName);
        }

        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(SimpleType propertyType, String propertyName, String otherVariableName)
        {
            return CompareValueEquality(propertyName, otherVariableName);
        }

        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(StringReferenceType propertyType, String propertyName, String otherVariableName)
        {
            return CompareValueEquality(propertyName, otherVariableName);
        }

        /// <summary>
        /// this.PropertyName == other.PropertyName
        /// </summary>
        private static CodeBinaryOperatorExpression CompareValueEquality(string propertyName, string otherVariableName)
        {
            return new CodeBinaryOperatorExpression(
                ThisPropertyReference(propertyName),
                CodeBinaryOperatorType.ValueEquality,
                OtherPropertyReference(propertyName, otherVariableName));
        }

        /// <summary>
        /// Object.Equals(a.Property, b.Property)
        /// </summary>
        private static CodeExpression CompareObjectEquality(string propertyName, string otherVariableName)
        {
            return new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof (Object)),
                "Equals",
                ThisPropertyReference(propertyName),
                OtherPropertyReference(propertyName, otherVariableName));
        }

        private static CodeBinaryOperatorExpression ComparePropertyValueEqualityExpression(NullableType propertyType, String propertyName, String otherVariableName)
        {
            return CompareValueEquality(propertyName, otherVariableName);
        }

        /// <summary>
        /// (this.Property.Length == other.Property.Length) 
        ///  && Enumerable.Zip(a, b, (a, b) => a == b).All(areEqual => areEqual);
        /// </summary>
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

            var thisPropertyReference = ThisPropertyReference(propertyName);
            var otherPropertyReference = OtherPropertyReference(propertyName, otherVariableName);

            var sameArrayLength = new CodeBinaryOperatorExpression(
                new CodePropertyReferenceExpression(thisPropertyReference, "Length"),
                CodeBinaryOperatorType.ValueEquality,
                new CodePropertyReferenceExpression(otherPropertyReference, "Length"));

            var zipExpression = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(typeof (System.Linq.Enumerable)),
                    "Zip"),
                thisPropertyReference,
                otherPropertyReference,
                new CodeSnippetExpression("(a, b) => Object.Equals(a,b)"));

            var zipPairwiseEquality =
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeTypeReferenceExpression(typeof (System.Linq.Enumerable)),
                        "All"),
                    zipExpression,
                    new CodeSnippetExpression("areEqual => areEqual")
                    );

            return new CodeBinaryOperatorExpression(sameArrayLength,
                                                    CodeBinaryOperatorType.BooleanAnd, zipPairwiseEquality);
        }

        private static CodePropertyReferenceExpression OtherPropertyReference(string propertyName, string otherVariableName)
        {
            return new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(otherVariableName), propertyName);
        }

        private static CodePropertyReferenceExpression ThisPropertyReference(string propertyName)
        {
            return new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), propertyName);
        }

        private static CodeExpression ComparePropertyValueEqualityExpression(TypeNameTypeNode propertyType, String propertyName, String otherVariableName)
        {
            return CompareObjectEquality(propertyName, otherVariableName);
        }
    }
}