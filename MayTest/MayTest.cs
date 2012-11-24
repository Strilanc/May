using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Strilanc.Value;
using System.Linq;

[TestClass]
public class MayTest {
    [TestMethod]
    public void MayValue() {
        Assert.IsTrue(new May<int>(1).HasValue);
        Assert.IsTrue(new May<int>(1).ForceGetValue() == 1);
        Assert.IsTrue(new May<string>("5").HasValue);
        Assert.IsTrue(new May<string>("5").ForceGetValue() == "5");

        Assert.IsTrue(!May<int>.NoValue.HasValue);
        Assert.IsTrue(!May<string>.NoValue.HasValue);
        Assert.IsTrue(!new May<int>().HasValue);
        Assert.IsTrue(!new May<string>().HasValue);
        Assert.IsTrue(!default(May<int>).HasValue);
        Assert.IsTrue(!default(May<string>).HasValue);

        TestUtil.AssertThrows<InvalidOperationException>(() => new May<int>().ForceGetValue());
        TestUtil.AssertThrows<InvalidOperationException>(() => new May<string>().ForceGetValue());
        TestUtil.AssertThrows<InvalidOperationException>(() => default(May<int>).ForceGetValue());
        TestUtil.AssertThrows<InvalidOperationException>(() => default(May<string>).ForceGetValue());
    }
    [TestMethod]
    public void MayEquals() {
        // --- all forms of no value are equivalent
        // typed == operators
        Assert.IsTrue(May.NoValue == May.NoValue);
        Assert.IsTrue(May<string>.NoValue == May<int>.NoValue);
        Assert.IsTrue(May.NoValue == May<int>.NoValue);
        Assert.IsTrue(May<int>.NoValue == May.NoValue);
        Assert.IsTrue(May<int>.NoValue == default(May<string>));
        Assert.IsTrue(May<int>.NoValue == new May<bool>());
        Assert.IsFalse(May.NoValue != May.NoValue);
        Assert.IsFalse(May<string>.NoValue != May<int>.NoValue);
        Assert.IsFalse(May.NoValue != May<int>.NoValue);
        Assert.IsFalse(May<int>.NoValue != May.NoValue);
        Assert.IsFalse(May<int>.NoValue != default(May<string>));
        Assert.IsFalse(May<int>.NoValue != new May<bool>());
        // untyped Equals methods
        var ee = new object[] {
            May.NoValue, 
            May<int>.NoValue, 
            May<string>.NoValue, 
            new May<object>(), 
            default(May<bool>)
        };
        foreach (var e1 in ee) {
            foreach (var e2 in ee) {
                Assert.IsTrue(e1.Equals(e2));
                Assert.IsTrue(e1.GetHashCode() == e2.GetHashCode());
                Assert.IsTrue(((IMayHaveValue)e1).Equals((IMayHaveValue)e2));
            }
        }

        // --- anything differing by has value or by value or by not being a May is not equivalent
        // same-typed == operators
        var mm = new[] {
            new May<int>(1), 
            new May<int>(2), 
            new May<int>()
        };
        for (var i = 0; i < mm.Length; i++) {
            for (var j = 0; j < mm.Length; j++) {
                Assert.IsTrue((mm[i] == mm[j]) == (i == j));
                Assert.IsTrue((mm[i] != mm[j]) == (i != j));
            }
        }
        // untyped Equals methods
        var oo = new object[] {
            May<int>.NoValue,
            new May<uint>(1), 
            new May<int>(1), 
            new May<int>(2), 
            new May<string>("1"), 
            new May<string>("2"), 
            "2", // unwrapped != wrapped in may
            null // null != NoValue
        };
        for (var i = 0; i < oo.Length; i++) {
            for (var j = 0; j < oo.Length; j++) {
                Assert.IsTrue((Equals(oo[i], oo[j])) == (i == j));
            }
        }
    }
    [TestMethod]
    public void ValueCasts() {
        ((int)1.Maybe()).AssertEquals(1);
        ((May<int>)2).AssertEquals(new May<int>(2));
        TestUtil.AssertThrows<InvalidCastException>(() => (int)May<int>.NoValue);
    }
    [TestMethod]
    public void MayElse() {
        var n = May<int>.NoValue;
        var y = new May<int>(1);
        Func<May<int>> throwsM = () => { throw new ArgumentException(); };
        Func<int> throwsT = () => { throw new ArgumentException(); };

        // else T
        Assert.IsTrue(n.Else(2) == 2);
        Assert.IsTrue(y.Else(2) == 1);

        // else Func<T>
        Assert.IsTrue(n.Else(() => 2) == 2);
        Assert.IsTrue(y.Else(() => 2) == 1);
        Assert.IsTrue(y.Else(throwsT) == y);
        TestUtil.AssertThrows<ArgumentException>(() => n.Else(throwsT));


        // else May<T>
        Assert.IsTrue(n.Else(y) == y);
        Assert.IsTrue(y.Else(n) == y);
        Assert.IsTrue(n.Else(2.Maybe()) == 2.Maybe());
        Assert.IsTrue(y.Else(2.Maybe()) == y);
        Assert.IsTrue(n.Else(May.NoValue) == May.NoValue);
        Assert.IsTrue(y.Else(May.NoValue) == y);

        // else Func<May<T>>
        Assert.IsTrue(n.Else(() => y) == y);
        Assert.IsTrue(y.Else(() => n) == y);
        Assert.IsTrue(n.Else(() => 2.Maybe()) == 2.Maybe());
        Assert.IsTrue(y.Else(() => 2.Maybe()) == y);
        Assert.IsTrue(n.Else(() => new May<int>()) == May.NoValue);
        Assert.IsTrue(y.Else(() => new May<int>()) == y);
        Assert.IsTrue(y.Else(throwsM) == y);
        TestUtil.AssertThrows<ArgumentException>(() => n.Else(throwsM));

        // else default
        Assert.IsTrue(n.ElseDefault() == 0);
        Assert.IsTrue(y.ElseDefault() == 1);
        
        // else action
        var x = 0;
        y.ElseDo(() => { x += 1; });
        Assert.IsTrue(x == 0);
        n.ElseDo(() => { x += 1; });
        Assert.IsTrue(x == 1);
    }
    [TestMethod]
    public void MayIfThenDo() {
        var i = 0;
        1.Maybe().IfThenDo(e => { i += e + 1; }).HasValue.AssertIsTrue();
        i.AssertEquals(2);
        May<int>.NoValue.IfThenDo(e => { i += e + 1; }).HasValue.AssertIsFalse();
        i.AssertEquals(2);
    }
    [TestMethod]
    public void Maybe() {
        1.Maybe().AssertEquals(new May<int>(1));
        "5".Maybe().AssertEquals(new May<string>("5"));
        ((string)null).Maybe().AssertEquals(new May<string>(null));
    }
    [TestMethod]
    public void MayUnwrap() {
        1.Maybe().Maybe().Unwrap().AssertEquals(1.Maybe());
        new May<int>().Maybe().Unwrap().AssertEquals(May.NoValue);
        new May<May<int>>().Unwrap().AssertEquals(May.NoValue);
    }
    [TestMethod]
    public void MaySelect() {
        1.Maybe().Select(e => e + 1).AssertEquals(2.Maybe());
        May<int>.NoValue.Select(e => e + 2).AssertEquals(May.NoValue);
    }
    [TestMethod]
    public void MayBind() {
        1.Maybe().Bind(e => 1.Maybe()).AssertEquals(1.Maybe());
        1.Maybe().Maybe().Bind(e => e).AssertEquals(1.Maybe());
        May<int>.NoValue.Bind(e => e.Maybe()).AssertEquals(May.NoValue);
    }
    [TestMethod]
    public void MaySelectMany() {
        (from e1 in 1.Maybe() from e2 in 2.Maybe() select e1 + e2).AssertEquals(3.Maybe());
        (from e1 in new May<int>() from e2 in 2.Maybe() select e1 + e2).AssertEquals(May.NoValue);
        (from e1 in new May<int>() from e2 in new May<int>() select e1 + e2).AssertEquals(May.NoValue);
        (from e1 in 1.Maybe() from e2 in new May<int>() select e1 + e2).AssertEquals(May.NoValue);
    }
    [TestMethod]
    public void MayWhere() {
        Assert.IsTrue(1.Maybe().Where(e => e == 1) == 1.Maybe());
        Assert.IsTrue(2.Maybe().Where(e => e == 1) == May.NoValue);
        Assert.IsTrue(new May<int>().Where(e => e == 1) == May.NoValue);
    }
    [TestMethod]
    public void MayNullable() {
        1.Maybe().AsNullable().AssertEquals(1);
        new May<int>().AsNullable().AssertEquals((int?)null);
        ((int?)null).AsMay().AssertEquals(May.NoValue);
        ((int?)1).AsMay().AssertEquals(1.Maybe());
    }
    [TestMethod]
    public void MayCombine() {
        var r = Enumerable.Range(0, 10).Select(e => e.Maybe()).ToArray();
        
        // all present
        r[0].Combine(r[1], (e0, e1) => e0 + e1).AssertEquals(1.Maybe());
        r[0].Combine(r[1], r[2], (e0, e1, e2) => e0 + e1 + e2).AssertEquals(3.Maybe());
        r[0].Combine(r[1], r[2], r[3], (e0, e1, e2, e3) => e0 + e1 + e2 + e3).AssertEquals(6.Maybe());
        //r[0].Combine(r[1], r[2], r[3], r[4], (e0, e1, e2, e3, e4) => e0 + e1 + e2 + e3 + e4).AssertEquals(10.Maybe());
        
        // one missing
        for (var i = 0; i < r.Length; i++) {
            var s = r.ToArray();
            s[i] = May.NoValue;
            if (i < 2) s[0].Combine(s[1], (e0, e1) => e0 + e1).AssertEquals(May.NoValue);
            if (i < 3) s[0].Combine(s[1], s[2], (e0, e1, e2) => e0 + e1 + e2).AssertEquals(May.NoValue);
            if (i < 4) s[0].Combine(s[1], s[2], s[3], (e0, e1, e2, e3) => e0 + e1 + e2 + e3).AssertEquals(May.NoValue);
            //if (i < 5) s[0].Combine(s[1], s[2], s[3], s[4], (e0, e1, e2, e3, e4) => e0 + e1 + e2 + e3 + e4).AssertEquals(May.NoValue);
        }
    }
}
