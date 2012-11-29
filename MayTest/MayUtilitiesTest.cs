using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Strilanc.Value;
using System.Linq;

[TestClass]
public class MayUtilitiesTest {
    [TestMethod]
    public void MayAggregate() {
        0.Range().MayAggregate((e1, e2) => e1 + e2).AssertEquals(May.NoValue);
        2.Range().MayAggregate((e1, e2) => e1 * e2).AssertEquals(0.Maybe());
        5.Range().MayAggregate((e1, e2) => e1 + e2).AssertEquals(10.Maybe());
    }
    [TestMethod]
    public void MayMin() {
        0.Range().MayMin().AssertEquals(May.NoValue);
        2.Range().MayMin().AssertEquals(0.Maybe());
        4.Range().Skip(1).Reverse().MayMin().AssertEquals(1.Maybe());
    }
    [TestMethod]
    public void MayMax() {
        0.Range().MayMax().AssertEquals(May.NoValue);
        2.Range().MayMax().AssertEquals(1.Maybe());
        4.Range().Skip(1).Reverse().MayMax().AssertEquals(3.Maybe());
    }
    [TestMethod]
    public void MayMinBy() {
        0.Range().MayMinBy(e => -e).AssertEquals(May.NoValue);
        2.Range().MayMinBy(e => -e).AssertEquals(1.Maybe());
        4.Range().Skip(1).Reverse().MayMinBy(e => -e).AssertEquals(3.Maybe());
    }
    [TestMethod]
    public void MayMaxBy() {
        0.Range().MayMaxBy(e => -e).AssertEquals(May.NoValue);
        2.Range().MayMaxBy(e => -e).AssertEquals(0.Maybe());
        4.Range().Skip(1).Reverse().MayMaxBy(e => -e).AssertEquals(1.Maybe());
    }
    [TestMethod]
    public void MayFirst() {
        0.Range().MayFirst().AssertEquals(May.NoValue);
        1.Range().MayFirst().AssertEquals(0.Maybe());
        2.Range().MayFirst().AssertEquals(0.Maybe());
    }
    [TestMethod]
    public void MayLast() {
        0.Range().MayLast().AssertEquals(May.NoValue);
        1.Range().MayLast().AssertEquals(0.Maybe());
        2.Range().MayLast().AssertEquals(1.Maybe());
    }
    [TestMethod]
    public void MaySingle() {
        0.Range().MaySingle().AssertEquals(May.NoValue);
        1.Range().MaySingle().AssertEquals(0.Maybe());
        TestUtil.AssertThrows<ArgumentOutOfRangeException>(() =>
            2.Range().MaySingle());
    }
    [TestMethod]
    public void WhereHasValue() {
        0.Range().Select(e => e.Maybe()).WhereHasValue().SequenceEqual(0.Range()).AssertIsTrue();
        10.Range().Select(e => e.Maybe()).WhereHasValue().SequenceEqual(10.Range()).AssertIsTrue();
        10.Range().Select(e => e == 5 ? May.NoValue : e.Maybe()).WhereHasValue().SequenceEqual(10.Range().Where(e => e != 5)).AssertIsTrue();
    }
    [TestMethod]
    public void MayAll() {
        0.Range().Select(e => e.Maybe()).MayAll().Select(e => e.SequenceEqual(0.Range())).AssertEquals(true.Maybe());
        10.Range().Select(e => e.Maybe()).MayAll().Select(e => e.SequenceEqual(10.Range())).AssertEquals(true.Maybe());
        10.Range().Select(e => e == 5 ? May.NoValue : e.Maybe()).MayAll().AssertEquals(May.NoValue);
    }
    [TestMethod]
    public void MayNullable() {
        1.Maybe().AsNullable().AssertEquals(1);
        new May<int>().AsNullable().AssertEquals((int?)null);
        ((int?)null).AsMay().AssertEquals(May.NoValue);
        ((int?)1).AsMay().AssertEquals(1.Maybe());
    }
}
