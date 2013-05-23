using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Test.Diesel.TestHelpers
{
    public class EqualityTesting
    {

        /// <summary>
        /// Check that Equals and GetHashCode are well behaved with Value semantics.
        /// Given three equal but different instances and a number of unequal instances, 
        /// verify the Equals behaves correctly: 
        /// The Equals relation must be Reflexive, Transitive and Symmetrical.
        /// </summary>
        /// <typeparam name="T">The type under test.</typeparam>
        /// <param name="a">An instance</param>
        /// <param name="b">Another instance, with the same values as a.</param>
        /// <param name="c">Another instance, with the same values as a and b.</param>
        /// <param name="unequalInstances">These should not equal a, b and c. 
        /// It is recommended to supply one per property on the type T, 
        /// and have only one property per instance differ from the values of the
        /// same properties on a, b and c.</param>
        public static void TestEqualsAndGetHashCode<T>(T a, T b, T c, params T[] unequalInstances)
        {
            // Reflexive
            Assert.AreEqual(a, a, "Expected A=A");

            // Transitivity
            Assert.AreEqual(a, b, "Expected A=B");
            Assert.AreEqual(b, c, "Expected B=C");
            Assert.AreEqual(a, c, "Expected A=C");

            // Symmetrical
            Assert.AreEqual(b, a, "Expected B=A");
            Assert.AreNotEqual(a, null, "Expected a != null");
            Assert.AreNotEqual(null, a, "Expected null != a");

            var otherType = new Object();
            Assert.AreNotEqual(a, otherType, "Expected a != object of other type.");

            for (int i = 0; i < unequalInstances.Length; i++)
            {
                T unequalInstance = unequalInstances[i];
                Assert.AreNotEqual(a, unequalInstance, "Expected a != unequal instances (index {0})", i);
                Assert.DoesNotThrow(() => unequalInstance.GetHashCode(), "Expected GetHashCode to not raise exception for unequal instances (index {0})", i);
            }

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), "Expected equal hashcode when objects are equal.");
        }

        public static void TestEqualityOperators<T>(T a, T b, T c, params T[] unequalInstances)
        {
            // we don't know if the operators are defined on generic type T, so we use dynamic 
            dynamic da = a;
            dynamic db = b;
            dynamic dc = c;

            // Reflexive
            // ReSharper disable EqualExpressionComparison
            Assert.IsTrue(da == da, "Expected A=A");
            // ReSharper restore EqualExpressionComparison

            // Transitivity
            Assert.IsTrue(da == db, "Expected A=B");
            Assert.IsTrue(db == dc, "Expected B=C");
            Assert.IsTrue(da == dc, "Expected A=C");

            // Symmetrical
            Assert.IsTrue(db == da, "Expected B=A");
            Assert.IsTrue(da != null, "Expected a != null");
            Assert.IsTrue(null != da, "Expected null != a");

            Assert.IsFalse(da == null, "Expected not (A == null)");
            Assert.IsFalse(null == da, "Expected not (null == A)");

            var otherType = new Object();
            Assert.AreNotEqual(a, otherType, "Expected a != object of other type.");

            for (int i = 0; i < unequalInstances.Length; i++)
            {
                dynamic unequal = unequalInstances[i];
                Assert.IsTrue(da != unequal, "Expected a != unequal instances (index {0})", i);
            }
        }
    }
}
