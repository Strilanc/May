using System;
using System.Collections.Generic;
using System.Linq;

namespace Strilanc.Value {
    ///<summary>Utility methods that involve May&lt;T&gt; but with a focus on other types.</summary>
    public static class MayUtilities {
        /// <summary>
        /// Returns the result of using a folder function to combine all the items in the sequence into one aggregate item.
        /// If the sequence is empty, the result is NoValue.
        /// </summary>
        public static May<T> MayAggregate<T>(this IEnumerable<T> sequence, Func<T, T, T> folder) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (folder == null) throw new ArgumentNullException("folder");
            return sequence.Aggregate(
                May<T>.NoValue,
                (a, e) => a.Match(v => folder(v, e), e));
        }

        /// <summary>
        /// Returns the minimum value in a sequence, as determined by the given comparer or else the type's default comparer.
        /// If the sequence is empty, the result is NoValue.
        /// </summary>
        public static May<T> MayMin<T>(this IEnumerable<T> sequence, IComparer<T> comparer = null) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            var c = comparer ?? Comparer<T>.Default;
            return sequence.MayAggregate((e1, e2) => c.Compare(e1, e2) <= 0 ? e1 : e2);
        }

        /// <summary>
        /// Returns the maximum value in a sequence, as determined by the given comparer or else the type's default comparer.
        /// If the sequence is empty, the result is NoValue.
        /// </summary>
        public static May<T> MayMax<T>(this IEnumerable<T> sequence, IComparer<T> comparer = null) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            var c = comparer ?? Comparer<T>.Default;
            return sequence.MayAggregate((e1, e2) => c.Compare(e1, e2) >= 0 ? e1 : e2);
        }

        /// <summary>
        /// Returns the minimum value in a sequence, as determined by projecting the items and using the given comparer or else the type's default comparer.
        /// If the sequence is empty, the result is NoValue.
        /// </summary>
        public static May<TItem> MayMinBy<TItem, TCompare>(this IEnumerable<TItem> sequence, Func<TItem, TCompare> projection, IComparer<TCompare> comparer = null) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            var c = comparer ?? Comparer<TCompare>.Default;
            return sequence
                .Select(e => new { v = e, p = projection(e)})
                .MayAggregate((e1, e2) => c.Compare(e1.p, e2.p) <= 0 ? e1 : e2)
                .Select(e => e.v);
        }

        /// <summary>
        /// Returns the maximum value in a sequence, as determined by projecting the items and using the given comparer or else the type's default comparer.
        /// If the sequence is empty, the result is NoValue.
        /// </summary>
        public static May<TItem> MayMaxBy<TItem, TCompare>(this IEnumerable<TItem> sequence, Func<TItem, TCompare> projection, IComparer<TCompare> comparer = null) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            var c = comparer ?? Comparer<TCompare>.Default;
            return sequence
                .Select(e => new { v = e, p = projection(e) })
                .MayAggregate((e1, e2) => c.Compare(e1.p, e2.p) >= 0 ? e1 : e2)
                .Select(e => e.v);
        }

        ///<summary>Returns the first item in a sequence, or else NoValue if the sequence is empty.</summary>
        public static May<T> MayFirst<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            using (var e = sequence.GetEnumerator())
                if (e.MoveNext())
                    return e.Current;
            return May.NoValue;
        }

        ///<summary>Returns the last item in a sequence, or else NoValue if the sequence is empty.</summary>
        public static May<T> MayLast<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");

            // try to skip to the last item without enumerating when possible
            var list = sequence as IList<T>;
            if (list != null) {
                if (list.Count == 0) return May.NoValue;
                return list[list.Count - 1];
            }

            return sequence.MayAggregate((e1, e2) => e2);
        }

        ///<summary>Returns the single item in a sequence, NoValue if the sequence is empty, or throws an exception if there is more than one item.</summary>
        public static May<T> MaySingle<T>(this IEnumerable<T> sequence) {
            if (sequence == null) throw new ArgumentNullException("sequence");

            using (var e = sequence.GetEnumerator()) {
                if (!e.MoveNext()) return May.NoValue;
                var result = e.Current;
                //note: this case is an exception to match the semantics of SingleOrDefault, not because it's the best approach
                if (e.MoveNext()) throw new ArgumentOutOfRangeException("sequence", "Expected either no items or a single item.");
                return result;
            }
        }

        /// <summary>
        /// Enumerates the values in the potential values in the sequence.
        /// The potential values that contain no value are skipped.
        /// </summary>
        public static IEnumerable<T> WhereHasValue<T>(this IEnumerable<May<T>> sequence) {
            return sequence.Where(e => e.HasValue).Select(e => (T)e);
        }

        /// <summary>
        /// Enumerates the values in all the potential values in the sequence.
        /// However, if any of the potential values contains no value then the entire result is no value.
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
