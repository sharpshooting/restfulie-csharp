using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpShooting.Tests
{
    public class TestHelpers
    {
        public static dynamic TryGetAndThrow(dynamic value)
        {
            return value;
        }

        public static void ExpectExceptionTypeOf<T>(Action action)
        {
            try
            {
                action();

                Assert.Fail("Was expecting exception of type <{0}> but got no exception.", typeof(T).FullName);
            }
            catch (Exception exception)
            {
                Assert.IsInstanceOfType(exception, typeof(T), "Exception type is wrong.", typeof(T).FullName, exception.GetType().FullName);
            }
        }
    }
}
