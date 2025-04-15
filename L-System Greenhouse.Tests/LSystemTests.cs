namespace L_System_Greenhouse.Tests;

using System.Collections.Generic;
using LSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class LSystemTests
{
    [TestMethod]
    public void TestRewrite()
    {
        List<Production> productions = [
            new ('A', "FAB"),
            new ('B', "XABZ")
        ];
        
        LSystem lSystem = new(
            productions,
            "ABAB",
            2);

        using (CancellationTokenSource cancellationTokenSource = new())
        {
            Assert.AreEqual("FFABXABZXFABXABZZFFABXABZXFABXABZZ", lSystem.Rewrite(cancellationTokenSource.Token));
        }
    }
}
