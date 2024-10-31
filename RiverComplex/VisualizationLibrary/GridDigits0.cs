using MeshLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationLibrary
{
    public class GridDigits0
    {
        Mesh M;
        Bitmap b;
        Graphics g;
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
        //
        Color DigitColor;
        Color GridColor;
        public Color ColorGrid
        {
            get { return GridColor; }
            set { GridColor = ColorGrid; }
        }
        //
        float widthPen;
        public float PenWidth
        {
            get { return widthPen; }
            set { widthPen = value; }
        }
        //
        Font f;
        //
        public int W, H;
        //
        bool black = false;
        public bool Black
        {
            get { return black; }
            set { black = value; }
        }
        //
        public GridDigits0(Mesh M, int WS, int HS, Color GridColor, float widthPen, Font f)
        {
            this.M = M;
            this.W = WS;
            this.H = HS;
            this.GridColor = GridColor;
            this.widthPen = widthPen;
            this.f = f;
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
        }
        //
        public void DrawGrid(bool Text, double[] F = null)
        {
            Pen pen0 = new Pen(Brushes.Red, widthPen);
            b = new Bitmap(W + 1, H + 1);
            g = Graphics.FromImage(b);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
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
            for (int i = 0; i < M.CountElems; i++)
            {
                int[] Knots = M.AreaElems[i];
                Point Point1 = new Point(Xpx[Knots[0]], Ypx[Knots[0]]);
                Point Point2 = new Point(Xpx[Knots[1]], Ypx[Knots[1]]);
                Point Point3 = new Point(Xpx[Knots[2]], Ypx[Knots[2]]);
                //
                g.DrawLine(pen0, Point1, Point2);
                g.DrawLine(pen0, Point2, Point3);
                g.DrawLine(pen0, Point3, Point1);
            }
            //
            if ((Text) && (F != null))
            {
                Brush BrushText = new SolidBrush(Color.Black);
                for (int i = 0; i < M.CountKnots; i++)
                {
                    // координаты отрисовки цифр
                    string res = String.Format("{0:f2}", F[i]);
                    //
                    Point point = new Point(Xpx[i] + 10, Ypx[i]);
                    //
                    g.DrawString(res, f, BrushText, point);
                    // изменение координат
                }
            }
        }
        //
        public void GridBitmap(Mesh M)
        {

            int sc = 600;
            // множителями задаются пропорции
            this.W = Convert.ToInt32(sc * (M.X[M.RightKnots[0]]) - M.X[0] * 8); this.H = Convert.ToInt32(sc * (M.Y[M.LeftKnots[0]] - M.Y[0]) * 4);
            int Count = M.CountKnots;
            int CountKnots = Count / 2;
            // отрисовываем первую половину
            int[] Xpx = new int[CountKnots];
            int[] Ypx = new int[CountKnots];
            // битмап, в который все будет писаться
            Bitmap outb = new Bitmap(W + 1, H + 1);
            //
            for (int i = 0; i < CountKnots; i++)
            {
                // кооринаты сетки в пикселях
                Xpx[i] = Convert.ToInt32(X01[i] * W);
                Ypx[i] = Convert.ToInt32(Y01[i] * H);
            }
            //
            Graphics gg = Graphics.FromImage(outb);
            gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pen0 = new Pen(Brushes.Black, 1.0f);
            for (int i = 0; i < M.CountElems; i++)
            {
                int[] Knots = M.AreaElems[i];
                if (Knots.Max() < CountKnots)
                {
                    Point Point1 = new Point(Xpx[Knots[0]], Ypx[Knots[0]]);
                    Point Point2 = new Point(Xpx[Knots[1]], Ypx[Knots[1]]);
                    Point Point3 = new Point(Xpx[Knots[2]], Ypx[Knots[2]]);
                    //
                    gg.DrawLine(pen0, Point1, Point2);
                    gg.DrawLine(pen0, Point2, Point3);
                    gg.DrawLine(pen0, Point3, Point1);
                }
            }
            gg.Save();
                //отрисовка в файл в хорошем разрешении
            outb.Save("BitmapGrid.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
