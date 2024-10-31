using MeshLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationLibrary
{
    public class ColorVectors
    {
        Mesh M;
        Bitmap b;
        public Bitmap GetBitmap
        {
            get { return b; }
        }
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
        /// <summary>
        /// кратность отображения векторов (каждая CX в строка отображает вектора)
        /// </summary>
        int CX;
        /// <summary>
        /// кратность отображения векторов (каждsq CY в cnjk,br отображает вектора)
        /// </summary>
        int CY;
        /// <summary>
        /// Длина максимальной стрелки в пикселях
        /// </summary>
        int MaxArrowLength = 21;
        double[] X01, Y01;
        int Nx, Ny;
        //
        bool black = false;
        public bool Black
        {
            get { return black; }
            set { black = value;} 
        }
        /// <summary>
        /// Цветные вектора
        /// </summary>
        /// <param name="M">Сетка</param>
        /// <param name="WS">Ширина окна</param>
        /// <param name="HS">Высота окна</param>
        /// <param name="CX">Каждую какую строку векторизовать</param>
        /// <param name="CY">Каждый какой столбец векторизовать</param>
        /// <param name="MaxColor">Цвет, соответствующий минимальному вектору</param>
        /// <param name="MinColor">Цвет, соответствующий максимальному вектору</param>
        /// <param name="widthPen">Толщина вектора</param>
        /// <param name="MaxArrowLength">Длина максимального вектора в пикселях</param>
        public ColorVectors(Mesh M, int WS, int HS, int CX, int CY, Color MaxColor, Color MinColor, float widthPen, int MaxArrowLength=21)
        {
            this.M = M;
            this.WS = WS; 
            this.HS = HS;
            this.CX = CX;
            this.CY = CY;
            this.Nx = M.CountBottom;
            this.Ny = M.CountLeft;
            this.MaxArrowLength = MaxArrowLength;
            this.widthPen = widthPen;
            //
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
            //for (int i = 0; i < Count; i++)
            //{
            //    X01[i] = (M.X[i] - MinX) / MX;
            //    Y01[i] = (MaxY - (M.Y[i] - MinY)) / MY;
            //}
            int idx = 0;
            for (int i = 0; i < Nx; i += CX)
            {
                for (int j = 0; j < Ny; j += CY)
                {
                    idx = i * Ny + j;
                    X01[idx] = (M.X[idx] - MinX) / MX;
                    Y01[idx] = (MY - (M.Y[idx] - MinY)) / MY;
                }
            }
            //получение градиента для данного количества векторов (+1 пиксель к стрелке = + 1 к градиенту цвета)
            ColorGradient cg = new ColorGradient();
            Color[] colors = cg.GetColorIsoLines(MaxArrowLength, MaxColor, MinColor);
            pens = new Pen[MaxArrowLength];
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
            //
            for (int i = 0; i < MaxArrowLength; i++)
            {
                pens[i] = new Pen(colors[i], widthPen);
                //pens[i].EndCap = LineCap.ArrowAnchor;// стрелка на конце
                pens[i].CustomEndCap = bigArrow;
            }
            penBlack = new Pen(Color.Black, widthPen);
            penBlack.CustomEndCap = bigArrow;
        }
        //
        public void DrawColorVectors(double[] U, double [] V, int W, int H)
        {
            try
            {
                //
                b = new Bitmap(WS + MaxArrowLength+1, HS + MaxArrowLength+1);
                g = Graphics.FromImage(b);
                //
                int[] Xpx = new int[Count];
                int[] Ypx = new int[Count];
                double[] F01 = new double[Count];
                //
                for (int i = 0; i < Count; i++)
                {
                    // кооринаты сетки в пикселях
                    Xpx[i] = Convert.ToInt32(X01[i] * W);
                    Ypx[i] = Convert.ToInt32(Y01[i] * H);
                }
                //
                double MaxU = U.Max();
                double MinU = U.Min();
                double MaxV = V.Max();
                double MinV = V.Min();
                // какое изменение функции воответствует +1 пикселю
                double FonPx = ((MaxU - MinU) + (MaxV - MinV)) / MaxArrowLength;
                //
                int xpx, ypx, xa, ya; int idx, dux, dvy;
                for (int i = 0; i < Nx; i += CX)
                {
                    for (int j = 0; j < Ny; j += CY)
                    {
                        idx = i * Ny + j;
                        // координаты рассматриваемого узла
                        xpx = Xpx[idx];
                        ypx = Ypx[idx];
                        //координаты проекций вертикальной и горизонтальной функции
                        dux = (int)((U[idx] - MinU) / FonPx);
                        dvy = Convert.ToInt32((V[idx] - MinV) / FonPx);
                        xa = xpx + dux;
                        ya = ypx - dvy;
                        // рисуем стрелку
                        if(black==false)
                            g.DrawLine(pens[dux + dvy], xpx, ypx, xa, ya);
                        else
                            g.DrawLine(penBlack, xpx, ypx, xa, ya);
                    }
                }
            }
            catch (Exception ex)
            { }
                
        }
        /// <summary>
        /// Прорисовка увеличенного фрагмента изображения
        /// </summary>
        /// <param name="U">горионтальная функция</param>
        /// <param name="V">вертикальная функция</param>
        /// <param name="px">X-координата левого верхнего угла прямоугольника отрисовки</param>
        /// <param name="py">Y-координата левого верхнего угла прямоугольника отрисовки</param>
        /// <param name="Wzoomed">Общая ширина рисунка (увеличенного) </param>
        /// <param name="Hzoomed">Общая высота рисунка (увеличенного) </param>
        public void DrawColorVectors(double[] U, double[] V,int px, int py, int Wzoomed, int Hzoomed)
        {
            try
            {
                //
                b = new Bitmap(WS + MaxArrowLength+1, HS + MaxArrowLength+1);
                g = Graphics.FromImage(b);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //
                int[] Xpx = new int[Count];
                int[] Ypx = new int[Count];
                double[] F01 = new double[Count];
                //
                for (int i = 0; i < Count; i++)
                {
                    // кооринаты сетки в пикселях (увеличенной мастаб) с учетом отражения Y
                    Xpx[i] = Convert.ToInt32(X01[i] * Wzoomed);
                    Ypx[i] = Convert.ToInt32(Y01[i] * Hzoomed);
                }
                //
                double MaxU = U.Max();
                double MinU = U.Min();
                double MaxV = V.Max();
                double MinV = V.Min();
                // какое изменение функции воответствует +1 пикселю
                double FonPx = ((MaxU - MinU) + (MaxV - MinV)) / MaxArrowLength;
                //координаты в пикселях всех границ отрисовки с учетом отражения Y
                int xl = px; 
                int xr = px + WS;
                int yt = py; 
                int yb = py + HS;
                //
                int xpx, ypx, xa, ya; int idx, dux, dvy;
                //
                for (int i = 0; i < Nx; i += CX)
                {
                    for (int j = 0; j < Ny; j += CY)
                    {
                        idx = i * Ny + j;
                        // координаты рассматриваемого узла
                        xpx = Xpx[idx];
                        ypx = Ypx[idx];
                        //если координаты узла выходят за границу отрисовки, то не отрисовываем
                        if ((xpx < xl) || (xpx > xr))
                            continue;
                        if ((ypx < yt) || (ypx > yb))
                            continue;
                        //координаты проекций вертикальной и горизонтальной функции
                        dux = Convert.ToInt32((U[idx] - MinU) / FonPx);
                        dvy = Convert.ToInt32((V[idx] - MinV) / FonPx);
                        xa = xpx + dux;
                        ya = ypx - dvy;
                        // рисуем стрелку
                        if(black == false)
                            g.DrawLine(pens[dux + dvy], xpx - xl, ypx - yt, xa - xl, ya - yt);
                        else
                            g.DrawLine(penBlack, xpx - xl, ypx - yt, xa - xl, ya - yt);
                    }
                }
            }
            catch (Exception ex)
            { }

        }
        public void line()
        {
           
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255), 8);
            pen.StartCap = LineCap.ArrowAnchor;
            pen.EndCap = LineCap.RoundAnchor;
            g.DrawLine(pen, 20, 175, 300, 175);
        }
    }
}
