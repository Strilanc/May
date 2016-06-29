using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Strilanc.Value {
    ///<summary>
    ///A potential value that may contain no value or may contain a value of type T.
    ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
    ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
    ///</summary>
    [DebuggerDisplay("{ToString()}")]
    public struct May<T> : IMayHaveValue, IEquatable<May<T>> {
        ///<summary>
        ///A potential value containing no value.
        ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
        ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
        ///</summary>
        [Pure]
        public static May<T> NoValue { get { return default(May<T>); } }

        private readonly T _value;
        private readonly bool _hasValue;
        ///<summary>Determines if this potential value contains a value or not.</summary>
        [Pure]
        public bool HasValue { get { return _hasValue; } }

        ///<summary>Constructs a potential value containing the given value.</summary>
        [Pure]
        public May(T value) {
            this._hasValue = true;
            this._value = value;
        }

        ///<summary>Matches this potential value into either a function expecting a value or a function expecting no value, returning the result.</summary>
        [Pure]
        public TOut Match<TOut>(Func<T, TOut> valueProjection, Func<TOut> alternativeFunc) {
            if (valueProjection == null) throw new ArgumentNullException("valueProjection");
            if (alternativeFunc == null) throw new ArgumentNullException("alternativeFunc");
            return _hasValue ? valueProjection(_value) : alternativeFunc();
        }

        ///<summary>Returns a potential value containing no value.</summary>
        public static implicit operator May<T>(MayNoValue noValue) {
            return NoValue;
        }
        ///<summary>Returns a potential value containing the given value.</summary>
        public static implicit operator May<T>(T value) {
            return new May<T>(value);
        }
        ///<summary>Returns the value contained in the potential value, throwing a cast exception if the potential value contains no value.</summary>
        public static explicit operator T(May<T> potentialValue) {
            if (!potentialValue._hasValue) throw new InvalidCastException("No Value");
            return potentialValue._value;
        }
        
        ///<summary>Determines if two potential values are equivalent.</summary>
        public static bool operator ==(May<T> potentialValue1, May<T> potentialValue2) {
            return potentialValue1.Equals(potentialValue2);
        }
        ///<summary>Determines if two potential values are not equivalent.</summary>
        public static bool operator !=(May<T> potentialValue1, May<T> potentialValue2) {
            return !potentialValue1.Equals(potentialValue2);
        }
        
        ///<summary>Determines if two potential values are equivalent.</summary>
        public static bool operator ==(May<T> potentialValue1, IMayHaveValue potentialValue2) {
            return potentialValue1.Equals(potentialValue2);
        }
        ///<summary>Determines if two potential values are not equivalent.</summary>
        public static bool operator !=(May<T> potentialValue1, IMayHaveValue potentialValue2) {
            return !potentialValue1.Equals(potentialValue2);
        }

        ///<summary>Returns the hash code for this potential value.</summary>
        public override int GetHashCode() {
            return !_hasValue ? 0 
                 : ReferenceEquals(_value, null) ? -1 
                 : _value.GetHashCode();
        }
        ///<summary>
        ///Determines if this potential value is equivalent to the given potential value.
        ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
        ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
        ///</summary>
        public bool Equals(May<T> other) {
            if (other._hasValue != this._hasValue) return false;
            return !this._hasValue || Equals(_value, other._value);
        }
        ///<summary>
        ///Determines if this potential value is equivalent to the given potential value.
        ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
        ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
        ///</summary>
        public bool Equals(IMayHaveValue other) {
            if (other is May<T>) return Equals((May<T>)other);
            // potential values containing no value are always equal
            return other != null && !this.HasValue && !other.HasValue;
        }
        ///<summary>
        ///Determines if this potential value is equivalent to the given object.
        ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
        ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
        ///</summary>
        public override bool Equals(object obj) {
            if (obj is May<T>) return Equals((May<T>)obj);
            return Equals(obj as IMayHaveValue);
        }
        ///<summary>Returns a string representation of this potential value.</summary>
        public override string ToString() {
            return _hasValue
                 ? String.Format("Value: {0}", _value)
                 : "No Value";
        }
    }
}
