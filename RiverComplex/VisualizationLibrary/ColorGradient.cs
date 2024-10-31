using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeshLibrary;

namespace VisualizationLibrary
{
    public class ColorGradient
    {
        Mesh M;
        Bitmap b;
        public Bitmap GetBitmap
        {
            get 
            {
                return b;
            }
        }
        int Count;
        //
        double[] X01, Y01;
        /// <summary>
        /// Цвет, применяющийся для максимального значения функции
        /// </summary>
        Color MaxColor;
        /// <summary>
        /// Цвет, применяющийся для минимального значения функции
        /// </summary>
        Color MinColor;
        /// <summary>
        /// Количество градаций между выбранными цветами MaxColor и MinColor
        /// </summary>
        int CountGradation;
        Color[] Colors;
        public int W, H;
        //
        int[][] m = new int[6][];
        int [][] per = new int[6][];
        public ColorGradient()
        {
            FillColorMatrix();
        }
        public ColorGradient(Color MaxColor, Color MinColor, Mesh M)
        {
            this.MaxColor = MaxColor;
            this.MinColor = MinColor;
            this.M = M;
            
            Count = M.CountKnots;

            double MinX = M.X.Min();
            double MX = M.X.Max() - MinX;
            double MinY = M.Y.Min();
            double MaxY = M.Y.Max();
            double MY = MaxY - MinY;
            //
            X01 = new double[Count];
            Y01 = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                X01[i] = (M.X[i] - MinX) / MX;
                Y01[i] = (MY - (M.Y[i] - MinY)) / MY;
            }
            //
            //
            FillColorMatrix();
            GetColorGradient();
        }
        public Bitmap FillArea(int W, int H, double[] F)
        {
            this.W = W; this.H = H;
            //
            int[] Xpx = new int[Count];
            int[] Ypx = new int[Count];
            double [] F01 = new double[Count];
            //
            double MinF = F.Min();
            double MaxF = F.Max();
            double MF = MaxF - MinF;
            // шаг по функции, соответствующий 1 градаци цвета
            double df = MF/(CountGradation-1);
            //
            for (int i = 0; i < Count; i++)
            {
                // обезразмеренная функция
                F01[i] = (F[i] - MinF) / MF;
                // кооринаты сетки в пикселях
                Xpx[i] = Convert.ToInt32(X01[i] * W);
                Ypx[i] = Convert.ToInt32(Y01[i] * H);
            }
            // битмап, в который все будет писаться
            b = new Bitmap(W+1, H+1);
            //
            int[] Knots = new int[3];
            int Nx, Ny, x, y, c;
            double dx, dy, eta, zeta, N0, N1, N2, n , f;
            int[] X = new int[3];
            int[] Y = new int[3];
            try
            {
                for (int i = 0; i < M.CountElems; i++)
                {
                    Knots = M.AreaElems[i];
                    X[0] = Xpx[Knots[0]]; X[1] = Xpx[Knots[1]]; X[2] = Xpx[Knots[2]];
                    Y[0] = Ypx[Knots[0]]; Y[1] = Ypx[Knots[1]]; Y[2] = Ypx[Knots[2]];
                    //
                    Nx = X.Max() - X.Min() + 10;
                    Ny = Y.Max() - Y.Min() + 10;
                    n = 1.0 * Nx / Ny;
                    dx = 1.0 / Nx;
                    dy = 1.0 / Ny;
                    //
                    for (int e = 0; e < Ny; e++)
                    {
                        eta = e * dy;
                        N1 = eta;
                        for (int z = 0; z < Nx - n * e; z++)
                        {
                            zeta = z * dx;
                            //
                            N0 = 1 - eta - zeta;
                            N2 = zeta;

                            //
                            x = Convert.ToInt32(N0 * X[0] + N1 * X[1] + N2 * X[2]);
                            y = Convert.ToInt32(N0 * Y[0] + N1 * Y[1] + N2 * Y[2]);
                            f = N0 * F[Knots[0]] + N1 * F[Knots[1]] + N2 * F[Knots[2]];
                            //
                            c = Convert.ToInt32((f - MinF) / df);
                            b.SetPixel(x, y, Colors[c]);
                        }
                    }
                }           
                //отрисовка в файл в хорошем разрешении
                OutBitmapFill(F);
                return b;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        void OutBitmapFill(double[] F)
        {
            
            int sc = 600;
            // множителями задаются пропорции
            this.W = Convert.ToInt32(sc* (M.X[M.RightKnots[0]]) - M.X[0]*4); this.H = Convert.ToInt32(sc * (M.Y[M.LeftKnots[0]] - M.Y[0]) * 2);
            // отрисовываем первую половину
            int[] Xpx = new int[Count / 2];
            int[] Ypx = new int[Count / 2];
            double[] F01 = new double[Count / 2];
            //
            double MinF = F.Min();
            double MaxF = F.Max();
            double MF = MaxF - MinF;
            // шаг по функции, соответствующий 1 градаци цвета
            double df = MF / (CountGradation - 1);
            //
            for (int i = 0; i < Count / 2; i++)
            {
                // обезразмеренная функция
                F01[i] = (F[i] - MinF) / MF;
                // кооринаты сетки в пикселях
                Xpx[i] = Convert.ToInt32(X01[i] * W);
                Ypx[i] = Convert.ToInt32(Y01[i] * H);
            }

            Bitmap Outb = new Bitmap(W + 1, H + 1);
            try
            {
                int[] Knots = new int[3];
                int Nx, Ny, x, y, c;
                double dx, dy, eta, zeta, N0, N1, N2, n, f;
                int[] X = new int[3];
                int[] Y = new int[3];
                for (int i = 0; i < M.CountElems; i++)
                {
                    Knots = M.AreaElems[i];
                    if (Knots.Max() < (Count / 2))
                    {
                        X[0] = Xpx[Knots[0]]; X[1] = Xpx[Knots[1]]; X[2] = Xpx[Knots[2]];
                        Y[0] = Ypx[Knots[0]]; Y[1] = Ypx[Knots[1]]; Y[2] = Ypx[Knots[2]];
                        //
                        Nx = X.Max() - X.Min() + 10;
                        Ny = Y.Max() - Y.Min() + 10;
                        n = 1.0 * Nx / Ny;
                        dx = 1.0 / Nx;
                        dy = 1.0 / Ny;
                        //
                        for (int e = 0; e < Ny; e++)
                        {
                            eta = e * dy;
                            N1 = eta;
                            for (int z = 0; z < Nx - n * e; z++)
                            {
                                zeta = z * dx;
                                //
                                N0 = 1 - eta - zeta;
                                N2 = zeta;

                                //
                                x = Convert.ToInt32(N0 * X[0] + N1 * X[1] + N2 * X[2]);
                                y = Convert.ToInt32(N0 * Y[0] + N1 * Y[1] + N2 * Y[2]);
                                f = N0 * F[Knots[0]] + N1 * F[Knots[1]] + N2 * F[Knots[2]];
                                //
                                c = Convert.ToInt32((f - MinF) / df);
                                Outb.SetPixel(x, y, Colors[c]);
                            }
                        }
                    }
                }
                //
                Outb.Save("Bitmap.png", System.Drawing.Imaging.ImageFormat.Jpeg);
                //Outb.Save("Bitmap" + DateTime.Now.Ticks.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch
            {
                Outb.Save("Bitmap.png", System.Drawing.Imaging.ImageFormat.Jpeg);
                //Outb.Save("Bitmap" + DateTime.Now.Ticks.ToString()+".png", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        /// <summary>
        /// Перерисовка фрагмента заливки
        /// </summary>
        /// <param name="px">X-координаты левого верхнего угла</param>
        /// <param name="py">Y-координаты левого верхнего угла</param>
        /// <param name="Wzoomed">Общая ширина увеличенного рисунка</param>
        /// <param name="Hzoomed">Общая высота увеличенного рисунка</param>
        /// <param name="F"></param>
        /// <returns></returns>
        public Bitmap FillPartArea(int px, int py, int Wzoomed, int Hzoomed, double[] F)
        {
            //
            int[] Xpx = new int[Count];
            int[] Ypx = new int[Count];
            double[] F01 = new double[Count];
            //
            double MinF = F.Min();
            double MaxF = F.Max();
            double MF = MaxF - MinF;
            // шаг по функции, соответствующий 1 градаци цвета
            double df = MF / (CountGradation - 1);
            //
            for (int i = 0; i < Count; i++)
            {
                // обезразмеренная функция
                F01[i] = (F[i] - MinF) / MF;
                // кооринаты сетки в пикселях
                Xpx[i] = Convert.ToInt32(X01[i] * Wzoomed);
                Ypx[i] = Convert.ToInt32(Y01[i] * Hzoomed);
            }
            // битмап, в который все будет писаться
            b = new Bitmap(W + 1, H + 1);
            //
            int xl = px; //- W / 2;
            int xr = px + W ;// / 2;
            int yt = py;// + H;// / 2;
            int yb = py + H;// -H;// / 2;
            //
            int[] Knots = new int[3];
            int Nx, Ny, x, y, c, XMin, XMax, YMin, YMax;
            double dx, dy, eta, zeta, N0, N1, N2, n, f;
            int[] X = new int[3];
            int[] Y = new int[3];
            try
            {
                for (int i = 0; i < M.CountElems; i++)
                {
                    Knots = M.AreaElems[i];
                    X[0] = Xpx[Knots[0]]; X[1] = Xpx[Knots[1]]; X[2] = Xpx[Knots[2]];
                    Y[0] = Ypx[Knots[0]]; Y[1] = Ypx[Knots[1]]; Y[2] = Ypx[Knots[2]];
                    XMax = X.Max();
                    XMin = X.Min();
                    YMax = Y.Max();
                    YMin = Y.Min();
                    //
                    if ((XMin < xl) || (XMax > xr))
                        continue;
                    if ((YMin < yt) || (YMax > yb))
                        continue;
                    //
                    Nx = XMax - XMin + 10;
                    Ny = YMax - YMin + 10;
                    n = 1.0 * Nx / Ny;
                    dx = 1.0 / Nx;
                    dy = 1.0 / Ny;
                    //
                    for (int e = 0; e < Ny; e++)
                    {
                        eta = e * dy;
                        N1 = eta;
                        for (int z = 0; z < Nx - n * e; z++)
                        {
                            zeta = z * dx;
                            //
                            N0 = 1 - eta - zeta;
                            N2 = zeta;
                            //
                            x = Convert.ToInt32(N0 * X[0] + N1 * X[1] + N2 * X[2] - xl);
                            y = Convert.ToInt32(N0 * Y[0] + N1 * Y[1] + N2 * Y[2] - yt);
                            f = N0 * F[Knots[0]] + N1 * F[Knots[1]] + N2 * F[Knots[2]];
                            //
                            c = Convert.ToInt32((f - MinF) / df);
                            b.SetPixel(x, y, Colors[c]);
                        }
                    }
                }
                //
                return b;
            }
            catch (Exception exc)
            {
                return null;
            }
        }


        private void FillColorMatrix()
        {
            //заполнение цветами и переходами для заливки
            for (int i = 0; i < 6; i++)
            {
                m[i] = new int[3];
                per[i] = new int[3];
            }
            //заполнение цветами переходов от Min к Max   
            m[0][0] = 255; m[0][2] = 255;// фиолетовый  --- Min
            m[1][2] = 255;// синий
            m[2][1] = 255; m[2][2] = 255;// голубой
            m[3][1] = 255;// зеленый
            m[4][0] = 255; m[4][1] = 255;// желтый
            m[5][0] = 255;// красный --- Max
            /////////////////матрица переходов//////////////////////////////
            per[0][0] = -1;
            per[1][1] = 1;
            per[2][2] = -1;
            per[3][0] = 1;
            per[4][1] = -1;
            per[5][2] = 1;
        }
        public Color[] GetColorIsoLines(int count, Color MaxColor, Color MinColor)
        {
            Color[] color = new Color[count];
            //вычисляем промежуток цветов, в которые попадают выбранные пользователем цвета
            int indexMax = GetIndex(per, MaxColor);
            int indexMin = GetIndex(per, MinColor);
            //
            //количество переходов по основным цветам
            int CountGradation;
            //в зависимости от перехода
            if (indexMax > indexMin)
                CountGradation = (indexMax - indexMin) * 254;
            else
                CountGradation = (5 - indexMin + indexMax + 1) * 254;
            int step = CountGradation / (count - 1);
            //
            int ch = 0, ad = 0, id=0;
            for (int i = 0; i < count; i++)
            {
                ch = (i * step) / 254;
                ad = i * step - ch * 254;
                id = (indexMin + ch) % 6;
                color[i] = Color.FromArgb(m[id][0] + ad * per[id][0], m[id][1] + ad * per[id][1], m[id][2] + ad * per[id][2]);
            }
            return color;
        }

        

        private void GetColorGradient()
        {
            int indexMax = GetIndex(per, MaxColor);
            int indexMin = GetIndex(per, MinColor);
            //
            //количество переходов по основным цветам
            //в зависимости от перехода
            if (indexMax > indexMin)
                CountGradation = (indexMax - indexMin) * 254+1;
            else
                CountGradation = (5 - indexMin + indexMax + 1) * 254+1;
            //
            Colors = new Color[CountGradation];
            // заполнение массива цветов градиентом от меньшего к большему
            int ch = 0, ad = 0, id = 0;
            for (int i = 0; i < CountGradation; i++)
            {
                ch = i / 254;
                ad = i - ch * 254;
                id = (indexMin + ch) % 6;
                Colors[i] = Color.FromArgb(m[id][0] + ad * per[id][0], m[id][1] + ad * per[id][1], m[id][2] + ad * per[id][2]);
            }
        }
       
        int GetIndex(int[][] mas, Color a)
        {
            if (a.R == 255)
            {
                if (a.G == 255)
                    return 4;
                else
                {
                    if (a.B == 255)
                        return 0;
                    else
                        return 5;
                }
            }
            //
            if (a.G == 255)
            {
                if (a.B == 255)
                    return 2;
                else
                    return 3;
            }
            else
                return 1;
        }
    }
}
