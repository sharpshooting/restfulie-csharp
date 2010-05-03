using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpShooting.Tests
{
    public static class FluentTestExtensions
    {
        public static void ShouldBeEqualTo<T>(this T actualValue, T expectedValue)
        {
            Assert.AreEqual(expectedValue, actualValue);
        }

        public static void ShouldNotBeEqualTo<T>(this T actualValue, T notExpectedValue)
        {
            Assert.AreNotEqual(notExpectedValue, actualValue);
        }

        public static void ShouldBeEqualTo<T>(this IEnumerable<T> actualValue, IEnumerable<T> expectedValue)
        {
            actualValue.SequenceEqual(expectedValue);
        }

        public static void ShouldNotBeEqualTo<T>(this IEnumerable<T> actualValue, IEnumerable<T> notExpectedValue)
        {
            Assert.AreNotEqual(notExpectedValue, actualValue);
        }

        public static void ShouldBeNull<T>(this T actualValue) where T : class
        {
            Assert.IsNull(actualValue);
        }
    }
}