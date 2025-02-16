﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeshLibrary;

namespace UnitTestProject_HL
{
    [TestClass]
    public class UnitTestCVMesh
    {
        [TestMethod]
        public void TestCVMeshStructureNotChanged()
        {
            double[] Xt = new double[] { 0, 1, 2, 3, 4 };
            double[] Yt = new double[] { 2, 2, 2, 2, 2 };
            double[] Yb = new double[] { 0, 0, 0, 0, 0 };
            Area ar = new Area(Xt, Yt, Xt, Yb);
            Parameter[] prs = new Parameter[] { new AlgParameter(10, 10, 0, 1, 1) };
            MeshBuilder mb = new MeshBuilder(ar, prs, true);
            //
            mb.GenerateMesh(true);
            Mesh MeshInitial = mb.FinalMesh;
            //
            mb.ChangeArea(ar);
            mb.GenerateMesh(false);
            Mesh MeshSecond = mb.FinalMesh;
            //
            Yb[0] = 0.05;
            Yb[1] = 0.05;
            Yb[2] = 0.05;
            Yb[3] = 0.05;
            Yb[4] = 0.05;
            ar = new Area(Xt, Yt, Xt, Yb);
            mb.ChangeArea(ar);
            mb.GenerateMesh(false);
            Mesh MeshThird = mb.FinalMesh;
            //
            //Assert.AreEqual(3, 5);
            for (int i = 0; i < MeshInitial.CVolumes.Length; i++)
            {
                Assert.AreEqual(MeshInitial.CVolumes[i].Length, MeshSecond.CVolumes[i].Length);
                for (int j = 0; j < MeshInitial.CVolumes[i].Length; j++)
                {
                    Assert.AreEqual(MeshInitial.CVolumes[i][j], MeshSecond.CVolumes[i][j]);
                    Assert.AreEqual(MeshInitial.CVolumes[i][j], MeshThird.CVolumes[i][j]);
                }
            }
            //
            for (int i = 0; i < MeshInitial.CTop.Length; i++)
            {
                Assert.AreEqual(MeshInitial.CTop[i], MeshSecond.CTop[i]);
            }
            //
            for (int i = 0; i < MeshInitial.CBottom.Length; i++)
            {
                Assert.AreEqual(MeshInitial.CBottom[i], MeshSecond.CBottom[i]);
            }
            //
            for (int i = 0; i < MeshInitial.CV_WallKnotsDistance.Length; i++)
            {
                Assert.AreEqual(MeshInitial.CV_WallKnotsDistance[i], MeshSecond.CV_WallKnotsDistance[i]);
            }
            //
            for (int i = 0; i < MeshInitial.S.Length; i++)
            {
                if (MeshInitial.S[i] != null)
                {
                    Assert.AreEqual(MeshInitial.S[i].Length, MeshSecond.S[i].Length);
                    for (int j = 0; j < MeshInitial.S[i].Length-1; j++)
                    {
                        Assert.AreEqual(MeshInitial.S[i][j], MeshSecond.S[i][j]);
                        Assert.AreNotEqual(MeshInitial.S[i][j], MeshThird.S[i][j]);
                    }
                }
            }
        }
    }
}
