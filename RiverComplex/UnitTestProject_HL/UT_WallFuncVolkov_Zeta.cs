using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiverTaskLibrary;

namespace UnitTestProject_HL
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UT_WallFuncVolkov_Zeta
    {

        [TestMethod]
        public void TestMethod1()
        {
            double B = 5.3, kappa = 0.4, rho = 1000, cm = 0.09, dy = 0.2 / 27.0, mu = 0.001, k = 0.0012;
            double cm14 = Math.Pow(cm, 0.25);
            double kp = k;
            double Dy = dy;
            double Re = rho * cm14 * Math.Sqrt(Math.Abs(kp)) * Dy / mu;
            //
            double u_plus = 0;
            double u_c = Math.Sqrt(Re);
            double f = 0, df = 0;
            //
            u_c = Math.Sqrt(Re);
            //
            for (int i = 0; i < 5; i++)
            {
                f = u_c + (Math.Exp(kappa * u_c) - 1 - kappa * u_c - 0.5 * kappa * u_c * kappa * u_c - 1.0 / 6.0 * kappa * u_c * kappa * u_c * kappa * u_c) * Math.Exp(-kappa * B) - Re / u_c;
                df = 1 + (kappa * Math.Exp(kappa * u_c) - kappa - kappa * kappa * u_c - 0.5 * kappa * kappa * kappa * u_c * u_c) * Math.Exp(-kappa * B) + Re / u_c / u_c;
                u_plus = u_c - f / df;
                u_c = u_plus;
            }
            Assert.AreEqual(u_plus, 10.07717396, 0.00000001);
            //
            u_c = 1.0 / kappa * Math.Log(Re) + B;
            //
            for (int i = 0; i < 5; i++)
            {
                f = u_c - B - 1.0 / kappa * Math.Log(Math.Exp(-kappa * B) * (1 + kappa * u_c + 1.0 / 2.0 * kappa * kappa * u_c * u_c + 1.0 / 6.0 * kappa * kappa * kappa * u_c * u_c * u_c) + Re / u_c - u_c);
                df = 1 - (Math.Exp(-kappa * B) * (kappa + kappa * kappa * u_c + 1.0 / 2.0 * kappa * kappa * kappa * u_c * u_c) - Re / u_c / u_c - 1) / (Math.Exp(-kappa * B) * (1 + kappa * u_c + 1.0 / 2.0 * kappa * kappa * u_c * u_c + 1.0 / 6.0 * kappa * kappa * kappa * u_c * u_c * u_c) + Re / u_c - u_c) / kappa;
                u_plus = u_c - f / df;
                u_c = u_plus;
            }
            Assert.AreEqual(u_plus, 10.07717396,0.00000001);
        }
        [TestMethod]
        public void RiverAlgCheck()
        {
            BedPhysicsParams p = new BedPhysicsParams_PPG();
            p.cx = 0.45;
            p.d = 0.00069;
            p.eps = 0.375;
            p.g = 9.8;
            p.kappa = 0.4;
            p.rhoS = 2650;
            p.rhoW = 1000;
            p.tf = 32;
            //
            int Nx = 700;
            double dt = 0.005;
            int Nt = 360;
            double L = 4.5;
            double dx = L / (Nx - 1);

            double[] Tau = new double[Nx];
            double x = 0;
            double Fa = (p.rhoS - p.rhoW) * p.g * p.tf;
            double tauC = 3.0 / 8.0 * p.kappa * p.kappa * Fa * p.d / p.cx;
            double[] X = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                x = dx * i;
                X[i] = x;
                Tau[i] = 16 * tauC * Math.Cos(2 * Math.PI * x);
            }
            BedLoadTask_PPG task1 = new BedLoadTask_PPG();
            RiverTaskLibrary.BoundaryCondition bc = new RiverTaskLibrary.BoundaryCondition();
            task1.ReStartBaseBedLoadTask(bc, p, dx, Nx, dt);
            task1.MaxIterNoLine = 15;
            double[] Zzz = task1.MySolve(Tau, null, null, Nt);
            BedLoadTask_PPG task2 = new BedLoadTask_PPG();
            bc.Inlet = RiverTaskLibrary.TypeBoundaryCondition.Neumann_boundary_conditions;
            bc.Outlet = RiverTaskLibrary.TypeBoundaryCondition.Neumann_boundary_conditions;
            task2.ReStartBaseBedLoadTask(bc, p, dx, Nx, dt);
            task2.MaxIterNoLine = 15;
            double[] Zz1 = null;
            for (int i = 0; i < Nt; i++)
                Zz1 = task2.Solve(Tau);
            double[][] zz = new double[3][];
            zz[0] = Zzz;
            zz[1] = Zz1;
            zz[2] = Tau;
            //ChartForm ch = new ChartForm();
            //ch.DataX = X;
            //ch.DataY = zz;
            //ch.Names = new string[] { "mySolve", "Solve", "Tau" };
            //ch.Show();
        }
    }
}
