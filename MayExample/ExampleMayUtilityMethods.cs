using System;
using System.Numerics;
using Strilanc.Value;

namespace MayExample {
    public static class ExampleMayUtilityMethods {
        ///<summary>Returns the signed 32bit integer represented by the given string, if there is one.</summary>
        public static May<int> MayParseInt32(this string text) {
            int result;
            if (!Int32.TryParse(text, out result)) return May.NoValue;
            return result;
        }
        ///<summary>Returns the unsigned 32bit integer represented by the given string, if there is one.</summary>
        public static May<uint> MayParseUInt32(this string text) {
            uint result;
            if (!UInt32.TryParse(text, out result)) return May.NoValue;
            return result;
        }
        ///<summary>Returns the double represented by the given string, if there is one.</summary>
        public static May<double> MayParseDouble(this string text) {
            double result;
            if (!Double.TryParse(text, out result)) return May.NoValue;
            return result;
        }
        ///<summary>Returns the big integer represented by the given string, if there is one.</summary>
        public static May<BigInteger> MayParseBigInteger(this string text) {
            BigInteger result;
            if (!BigInteger.TryParse(text, out result)) return May.NoValue;
            return result;
        }

        ///<summary>Returns the non-negative square root of the given value, unless no real solution exists.</summary>
        public static May<double> MaySqrt(this double value) {
            if (value >= 0) return Math.Sqrt(value);
            return May.NoValue;
        }
    }
}
