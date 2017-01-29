using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Strilanc.Value {
    ///<summary>
    ///A non-generic lack-of-value type, equivalent to generic likes like lack-of-int.
    ///Use Strilanc.Value.May.NoValue to get an instance.
    ///Note: All forms of no value are equal, including May.NoValue, May&lt;T&gt;.NoValue, May&lt;AnyOtherT&gt;.NoValue, default(May&lt;T&gt;) and new May&lt;T&gt;().
    ///Note: Null is NOT equivalent to new May&lt;object&gt;(null) and neither is equivalent to new May&lt;string&gt;(null).
    ///</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerDisplay("{ToString()}")]
    public struct MayNoValue : IMayHaveValue {
        ///<summary>Determines if this potential value contains a value or not (it doesn't).</summary>
        public bool HasValue { get { return false; } }
        ///<summary>Returns the hash code for a lack of potential value.</summary>
        public override int GetHashCode() {
            return 0;
        }
        ///<summary>Determines if the given potential value contains no value.</summary>
        public bool Equals(IMayHaveValue other) {
            return other != null && !other.HasValue;
        }
        ///<summary>Determines if the given object is a potential value containing no value.</summary>
        public override bool Equals(object obj) {
            return Equals(obj as IMayHaveValue);
        }
        ///<summary>Determines if two lack of values are equal (they are).</summary>
        public static bool operator ==(MayNoValue noValue1, MayNoValue noValue2) {
            return true;
        }
        ///<summary>Determines if two lack of values are not equal (they're not).</summary>
        public static bool operator !=(MayNoValue noValue1, MayNoValue noValue2) {
            return false;
        }
        ///<summary>Returns a string representation of this lack of value.</summary>
        public override string ToString() {
            return "No Value";
        }
    }
}
