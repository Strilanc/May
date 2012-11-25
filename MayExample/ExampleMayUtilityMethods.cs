using System;
using System.Collections.Generic;
using System.Linq;
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

        ///<summary>Returns the first item in the sequence, unless it's empty.</summary>
        public static May<T> MayFirst<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            using (var e = sequence.GetEnumerator())
                if (e.MoveNext())
                    return e.Current;
            return May.NoValue;
        }

        /// <summary>
        /// Enumerates the values in the potential values in the sequence.
        /// The potential values that contain no value are skipped.
        /// </summary>
        public static IEnumerable<T> WhereHasValue<T>(this IEnumerable<May<T>> sequence) {
            return sequence.Where(e => e.HasValue).Select(e => (T)e);
        }
        /// <summary>
        /// Returns a sequence of the actual values in the potential values in the sequence.
        /// However, if any of the potential values contains no value then the result is no value.
        /// </summary>
        public static May<IEnumerable<T>> MayAll<T>(this IEnumerable<May<T>> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            var result = new List<T>();
            foreach (var potentialValue in sequence)
                if (!potentialValue.IfHasValueThenDo(result.Add).HasValue)
                    return May.NoValue;
            return result;
        }
    }
}
