using System;
using MeshLibrary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationLibrary
{
    public class IsoLinesBitmap
    {
        Mesh M;
        Bitmap b;
        public Bitmap GetBitmap
        {
            get { return b; }
        }
        //
        Graphics g;
        Pen[] pens;
        Pen penBlack;
        float widthPen;
        int Count;
        //
        int WS, HS;
        public int W
        {
            get { return WS; }
        }
        public int H
        {
            get { return HS; }
        }
        //
        bool black = false;
        public bool Black
        {
            get { return black; }
            set { black = value; }
        }
        //
        int IsoLine = 10;
        double[] X01, Y01;
        double[] BCountsX01, BCountsY01;
        public int IsoCount
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
        public IsoLinesBitmap(Mesh M, int WS, int HS, Color MaxColor, Color MinColor, float widthPen, int IsoCount)
        {
            this.M = M;
            this.WS = WS; 
            this.HS = HS;
            this.widthPen = widthPen;
            //
            IsoLine = IsoCount;
            Count = M.CountKnots;
            //
            double MinX = M.X.Min();
            double MX = M.X.Max() - MinX;
            double MinY = M.Y.Min();
            double MaxY = M.Y.Max();
            double MY = MaxY - MinY;
            //обезразмеренные координаты сетки
            X01 = new double[Count];
            Y01 = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                X01[i] = (M.X[i] - MinX) / MX;
                Y01[i] = (MY - (M.Y[i] - MinY)) / MY;
            }
            //получение градиента для данного количества векторов (+1 пиксель к стрелке = + 1 к градиенту цвета)
            ColorGradient cg = new ColorGradient();
            Color[] colors = cg.GetColorIsoLines(IsoLine, MaxColor, MinColor);
            pens = new Pen[IsoLine];
            //
            for (int i = 0; i < IsoLine; i++)
                pens[i] = new Pen(colors[i], widthPen);
            penBlack = new Pen(Color.Black, widthPen);
            //
            int ch = 0;
            BCountsX01 = new double[M.CountBottom * 2 + M.CountLeft * 2];
            BCountsY01 = new double[M.CountBottom * 2 + M.CountLeft * 2];
            for (int i = 0; i < M.CountLeft; i++)
            {
                BCountsX01[ch] = X01[M.LeftKnots[i]];
                BCountsY01[ch++] = Y01[M.LeftKnots[i]];
            }

            for (int i = 0; i < M.CountBottom; i++)
            {
                BCountsX01[ch] = X01[M.BottomKnots[i]];
                BCountsY01[ch++] = Y01[M.BottomKnots[i]];
            }
            for (int i = 0; i < M.CountLeft; i++)
            {
                BCountsX01[ch] = X01[M.RightKnots[i]];
                BCountsY01[ch++] = Y01[M.RightKnots[i]];
            }
            for (int i = 0; i < M.CountBottom; i++)
            {
                BCountsX01[ch] = X01[M.TopKnots[i]];
                BCountsY01[ch++] = Y01[M.TopKnots[i]];
            }


        }
        public void CalcIsoLines(double[] F, double Max=0, double Min=0)
        {
            try
            {
                //
                X = new double[IsoLine][];
                Y = new double[IsoLine][];
                List<double>[] CoordsX = new List<double>[IsoLine];
                List<double>[] CoordsY = new List<double>[IsoLine];
                Pen pen0 = new Pen(Color.Black, 1);
                
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
                    Max = F.Max();
                    Min = F.Min();
                }
                // шаг изолиний  maxv=1
                double DV = (Max - Min) / (IsoLine + 1);
                for (uint i = 0; i < IsoLine; i++)
                {
                    double Value = Min + DV * (i + 1);
                    IsoValue[i] = Value;
                }
                double[] Xn = { 0, 0, 0 };
                double[] Yn = { 0, 0, 0 };
                double[] Fn = { 0, 0, 0 };
                // цикл по КЭ
                for (int elem = 0; elem < M.CountElems; elem++)
                {
                    // цикл по узлам КЭ
                    double pmin = 0, pmax = 0;
                    int[] Knot = M.AreaElems[elem];
                    for (uint i = 0; i < 3; i++)
                    {

                        Xn[i] = X01[Knot[i]];
                        Yn[i] = Y01[Knot[i]];
                        Fn[i] = F[Knot[i]];
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
                    double[] xline = { 0, 0, 0, 0 };
                    double[] yline = { 0, 0, 0, 0 };
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

                                if (Math.Abs(pa - pb) < 0.0000001) continue;
                                // --- условие прохождения изолинии через грань
                                if ((pa >= pt && pt >= pb) || (pb >= pt && pt >= pa))
                                {
                                    // работа с пропорцией
                                    double xt = (2.0 * pt - pa - pb) / (pb - pa);
                                    xline[liz] = 0.5 * ((1.0 - xt) * xN + (1.0 + xt) * xE);
                                    yline[liz] = 0.5 * ((1.0 - xt) * yN + (1.0 + xt) * yE);
                                    liz++;
                                }
                            } // -- конец цикла по граням --------------------
                            // запись изолинии
                            if (liz > 1)
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
            }
            catch { }
        }
        //
        public void DrawIsoLines(int px, int py, int Wzoomed, int Hzoomed)
        {
            b = new Bitmap(WS + 1, HS + 1);
            g = Graphics.FromImage(b);
            //
            int[][] Xpx = new int[IsoLine][];
            int[][] Ypx = new int[IsoLine][];
            double[] F01 = new double[Count];
            //
            for (int i = 0; i < IsoLine; i++)
            {
                int count = X[i].Length;
                Xpx[i] = new int[count];
                Ypx[i] = new int[count];
                for (int j = 0; j < count; j++)
                {
                    // кооринаты сетки в пикселях
                    Xpx[i][j] = Convert.ToInt32(X[i][j] * Wzoomed);
                    Ypx[i][j] = Convert.ToInt32(Y[i][j] * Hzoomed);
                }
            }
            //
            float size = 11.0f;
             //координаты в пикселях всех границ отрисовки с учетом отражения Y
                int xl = px; 
                int xr = px + WS;
                int yt = py; 
                int yb = py + HS;
                //

                for (int l = 0; l < IsoLine; l++)
                {
                    for (int i = 0; i < X[l].Length - 1; i++)
                    {
                        //если координаты узла выходят за границу отрисовки, то не отрисовываем
                        if ((Math.Min(Xpx[l][i], Xpx[l][i+1]) < xl) || (Math.Max(Xpx[l][i], Xpx[l][i+1]) > xr))
                            continue;
                        if ((Math.Min(Ypx[l][i], Ypx[l][i+1]) < yt) || (Math.Max(Ypx[l][i], Ypx[l][i+1]) > yb))
                            continue;
                        Point Point1 = new Point(Xpx[l][i] - xl, Ypx[l][i] - yt);
                        Point Point2 = new Point(Xpx[l][i+1] - xl, Ypx[l][i+1] - yt);
                        //
                        g.DrawLine(pens[l], Point1, Point2);

                    }
                }
                if (black)
                {
                    Font f = new Font("Times New Roman", size);
                    Brush BrushText = Brushes.Black;
                    int c2 = 0;
                    for (int i = 0; i < IsoLine; i++)
                    {
                        c2 = X[i].Length / 2;
                        // координаты отрисовки цифр
                        int cx = Xpx[i][c2];
                        int cy = Ypx[i][c2];
                        string res = String.Format("{0:f2}", IsoValue[i]);
                        //
                        RectangleF recWhite;
                        if (Math.Abs(Xpx[i][c2] - Xpx[i][c2 + 3]) > Math.Abs(Ypx[i][c2] - Ypx[i][c2 + 3]))
                        {
                            recWhite = new RectangleF(cx, cy, size * (res.Length - 1), size * 2);
                            cy -= (int)size;
                        }
                        else
                        {
                            recWhite = new RectangleF(cx, cy, size * 2, size * 2);
                            cx -= (int)size * 2;
                        }
                        g.FillRectangle(Brushes.White, recWhite);
                        Point point = new Point(cx, cy);

                        g.DrawString(res, f, BrushText, point);
                        // изменение координат
                    }
                }
            // дописать отрисовку области
                int x1, x2, y1, y2;
                for (int i = 0; i < BCountsX01.Length-1; i++)
                {
                    x1 = Convert.ToInt32(BCountsX01[i] * Wzoomed);
                    x2 = Convert.ToInt32(BCountsX01[i+1] * Wzoomed);
                    y1 = Convert.ToInt32(BCountsY01[i] * Hzoomed);
                    y2 = Convert.ToInt32(BCountsY01[i + 1] * Hzoomed);
                    if ((Math.Min(x1, x2) < xl) || (Math.Max(x1, x2) > xr))
                        continue;
                    if ((Math.Min(y1, y2) < yt) || (Math.Max(y1, y2) > yb))
                        continue;
                    //
                    g.DrawLine(penBlack, x1 - xl, y1 - yt, x2 - xl, y2 - yt);
                }
        
                        
        }

        public void Izo(Mesh Mesh, double[] Massive, double Max, double Min, int CountIsoLines)
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
            double DV = (Max - Min) / (IsoLine + 1);
            for (uint i = 0; i < IsoLine; i++)
            {
                double Value = Min + DV * (i + 1);
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
                double[] xline = { 0, 0, 0, 0 };
                double[] yline = { 0, 0, 0, 0 };
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

                            if (Math.Abs(pa - pb) < 0.0000001) continue;
                            // --- условие прохождения изолинии через грань
                            if ((pa >= pt && pt >= pb) || (pb >= pt && pt >= pa))
                            {
                                // работа с пропорцией
                                double xt = (2.0 * pt - pa - pb) / (pb - pa);
                                xline[liz] = 0.5 * ((1.0 - xt) * xN + (1.0 + xt) * xE);
                                yline[liz] = 0.5 * ((1.0 - xt) * yN + (1.0 + xt) * yE);
                                liz++;
                            }
                        } // -- конец цикла по граням --------------------
                        // запись изолинии
                        if (liz > 1)
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


        

    }
}
