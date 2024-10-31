using System;
using MeshLibrary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationLibrary
{
    public class IsoLines
    {
        int IsoLine = 10;
        public int Count
        {
            get { return IsoLine; }
        }
        double[][] X, Y;
        public double[][] GetX
        {
            get
            {
                return X;
            }
        }
        public double[][] GetY
        {
            get
            {
                return Y;
            }
        }
        double[] IsoValue;
        public double Value(int idx)
        {
            return IsoValue[idx];
        }
        /// <summary>
        /// Генерация изолиний
        /// </summary>
        /// <param name="Mesh">Объект секти</param>
        /// <param name="Massive">Маасив функции</param>
        /// <param name="Max">Верхний предел функции, в отором ищутся изолинии</param>
        /// <param name="Min">Нижний предел функции, в отором ищутся изолинии</param>
        /// <param name="CountIsoLines">Количество изолний</param>
        public double[] Izo(Mesh Mesh, double[] Massive, double Max, double Min, int CountIsoLines)
        {

            if (Massive != null)
            {
                for (int i = 0; i < Massive.Length; i++)
                {
                    if (double.IsNaN(Massive[i]))
                    {
                        Massive = null;
                        break;
                    }
                }
            }
            //
            if (Massive != null)
            {
                IsoLine = CountIsoLines;
                X = new double[IsoLine][];
                Y = new double[IsoLine][];
                List<double>[] CoordsX = new List<double>[IsoLine];
                List<double>[] CoordsY = new List<double>[IsoLine];
                Pen pen0 = new Pen(Color.Black, 1);
                #region Отрисовка изолиний
                // Вспомогательные массивы для отрисовки значений изолиний
                IsoValue = new double[IsoLine];
                for (uint i = 0; i < IsoLine; i++)
                {
                    IsoValue[i] = 0;
                    CoordsX[i] = new List<double>();
                    CoordsY[i] = new List<double>();
                }
                if ((Max == 0) && (Min == 0))
                {
                    Max = Massive.Max();
                    Min = Massive.Min();
                }
                // шаг изолиний  maxv=1
                double DV = (Max - Min) / (IsoLine - 1);
                for (uint i = 0; i < IsoLine; i++)
                {
                    double Value = Min + DV * i;
                    IsoValue[i] = Value;
                }
                double[] Xn = { 0, 0, 0 };
                double[] Yn = { 0, 0, 0 };
                double[] Fn = { 0, 0, 0 };
                // цикл по КЭ
                for (int elem = 0; elem < Mesh.CountElems; elem++)
                {
                    // цикл по узлам КЭ
                    double pmin = 0, pmax = 0;
                    int[] Knot = Mesh.AreaElems[elem];
                    for (uint i = 0; i < 3; i++)
                    {

                        Xn[i] = Mesh.X[Knot[i]];
                        Yn[i] = Mesh.Y[Knot[i]];
                        Fn[i] = Massive[Knot[i]];
                        if (i == 0)
                        {
                            pmin = Fn[i];
                            pmax = Fn[i];
                        }
                        else
                        {
                            if (pmin > Fn[i]) pmin = Fn[i];
                            if (pmax < Fn[i]) pmax = Fn[i];
                        }
                    }
                    //
                    // построение изолиний
                    double pt, pa, pb, xN, xE, yN, yE;
                    double[] xline = { 0, 0 };
                    double[] yline = { 0, 0 };
                    //
                    // цикл по изолиниям
                    uint liz;
                    for (uint l = 0; l < IsoLine; l++)
                    {
                        // значение изолинии
                        pt = IsoValue[l];
                        // условие наличия текущей изолинии в области КЭ
                        if (pmax >= pt && pmin <= pt)
                        {
                            liz = 0;
                            // ----- цикл по граням текущего элемента -----------
                            uint CountVert = 3;
                            for (uint m = 0; m < CountVert; m++)
                            {
                                pa = Fn[m]; pb = Fn[(m + 1) % CountVert];
                                xN = Xn[m]; xE = Xn[(m + 1) % CountVert];
                                yN = Yn[m]; yE = Yn[(m + 1) % CountVert];

                                double ps = Math.Abs(pa + pb + 0.0000001);
                                if (Math.Abs(pa - pb) / ps < 0.00001) continue;
                                // --- условие прохождения изолинии через грань
                                if ((pa >= pt && pt >= pb) || (pb >= pt && pt >= pa))
                                {
                                    // работа с пропорцией
                                    double xt = (2.0f * pt - pa - pb) / (pb - pa);
                                    xline[liz] = 0.5 * ((1.0 - xt) * xN + (1.0 + xt) * xE);
                                    yline[liz] = 0.5 * ((1.0 - xt) * yN + (1.0 + xt) * yE);
                                    liz++;
                                    if (liz == 2)
                                        break;

                                }
                            } // -- конец цикла по граням --------------------
                            // запись изолинии
                            if (liz == 2)
                            {
                                CoordsX[l].Add(xline[0]);
                                CoordsX[l].Add(xline[1]);
                                CoordsY[l].Add(yline[0]);
                                CoordsY[l].Add(yline[1]);
                            } // --- условие наличия изолинии на элементе ------
                        }
                    }// --- конец цикла по изолиниям на элементе ---------
                    //
                }//-----конец цикла по элементам-------
                for (int i = 0; i < IsoLine; i++)
                {
                    int count = CoordsX[i].Count;
                    X[i] = new double[count];
                    Y[i] = new double[count];
                    for (int j = 0; j < count; j++)
                    {
                        X[i][j] = CoordsX[i][j];
                        Y[i][j] = CoordsY[i][j];
                    }
                }
                #endregion
            }
            double[] MinMaxF = new double[2];
            MinMaxF[1] = Massive.Max();
            MinMaxF[0] = Massive.Min();
            return MinMaxF;
        }

    }
}
