//---------------------------------------------------------------------------
//      Класс TSolver предназначен для решения САУ - прогонкой 
//                              Потапов И.И.
//                        - (C) Copyright 2015 -
//                          ALL RIGHT RESERVED
//                               31.07.15
//---------------------------------------------------------------------------
//          Реализация библиотеки для решения одномерных задач
//---------------------------------------------------------------------------
using System;

namespace RiverTaskLibrary
{
    /// <summary>
    /// Tri-Diagonal Matrix Algorithm
    /// </summary>
    class TSolver
    {
        /// <summary>
        /// вектор верхней кодиагонали
        /// </summary>
        double[] p = null;
        /// <summary>
        /// вектор правой части
        /// </summary>
        double[] q = null;
        /// <summary>
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="imax">Значение i для последнего КО по Х</param>
        /// <param name="jmax">Значение j для последнего КО по Y</param>
        public TSolver(int Count)
        {
            p = new double[Count];
            q = new double[Count];
        }
        /// <summary>
        /// Метод техдиагональной матричной прогонки 
        /// </summary>
        /// <param name="Aw">Коэффициенты дискретной схемы - запад</param>
        /// <param name="Ap">Коэффициенты дискретной схемы - центр</param>
        /// <param name="Ae">Коэффициенты дискретной схемы - восток</param>
        /// <param name="sc">Правая часть</param>
        /// <param name="X">Решение</param>
        public void Solver(double[] Aw, double[] Ap, double[] Ae, double[] sc, double[] X)
        {
            int imax = p.Length;
            double denom, temp;
            int i;
            // прямой ход для вычисления коэффициентов p и q 
            p[0] = 0;
            q[0] = X[0];
            for (i = 1; i < imax; i++)
            {
                denom = Ap[i] - Aw[i] * p[i - 1];
                p[i] = Ae[i] / denom;
                temp = sc[i];
                q[i] = (temp + Aw[i] * q[i - 1]) / denom;
            }
            // обратный ход
            for (i = imax - 1; i > 0; i--)
                X[i] = X[i + 1] * p[i] + q[i];
        }
    }
}
