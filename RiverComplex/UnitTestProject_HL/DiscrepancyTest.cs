using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiverComplex;

namespace UnitTestProject_HL
{
    [TestClass]
    public class DiscrepancyTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            double[] X = new double[] { 0.5, 0.5, 0.5, 0.5,   1.5, 1.5, 1.5, 1.5,   2.5, 2.5, 2.5, 2.5,   3.0, 3.0, 3.0, 3.0,   3.5, 3.5, 3.5, 3.5,   4.5, 4.5, 4.5, 4.5, };
            double[] Y = new double[] { 2, 3, 4, 5,   2.5, 3.5, 4, 5,   2, 3, 4, 5,   1.75, 2.75, 3.75, 5,   1.25, 2.5, 3.5, 5,   1, 2, 3, 5,   };
            double [] U = new double[] { 0.4, 0.5, 0.5, 0.5,   0.5, 0.5, 0.5, 0.5,    0.4, 0.5, 0.5, 0.5,    0.35, 0.5, 0.5, 0.5,    0.3, 0.5, 0.5, 0.5,   0, 0.4, 0.5, 0.5, };
            //
            double [] ex = new double[]{ 1, 1, 1, 1, 1, 1, 1,    2, 2, 2, 2, 2, 2,    3, 3, 3, 3, 3, 3,     4, 4, 4, 4, 4, 4, 4};
            double [] ey = new double[]{ 1, 1.5, 2, 2.5, 3, 3.5, 4,     1.5, 2, 2.5, 3, 3.5, 4,     1.5, 2, 2.5, 3, 3.5, 4,    1, 1.5, 2, 2.5, 3, 3.5, 4 };
            double [] eu = new double[]{ 0, 0.3, 0.4, 0.5, 0.5, 0.5, 0.5,     0.3, 0.4, 0.5, 0.5, 0.5, 0.5,     0.3, 0.4, 0.5, 0.5, 0.5, 0.5,   0, 0.3, 0.4, 0.5, 0.5, 0.5, 0.5 };
            //
            Form1 f1 = new Form1();
            double[][] C2 = f1.CalcDiscreapancy(X, Y, U, null, null, null, ex, ey, eu, null, null, null);
            Assert.AreEqual(4, C2.Length);
            Assert.AreEqual(3, C2[0].Length);
            Assert.AreEqual(3, C2[1].Length);
            Assert.AreEqual(3, C2[2].Length);
            Assert.AreEqual(2, C2[3].Length);
            for (int i = 0; i < C2.Length; i++)
                for (int j=0;j<C2[i].Length;j++)
                    Assert.AreEqual(0, C2[i][j]);
        }
    }
}
