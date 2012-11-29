using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

internal static class TestUtil {
    public static IEnumerable<int> Range(this int count) {
        return Enumerable.Range(0, count);
    } 
    public static void AssertThrows<TException>(Func<object> func) where TException : Exception {
        AssertThrows<TException>(new Action(() => func()));
    }
    public static void AssertThrows<TException>(Action action) where TException : Exception {
        try {
            action();
            Assert.Fail("Expected an exception of type {0}, but no exception was thrown.",
                        typeof(TException).FullName);
        } catch (TException) {
            // pass!
        } catch (Exception ex) {
            Assert.Fail("Expected an exception of type {0}, but received one of type {1}: {2}.",
                        typeof(TException).FullName,
                        ex.GetType().FullName,
                        ex);
        }
    }
    public static void AssertEquals<T1, T2>(this T1 actual, T2 expected) {
        Assert.AreEqual(actual: actual, expected: expected);
    }
    public static void AssertIsTrue(this bool value) {
        Assert.IsTrue(value);
    }
    public static void AssertIsFalse(this bool value) {
        Assert.IsFalse(value);
    }
}
