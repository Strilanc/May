using System;
using System.ComponentModel;

namespace Strilanc.Value {
    ///<summary>
    ///A potential value that may or may not contain an unknown value of unknown type.
    ///All implementations should compare equal and have a hash code of 0 when HasValue is false.
    ///</summary>
    ///<remarks>
    ///Used to allow comparisons of the raw May.NoValue to generic ones like May&lt;int&gt;.NoValue.
    ///Also used as the result type of the 'do action if value present' method, but only because there is no standard void or unit type.
    ///</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IMayHaveValue : IEquatable<IMayHaveValue> {
        ///<summary>Determines if this potential value contains a value or not.</summary>
        bool HasValue { get; }
    }
}
