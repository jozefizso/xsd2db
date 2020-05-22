using System;
using AssertEx = NUnit.Framework.Assert;

namespace Xsd2Db.CommandLineParser.Test
{
    /// <summary>
    /// Wrapper for NUnit Assert class to provide compatibility
    /// for old NUnit API
    /// </summary>
    public static class Assertion
    {
        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="T:NUnit.Framework.AssertionException" />.
        /// </summary>
        public static void Assert(bool condition)
        {
            AssertEx.IsTrue(condition);
        }

        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
        /// an <see cref="T:NUnit.Framework.AssertionException" />.
        /// </summary>
        public static void Assert(string message, bool condition)
        {
            AssertEx.IsTrue(condition, message);
        }

        /// <summary>
        /// Verifies that two ints are equal. If they are not, then an
        /// <see cref="T:NUnit.Framework.AssertionException" /> is thrown.
        /// </summary>
        public static void AssertEquals(string message, int expected, int actual)
        {
            AssertEx.AreEqual(expected, actual, message);
        }

        /// <summary>
        /// Verifies that two objects are equal.  Two objects are considered
        /// equal if both are null, or if both have the same value. NUnit
        /// has special semantics for some object types.
        /// If they are not equal an <see cref="T:NUnit.Framework.AssertionException" /> is thrown.
        /// </summary>
        public static void AssertEquals(string message, object expected, object actual)
        {
            AssertEx.AreEqual(expected, actual, message);
        }
    }
}
