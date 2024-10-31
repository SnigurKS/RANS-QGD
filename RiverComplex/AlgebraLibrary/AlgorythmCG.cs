using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
     [Serializable]
    public class AlgorythmCG: Algorythm
    {
        protected double[] r;
        protected double[] r0;
        protected double[] p;
        protected double rho = 0, beta = 0, alpha = 0;
        protected double er = 0.0001; // значимые цифры float 6-7 
         //
        public override string Name
        {
            get { return "Метод сопряженных градиентов"; }
        }
        public override void Solve(SBand A)
        {
            try
            {
                int N = A.Size;
                double[] X = A.GetX;// Начальное приближение
                double[] z = new double[N];
                double[] Right = A.GetRight;
                InitialMassives(N);
                //начальная невязка
                r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                for (int j = 0; j < N; j++)
                    z[j] = r[j];
                //
                for (int i = 1; i < N + 1; i++)
                {
                    rho = A.Multiplucation(r, r);
                    alpha = rho / A.Multiplucation(A.MultiplyToMatrix(z), z);
                    //
                    p = A.MultiplyToMatrix(z);
                    for (int j = 0; j < N; j++)
                    {
                        r0[j] = r[j];
                        X[j] += alpha * z[j];
                        r[j] -= alpha * p[j];
                    }
                    //встряска решения
                    if (i % 1000 == 0)
                        r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                    //
                    beta = A.Multiplucation(r, r) / A.Multiplucation(r0, r0);
                    //
                    if (Math.Abs(MinMax(z) * alpha / MinMax(X)) < er)
                        break;
                    //
                    for (int j = 0; j < N; j++)
                        z[j] = r[j] + beta * z[j];

                }
                //
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }
        public override void Solve(SRowPacked A)
        {
            try
            {
                int N = A.Size;
                double [] X = A.GetX;// Начальное приближение
                double[] z = new double[N];
                double[] Right = A.GetRight;
                InitialMassives(N);
                //начальная невязка
                r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                for (int j = 0; j < N; j++)
                    z[j] = r[j];
                //
                for (int i = 1; i < N + 1; i++)
                {
                    rho = A.Multiplucation(r, r);
                    alpha = rho / A.Multiplucation(A.MultiplyToMatrix(z), z);
                    //
                    p = A.MultiplyToMatrix(z);
                    for (int j = 0; j < N; j++)
                    {
                        r0[j] = r[j];
                        X[j] += alpha * z[j];
                        r[j] -= alpha * p[j];
                    }
                    //встряска решения
                    if (i % 1000 == 0)
                        r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                    //
                    beta = A.Multiplucation(r, r) / A.Multiplucation(r0, r0);
                    //
                    if (Math.Abs(MinMax(z) * alpha / MinMax(X)) < er)
                        break;
                    //
                    for (int j = 0; j < N; j++)
                        z[j] = r[j] + beta * z[j];

                }
                //
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }

        public override void Solve(SSquare A)
        {
            try
            {
                int N = A.Size;
                double[] X = A.GetX;// Начальное приближение
                double[] z = new double[N];
                double[] Right = A.GetRight;
                InitialMassives(N);
                //начальная невязка
                r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                for (int j = 0; j < N; j++)
                    z[j] = r[j];
                //
                for (int i = 1; i < N + 1; i++)
                {
                    rho = A.Multiplucation(r, r);
                    alpha = rho / A.Multiplucation(A.MultiplyToMatrix(z), z);
                    //
                    p = A.MultiplyToMatrix(z);
                    for (int j = 0; j < N; j++)
                    {
                        r0[j] = r[j];
                        X[j] += alpha * z[j];
                        r[j] -= alpha * p[j];
                    }
                    //встряска решения
                    if (i % 1000 == 0)
                        r = A.Subtraction(Right, A.MultiplyToMatrix(X));
                    //
                    beta = A.Multiplucation(r, r) / A.Multiplucation(r0, r0);
                    //
                    if (Math.Abs(MinMax(z) * alpha / MinMax(X)) < er)
                        break;
                    //
                    for (int j = 0; j < N; j++)
                        z[j] = r[j] + beta * z[j];

                }
                //
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }

        public double MinMax(double[] m)
        {
            double min = 100000;
            double max = 0;
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] < min)
                    min = m[i];
                if (m[i] > max)
                    max = m[i];
            }
            return Math.Abs(max - min);
        }
        //
        protected virtual void InitialMassives(int Size)
        {
            r = new double[Size];
            r0 = new double[Size];
            p = new double[Size];
        }
    }
}
