using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeshLibrary;
using HydrodynamicLibrary;

namespace VisualizationLibrary
{
    public partial class MainForm : Form
    {
        Bitmap bm;
        int xshift = 10;
        int yshift = 10;
        RectangleF Rec;
        double MinX, MinY, MaxX, MaxY;
        protected int WS = 500;
        protected int HS = 500;
        //
        Mesh M;
        Graphics g;
        double[] U, V, P, S, E, K, Nu, rightE, rightK, ReT, Tau, Cnt;
        // Common Parameters
        bool Black = false;
        float PenWidth = 1.5f;
        Font f = new Font("Times New Roman", 12f);
        Color Max = Color.Red;
        Color Min = Color.Blue;
        string[] nclr = { "Cyan", "Fuchsia", "Yellow", "Red", "Blue", "Lime", "Aqua" };
        //---// Grid Paint Parametes
        bool Grid = false;
        bool GridU = true, GridV = false, GridP = false, GridTau = false;
        bool GridS = false, GridE = false, GridK = false, GridNu = false;
        bool GridRightE = false, GridRightK = false, GridReT = false, GridCnt = false;
        Color GridUColor = Color.Red;
        Color GridVColor = Color.Green;
        Color GridPColor = Color.Blue;
        Color GridSColor = Color.Yellow;
        Color GridKColor = Color.Brown;
        Color GridEColor = Color.Magenta;
        Color GridNuColor = Color.Coral;
        Color GridREColor = Color.HotPink;
        Color GridRKColor = Color.Peru;
        Color GridReTColor = Color.Turquoise;
        Color GridTauColor = Color.Pink;
        Color GridColor = Color.Purple;
        //---// IsoLines parameters
        bool IsoLines = true;
        int IsoCount = 20;
        /// <summary>
        /// 0 - U, 1 - V, 2 - P
        /// </summary>
        int FuncIdx = 0;
        double[] MinF, MaxF;
        bool minmax = true;
        //---// Vectors Options
        bool Vector = false;
        int ArrowLength = 40;
        int CX = 2, CY = 2;
        // -- Fill Options
        bool Fill = false;
        //
        int ZoomCount = 10;
        Point[] p = null;
        int ch = 0;
        ColorGradient c;
        ColorVectors v;
        IsoLines il;
        //
        public int angle = 10;
        //
        //GridDigits0 gd;
        //IsoLinesBitmap ib; 
        ////
        //double[] Data;
        public MainForm(double [] U, double[] V, double [] P, Mesh Mesh)
        {
            InitializeComponent();
            colorDialog1.CustomColors = new int[] { 0xFFFF00, 0xFF0000, 0xFF00FF, 0x0000FF, 0x00FF00, 0x00FFFF };
            Color b = Color.FromArgb(0xFF0000);
            p = new Point[ZoomCount];
            this.P = P;
            this.V = V;
            this.U = U;
            this.M = Mesh;
            //
            //Data[i * 30 + j] = i;
            //
            //
            MaxF = new double[] { U.Max(), V.Max(), P.Max() };
            MinF = new double[] { U.Min(), V.Min(), P.Min() };
            TrackbarMinMax();
            WS = this.Width - 100;
            HS = this.Height - 100;
            //
            MinX = M.X.Min(); MinY = M.Y.Min(); MaxX = M.X.Max(); MaxY = M.Y.Max();
            Rec = new RectangleF((float)MinX, (float)MaxY, (float)(MaxX - MinX), (float)(MaxY - MinY));
            
            //
            //il = new IsoLines();
            //il.Izo(M, Data,0, 0, 15);
            ////
            //c = new ColorGradient(Color.Red, Color.Blue, M);
            //c.FillArea(WS, HS, Data);
            //
            //v = new ColorVectors(M, WS, HS, 2, 2, Color.Red, Color.Blue, 1.0f);
            //v.Black = true;
            //v.DrawColorVectors(Data, Data, WS, HS);
            //Font f = new Font("Times New Roman", 11.0f,GraphicsUnit.Pixel);
            //gd = new GridDigits(M, WS, HS, Color.Brown, 1f, f);
            //gd.DrawGrid(true, Data);

            //ib = new IsoLinesBitmap(M, WS, HS, Color.Red, Color.Blue, 1.0f, 10);
            //ib.CalcIsoLines(Data);
            //ib.DrawIsoLines(0, 0, WS, HS); 
        }
        public MainForm(double[] U, double[] V, double[] P, double[] S, double[] e, double[] k, double [] Nu, Mesh Mesh, double[] Tau= null, double[] ReT=null, double[] rightE=null, double[] rightK=null, double[] Continuity = null)
        {
            InitializeComponent();
            colorDialog1.CustomColors = new int[] { 0xFFFF00, 0xFF0000, 0xFF00FF, 0x0000FF, 0x00FF00, 0x00FFFF };
            Color b = Color.FromArgb(0xFF0000);
            p = new Point[ZoomCount];
            this.P = P;
            this.V = V;
            this.U = U;
            this.S = S;
            this.E = e;
            this.K = k;
            this.Nu = Nu;
            this.Tau = Tau;
            this.rightE = rightE;
            this.rightK = rightK;
            this.ReT = ReT;
            this.Cnt = Continuity;
            this.M = Mesh;
            //
            //Data[i * 30 + j] = i;
            //
            //для загруженного решения после сериализации
            if (Cnt == null)
            {
                MaxF = new double[] { U.Max(), V.Max(), P.Max(), S.Max(), e.Max(), k.Max(), Nu.Max(), rightE.Max(), rightK.Max(), ReT.Max(), Tau.Max() };
                MinF = new double[] { U.Min(), V.Min(), P.Min(), S.Min(), e.Min(), k.Min(), Nu.Min(), rightE.Min(), rightK.Min(), ReT.Min(), Tau.Min() };
            }
            else
            {
                MaxF = new double[] { U.Max(), V.Max(), P.Max(), S.Max(), e.Max(), k.Max(), Nu.Max(), rightE.Max(), rightK.Max(), ReT.Max(), Tau.Max(), Cnt.Max() };
                MinF = new double[] { U.Min(), V.Min(), P.Min(), S.Min(), e.Min(), k.Min(), Nu.Min(), rightE.Min(), rightK.Min(), ReT.Min(), Tau.Min(), Cnt.Min() };
            }
            TrackbarMinMax();
            WS = this.Width - 100;
            HS = this.Height - 100;
            //
            MinX = M.X.Min(); MinY = M.Y.Min(); MaxX = M.X.Max(); MaxY = M.Y.Max();
            Rec = new RectangleF((float)MinX, (float)MaxY, (float)(MaxX - MinX), (float)(MaxY - MinY));
            //
            if (Tau == null)
            {
                chTau.Enabled = false;
                GridColorTau.Enabled = false;
                rTau.Enabled = false;
            }
            if (ReT == null)
            {
                chReT.Enabled = false;
                GridColorReT.Enabled = false;
                rReT.Enabled = false;
            }
            if (rightE == null)
            {
                chRE.Enabled = false;
                GridColorRE.Enabled = false;
                rRightE.Enabled = false;
            }
            if (rightK == null)
            {
                chrK.Enabled = false;
                GridColorRK.Enabled = false;
                rRightK.Enabled = false;
            }
            //
            if (Cnt == null)
            {
                chCnt.Enabled = false;
                rCnt.Enabled = false;
            }
            //
            //il = new IsoLines();
            //il.Izo(M, Data,0, 0, 15);
            ////
            //c = new ColorGradient(Color.Red, Color.Blue, M);
            //c.FillArea(WS, HS, Data);
            //
            //v = new ColorVectors(M, WS, HS, 2, 2, Color.Red, Color.Blue, 1.0f);
            //v.Black = true;
            //v.DrawColorVectors(Data, Data, WS, HS);
            //Font f = new Font("Times New Roman", 11.0f,GraphicsUnit.Pixel);
            //gd = new GridDigits(M, WS, HS, Color.Brown, 1f, f);
            //gd.DrawGrid(true, Data);

            //ib = new IsoLinesBitmap(M, WS, HS, Color.Red, Color.Blue, 1.0f, 10);
            //ib.CalcIsoLines(Data);
            //ib.DrawIsoLines(0, 0, WS, HS); 
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            if (Grid)
            {
                DrawGrid();
                if (n == 1)
                {
                    if ((GridU) && (U != null))
                        DrawText(U, bGridColorU.BackColor, 10, 0);
                    if ((GridV) && (V != null))
                        DrawText(V, bGridColorV.BackColor, 10, 10);
                    if ((GridP) && (P != null))
                        DrawText(P, bGridColorP.BackColor, 10, 20);
                    if ((GridS) && (S != null))
                        DrawText(S, GridColorS.BackColor, 10, 30);
                    if ((GridE) && (E != null))
                        DrawText(E, GridColorE.BackColor, 20, 0);
                    if ((GridK) && (K != null))
                        DrawText(K, GridColorK.BackColor, 20, 10);
                    if ((GridNu) && (Nu != null))
                        DrawText(Nu, GridColorNu.BackColor, 20, 20);
                    if ((GridRightE) && (rightE != null))
                        DrawText(rightE, GridColorRE.BackColor, 30, 0);
                    if ((GridRightK) && (rightK != null))
                        DrawText(rightK, GridColorRK.BackColor, 30, 10);
                    if ((GridReT) && (ReT != null))
                        DrawText(ReT, GridColorReT.BackColor, 30, 10);
                    if ((GridTau) && (Tau != null))
                        DrawText(Tau, GridColorReT.BackColor, 30, 10);
                    if ((GridCnt) && (Cnt != null))
                        DrawText(Cnt, GridColorReT.BackColor, 30, 10);
                }
            }
            if ((IsoLines) && (il!=null))
                DrawIso(il, Black);
            if((Fill)&&(c!=null))
                e.Graphics.DrawImage(bm, 20, 20, this.Width - 100, this.Height - 100);
            if ((Vector) && (v != null))
                e.Graphics.DrawImage(bm, 20, 20, this.Width - 100 + ArrowLength + 1, this.Height - 100 + ArrowLength + 1);
            
           
        }
        int n = 1;
        void DrawGrid()
        {
            Pen pen0 = new Pen(GridColor, PenWidth);
            //
            int[] kn = M.AreaElems[0];
            double minp = 100;
            double p1p2 = Math.Abs(ScaleX(M.X[kn[0]]) - ScaleX(M.X[kn[1]]));
            if (p1p2 < minp)
                minp = p1p2;
            //
            p1p2 = Math.Abs(ScaleY(M.Y[kn[0]]) - ScaleY(M.Y[kn[1]]));
            if (p1p2 < minp)
                minp = p1p2;
            //
            n = 1;
            //
            if ((minp < 10))
                n = (int)Math.Round(20.0 / minp, MidpointRounding.AwayFromZero) - 1;
            if (minp == 0)
                for (int i = 1; i < M.CountBottom; i++)
                {
                    if (Math.Abs(ScaleX(M.X[M.BottomKnots[0]]) - ScaleX(M.X[M.BottomKnots[i]])) != 0)
                    {
                        if (i == 1)
                            n = 1;
                        else
                            n = i*20;
                        break;
                    }
                }
                    //if (minp < 10)
                    //{
                    //    int lup = M.LeftKnots[0];
                    //    g.FillRectangle(new SolidBrush(GridColor), ScaleX(Rec.X), ScaleY(Rec.Y), ScaleX(Rec.Width), this.Height-20);
                    //}
                    //else
                    //{
                    for (int i = 0; i < M.CountElems; i += n)
                    {
                        int[] Knots = M.AreaElems[i];
                        int p1x = ScaleX(M.X[Knots[0]]);
                        int p1y = ScaleY(M.Y[Knots[0]]);
                        Point Point1 = new Point(p1x, p1y);
                        //
                        int p2x = ScaleX(M.X[Knots[1]]);
                        int p2y = ScaleY(M.Y[Knots[1]]);
                        Point Point2 = new Point(p2x, p2y);
                        //
                        int p3x = ScaleX(M.X[Knots[2]]);
                        int p3y = ScaleY(M.Y[Knots[2]]);
                        Point Point3 = new Point(p3x, p3y);
                        //
                        g.DrawLine(pen0, Point1, Point2);
                        g.DrawLine(pen0, Point2, Point3);
                        g.DrawLine(pen0, Point3, Point1);
                        //
                        //if ((!GridU) & (!GridV) & (!GridP) & (!GridE) & (!GridK) & (!GridNu) & (!GridRightE) & (!GridRightK) & (!GridS) && (!GridReT) && (!GridTau))
                        //{
                        //    g.DrawString(Knots[0].ToString(), f, Brushes.Black, Point1);
                        //    g.DrawString(Knots[1].ToString(), f, Brushes.Black, Point2);
                        //    g.DrawString(Knots[2].ToString(), f, Brushes.Black, Point3);
                        //    //g.DrawString(i.ToString(), f, Brushes.Black, new PointF((p1x+p2x+p3x)/3.0f, (p1y+p2y+p3y)/3.0f));
                        //}

                    }
            //}
                    // не работает - в битмап нужно писать попиксельно, линии почему-то не чертит
                    //GridDigits0 gd0 = new GridDigits0(M, WS, HS, GridColor, 2.0f, new Font("Arail", 8.0f));
                    //gd0.GridBitmap(M);
            //
            // отрисовка экспериентальных узлов на сетке
                    if (angle == 10)
                    {
                        KwollExp kExp = new KwollExp();
                        KwollGeometry kg = new KwollGeometry(1000, 1.5, 6, 1);
                        double[] x10_12;
                        kExp.SlipData10(kg.ZeroPoint10, out x10_12);
                        double[] xexp = x10_12;
                        double[] yexp = kExp.y_10;
                        for (int i = 0; i < xexp.Length; i += n)
                        {
                            double y_cur = yexp[i];//0.2*0.35; 
                            int p1x = ScaleX(xexp[i]) - 1;
                            int p1y = ScaleY(y_cur) - 1;
                            //
                            Pen all = new Pen(Brushes.Blue);
                            Pen notall = new Pen(Brushes.Gray);
                            if (!double.IsNaN(kExp.TKE_10[i]))
                                g.DrawRectangle(all, p1x, p1y, 2f, 2f);
                            else
                                g.DrawRectangle(notall, p1x, p1y, 2f, 2f);
                            if (GridU)
                                g.DrawString(kExp.u_t_10[i].ToString("G4", System.Globalization.CultureInfo.InvariantCulture), f, Brushes.Gold, p1x, p1y);
                            if (GridK)
                                g.DrawString(kExp.TKE_10[i].ToString("G4", System.Globalization.CultureInfo.InvariantCulture), f, Brushes.Gold, p1x, p1y);
                        }
                    }
                    //
                    if (angle == 20)
                    {
                        KwollExp kExp = new KwollExp();
                        KwollGeometry kg = new KwollGeometry(1000, 1.5, 6, 1);
                        double[] x20_12;
                        kExp.SlipData20(kg.ZeroPoint20, out x20_12);
                        double[] xexp = x20_12;
                        double[] yexp = kExp.y_20;
                        for (int i = 0; i < xexp.Length; i += n)
                        {
                            double y_cur = yexp[i];//0.2*0.35; 
                            int p1x = ScaleX(xexp[i]) - 1;
                            int p1y = ScaleY(y_cur) - 1;
                            //
                            Pen all = new Pen(Brushes.Blue);
                            Pen notall = new Pen(Brushes.Gray);
                            if (!double.IsNaN(kExp.TKE_20[i]))
                                g.DrawRectangle(all, p1x, p1y, 2f, 2f);
                            else
                                g.DrawRectangle(notall, p1x, p1y, 2f, 2f);
                            if (GridU)
                                g.DrawString(kExp.u_t_20[i].ToString("G4", System.Globalization.CultureInfo.InvariantCulture), f, Brushes.Gold, p1x, p1y);
                            if (GridK)
                                g.DrawString(kExp.TKE_20[i].ToString("G4", System.Globalization.CultureInfo.InvariantCulture), f, Brushes.Gold, p1x, p1y);
                        }
                    }
            //
                    if (angle == 30)
                    {
                        KwollExp kExp = new KwollExp();
                        KwollGeometry kg = new KwollGeometry(1000, 1.5, 6, 1);
                        double[] x30_12;
                        kExp.SlipData30(kg.ZeroPoint30, out x30_12);
                        double[] xexp = x30_12;
                        double[] yexp = kExp.y_30;
                        for (int i = 0; i < xexp.Length; i += n)
                        {
                            double y_cur = yexp[i];//0.2*0.35; 
                            int p1x = ScaleX(xexp[i]) - 1;
                            int p1y = ScaleY(y_cur) - 1;
                            //
                            Pen all = new Pen(Brushes.Blue);
                            Pen notall = new Pen(Brushes.Gray);
                            if (!double.IsNaN(kExp.TKE_30[i]))
                                g.DrawRectangle(all, p1x, p1y, 2f, 2f);
                            else
                                g.DrawRectangle(notall, p1x, p1y, 2f, 2f);
                            if (GridU)
                                g.DrawString(kExp.u_t_30[i].ToString("G4", System.Globalization.CultureInfo.InvariantCulture), f, Brushes.Gold, p1x, p1y);
                            if (GridK)
                                g.DrawString(kExp.TKE_30[i].ToString("G4", System.Globalization.CultureInfo.InvariantCulture), f, Brushes.Gold, p1x, p1y);
                        }
                    }

        }
        private void DrawText(double[] F, Color C, int dx, int dy)
        {
            Brush BrushText = new SolidBrush(C);
            for (int i = 0; i < M.CountKnots; i++)
            {
                // координаты отрисовки цифр
                int cx = ScaleX(M.X[i]);
                int cy = ScaleY(M.Y[i]);
                string res = String.Format("{0:f2}", F[i]);
                //string res = i.ToString();
                //
                Point point = new Point(cx + dx, cy + dy);
                //
                g.DrawString(res, f, BrushText, point);
                // изменение координат
            }
            
        }
        private void DrawAreaBoundaries()
        {
            Pen pen = new Pen(Color.Brown, PenWidth+0.5f);
            for (int p = 0; p < M.CountLeft - 1; p++)
            {
                int num1 = M.LeftKnots[p];
                int num2 = M.LeftKnots[p + 1]; ;
                //
                Point pp1 = new Point(ScaleX(M.X[num1]), ScaleY(M.Y[num1]));
                Point pp2 = new Point(ScaleX(M.X[num2]), ScaleY(M.Y[num2]));
                g.DrawLine(pen, pp1, pp2);
                //
            }
            for (int p = 0; p < M.CountRight - 1; p++)
            {
                int num1 = M.RightKnots[p];
                int num2 = M.RightKnots[p + 1]; ;
                //
                Point pp1 = new Point(ScaleX(M.X[num1]), ScaleY(M.Y[num1]));
                Point pp2 = new Point(ScaleX(M.X[num2]), ScaleY(M.Y[num2]));
                g.DrawLine(pen, pp1, pp2);

            }
            //
            for (int p = 0; p < M.CountBottom - 1; p++)
            {
                int num1 = M.BottomKnots[p];
                int num2 = M.BottomKnots[p + 1];
                //
                Point pp1 = new Point(ScaleX(M.X[num1]), ScaleY(M.Y[num1]));
                Point pp2 = new Point(ScaleX(M.X[num2]), ScaleY(M.Y[num2]));
                g.DrawLine(pen, pp1, pp2);
            }
            for (int p = 0; p < M.CountTop - 1; p++)
            {
                //
                int num1 = M.TopKnots[p];
                int num2 = M.TopKnots[p + 1]; ;
                //
                Point pp1 = new Point(ScaleX(M.X[num1]), ScaleY(M.Y[num1]));
                Point pp2 = new Point(ScaleX(M.X[num2]), ScaleY(M.Y[num2]));
                g.DrawLine(pen, pp1, pp2);

            }
        }
        void DrawIso(IsoLines Iso, bool Text)
        {
            TrackbarMinMax();
            //
            if (il != null)
            {
                int count = Iso.Count;
                Pen pen0 = new Pen(Brushes.Black, PenWidth);
                Pen[] pen = new Pen[count];
                //
                if (Text)
                {
                    for (int i = 0; i < count; i++)
                        pen[i] = pen0;
                }
                else
                {
                    ColorGradient CGrad = new ColorGradient();
                    Color[] c = CGrad.GetColorIsoLines(il.Count, Max, Min);
                    for (int i = 0; i < count; i++)
                        pen[i] = new Pen(c[i], 1.5f);
                }
                //
                //
                double[][] X = Iso.GetX;
                double[][] Y = Iso.GetY;
                //
                for (int l = 0; l < count; l++)
                {
                    for (int i = 0; i < X[l].Length - 1; i+=2)
                    {
                        int cx = ScaleX(X[l][i]);
                        int cy = ScaleY(Y[l][i]);
                        Point Point1 = new Point(cx, cy);
                        //
                        cx = ScaleX(X[l][i + 1]);
                        cy = ScaleY(Y[l][i + 1]);
                        Point Point2 = new Point(cx, cy);
                        //
                        g.DrawLine(pen[l], Point1, Point2);

                    }
                }
                if (Text)
                {
                    float size = f.Size;
                    Brush BrushText = Brushes.Black;
                    int c2 = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (X[i].Length > 4)
                        {
                            c2 = X[i].Length / 4;
                            // координаты отрисовки цифр
                            int cx = ScaleX(X[i][c2]);
                            int cy = ScaleY(Y[i][c2]);
                            string res = String.Format("{0:f2}", Iso.Value(i));
                            //
                            RectangleF recWhite;
                            if (Math.Abs(X[i][c2] - X[i][c2 + 3]) > Math.Abs(Y[i][c2] - Y[i][c2 + 3]))
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
                }
                DrawAreaBoundaries();
            }
            else
                ReadParametersAndCalc();
        }
        //
        /// <summary>
        /// Расчет экранной координаты по х
        /// </summary>
        /// <param name="xa"></param>
        
        protected int ScaleX(double xa)
        {
            int ix = (int)(WS * (xa - MinX) / Rec.Width) + xshift;
            return ix;
        }
        /// <summary>
        /// Расчет экранной координаты по y
        /// </summary>
        /// <param name="xa"></param>
        /// <returns></returns>
        protected int ScaleY(double yc)
        {
            int iy = (int)(HS - HS * (yc - MinY) / Rec.Height) + yshift;
            return iy;
        }
        private void ReadParametersAndCalc()
        {
            if (IsoLines)
            {
                il = new IsoLines();
                double Min = Convert.ToDouble(txtMinF.Text);
                double Max = Convert.ToDouble(txtMaxF.Text);
                double[] MinMaxF = new double[2];
                if (FuncIdx == 0)
                    MinMaxF = il.Izo(M, U, Max, Min, IsoCount);
                if (FuncIdx == 1)
                    MinMaxF = il.Izo(M, V, Max, Min, IsoCount);
                if (FuncIdx == 2)
                    MinMaxF = il.Izo(M, P, Max, Min, IsoCount);
                if (FuncIdx == 3)
                    MinMaxF = il.Izo(M, S, Max, Min, IsoCount);
                if (FuncIdx == 4)
                    MinMaxF = il.Izo(M, E, Max, Min, IsoCount);
                if (FuncIdx == 5)
                    MinMaxF = il.Izo(M, K, Max, Min, IsoCount);
                if (FuncIdx == 6)
                    MinMaxF = il.Izo(M, Nu, Max, Min, IsoCount);
                if (FuncIdx == 7)
                    MinMaxF = il.Izo(M, rightE, Max, Min, IsoCount);
                if (FuncIdx == 8)
                    MinMaxF = il.Izo(M, rightK, Max, Min, IsoCount);
                 if (FuncIdx == 9)
                     MinMaxF = il.Izo(M, ReT, Max, Min, IsoCount);
                if (FuncIdx == 10)
                    MinMaxF = il.Izo(M, Tau, Max, Min, IsoCount);
                if (FuncIdx == 11)
                    MinMaxF = il.Izo(M, Cnt, Max, Min, IsoCount);
                lblInterval.Text = "Min=" + MinMaxF[0].ToString() + ", Max=" + MinMaxF[1].ToString();
            }
            if (Vector)
            {
                v = new ColorVectors(M, WS, HS, CX, CY, Max, Min, PenWidth, ArrowLength);
                v.Black = Black;
                v.DrawColorVectors(U, V, WS, HS);
                bm = v.GetBitmap;
            }
            if (Fill)
            {
                c = new ColorGradient(Max, Min, M);
                if (FuncIdx == 0)
                    c.FillArea(WS, HS, U);
                if (FuncIdx == 1)
                    c.FillArea(WS, HS, V);
                if (FuncIdx == 2)
                    c.FillArea(WS, HS, P);
                if (FuncIdx == 3)
                    c.FillArea(WS, HS, S);
                if (FuncIdx == 4)
                    c.FillArea(WS, HS, E);
                if (FuncIdx == 5)
                    c.FillArea(WS, HS, K);
                if (FuncIdx == 6)
                    c.FillArea(WS, HS, Nu);
                if (FuncIdx == 7)
                    c.FillArea(WS, HS, rightE);
                if (FuncIdx == 8)
                    c.FillArea(WS, HS, rightK);
                 if (FuncIdx == 9)
                    c.FillArea(WS, HS, ReT);
                 if (FuncIdx == 10)
                     c.FillArea(WS, HS, Tau);
                 if (FuncIdx == 11)
                     c.FillArea(WS, HS, Cnt);
                bm = c.GetBitmap;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            WS = this.Width - MainPanel.Width - 40;
            HS = this.Height - 100;
            xshift = 10;
            yshift = 10;
            x = 0; y = 0;
            ch = 0;
            Invalidate();
        }
        int x = 0, y = 0;
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ch >= ZoomCount)
                    MessageBox.Show("Это масимальное увеличение, чтобы увеличить больше, измените настройки.");
                else
                {
                    WS *= 2 ;
                    HS *= 2;
                    p[ch] = e.Location;
                    xshift = ClientRectangle.Width / 2 - (p[ch].X - xshift) * 2;
                    yshift = ClientRectangle.Height / 2 - (p[ch].Y - yshift) * 2;
                }

                if (Fill)
                {
                    x = (p[ch].X + x) * 2 - c.W / 2;
                    y = (p[ch].Y + y) * 2 - c.H / 2;
                    if(FuncIdx==0)
                        c.FillPartArea(x, y, WS, HS, U);
                    if (FuncIdx == 1)
                        c.FillPartArea(x, y, WS, HS, V);
                    if (FuncIdx == 2)
                        c.FillPartArea(x, y, WS, HS, P);
                    if (FuncIdx == 3)
                        c.FillPartArea(x, y, WS, HS, S);
                    if (FuncIdx == 4)
                        c.FillPartArea(x, y, WS, HS, E);
                    if (FuncIdx == 5)
                        c.FillPartArea(x, y, WS, HS, K);
                    if (FuncIdx == 6)
                        c.FillPartArea(x, y, WS, HS, Nu);
                    if (FuncIdx == 7)
                        c.FillPartArea(x, y, WS, HS, rightE);
                    if (FuncIdx == 8)
                        c.FillPartArea(x, y, WS, HS, rightK);
                    if (FuncIdx == 9)
                        c.FillPartArea(x, y, WS, HS, ReT);
                    if (FuncIdx == 10)
                        c.FillPartArea(x, y, WS, HS, Tau);
                    if (FuncIdx == 11)
                        c.FillPartArea(x, y, WS, HS, Cnt);
                    bm = c.GetBitmap;
                }

                if (Vector)
                {
                    x = (p[ch].X + x) * 2 - v.W / 2;
                    y = (p[ch].Y + y) * 2 - v.H / 2;
                    v.DrawColorVectors(U, V, x, y, WS, HS);
                    bm = v.GetBitmap;
                }
                //
                //if (ib != null)
                //{
                //    x = (p[ch].X + x) * 2 - ib.W / 2;
                //    y = (p[ch].Y + y) * 2 - ib.H / 2;
                //    ib.DrawIsoLines(x, y, WS, HS);
                //}
                ch++;
                Invalidate();
            }
            else
            {
                ch--; 
                if (ch >= 0)
                {
                    WS /= 2;
                    HS /= 2;
                    xshift = (xshift - ClientRectangle.Width / 2) / 2 + p[ch].X;
                    yshift = (yshift - ClientRectangle.Height / 2) / 2 + p[ch].Y;
                    Invalidate();
                }
                else
                    ch = 0;
               
                    if (Fill)
                    {
                        x = (x + c.W / 2 - 2 * p[ch].X) / 2;
                        y = (y + c.H / 2 - 2 * p[ch].Y) / 2;
                        if (FuncIdx == 0)
                            c.FillPartArea(x, y, WS, HS, U);
                        if (FuncIdx == 1)
                            c.FillPartArea(x, y, WS, HS, V);
                        if (FuncIdx == 2)
                            c.FillPartArea(x, y, WS, HS, P);
                        if (FuncIdx == 3)
                            c.FillPartArea(x, y, WS, HS, S);
                        if (FuncIdx == 4)
                            c.FillPartArea(x, y, WS, HS, E);
                        if (FuncIdx == 5)
                            c.FillPartArea(x, y, WS, HS, K);
                        if (FuncIdx == 6)
                            c.FillPartArea(x, y, WS, HS, Nu);
                        if (FuncIdx == 7)
                            c.FillPartArea(x, y, WS, HS, rightE);
                        if (FuncIdx == 8)
                            c.FillPartArea(x, y, WS, HS, rightK);
                        if (FuncIdx == 9)
                            c.FillPartArea(x, y, WS, HS, ReT);
                        if (FuncIdx == 10)
                            c.FillPartArea(x, y, WS, HS, Tau);
                        if (FuncIdx == 11)
                            c.FillPartArea(x, y, WS, HS, Cnt);
                        bm = c.GetBitmap;

                    }
                    //
                    if (Vector)
                    {
                        x = (x + v.W / 2 - 2 * p[ch].X) / 2;
                        y = (y + v.H / 2 - 2 * p[ch].Y) / 2;
                        v.DrawColorVectors(U, V, x, y, WS, HS);
                        bm = v.GetBitmap;
                    }
                    //
                    
                }
               
                
            
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            if ((WS != this.Width - MainPanel.Width - 40) || (HS != this.Height - 100))
            {
                WS = this.Width - MainPanel.Width - 40;
                HS = this.Height - 100;
                Invalidate();
            }
        }

        private void chU_CheckedChanged(object sender, EventArgs e)
        {
            GridU = chU.Checked;
        }

        private void chV_CheckedChanged(object sender, EventArgs e)
        {
            GridV = chV.Checked;
        }

        private void chP_CheckedChanged(object sender, EventArgs e)
        {
            GridP = chP.Checked;
        }

        private void GridColorU_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridUColor = colorDialog1.Color;
            bGridColorU.BackColor = GridUColor;
        }

        private void bGridColorV_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridVColor = colorDialog1.Color;
            bGridColorV.BackColor = GridVColor;
        }

        private void bGridColorP_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridPColor = colorDialog1.Color;
            bGridColorP.BackColor = GridPColor;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridColor = colorDialog1.Color;
            bGridColor.BackColor = GridColor;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            PenWidth = (float)numPenWidth.Value;
        }

        private void GridFont_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog(this);
            f = fontDialog1.Font;
            GridFont.Text = f.Name.Substring(0, 6) + ", " + f.Size;
        }

        private void bMaxColor_Click(object sender, EventArgs e)
        {
            colorDialog1.SolidColorOnly = true;
            colorDialog1.AllowFullOpen = false;
            colorDialog1.AnyColor = false;
            colorDialog1.FullOpen = false;
            //
            colorDialog1.ShowDialog(this);
            if (nclr.Any(item => item == colorDialog1.Color.Name))
            {
                Max = colorDialog1.Color;
                bMaxColor.BackColor = Max;
            }
            else
                MessageBox.Show("Выберите цвет из числа дополнительных!", "Выбор цвета заливки");
            
            
        }

        private void bMinColor_Click(object sender, EventArgs e)
        {
            colorDialog1.SolidColorOnly = true;
            colorDialog1.AllowFullOpen = false;
            colorDialog1.AnyColor = false;
            colorDialog1.FullOpen = false;
            //
            colorDialog1.ShowDialog(this);
            if (nclr.Any(item => item == colorDialog1.Color.Name))
            {
                Min = colorDialog1.Color;
                bMinColor.BackColor = Min;
            }
            else
                MessageBox.Show("Выберите цвет из числа дополнительных!", "Выбор цвета заливки");
        }

        private void numIsoCount_ValueChanged(object sender, EventArgs e)
        {
            IsoCount = (int)numIsoCount.Value;
        }

        private void rV_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 1;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
            //TrackbarMinMax();
        }

        private void rU_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 0;
            //TrackbarMinMax();
        }

        private void rP_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 2;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
            //TrackbarMinMax();
        }
        private void rS_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 3;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rE_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 4;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rK_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 5;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rNu_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 6;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rRightE_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 7;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rRightK_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 8;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rReT_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 9;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rTau_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 10;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        private void rCnt_CheckedChanged(object sender, EventArgs e)
        {
            FuncIdx = 11;
            txtMaxF.Text = "0";
            txtMinF.Text = "0";
        }
        void TrackbarMinMax()
        {
            if (Math.Abs(MaxF[FuncIdx]) < 0.1)
            {
                trackBar1.Minimum = (int)(MinF[FuncIdx] * 100);
                trackBar1.Maximum = (int)(MaxF[FuncIdx] * 100);
            }
            else if (Math.Abs(MaxF[FuncIdx]) < Int32.MaxValue)
            {
                trackBar1.Minimum = (int)(MinF[FuncIdx]);
                trackBar1.Maximum = (int)(MaxF[FuncIdx]);
            }
            else
            {
                MessageBox.Show("Значения функции превышают предел Int32");
            }
        }

        private void txtMinF_MouseDown(object sender, MouseEventArgs e)
        {
            trackBar1.Enabled = true;
            minmax = true;
        }
        private void txtMaxF_MouseDown(object sender, MouseEventArgs e)
        {
            trackBar1.Enabled = true;
            minmax = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (minmax)
            {
                txtMinF.Text = Convert.ToString(trackBar1.Value / 100.0);
            }
            else
            {
                txtMaxF.Text = Convert.ToString(trackBar1.Value / 100.0);
            }
        }

        private void trackBar1_Leave(object sender, EventArgs e)
        {
            trackBar1.Enabled = false;
        }

        private void rGrid_CheckedChanged(object sender, EventArgs e)
        {
            Grid = rGrid.Checked;
            panelGrid.Enabled = Grid;
        }

        private void rIzo_CheckedChanged(object sender, EventArgs e)
        {
            IsoLines = rIzo.Checked;
            panelIso.Enabled = IsoLines;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ArrowLength = (int)numericUpDown1.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            CX = (int)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            CY = (int)numericUpDown5.Value;
        }

        private void rVector_CheckedChanged(object sender, EventArgs e)
        {
            Vector = rVector.Checked;
            panelVector.Enabled = Vector;
        }

        private void rFill_CheckedChanged(object sender, EventArgs e)
        {
            Fill = rFill.Checked;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == ">")
            {
                MainPanel.Visible = false;
                button5.Text = "<";
                //WS = this.Width - 100;
                //Invalidate();
            }
            else
            {
                MainPanel.Visible = true;
                button5.Text = ">";
                //WS = this.Width - MainPanel.Width - 40; ;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Black = chBlack.Checked;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ReadParametersAndCalc();
            WS = this.Width - MainPanel.Width - 40;
            HS = this.Height - 100;
            x = 0; y = 0;
            Invalidate();
        }

        private void btnS_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridSColor = colorDialog1.Color;
            GridColorS.BackColor = GridSColor;
        }

        private void btnE_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridEColor = colorDialog1.Color;
            GridColorE.BackColor = GridEColor;
        }

        private void btnK_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridKColor = colorDialog1.Color;
            GridColorK.BackColor = GridKColor;
        }
        private void GridColorNu_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridColorNu.BackColor = colorDialog1.Color;
        }
        private void GridColorRE_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridColorRE.BackColor = colorDialog1.Color;
        }
        private void GridColorRK_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridColorRK.BackColor = colorDialog1.Color;
        }
        private void GridColorReT_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridColorReT.BackColor = colorDialog1.Color;
        }
        private void GridColorTau_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(this);
            GridColorTau.BackColor = colorDialog1.Color;
        }
        private void chS_CheckedChanged(object sender, EventArgs e)
        {
            GridS = chS.Checked;
        }

        private void chE_CheckedChanged(object sender, EventArgs e)
        {
            GridE = chE.Checked;
        }

        private void chK_CheckedChanged(object sender, EventArgs e)
        {
            GridK = chK.Checked;
        }

        private void chNu_CheckedChanged(object sender, EventArgs e)
        {
            GridNu = chNu.Checked;
        }

        private void chRE_CheckedChanged(object sender, EventArgs e)
        {
            GridRightE = chRE.Checked;
        }

        private void chrK_CheckedChanged(object sender, EventArgs e)
        {
            GridRightK = chrK.Checked;
        }
        private void chReT_CheckedChanged(object sender, EventArgs e)
        {
            GridReT = chReT.Checked;
        }

        private void chTau_CheckedChanged(object sender, EventArgs e)
        {
            GridTau = chTau.Checked;
        }

        private void chCnt_CheckedChanged(object sender, EventArgs e)
        {
            GridCnt = chCnt.Checked;
        }

     

        

        

     

       

       

        

        

        

        

        

        

         
    }
}
