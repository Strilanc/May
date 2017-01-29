﻿using System;
using System.Diagnostics.Contracts;

namespace Strilanc.Value {
    ///<summary>Utility methods for the generic May type.</summary>
    public static class May {
        ///<summary>
        ///A potential value containing no value. Implicitely converts to a no value of any generic May type.
        ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
        ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
        ///</summary>
        public static MayNoValue NoValue { get { return default(MayNoValue); } }

        ///<summary>Returns a potential value containing the given value.</summary>
        public static May<T> Maybe<T>(this T value) {
            return new May<T>(value);
        }
        ///<summary>Matches this potential value either into a function expecting a value or against an alternative value.</summary>
        public static TOut Match<TIn, TOut>(this May<TIn> potentialValue, Func<TIn, TOut> valueProjection, TOut alternative) {
            if (valueProjection == null) throw new ArgumentNullException("valueProjection");
            return potentialValue.Match(valueProjection, () => alternative);
        }
        ///<summary>Returns the potential result of potentially applying the given function to this potential value.</summary>
        public static May<TOut> Bind<TIn, TOut>(this May<TIn> potentialValue, Func<TIn, May<TOut>> projection) {
            if (projection == null) throw new ArgumentNullException("projection");
            return potentialValue.Match(projection, () => NoValue);
        }
        ///<summary>Returns the value contained in the given potential value, if any, or else the result of evaluating the given alternative value function.</summary>
        public static T Else<T>(this May<T> potentialValue, Func<T> alternativeFunc) {
            if (alternativeFunc == null) throw new ArgumentNullException("alternativeFunc");
            return potentialValue.Match(e => e, alternativeFunc);
        }
        ///<summary>Flattens a doubly-potential value, with the result containing a value only if both levels contained a value.</summary>
        public static May<T> Unwrap<T>(this May<May<T>> potentialValue) {
            return potentialValue.Bind(e => e);
        }
        ///<summary>Returns the value contained in the given potential value, if any, or else the result of evaluating the given alternative potential value function.</summary>
        public static May<T> Else<T>(this May<T> potentialValue, Func<May<T>> alternative) {
            if (alternative == null) throw new ArgumentNullException("alternative");
            return potentialValue.Match(e => e.Maybe(), alternative);
        }
        ///<summary>Returns the value contained in the given potential value, if any, or else the given alternative value.</summary>
        public static T Else<T>(this May<T> potentialValue, T alternative) {
            return potentialValue.Else(() => alternative);
        }
        ///<summary>Returns the value contained in the given potential value, if any, or else the given alternative potential value.</summary>
        public static May<T> Else<T>(this May<T> potentialValue, May<T> alternative) {
            return potentialValue.Else(() => alternative);
        }
        ///<summary>Returns the result of potentially applying a function to this potential value.</summary>
        public static May<TOut> Select<TIn, TOut>(this May<TIn> value, Func<TIn, TOut> projection) {
            if (projection == null) throw new ArgumentNullException("projection");
            return value.Bind(e => projection(e).Maybe());
        }
        ///<summary>Returns the same value, unless the contained value does not match the filter in which case a no value is returned.</summary>
        public static May<T> Where<T>(this May<T> value, Func<T, bool> filter) {
            if (filter == null) throw new ArgumentNullException("filter");
            return value.Bind(e => filter(e) ? e.Maybe() : NoValue);
        }
        ///<summary>Projects optional values, returning a no value if anything along the way is a no value.</summary>
        public static May<TOut> SelectMany<TIn, TMid, TOut>(this May<TIn> source,
                                                            Func<TIn, May<TMid>> maySelector,
                                                            Func<TIn, TMid, TOut> resultSelector) {
            if (maySelector == null) throw new ArgumentNullException("maySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return source.Bind(s => maySelector(s).Select(m => resultSelector(s, m)));
        }
        ///<summary>Combines the values contained in several potential values with a projection function, returning no value if any of the inputs contain no value.</summary>
        public static May<TOut> Combine<TIn1, TIn2, TOut>(this May<TIn1> potentialValue1,
                                                          May<TIn2> potentialValue2,
                                                          Func<TIn1, TIn2, TOut> resultSelector) {
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return from v1 in potentialValue1
                   from v2 in potentialValue2
                   select resultSelector(v1, v2);
        }
        ///<summary>Combines the values contained in several potential values with a projection function, returning no value if any of the inputs contain no value.</summary>
        public static May<TOut> Combine<TIn1, TIn2, TIn3, TOut>(this May<TIn1> potentialValue1,
                                                                May<TIn2> potentialValue2,
                                                                May<TIn3> potentialValue3,
                                                                Func<TIn1, TIn2, TIn3, TOut> resultSelector) {
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return from v1 in potentialValue1
                   from v2 in potentialValue2
                   from v3 in potentialValue3
                   select resultSelector(v1, v2, v3);
        }
        ///<summary>Combines the values contained in several potential values with a projection function, returning no value if any of the inputs contain no value.</summary>
        public static May<TOut> Combine<TIn1, TIn2, TIn3, TIn4, TOut>(this May<TIn1> potentialValue1,
                                                                      May<TIn2> potentialValue2,
                                                                      May<TIn3> potentialValue3,
                                                                      May<TIn4> potentialValue4,
                                                                      Func<TIn1, TIn2, TIn3, TIn4, TOut> resultSelector) {
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return from v1 in potentialValue1
                   from v2 in potentialValue2
                   from v3 in potentialValue3
                   from v4 in potentialValue4
                   select resultSelector(v1, v2, v3, v4);
        }
        /// <summary>
        /// Potentially runs an action taking the potential value's value.
        /// No effect if the potential value is no value.
        /// Returns an IMayHaveValue that has a value iff the action was run.
        /// </summary>
        public static IMayHaveValue IfHasValueThenDo<T>(this May<T> potentialValue, Action<T> hasValueAction) {
            if (hasValueAction == null) throw new ArgumentNullException("hasValueAction");
            return potentialValue.Select(e => {
                hasValueAction(e);
                return 0;
            });
        }
        ///<summary>Runs the given no value action if the given potential value does not contain a value, and otherwise does nothing.</summary>
        public static void ElseDo(this IMayHaveValue potentialValue, Action noValueAction) {
            if (potentialValue == null) throw new ArgumentNullException("potentialValue");
            if (noValueAction == null) throw new ArgumentNullException("noValueAction");
            if (!potentialValue.HasValue) noValueAction();
        }
        ///<summary>Returns the value contained in the given potential value, if any, or else the type's default value.</summary>
        public static T ElseDefault<T>(this May<T> potentialValue) {
            return potentialValue.Else(default(T));
        }

        ///<summary>Returns the value contained in the potential value, or throws an InvalidOperationException if it contains no value.</summary>
        public static T ForceGetValue<T>(this May<T> potentialValue) {
            return potentialValue.Match(
                e => e, 
                () => { throw new InvalidOperationException("No Value"); });
        }
    }
}
