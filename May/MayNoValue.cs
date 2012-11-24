using System.ComponentModel;

namespace Strilanc.Value {
    ///<summary>
    ///A non-generic lack-of-value type, equivalent to generic likes like lack-of-int.
    ///Use Strilanc.Value.May.NoValue to get an instance.
    ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
    ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
    ///</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct MayNoValue : IMayHaveValue {
        ///<summary>Determines if this potential value contains a value or not (it doesn't).</summary>
        public bool HasValue { get { return false; } }
        public override int GetHashCode() {
            return 0;
        }
        public bool Equals(IMayHaveValue other) {
            return other != null && !other.HasValue;
        }
        public override bool Equals(object obj) {
            return Equals(obj as IMayHaveValue);
        }
        public static bool operator ==(MayNoValue noValue1, MayNoValue noValue2) {
            return true;
        }
        public static bool operator !=(MayNoValue noValue1, MayNoValue noValue2) {
            return false;
        }
    }
}
