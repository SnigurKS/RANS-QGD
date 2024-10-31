using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
     [Serializable]
    public class AlgorythmBiStableCG: AlgorythmCG
    {
         public override string Name
         {
             get
             {
                 return "Стабильный метод бисопряженных градиентов";
             }
         }
        double rho0 = 1, omega = 1;
        //
        double[] p0;
        double[] z;
        double[] s;
        double[] q;
        double[] qt;
        double[] t;
        double[] v;
        double[] rt;
        public override void Solve(SBand A)
        {
            try
            {
                int N = A.Size;
                double[] X = A.GetX;// Начальное приближение
                double[] Right = A.GetRight;
                InitialMassives(N);
                //
                r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                for (int j = 0; j < N; j++)
                    rt[j] = r[j];
                //
                rho = 1; alpha = 1; beta = 0;
                for (int i = 1; i < N + 1; i++)
                {
                    rho = A.Multiplucation(rt, r);
                    beta = rho / rho0 * alpha / omega;
                    for (int j = 0; j < N; j++)
                        p[j] = r[j] + beta * (p[j] - omega * v[j]);
                    v = A.MultiplyToMatrix(p);
                    alpha = rho / A.Multiplucation(rt, v);
                    for (int j = 0; j < N; j++)
                        s[j] = r[j] - alpha * v[j];
                    t = A.MultiplyToMatrix(s);
                    omega = A.Multiplucation(t, s) / A.Multiplucation(t, t);
                    for (int j = 0; j < N; j++)
                    {
                        X[j] = X[j] + omega * s[j] + alpha * p[j];
                        r[j] = s[j] - omega * t[j];
                    }
                    //встряска решения
                    if (i % 1000 == 0)
                        r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                    //
                    rho0 = rho;
                    //
                    if (Math.Abs((MinMax(s) * omega + alpha * MinMax(p)) / MinMax(X)) < er)
                        break;
                }
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += "AlgorythmBiStableCG" + e.Message.ToString();
            }
        }
        //
        public override void Solve(SRowPacked A)
        {
            try
            {
                int N = A.Size;
                double[] X = A.GetX;// Начальное приближение
                double[] Right = A.GetRight;
                InitialMassives(N);
                //
                r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                for (int j = 0; j < N; j++)
                    rt[j] = r[j];
                //
                rho = 1; alpha = 1; beta = 0;
                for (int i = 1; i < N + 1; i++)
                {
                    rho = A.Multiplucation(rt, r);
                    beta = rho / rho0 * alpha / omega;
                    for (int j = 0; j < N; j++)
                        p[j] = r[j] + beta * (p[j] - omega * v[j]);
                    v = A.MultiplyToMatrix(p);
                    alpha = rho / A.Multiplucation(rt, v);
                    for (int j = 0; j < N; j++)
                        s[j] = r[j] - alpha * v[j];
                    t = A.MultiplyToMatrix(s);
                    omega = A.Multiplucation(t, s) / A.Multiplucation(t, t);
                    for (int j = 0; j < N; j++)
                    {
                        X[j] = X[j] + omega * s[j] + alpha * p[j];
                        r[j] = s[j] - omega * t[j];
                    }
                    //встряска решения
                    if (i % 1000 == 0)
                        r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                    //
                    rho0 = rho;
                    //
                    if (Math.Abs((MinMax(s) * omega + alpha * MinMax(p)) / MinMax(X)) < er)
                        break;
                }
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += "AlgorythmBiStableCG" + e.Message.ToString();
            }
        }
        public override void Solve(SSquare A)
        {
            try
            {
                int N = A.Size;
                double[] X = A.GetX;// Начальное приближение
                double[] Right = A.GetRight;
                InitialMassives(N);
                //
                r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                for (int j = 0; j < N; j++)
                    rt[j] = r[j];
                //
                rho = 1; alpha = 1; beta = 0;
                for (int i = 1; i < N + 1; i++)
                {
                    rho = A.Multiplucation(rt, r);
                    beta = rho / rho0 * alpha / omega;
                    for (int j = 0; j < N; j++)
                        p[j] = r[j] + beta * (p[j] - omega * v[j]);
                    v = A.MultiplyToMatrix(p);
                    alpha = rho / A.Multiplucation(rt, v);
                    for (int j = 0; j < N; j++)
                        s[j] = r[j] - alpha * v[j];
                    t = A.MultiplyToMatrix(s);
                    omega = A.Multiplucation(t, s) / A.Multiplucation(t, t);
                    for (int j = 0; j < N; j++)
                    {
                        X[j] = X[j] + omega * s[j] + alpha * p[j];
                        r[j] = s[j] - omega * t[j];
                    }
                    //встряска решения
                    if (i % 1000 == 0)
                        r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                    //
                    rho0 = rho;
                    //
                    if (Math.Abs((MinMax(s) * omega + alpha * MinMax(p)) / MinMax(X)) < er)
                        break;
                }
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += "AlgorythmBiStableCG" + e.Message.ToString();
            }
        }

        protected override void InitialMassives(int Size)
        {
            base.InitialMassives(Size);// r,r0,p
            //
            p0 = new double[Size];
            z = new double[Size];
            s = new double[Size];
            q = new double[Size];
            qt = new double[Size];
            t = new double[Size];
            v = new double[Size];
            rt = new double[Size];
        }
    }
}
