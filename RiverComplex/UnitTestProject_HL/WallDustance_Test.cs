using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MeshLibrary;
using HydrodynamicLibrary;

namespace UnitTestProject_HL
{
    [TestClass]
    public class WallDustance_Test
    {
        [TestMethod]
        public void GetDistanceTest()
        {
            double[] Xt = new double[] { 0, 1, 2, 3, 4 };
            double[] Yt = new double[] { 2, 2, 2, 2, 2 };
            double[] Yb = new double[] { 0, 0, 0, 0, 0 };
            Area ar = new Area(Xt, Yt, Xt, Yb);
            Parameter[] Prs = new Parameter[1];
            Parameter p = new AlgParameter(5, 5, 0, 1.0, 1.0);
            Prs[0] = p;
            MeshBuilder mb = new MeshBuilder(ar, Prs, false);
            //
            mb.GenerateMesh();
            Mesh m = mb.FinalMesh;
            Assert.AreEqual(m.GetNormalDistanceBottom(6), (Yt[1] - Yb[1]) / 4.0);
            Assert.AreEqual(m.GetNormalDistanceTop(8), (Yt[1] - Yb[1]) / 4.0);
        }
    }
}
