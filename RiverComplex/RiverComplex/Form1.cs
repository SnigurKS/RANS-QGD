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
using AlgebraLibrary;
using RiverTaskLibrary;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using HydrodynamicLibrary;
using VisualizationLibrary;
using ShowGraphic;
using System.Diagnostics;
using System.Globalization;

namespace RiverComplex
{
    public partial class Form1 : Form
    {
        int angle = 0;
        TimeSpan time = new TimeSpan();
        Stopwatch stopW = new Stopwatch();
        public bool OpenCL = false;
        public bool Cuda = false;
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        KwollGeometry kg;
        KwollExp kExp;
        //
        int Nx = 0;
        double dx = 0;
        //для сериализации
        int BeginIter = 0;
        int Dy = 200;
        int Dx = 385;
        double time_sec = 0;
        //
        Mesh m;
        RectangleF Rec;
        int WS, HS;
        WaterTaskEliz wte = null;
        BaseBedLoadTask rt = null;
        BinaryFormatter formatter = null;
        //Thread BackgroundThread = null;
        Algorythm alg = null;
        SSystem sys = null;
        WElizParameter w = null;
        BedPhysicsParams r = null;
        Parameter[] Prs = null;
        MeshBuilder mb = null;
        int parts = 1;
        int Layers = 0;
        int count = 0;
        int GlobalIteration = 0;
        double dt = 0.01;
        double H = 0.1, L = 2;
        double H2 = 0, L2 = 0;
        double[] Xt = null;
        double[] Yt = null;
        double BottomLayer = 0, TopLayer = 0;
        Graphics g;
        //
        double[][] BuffZeta;
        double[][] BuffTau;
        double[][] BuffTauT;
        double[][] BuffQb;
        double[][] BuffQs;
        int[] TimeFaces ;
        CheckBox OCl;
        CheckBox ch_Cuda;
        TextBox tb_alpha;
        Button FaceGr;
        ToolStripMenuItem ExactItem;
        double[] ExactU;
        double[] ExactV;
        double[] ExactP;
        double[] ExactE = null;
        double[] ExactK = null;
        double[] ExactS = null;
        int[][] ExactAreaElems;
        double[] ExactSk;
        int ExactCountElems;
        bool RecalcFlag = false;
        ToolStripMenuItem ts = null;
        bool BackwardStep = false;
        /// <summary>
        /// Значения профиля скоростей, кинетической энергии, диссипации и вязкости, загруженного из другого решения
        /// </summary>
        double[] U_start, V_start, K_start, E_start, W_start, NuT_start = null, S_start;
        /// <summary>
        /// Знаяения вертикальных координат для узлов профиля скорости U_start
        /// </summary>
        double[] Y_start = null;
        /// <summary>
        /// Значения градиента давления, при котором получен профиль скорости U_start
        /// </summary>
        double dpdx_start = 0;
        public Form1()
        {
            InitializeComponent();
            // Тест WaterTaskEliz
            count = (Layers + 1);
            OCl = new CheckBox();
            OCl.Location = new Point(10, 25);
            OCl.Text = "OpenCL";
            OCl.CheckedChanged += OCl_CheckedChanged;
            this.Controls.Add(OCl);
            //
            ch_Cuda = new CheckBox();
            ch_Cuda.Location = new Point(10, 45);
            ch_Cuda.Text = "Cuda";
            ch_Cuda.CheckedChanged += ch_Cuda_CheckedChanged;
            this.Controls.Add(ch_Cuda);
            //Angle of dunes (10,20,30) 
            // textbox в левом верхнем углу
            tb_alpha = new TextBox();
            tb_alpha.Location = new Point(10, 75);
            tb_alpha.Width = 50;
            tb_alpha.Text = "0";
            tb_alpha.TextChanged += tb_alpha_TextChanged;
            this.Controls.Add(tb_alpha);
            //
            FaceGr = new Button();
            FaceGr.Location = new Point(this.Width - 45 - 43, 27);
            FaceGr.Size = new System.Drawing.Size(64, 34);
            FaceGr.Text = "FaceGr.";
            FaceGr.Click += FaceGr_Click;
            FaceGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(FaceGr);
            //
            //Тест
            //kExp = new KwollExp(new string[] { "D10.txt", "D20.txt", "D30.txt", "Tuv_av.txt" });
            //alglib.spline2dinterpolant[] s = kExp.Interpolate10(); ;
            ToolStripMenuItem Uu = new ToolStripMenuItem("Velocity profile");
            ToolStripMenuItem [] U_c =  {new ToolStripMenuItem("Save"), new ToolStripMenuItem("Load"), new ToolStripMenuItem("Delete") };
            U_c[0].Click += Velocity_Save_Click;
            U_c[1].Click += Velocity_Load_Click;
            U_c[2].Click += Velocity_Delete_Click;
            Uu.DropDownItems.AddRange(U_c);
            taskToolStripMenuItem.DropDownItems.Add(Uu);
            //
            ts = new ToolStripMenuItem("Enable futher calc.");
            ts.Click += Recalc;
            ts.Checked = false;
            taskToolStripMenuItem.DropDownItems.Add(ts);            
            //
            ExactItem = new ToolStripMenuItem("Discrepancy");
            ExactItem.DropDownItems.Add("Save Exact Values");
            ExactItem.DropDownItems.Add("Load Exact Values");
            ExactItem.DropDownItems.Add("Clean Exact Values");
            //
            //----Добавить  подменю меню 
            ExactItem.DropDownItems[0].Click += UnloadExactValues;
            ExactItem.DropDownItems[1].Click += LoadExactValues;
            ExactItem.DropDownItems[2].Click += CleanExactValues;
            menuStrip1.Items.Add(ExactItem);
            ExactItem.DropDownItems[2].Enabled = false;
            //
            w = new WElizParameter(800, 0.005, 0.005, 0.001, 0.0001, 0.0001, 0.0369, 1.0, 0.1, 0.0001, false, 0.05);
            prpMainParameters.SelectedObject = w;
            lblUmax.Text = "Umax = " + (3.0 / 2.0 * w.Q / H).ToString() + ", Cu=" + (w.dt_local * (3.0 / 2.0 * w.Q / H)/areaControl1.dx).ToString() + ", tmin=" + (2*areaControl1.dx*areaControl1.dx/w.nu_m).ToString();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
        }

        

        private void Recalc(object sender, EventArgs e)
        {
            if (!RecalcFlag)
            {
                RecalcFlag = true;
                ts.Checked = true;
            }
            else
            {
                RecalcFlag = false;
                ts.Checked = false;
            }

        }

        void UnloadExactValues(object sender, EventArgs e)
        {
            SaveFileDialog Save = new SaveFileDialog();
            Save.DefaultExt = ".ext";
            Save.OverwritePrompt = true;
            Save.Filter = "ExactSolution files (*.ext)|*.ext|Все файлы (*.*)|*.*";
            Save.Title = "Save the exact solution";
            Save.ShowDialog();
            //
            try
            {
                List<Object> UVP = new List<object>(3);
                UVP.Add(wte.U);
                UVP.Add(wte.V);
                UVP.Add(wte.P);
                UVP.Add(m.AreaElems);
                UVP.Add(m.Sk);
                UVP.Add(m.CountElems);
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                FileStream fStream = new FileStream(Save.FileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(fStream, UVP);
                fStream.Close();
                UVP.Clear();
            }
            catch (Exception excpt)
            {
                MessageBox.Show("Не удалось выгрузить точное решение. Ошибка:" + excpt.Message);
            }
            Save = null;
        }
        void LoadExactValues(object sender, EventArgs e)
        {
            OpenFileDialog Open = new OpenFileDialog();
            Open.DefaultExt = ".ext";
            Open.Title = "Load the exact solution";
            Open.Filter = "ExactSolution files (*.ext)|*.ext|Все файлы (*.*)|*.*";
            Open.ShowDialog();
            try
            {
                FileStream fStream = new FileStream(Open.FileName, FileMode.Open, FileAccess.Read);
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                List<Object> UVP = (List<Object>)formatter.Deserialize(fStream);
                fStream.Close();
                ExactU = (double[])UVP[0];
                ExactV = (double[])UVP[1];
                ExactP = (double[])UVP[2];
                ExactAreaElems = (int[][])UVP[3];
                ExactSk = (double[])UVP[4];
                ExactCountElems = (int)UVP[5];
                //
                ExactItem.DropDownItems[2].Enabled = true;
            }
            catch (Exception excpt)
            {
                MessageBox.Show("Не удалось загрузить точное решение. Ошибка:" + excpt.Message);
            }
            Open = null;
        }
        private void CleanExactValues(object sender, EventArgs e)
        {
            ExactU = null;
            ExactV = null;
            ExactP = null;
            ExactE = null;
            ExactK = null;
            ExactAreaElems = null;
            ExactSk = null;
            ExactCountElems = 0;
            ExactItem.DropDownItems[2].Enabled = false;
           
        }
        private void FaceGr_Click(object sender, EventArgs e)
        {
            double dx = wte.Mesh.X[wte._Ny];
            double dy = wte.Mesh.Y[2] - wte.Mesh.Y[1];
            FaceGraph FG = null;
            if (BackwardStep)
            {
                L2 = 2.5;
                H2 = 0.5 * H;
                int Nx2 = (int)(L2 / dx) + 1;
                int Ny2 = (int)(H2 / dy) + 1;
                int Ny1 = wte.Mesh.CountRight - Ny2;
                FG = new FaceGraph(wte.U, wte.V, wte.P, wte.p_conv, wte.p_kinet, wte.u_press, wte.u_kinet, wte.u_cont, wte.Mesh.CountTop, wte.Mesh.CountRight, Nx2, Ny2, dx, dy);
            }
            else
            {
                //FG = new FaceGraph(wte.U, wte.V, wte.P, wte._Nx, wte._Ny, dx, dy, wte.E, wte.K, wte.S, wte.nu);
                kExp = new KwollExp(new string[] { "D10.txt", "D20.txt", "D30.txt", "Tuv_av.txt" });
                // под три волны
                kg = new KwollGeometry(Nx, 1.5, 1, N1);
                double[] x_12 = null, x_23 = null, x_123 = null, y_123 = null, ut_123 = null, vt_123 = null, tke_123 = null, nuT_123 = null, e_123 = null, Pk_123 = null, tau_123 = null;
                if (angle == 10)
                {
                    kExp.SlipData10(kg.ZeroPoint10 + 0.9 * Math.Round(DuneCount / 2.0, MidpointRounding.ToEven), out x_12);
                    kExp.SlipData10(kg.ZeroPoint10 + 0.9 * (Math.Round(DuneCount / 2.0, MidpointRounding.ToEven) + 1), out x_23);
                    //
                    x_123 = x_12.Concat(x_23).ToArray();
                    y_123 = kExp.y_10.Concat(kExp.y_10).ToArray();
                    ut_123 = kExp.u_t_10.Concat(kExp.u_t_10).ToArray();
                    vt_123 = kExp.v_t_10.Concat(kExp.v_t_10).ToArray();
                    tke_123 = kExp.TKE_10.Concat(kExp.TKE_10).ToArray();
                    nuT_123 = kExp.nuT_10.Concat(kExp.nuT_10).ToArray();
                    e_123 = kExp.e_10.Concat(kExp.e_10).ToArray();
                    Pk_123 = kExp.TP_10.Concat(kExp.TP_10).ToArray();
                    tau_123 = kExp.tuv_10.Concat(kExp.tuv_10).ToArray();
                }
                if (angle == 20)
                {
                    kExp.SlipData20(kg.ZeroPoint20 + 0.9 * Math.Round(DuneCount / 2.0, MidpointRounding.ToEven), out x_12);
                    kExp.SlipData20(kg.ZeroPoint20 + 0.9 * (Math.Round(DuneCount / 2.0, MidpointRounding.ToEven) + 1), out x_23);
                    //
                    x_123 = x_12.Concat(x_23).ToArray();
                    y_123 = kExp.y_20.Concat(kExp.y_20).ToArray();
                    ut_123 = kExp.u_t_20.Concat(kExp.u_t_20).ToArray();
                    vt_123 = kExp.v_t_20.Concat(kExp.v_t_20).ToArray();
                    tke_123 = kExp.TKE_20.Concat(kExp.TKE_20).ToArray();
                    nuT_123 = kExp.nuT_20.Concat(kExp.nuT_20).ToArray();
                    e_123 = kExp.e_20.Concat(kExp.e_20).ToArray();
                    Pk_123 = kExp.TP_20.Concat(kExp.TP_20).ToArray();
                    tau_123 = kExp.tuv_20.Concat(kExp.tuv_20).ToArray();
                }
                if (angle == 30)
                {
                    kExp.SlipData30(kg.ZeroPoint30 + 0.9 * Math.Round(DuneCount / 2.0, MidpointRounding.ToEven), out x_12);
                    kExp.SlipData30(kg.ZeroPoint30 + 0.9 * (Math.Round(DuneCount / 2.0, MidpointRounding.ToEven) + 1), out x_23);
                    //
                    x_123 = x_12.Concat(x_23).ToArray();
                    y_123 = kExp.y_30.Concat(kExp.y_30).ToArray();
                    ut_123 = kExp.u_t_30.Concat(kExp.u_t_30).ToArray();
                    vt_123 = kExp.v_t_30.Concat(kExp.v_t_30).ToArray();
                    tke_123 = kExp.TKE_30.Concat(kExp.TKE_30).ToArray();
                    nuT_123 = kExp.nuT_30.Concat(kExp.nuT_30).ToArray();
                    e_123 = kExp.e_30.Concat(kExp.e_30).ToArray();
                    Pk_123 = kExp.TP_30.Concat(kExp.TP_30).ToArray();
                    tau_123 = kExp.tuv_30.Concat(kExp.tuv_30).ToArray();
                }
                //
                //FG = new FaceGraph(wte.U, wte.V, wte.P, wte._Nx, wte._Ny, dx, dy, wte.W, wte.K, wte.S, wte.nuT, wte.CalcTauEverywhere(), wte.Pk, m.X, m.Y);
//!!!           //
                 FG = new FaceGraph(wte.U_plus, wte.V, wte.Y_plus, wte._Nx, wte._Ny, dx, dy, wte.W, wte.K, wte.S, wte.nuT, wte.CalcTauEverywhere(), wte.Pk, m.X, m.Y);

                //
                //FG.LoadExpData(x_123, y_123, ut_123, vt_123, tke_123, nuT_123, e_123, tau_123, Pk_123);
                //FG.LoadExpData(x10, kExp.y_10, kExp.u_t_10, kExp.v_t_10, kExp.TKE_10, kExp.nuT_10, kExp.e_10); // + kExp.tuv_10
                //double[][] L2 = CalcDiscreapancy(m.X, m.Y, wte.U, wte.V, wte.K, wte.TTauC, x_123, y_123, ut_123, vt_123, tke_123, tau_123);
            }
            FG.Show();
            
            // расчет tau_total
            //calcTauTotal();
            //
            //if (wte.ConvE != null)
            //{
            //    FaceGraph FG2 = new FaceGraph(wte.ConvK, wte.ConvE, wte.DiffK, wte._Nx, wte._Ny, dx, dy, wte.DiffE, wte.RegK, wte.RegE, wte.nuT, m.X, m.Y);
            //    FG2.Names = new[] { "CK", "CE", "DK", "DE", "RK", "RE", "nuT" };
            //    FG2.Show();
            //}
        }
        private void ch_Cuda_CheckedChanged(object sender, EventArgs e)
        {
            Cuda = ch_Cuda.Checked;
        }
        void OCl_CheckedChanged(object sender, EventArgs e)
        {
            OpenCL = OCl.Checked;
        }
        //
        private void tb_alpha_TextChanged(object sender, EventArgs e)
        {
            angle = Convert.ToInt32(tb_alpha.Text.ToString());
        }

        //
        #region Отрисовка
        IsoLines isoLines = new IsoLines();
        Pen pen = new Pen(Brushes.Black, 1.5f);
        Font f = new Font("TIMES", 10f);
        double MinX, MinY;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                double ddx = areaControl1.GetSetL / (meshLibControl1.ParameterObject[0].Nx - 1);
                if (wte == null)
                    lblUmax.Text = "Umax = " + (3.0 / 2.0 * w.Q / H).ToString() + ", ReH=" + (w.Q / w.nu_m).ToString("G5", CultureInfo.InvariantCulture) + ", Cu=" + (w.dt_local * (3.0 / 2.0 * w.Q / H) / ddx).ToString("G2", CultureInfo.InvariantCulture) + ", tmin=" + (2 * ddx * ddx / w.nu_m).ToString("G5", CultureInfo.InvariantCulture);
                else
                    lblUmax.Text = "Umax = " + (3.0 / 2.0 * w.Q / H).ToString() + ", ReH=" + (w.Q / w.nu_m).ToString("G5", CultureInfo.InvariantCulture) + ", Cu=" + (w.dt_local * (3.0 / 2.0 * w.Q / H) / ddx).ToString("G2", CultureInfo.InvariantCulture) + ", tmin=" + (2 * ddx * ddx / wte.nuT.Max()).ToString("G5", CultureInfo.InvariantCulture);
                if (wte != null)
                {
                    int IsoCount = 20;
                    WS = this.Width - Dx;
                    HS = this.Height - Dy;
                    MinX = m.X.Min();
                    MinY = m.Y.Min();
                    g = e.Graphics;
                    //
                    isoLines.Izo(m, wte.U, 0, 0, IsoCount);
                    double[][] X = isoLines.GetX;
                    double[][] Y = isoLines.GetY;
                    //
                    float size = f.Size;
                    Brush BrushText = Brushes.Black;
                    Brush BackColor = new SolidBrush(this.BackColor);
                    int c2 = 0, cx, cy;
                    //
                    for (int l = 0; l < IsoCount; l++)
                    {
                        for (int i = 0; i < X[l].Length - 1; i += 2)
                        {
                            cx = ScaleX(X[l][i]);
                            cy = ScaleY(Y[l][i]);
                            Point Point1 = new Point(cx, cy);
                            //
                            cx = ScaleX(X[l][i + 1]);
                            cy = ScaleY(Y[l][i + 1]);
                            Point Point2 = new Point(cx, cy);
                            //
                            g.DrawLine(pen, Point1, Point2);

                        }
                        //
                        if (X[l].Length > 3)
                        {
                            c2 = X[l].Length / 4;
                            // координаты отрисовки цифр
                            cx = ScaleX(X[l][c2]);
                            cy = ScaleY(Y[l][c2]);
                            string res = String.Format("{0:f2}", isoLines.Value(l));
                            //
                            RectangleF recWhite;
                            if (Math.Abs(X[l][c2] - X[l][c2 + 2]) > Math.Abs(Y[l][c2] - Y[l][c2 + 2]))
                            {
                                recWhite = new RectangleF(cx, cy, size * (res.Length - 1), size * 2);
                                cy -= (int)size;
                            }
                            else
                            {
                                recWhite = new RectangleF(cx, cy, size * 2, size * 2);
                                cx -= (int)size * 2;
                            }
                            g.FillRectangle(BackColor, recWhite);
                            Point point = new Point(cx, cy);

                            g.DrawString(res, f, BrushText, point);
                        }
                    }

                    Pen lpen = new Pen(Color.Brown, 1.5f);
                    for (int p = 0; p < m.CountLeft - 1; p++)
                    {
                        int num1 = m.LeftKnots[p];
                        int num2 = m.LeftKnots[p + 1]; ;
                        //
                        Point pp1 = new Point(ScaleX(m.X[num1]), ScaleY(m.Y[num1]));
                        Point pp2 = new Point(ScaleX(m.X[num2]), ScaleY(m.Y[num2]));
                        g.DrawLine(lpen, pp1, pp2);
                        //
                        num1 = m.RightKnots[p];
                        num2 = m.RightKnots[p + 1]; ;
                        //
                        pp1 = new Point(ScaleX(m.X[num1]), ScaleY(m.Y[num1]));
                        pp2 = new Point(ScaleX(m.X[num2]), ScaleY(m.Y[num2]));
                        g.DrawLine(lpen, pp1, pp2);

                    }
                    //
                    for (int p = 0; p < m.CountBottom - 1; p++)
                    {
                        int num1 = m.BottomKnots[p];
                        int num2 = m.BottomKnots[p + 1];
                        //
                        Point pp1 = new Point(ScaleX(m.X[num1]), ScaleY(m.Y[num1]));
                        Point pp2 = new Point(ScaleX(m.X[num2]), ScaleY(m.Y[num2]));
                        g.DrawLine(lpen, pp1, pp2);
                        //
                        num1 = m.TopKnots[p];
                        num2 = m.TopKnots[p + 1]; ;
                        //
                        pp1 = new Point(ScaleX(m.X[num1]), ScaleY(m.Y[num1]));
                        pp2 = new Point(ScaleX(m.X[num2]), ScaleY(m.Y[num2]));
                        g.DrawLine(lpen, pp1, pp2);

                    }
                }
                if (rt != null)
                {
                    Pen BedPen = new Pen(Color.SandyBrown, 2.0f);
                    int y = this.Height-50;
                    int x = 0;
                    
                    int cx1, cx2, cy1, cy2;
                    for (int i = 0; i < Nx-1; i++)
                    {
                        cy1 =  ScaleY(rt.Zeta[i])+10;
                        cy2 = ScaleY(rt.Zeta[i+1])+10;
                        cx1 = ScaleX(i*dx);
                        cx2 = ScaleX((i+1)*dx);
                        g.DrawLine(BedPen, cx1, cy1, cx2, cy2);
                    }
                }
                //    if (m != null)
                //{
                //    float xmin = (float)m.X.Min();

                //    float ymin = (float)m.Y.Min();
                //    float xmax = (float)m.X.Max();
                //    float ymax = (float)m.Y.Max();
                //    //
                //    Rec = new RectangleF(xmin, ymax, xmax - xmin, ymax - ymin);
                //    WS = this.Width - 200;
                //    HS = this.Height - 200;
                //    //
                //    Graphics g = this.CreateGraphics();
                //    Pen p = new Pen(Brushes.Black, 1.0f);
                //    Font f = new Font("TIMES", 8.0f);
                //    for (int i = 0; i < m.CountElems; i++)
                //    {
                //        Point[] pc = new Point[3];
                //        FPoint[] ps = new FPoint[3];
                //        for (int j = 0; j < 3; j++)
                //            ps[j] = new FPoint((float)m.X[m.AreaElems[i][j]], (float)m.Y[m.AreaElems[i][j]]);
                //        for (int j = 0; j < 3; j++)
                //            pc[j] = Mashtabing(ps[j]);
                //        //
                //        g.DrawLine(p, pc[0], pc[1]);
                //        g.DrawLine(p, pc[1], pc[2]);
                //        g.DrawLine(p, pc[2], pc[0]);
                //    }
                //    for (int i = 0; i < m.CountKnots; i++)
                //    {
                //        FPoint pp = new FPoint((float)m.X[i], (float)m.Y[i]);
                //        Point a = Mashtabing(pp);
                //        //
                //        float v = (float)wte.U[i];
                //        string line = String.Format("{0:F3}", v);
                //        g.DrawString(line, f, Brushes.Blue, a);
                //    }
                //}
            }
            catch (Exception ex)
            { }
        }
      
        protected Point Mashtabing(FPoint p)
        {
            float X = p.X;
            float Y = p.Y;
            int x = (int)((X - Rec.X) / Rec.Width * WS) + 10;
            int y = (int)((1 - (Rec.Y-Y) / Rec.Height) * HS) + 30;
            return new Point(x, y);
        }
        /// <summary>
        /// Расчет экранной координаты по х
        /// </summary>
        /// <param name="xa"></param>

        protected int ScaleX(double xa)
        {
            int ix = (int)(WS * (xa - MinX) / Rec.Width) + 10;
            return ix;
        }
        protected int ScaleX(double xa, double minX, double maxX)
        {
            int ix = (int)(WS * (xa - minX) / (maxX - minX)) + 10;
            return ix;
        }
        /// <summary>
        /// Расчет экранной координаты по y
        /// </summary>
        /// <param name="xa"></param>
        /// <returns></returns>
        protected int ScaleY(double yc)
        {
            int iy = (int)(HS - HS * (yc - MinY) / Rec.Height) + 90;
            return iy;
        }
        protected int ScaleY(double yc, double minY, double maxY)
        {
            int iy = (int)(HS - HS * (yc - minY) / (maxY - minY)) + 90;
            return iy;
        }

        #endregion
        //
        #region Сериализация
        private void btnSerialize_Click(object sender, EventArgs e)
        {
            if (wte != null)
            {
                wte.SerializeNow = true;
                //backgroundWorker.CancelAsync();
                //BackgroundThread.Join(); // ожидание окончания действий в фоновом потоке
                //while (wte.status == "Running")
                //{
                //    Thread.Sleep(1000);
                //}
                try
                {

                    do
                    {

                    } while (wte.status != "Stoped");

                    formatter = new BinaryFormatter();
                    formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    using (var fStream = new FileStream("./WaterTask.dat", FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        formatter.Serialize(fStream, wte);
                    }

                }
                catch (Exception ex)
                { }
            }
            else
                MessageBox.Show("Сериализовать можно только текущий расчет (поставить на паузу)");

        }

        private void btnDeSerialize_Click(object sender, EventArgs e)
        {
            if (wte != null)
            {
                if ((wte.status == "Stoped") && (formatter != null))
                {
                    formatter = new BinaryFormatter();
                    formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    using (var fStream = File.OpenRead("./WaterTask.dat"))
                    {
                        wte = (WaterTaskEliz)formatter.Deserialize(fStream);

                    }
                    wte.SerializeNow = false;
                    backgroundWorker.RunWorkerAsync();
                    MessageBox.Show("Расчет восстановлен", "Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("Сериализовать можно только текущий расчет (поставить на паузу)");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog2.ShowDialog();
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                Prs = meshLibControl1.ParameterObject;
                r = riverTaskControl1.ParameterObject;
                List<Object> allParam = new List<object>();
                SaveParameters(allParam);
                //
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                FileStream fStream = new FileStream(saveFileDialog2.FileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(fStream, allParam);
                fStream.Close();
                this.Text = saveFileDialog2.FileName;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось сериализовать параметры");
            }
        }

        private void SaveParameters(List<Object> allParam)
        {
            //Сохраняем все параметры listBox-ов
            //
            allParam.Add(algebraLibControl1.AlgorythmSelectedIndex); // вид алгоритма
            allParam.Add(algebraLibControl1.SystemSelectedIndex); // вид матрицы
            //
            allParam.Add(meshLibControl1.GetSetBottomLayer); // нижниый послой
            allParam.Add(meshLibControl1.GetSetTopLayer);// верхний подслой
            allParam.Add(meshLibControl1.GetSetParts); // количество частей
            int[] sindexes = meshLibControl1.GetSetSelectedIndexes; // методы разбиения для всех частей
            for (int i = 0; i < 3; i++)
                allParam.Add(sindexes[i]);
            //
            allParam.Add(areaControl1.GetSetH); // глубина
            allParam.Add(areaControl1.GetSetL);// длина
            int[] aindx = areaControl1.GetSetSelectedIndexes; // сплайны верхней и нижней границы
            for (int i = 0; i < 2; i++)
                allParam.Add(aindx[i]);
            //
            //DataTable[] dt = areaControl1.GetSetXYCoords;
            //for (int i = 0; i < dt.Length; i++)
            //    allParam.Add(dt[i]);
            allParam.Add(areaControl1.GetSetDataSet); // Таблица введенных значений точек границ
            //
            //Сохраняем параметры расчета propertyGrid
            allParam.Add(w); // параметры расчета гидродинамики
            allParam.Add(r); // параметры расчета дна
            allParam.Add(riverTaskControl1.GetSetSelectedIndex);
            allParam.Add(txtDt.Text);// шаг по времени
            allParam.Add(txtTime.Text); // время расчета
            allParam.Add(time.TotalSeconds);
            for (int i = 0; i < Prs.Length; i++)
                allParam.Add(Prs[i]); // параметры сеток
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                int ch = 0;
                List<Object> allParam = null;
                FileStream fStream = new FileStream(openFileDialog2.FileName, FileMode.Open, FileAccess.Read);
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                //formatter = new SoapFormatter();
                allParam = (List<Object>)formatter.Deserialize(fStream);
                fStream.Close();
                //
                ch = LoadParameters(ch, allParam);
                this.Text = openFileDialog2.FileName;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось загрузить параметры");
            }
        }

        private int LoadParameters(int ch, List<Object> allParam)
        {
            algebraLibControl1.AlgorythmSelectedIndex = (int)allParam[ch++];
            algebraLibControl1.SystemSelectedIndex = (int)allParam[ch++];
            //
            meshLibControl1.GetSetBottomLayer = (double)allParam[ch++];
            meshLibControl1.GetSetTopLayer = (double)allParam[ch++];
            meshLibControl1.GetSetParts = (int)allParam[ch++];
            int[] SInx = { (int)allParam[ch++], (int)allParam[ch++], (int)allParam[ch++] };
            meshLibControl1.GetSetSelectedIndexes = SInx;
            //
            H = (double)allParam[ch++];
            L = (double)allParam[ch++];
            areaControl1.GetSetH = H;
            areaControl1.GetSetL = L;
            int[] AInx = { (int)allParam[ch++], (int)allParam[ch++] };
            areaControl1.GetSetSelectedIndexes = AInx;
            //
            areaControl1.GetSetDataSet = (DataSet)allParam[ch++];
            //
            w = (WElizParameter)allParam[ch++];
            r = (BedPhysicsParams)allParam[ch++];
            riverTaskControl1.GetSetSelectedIndex = (int)allParam[ch++];
            txtDt.Text = (string)allParam[ch++];
            txtTime.Text = (string)allParam[ch++];
            if (allParam[ch] is double)
                time_sec = (double)allParam[ch++];
            //
            int countPrs = allParam.Count - ch;
            Prs = new Parameter[countPrs];
            for (int i = 0; i < countPrs; i++)
                Prs[i] = (Parameter)allParam[ch++];

            //
            prpMainParameters.SelectedObject = w;
            riverTaskControl1.ParameterObject = r;
            //
            Parameter[] TMB = new Parameter[3];
            if (meshLibControl1.GetSetTopLayer != 0)
            {
                TMB[0] = Prs[0];
                TMB[1] = Prs[1];
                if (BottomLayer != 0)
                    TMB[2] = Prs[2];
            }
            if (meshLibControl1.GetSetTopLayer == 0)
            {
                TMB[1] = Prs[0];
                if (BottomLayer != 0)
                    TMB[2] = Prs[1];
            }
            //
            meshLibControl1.ParameterObject = TMB;
            return ch;

        }

        private void saveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SaveFileDialog Save = new SaveFileDialog();
            Save.FileName = "Task";
            Save.Filter = "\"Data files (*.dat)|*.dat|All files (*.*)|*.*\"";
            Save.InitialDirectory = "\\";
            Save.Title = "Save the task done";
            Save.DefaultExt = ".dat";
            Save.OverwritePrompt = true;
            Save.ShowDialog();

            try
            {
                List<Object> AllTaskObject = new List<object>();
                AllTaskObject.Add(wte);
                AllTaskObject.Add(rt);
                AllTaskObject.Add(BuffTau);
                AllTaskObject.Add(BuffQb);
                AllTaskObject.Add(BuffZeta);
                AllTaskObject.Add(BuffQs);
                //
                SaveParameters(AllTaskObject);
                //
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                FileStream fStream = new FileStream(Save.FileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(fStream, AllTaskObject);
                fStream.Close();
                this.Text = Save.FileName;
                Save = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить решение. " + ex.Message);
            }
        }


        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Open = new OpenFileDialog();
            Open.InitialDirectory = "\\";
            Open.DefaultExt = ".dat";
            Open.Title = "Open the task done";
            Open.Filter = "\"Data files (*.dat)|*.dat|All files (*.*)|*.*\"";
            Open.ShowDialog();
            //
            this.Enabled = false;
            // форма ожидания
            Form newf = new Form();
            GenerateWaitForm(newf);
            newf.Show();
            Application.DoEvents();
            //
            try
            {
                List<Object> AllTasks = null;
                FileStream fStream = new FileStream(Open.FileName, FileMode.Open, FileAccess.Read);
                //
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                //formatter = new SoapFormatter();
                AllTasks = (List<Object>)formatter.Deserialize(fStream);
                fStream.Close();

                int ch = 0;
                wte = (WaterTaskEliz)AllTasks[ch++];
                rt = (BaseBedLoadTask)AllTasks[ch++];
                BuffTau = (double[][])AllTasks[ch++];
                BuffQb = (double[][])AllTasks[ch++];
                BuffZeta = (double[][])AllTasks[ch++];
                BuffQs = (double[][])AllTasks[ch++];
                ch = LoadParameters(ch, AllTasks);
                //
                m = wte.Mesh;
                Nx = wte._Nx;
                dx = rt.dx;
                Xt = new double[m.CountBottom];
                for (int i = 0; i < m.CountBottom; i++)
                    Xt[i] = m.X[m.BottomKnots[i]];
                Rec = new RectangleF((float)m.X.Min(), (float)m.Y.Max(), (float)(m.X.Max() - m.X.Min()), (float)(m.Y.Max() - m.Y.Min()));
                this.Text = Open.FileName;
                Open = null;
                //
                if (wte.nuT == null)
                {
                    wte.nuT = new double[wte.Mesh.CountKnots];
                    for (int i = 0; i < wte.Mesh.CountKnots; i++)
                        wte.nuT[i] = wte.C_m * wte.K[i] * wte.K[i] / (wte.E[i] + 1e-25);
                }
                //
                toolStripStatusLabel1.Text = "Количество треугольников = " + wte.CountTriangles.ToString() + ", узлов=" + wte.CountKnots;
                toolStripStatusLabel2.Text = " Q_in=" + wte.Get_Q_in + " Q_out=" + wte.Get_Q_out + " dQ=" + Convert.ToString((wte.Get_Q_in - wte.Get_Q_out) / wte.Get_Q_in * 100) + " %";
                //toolStripStatusLabel3.Text = "Количество итераций = " + wte.Iter + " tau' = " + tauPr.ToString() + " dy=" + ((wte.Mesh.Y[1] - wte.Mesh.Y[0])*100).ToString().Substring(0,5)+" см.";
                toolStripStatusLabel3.Text = "Количество итераций wtask = " + wte.Iter + ". Время: " + time_sec.ToString(); ;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить решение. " + ex.Message );
            }
            finally
            {
                newf.Close();
                this.Enabled = true;
                //
        }
            //this.SetTopLevel(true);
        }

        private void GenerateWaitForm(Form newf)
        {
            newf.Owner = this;
            newf.Text = "Loading...";
            newf.Width = 170;
            newf.Height = 220;
            PictureBox pb = new PictureBox();
            pb.Width = 150;
            pb.Height = 150;
            pb.LoadAsync("..\\..\\Resources\\load.png");
            newf.Controls.Add(pb);
            Label l = new Label();
            l.Text = "Loading...";
            l.Font = new System.Drawing.Font("TIMES", 14.0f);
            newf.Controls.Add(l);
            l.Size = new System.Drawing.Size(150, 50);
            l.Location = new Point(25, 150);
            newf.Enabled = false;
            newf.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Загрузка вертикального профиля скорости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Velocity_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog Open = new OpenFileDialog();
            Open.DefaultExt = ".uprof";
            Open.Title = "Load the velocity profile";
            Open.Filter = "VelocityProfile files (*.uprof)|*.ext|Все файлы (*.*)|*.*";
            Open.ShowDialog();
            try
            {
                FileStream fStream = new FileStream(Open.FileName, FileMode.Open, FileAccess.Read);
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                List<Object> Uprof = (List<Object>)formatter.Deserialize(fStream);
                fStream.Close();
                U_start = (double[])Uprof[0];
                V_start = (double[])Uprof[1];
                K_start = (double[])Uprof[2];
                E_start = (double[])Uprof[3];
                NuT_start = (double[])Uprof[4];
                Y_start = (double[])Uprof[5];
                dpdx_start = (double)Uprof[6];
                W_start = (double[])Uprof[7];
                if (Uprof.Count==9)
                    S_start = (double[])Uprof[8];

            }
            catch (Exception excpt)
            {
                MessageBox.Show("Не удалось загрузить профиль скорости. Ошибка:" + excpt.Message);
                U_start = null;
                V_start = null;
                K_start = null;
                E_start = null;
                NuT_start = null;
                Y_start = null;
                W_start = null;
            }
            Open = null;
        }
        /// <summary>
        /// Сохранение профиля скорости из серединного вертикального сечения области
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Velocity_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog Save = new SaveFileDialog();
            Save.DefaultExt = ".uprof";
            Save.OverwritePrompt = true;
            Save.Filter = "VelocityProfile files (*.uprof)|*.ext|Все файлы (*.*)|*.*";
            Save.Title = "Save the velocity profile";
            Save.ShowDialog();
            //
            try
            {
                if (wte != null)
                {
                    List<Object> Uprof = new List<object>(2);
                    U_start = new double[wte.Mesh.CountLeft];
                    V_start = new double[wte.Mesh.CountLeft];
                    K_start = new double[wte.Mesh.CountLeft];
                    E_start = new double[wte.Mesh.CountLeft];
                    NuT_start = new double[wte.Mesh.CountLeft];
                    Y_start = new double[wte.Mesh.CountLeft];
                    W_start = new double[wte.Mesh.CountLeft];
                    S_start = new double[wte.Mesh.CountLeft];
                    int midP = wte.Mesh.CountBottom * 3 / 4 * wte.Mesh.CountLeft;
                    for (int i = 0; i < U_start.Length; i++)
                    {
                        U_start[i] = wte.U[midP + i];
                        V_start[i] = wte.V[midP + i];
                        K_start[i] = wte.K[midP + i];
                        E_start[i] = wte.E[midP + i];
                        NuT_start[i] = wte.nuT[midP + i];
                        Y_start[i] = wte.Mesh.Y[midP + i];
                        W_start[i] = wte.W[midP + i];
                        S_start[i] = wte.S[midP + i];
                    }
                    Uprof.Add(U_start);
                    Uprof.Add(V_start);
                    Uprof.Add(K_start);
                    Uprof.Add(E_start);
                    Uprof.Add(NuT_start);
                    Uprof.Add(Y_start);
                    Uprof.Add(wte.dPdx);
                    Uprof.Add(W_start);
                    Uprof.Add(S_start);
                    //
                    formatter = new BinaryFormatter();
                    formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    FileStream fStream = new FileStream(Save.FileName, FileMode.Create, FileAccess.Write);
                    formatter.Serialize(fStream, Uprof);
                    fStream.Close();
                    Uprof.Clear();
                }
            }
            catch (Exception excpt)
            {
                MessageBox.Show("Не удалось выгрузить профиль скорости. Ошибка:" + excpt.Message);
            }
            Save = null;
        }
        //
        /// <summary>
        /// удаление ранее установленного профиля скорости и других профилей из решения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Velocity_Delete_Click(object sender, EventArgs e)
        {
            U_start = null;
            V_start = null;
            K_start = null;
            E_start = null;
            NuT_start = null;
            Y_start = null;
            W_start = null;
            //
            MessageBox.Show("Профиль скорости на входе в область удален");
        }
        #endregion
        int DuneCount = 6;
        double StabL = 1.5;
        int N1 = 1;
        int N2 = 8;
        private void button1_Click(object sender, EventArgs e)
        {
            //
            button1.Enabled = false;
            taskToolStripMenuItem.DropDownItems[3].Enabled = false;
            taskToolStripMenuItem.DropDownItems[4].Enabled = false;
            ExactItem.DropDownItems[0].Enabled = false;
            //
            stopW.Restart();
            // если выбраны оба флага, применяется CPU версия
            if ((OpenCL) && (Cuda))
            {
                OpenCL = false;
                Cuda = false;
            }
            //
            dt = Convert.ToDouble(txtDt.Text);
            double TimeInSec = Convert.ToDouble(txtTime.Text);
            GlobalIteration = (int)(TimeInSec / dt);
            //
           
            H = areaControl1.GetSetH;
            L = areaControl1.GetSetL;
            //
            alg = algebraLibControl1.GetAlgorythmObject();
            sys = algebraLibControl1.GetSysyemObject();
            //
            Prs = meshLibControl1.ParameterObject;
            parts = meshLibControl1.GetSetParts;
            TopLayer = meshLibControl1.GetSetTopLayer;
            BottomLayer = meshLibControl1.GetSetBottomLayer;
            //
            Nx = Prs[0].Nx * parts;
            //
            InitMethod();
            // ---Получение геометрии дна

            //
            // --одна волна
            //double[] ZZ = new double[m.CountBottom];
            //int n1 =Convert.ToInt32( 1.0 / dx);
            //int n2 = Convert.ToInt32(1.5 / dx);
            //for (int k = n1; k < n2; k++)
            //    ZZ[k] = 0.05 * Math.Sin(2 * 3.14 * k * dx);
            ////    
            //rt.SetZeta0(ZZ);
            //--
            // --бугорок
            //double[] ZZ = new double[m.CountBottom];
            //int n1 = Convert.ToInt32(0.5 / dx);
            //int n2 = Convert.ToInt32(0.7 / dx);
            //for (int k = n1; k < n2; k++)
            //    ZZ[k] = 0.04 * Math.Cos((3.14 * k * dx) / 2.0 / 0.1) * Math.Cos((3.14 * k * dx) / 2.0 / 0.1);
            //rt.SetZeta0(ZZ);
            //--
            
            // получение параметров расчетной области
            double[][] coords = areaControl1.GetInterpolatedCoords(Nx);
            double[] Z = coords[0]; // данные из формы
            dx = areaControl1.dx;
            //
            //if (angle != 0)
            //{
            //    kg = new KwollGeometry(Nx, StabL, DuneCount, N1, N2); // из эксперимента Kwoll
            //    if (angle == 10)
            //        Z = kg.Z10;
            //    if (angle == 20)
            //        Z = kg.Z20;
            //    if (angle == 30)
            //        Z = kg.Z30;
            //    areaControl1.GetSetL = DuneCount * 0.9 + StabL * (2 + N1 + N2);
            //}
            //
            //
            // --одна волна
            //int n1 = Convert.ToInt32(2.0 / dx);
            //int n2 = Convert.ToInt32(2.3 / dx);
            //for (int k = n1; k < n2; k++)
            //    Z[k] = 0.003 * Math.Cos(66.66666666 * Math.PI * k * dx);
            //
            //int n1 = Convert.ToInt32(1.0 / dx);
            //int n2 = Convert.ToInt32(1.5043 / dx);
            //for (int k = n1; k < n2; k++)
            //    Z[k] = 0.005043818168 * Math.Cos(39.65250002 * Math.PI * k * dx);
            //
            Yt = coords[1];
            Xt = coords[2];
            
            Area ar = new Area(Xt, Yt, Xt, Z, BottomLayer, TopLayer, parts);
            //
            mb = new MeshBuilder(ar, Prs, w.surf_flag);
            sys.OpenCL = this.OpenCL;
            alg.OpenCL = this.OpenCL;
            //mb.OpecCL = this.OpenCL;
            //mb.Cuda = this.Cuda;
            //
            //
            if (!RecalcFlag)
            {
                if (BackwardStep)
                {
                    L2 = 2.5;
                    H2 = 0.5 * H;
                    mb.GenerateMeshWithBackwardStep(L2, H2);
                }
                else
                    mb.GenerateMesh(true);
                m = mb.FinalMesh;
                Rec = new RectangleF((float)m.X.Min(), (float)m.Y.Max(), (float)(m.X.Max() - m.X.Min()), (float)(m.Y.Max() - m.Y.Min()));
                //
                r = riverTaskControl1.ParameterObject;
                rt = riverTaskControl1.GetRiverTaskObject();
                RiverTaskLibrary.BoundaryCondition BC = new RiverTaskLibrary.BoundaryCondition(RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed, RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed, 0.0, 0.0);
                rt.ReStartBaseBedLoadTask(BC, r, dx, m.CountBottom, dt);
                rt.SetZeta0(ar.YBottom);
                //
                //w = (WElizParameter)prpMainParameters.SelectedObject;
                w.time_b = dt;
                wte = new WaterTaskEliz(w, m, sys, alg, Cuda, OpenCL, U_start, V_start, K_start, E_start, NuT_start, dpdx_start, W_start, S_start);
                wte.ChangeMesh(m);
                wte.InitialConditions();
            }
            wte.err = "ok";
            rt.Message = "ok";
            rt.ReStartBaseBedLoadTask(r, dx, m.CountBottom, dt);
            wte.ChangeParameters(w);
            backgroundWorker.RunWorkerAsync();
            //BackgroundThread = new Thread(StartBackgroundThread);
            //BackgroundThread.IsBackground = true;
            //BackgroundThread.Start();

        }
        float tauPr = 0;
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            //{
            double[] Pbed = new double[m.CountKnots];
            double[] RS = new double[m.CountKnots];
            bool generate = true;
            int ch = 0;
            double percentage = 100.0 / (GlobalIteration);
            //progressBar1.Value = 0;
            wte.status = "Running";
            wte.ContinueToCalc = RecalcFlag;
            for (int i = BeginIter; i < GlobalIteration; i++)
            {
                Area ar = new Area(Xt, Yt, Xt, rt.Zeta, BottomLayer, TopLayer, parts);
                mb.ChangeArea(ar);
                if (BackwardStep)
                    mb.GenerateMeshWithBackwardStep(L2, H2);
                else
                {
                    mb.GenerateMesh(generate);
                    generate = false;  // -- если разбивка адаптивная, то закомментить
                }
                m = mb.FinalMesh;
                wte.ChangeMesh(m);
                //
                wte.Run();
                //wte.All_Calc_Parallel();
                //
                if (wte.SerializeNow)
                {
                    MessageBox.Show("Расчет приостановлен", "Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    wte.status = "Stoped";
                    BeginIter = i;
                    return;
                }
                if (wte.err != "ok")
                    break;
                // вычисление ошибки вычисления напряжения в случае прямого дна
                tauPr = 0; double a = 0;
                for (int k = 0; k < wte.BTau.Length; k++)
                {
                    a = Math.Abs(wte.BTau[k] - wte.BTau[0]) / wte.BTau[0];
                    if (a > tauPr)
                        tauPr = (float)a;
                }
                if (r.otstup < wte.Mesh.CountBottom)
                {
                    Pbed = GetPbed();        
                    //
                    RS = wte.GetRS;
                    //
                    rt.MySolve(wte.BTauC, null);
                    //rt.Solve(wte.BTau);
                    //if (dt != rt.dt)
                    //{
                    //    double leastTime = (GlobalIteration - i) * dt;
                    //    dt = rt.dt;
                    //    GlobalIteration = (int)(leastTime / rt.dt);
                    //    i = 0;
                    //    if (w.dt_local >= dt)
                    //        w.dt_local = dt / 10.0;
                    //    //
                    //    w.time_b = dt;
                    //    wte.ChangeTimeStep(w);
                    //    //txtDt.Text += "+" + dt.ToString();
                    //}
                }
                if ((wte.err != "ok") || (rt.Message != "ok"))
                {
                    Exception exc = new Exception(wte.err + rt.Message + "на " + i.ToString() + " итерации");
                    //
                    for (int j = 0; j < Nx - 1; j++)
                    {
                        BuffZeta[ch][j] = rt.Zeta[j];
                        BuffTau[ch][j] = wte.BTau[j];
                        BuffTauT[ch][j] = wte.TTau[j];
                        BuffQb[ch][j] = rt.Qb[j];
                        BuffQs[ch][j] = RS[j];
                    }
                    BuffZeta[ch][Nx - 1] = rt.Zeta[Nx - 1];
                    BuffQb[ch][Nx - 1] = BuffQb[ch][Nx - 2];
                    BuffTau[ch][Nx - 1] = BuffTau[ch][Nx - 2];
                    BuffTauT[ch][Nx - 1] = BuffTauT[ch][Nx - 2];
                    BuffQs[ch][Nx - 1] = BuffQs[ch][Nx - 2];
                    //
                    throw (exc);
                }
                // Репорт на форму
                backgroundWorker.ReportProgress((int)(percentage * (i + 1)));
                // буферизация данных 1D
                if (i == TimeFaces[ch])
                {
                    for (int j = 0; j < Nx - 1; j++)
                    {
                        BuffZeta[ch][j] = rt.Zeta[j];
                        BuffTau[ch][j] = wte.BTau[j];
                        BuffTauT[ch][j] = wte.TTau[j];
                        BuffQb[ch][j] = rt.Qb[j];
                        BuffQs[ch][j] = RS[j];
                    }
                    BuffZeta[ch][Nx - 1] = rt.Zeta[Nx - 1];
                    BuffQb[ch][Nx - 1] = BuffQb[ch][Nx - 2];
                    BuffTau[ch][Nx - 1] = BuffTau[ch][Nx - 2];
                    BuffTauT[ch][Nx - 1] = BuffTauT[ch][Nx - 2];
                    BuffQs[ch][Nx - 1] = BuffQs[ch][Nx - 2];
                    ch++;
                }

            }
            mb.ReturnNx(Nx);
            meshLibControl1.Invalidate();
            wte.status = "Done";
            //}
            //catch (Exception ex)
            //{
            //    e.Result = ex;
            //}
            //
            //тест устойчивости дна по работе в КиМ - все хорошо
            //int ch = 0;
            //double percentage = 100.0 / (GlobalIteration);
            //double[] BTau = new double[Nx];
            //for (int i = r.otstup; i < Nx; i++)
            //    BTau[i] = 1;//14.87 * Math.Cos(2.0 * Math.PI * (i - r.otstup) * dx);//
            ////
            //double[] RS = new double[Nx];
            //for (int i = 0; i < Nx; i++)
            //    RS[i] = 0.001;
            //for (int i = BeginIter; i < GlobalIteration; i++)
            //{
            //    rt.MySolve(BTau, null);
            //    for (int j = 0; j < Nx - 1; j++)
            //    {
            //        BuffZeta[ch][j] = rt.Zeta[j];
            //        BuffTau[ch][j] = BTau[j];
            //        BuffTauT[ch][j] = wte.TTau[j];
            //        BuffQb[ch][j] = rt.Qb[j];
            //        BuffQs[ch][j] = RS[j];
            //    }
            //    BuffZeta[ch][Nx - 1] = rt.Zeta[Nx - 1];
            //    BuffQb[ch][Nx - 1] = BuffQb[ch][Nx - 2];
            //    BuffTau[ch][Nx - 1] = BuffTau[ch][Nx - 2];
            //    BuffTauT[ch][Nx - 1] = BuffTauT[ch][Nx - 2];
            //    BuffQs[ch][Nx - 1] = BuffQs[ch][Nx - 2];
            //    // Репорт на форму
            //    backgroundWorker.ReportProgress((int)(percentage * (i + 1)));
            //    // буферизация данных 1D
            //    if (i == TimeFaces[ch])
            //    {
            //        for (int j = 0; j < Nx - 1; j++)
            //        {
            //            BuffZeta[ch][j] = rt.Zeta[j];
            //            BuffTau[ch][j] = BTau[j];
            //            BuffTauT[ch][j] = wte.TTau[j];
            //            BuffQb[ch][j] = rt.Qb[j];
            //            BuffQs[ch][j] = RS[j];
            //        }
            //        BuffZeta[ch][Nx - 1] = rt.Zeta[Nx - 1];
            //        BuffQb[ch][Nx - 1] = BuffQb[ch][Nx - 2];
            //        BuffTau[ch][Nx - 1] = BuffTau[ch][Nx - 2];
            //        BuffTauT[ch][Nx - 1] = BuffTauT[ch][Nx - 2];
            //        BuffQs[ch][Nx - 1] = BuffQs[ch][Nx - 2];
            //        ch++;
            //    }

            //}
        }

        private double[] GetPbed()
        {
            double[] Pb = new double[wte.Mesh.CountBottom];
            int ny = wte.Mesh.CountLeft;
            for (int h = 0; h < Pb.Length; h++)
            {
                Pb[h] = wte.P[h * ny] / 9.8 / 1000;
            }
            return Pb;
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show( "Расчет отменен!", "Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {
               MessageBox.Show( "Error: " + e.Error.Message,"Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                stopW.Stop();
                MessageBox.Show("Расчет окончен." + wte.err, "Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //
            button1.Enabled = true;
            taskToolStripMenuItem.DropDownItems[3].Enabled = true;
            taskToolStripMenuItem.DropDownItems[4].Enabled = true;
            ExactItem.DropDownItems[0].Enabled = true;
            //
            time = stopW.Elapsed;
            toolStripStatusLabel3.Text = "Количество итераций wtask = " + wte.Iter + " Выполнено: 100 %;";
            toolStripStatusLabel3.Text += "\n Общ.время: " + time.TotalSeconds+" c. Сетка: "+mb.timeAll.TotalMilliseconds+" мс. ";
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = "Количество треугольников = " + wte.CountTriangles.ToString() + ", узлов=" + wte.CountKnots;
            toolStripStatusLabel2.Text = " Q_in=" + wte.Get_Q_in + " Q_out=" + wte.Get_Q_out + " dQ=" + Convert.ToString((wte.Get_Q_in - wte.Get_Q_out) / wte.Get_Q_in * 100) + " %";
            //toolStripStatusLabel3.Text = "Количество итераций = " + wte.Iter + " tau' = " + tauPr.ToString() + " dy=" + ((wte.Mesh.Y[1] - wte.Mesh.Y[0])*100).ToString().Substring(0,5)+" см.";
            toolStripStatusLabel3.Text = "Количество итераций wtask = " + wte.Iter + " Выполнено: " + e.ProgressPercentage.ToString() + " %;";
        }

        private void canselToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
        }
        private void InitMethod()
        {
            // временные срезы для буферизации Zeta
            TimeFaces = new int[4];
            TimeFaces[0] = 0;
            TimeFaces[1] = GlobalIteration / 3;
            TimeFaces[2] = GlobalIteration * 2 / 3;
            TimeFaces[3] = GlobalIteration - 1;
            //
            BuffZeta = new double[TimeFaces.Length][];
            BuffTau = new double[TimeFaces.Length][];
            BuffTauT = new double[TimeFaces.Length][];
            BuffQb = new double[TimeFaces.Length][];
            BuffQs = new double[TimeFaces.Length][];
            for (int i = 0; i < TimeFaces.Length; i++)
            {
                BuffZeta[i] = new double[Nx];
                BuffTau[i] = new double[Nx];
                BuffTauT[i] = new double[Nx];
                BuffQb[i] = new double[Nx];
                BuffQs[i] = new double[Nx];
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            //// основная панель стается слева
            //MainPanel.Height = this.Height - 20;
            //MainPanel.Width = 250;
            // область ввода параметров растягивается по вертикали
            int ParamH = this.Height - 180 - 50;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Invalidate();
            H = areaControl1.GetSetH;
            lblUmax.Text = "Umax = " + (3.0 / 2.0 * w.Q / H).ToString();
            if (wte != null)
                if(wte.status!="Done")
            {
                toolStripStatusLabel2.Text = " Q_in=" + wte.Get_Q_in + " Q_out=" + wte.Get_Q_out + " dQ=" + Convert.ToString((wte.Get_Q_in - wte.Get_Q_out) / wte.Get_Q_in * 100) + " %";
                toolStripStatusLabel3.Text = "Количество итераций = " + wte.Iter;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == ">")
            {
                tabControl1.Visible = false;
                button3.Text = "<";
            }
            else if(button3.Text == "<")
            {
                tabControl1.Visible = true;
                button3.Text = ">";
            }
            //
            if (tabControl1.Visible)
                Dx = 385;
            else
                Dx = 50;
            Invalidate();
        }
   

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        
        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                w.time_b = Convert.ToDouble(txtDt.Text);
                int i = w.iter;
            }
            catch (Exception ex)
            { }
        }

        private void btnVisual2D_Click(object sender, EventArgs e)
        {
            if (wte != null)
            {
                double[] tau = wte.CalcTauEverywhere();
                //MainForm Main2D = new MainForm(wte.U, wte.V, wte.P, wte.S, wte.E, wte.K, wte.nuT, m, tau, wte.ReT, wte.rightE, wte.y_plus);
                MainForm Main2D = new MainForm(wte.U, wte.V, wte.P, wte.S, wte.W, wte.K, wte.nuT, m, tau, wte.ReT, wte.rightW, wte.rightK, wte.Continuity);
                //MainForm Main2D = new MainForm(wte.U, wte.V, wte.P, m);
                Main2D.Text = "Solution";
                Main2D.angle = angle;
                Main2D.Show();
            }
            if (ExactU != null)
            {
                double L2U = L2_discrepancy(ExactU, wte.U) * 100;
                double L2V = L2_discrepancy(ExactV, wte.V) * 100;
                double L2P = L2_discrepancy(ExactP, wte.P, true) * 100;
                //
                double[] C2U = С2_discrepancy(ExactU, wte.U, dx);
                double[] C2V = С2_discrepancy(ExactV, wte.V, dx);
                double[] C2P = С2_discrepancy(ExactP, wte.P, dx, true);
                double[] C2E, C2K, C2S;
                //
                if (ExactE != null)
                {
                    C2E = С2_discrepancy(ExactE, wte.E, dx);
                    C2K = С2_discrepancy(ExactK, wte.K, dx);
                    C2S = С2_discrepancy(ExactS, wte.S, dx);
                }
                //
                MainForm Main2D = new MainForm(C2U, C2V, C2P, m);
                Main2D.Text = "C2 error %";
                Main2D.Show();
                //
                double dy = wte.Mesh.Y[2] - wte.Mesh.Y[1];
                L2 = 2.5;
                H2 = 0.5 * H;
                int Nx2 = (int)(L2 / dx) + 1;
                int Ny2 = (int)(H2 / dy) + 1;
                FaceGraph FaceErr = new FaceGraph(C2U, C2V, C2P, m.CountTop, m.CountRight, dx, dy);
                FaceErr.Nx2 = Nx2;
                FaceErr.Ny2 = Ny2;
                string[] L2str = new string[3];
                L2str[0] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", L2U);
                L2str[1] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", L2V);
                L2str[2] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", L2P);
                FaceErr.Text = "C2 error, L2U = " + L2str[0] + "%, L2V = " + L2str[1] + "%, L2P = " + L2str[2] + "%.";
                FaceErr.Show();
            }
        }
        /// <summary>
        /// расчет С2 погрешности в процентах
        /// </summary>
        /// <param name="Exact">Точное решение</param>
        /// <param name="Calc">Расчетное решение</param>
        /// <param name="dx">Шаг по оси X</param>
        /// <returns>Поле погрешности в процентах %</returns>
        double[] С2_discrepancy(double[] Exact, double[] Calc, double dx, bool Pflag = false)
        {
            int N = Exact.Length;
            if (Exact.Length > Calc.Length)
                N = Calc.Length;
            double[] C2 = new double[N];
            //
            double diff = 0;
            if (Pflag)
                diff = Exact[10] - Calc[10];
            double maxExact = 0, minExact = 10000000;
            double val=0;
            for (int i = 0; i < N; i++)
            {
                val = Math.Abs(Exact[i]);
                if (maxExact < val)
                    maxExact = val;
                if (minExact > val)
                    minExact = val;
            }
            //
            val = Math.Abs(maxExact - minExact);
            if (val == 0) 
                val = maxExact;
            for (int i = 0; i < N; i++)
                C2[i] = Math.Abs(Calc[i] + diff - Exact[i]) / maxExact * 100;  
            //
            return C2;
        }
        /// <summary>
        /// Вычисление L2 погрешности в совпадающих по узлам и элементам сетках
        /// </summary>
        /// <param name="Exact">Точные значения</param>
        /// <param name="Calc">Расчетные значения</param>
        /// <param name="Press">Сравнивается ли давление (для вычисления сдвига по Y)</param>
        /// <returns></returns>
        double L2_discrepancy(double[] Exact, double[] Calc, bool Press=false)
        {
            int[][] AreaElems = ExactAreaElems;
            double[] Sk = ExactSk;
            int CountEl = ExactCountElems;
            if (Exact.Length > Calc.Length)
            {
                AreaElems = m.AreaElems;
                Sk = m.Sk;
                CountEl = m.CountElems;
            }
            double diff = 0;
            if (Press)
                diff = Exact[m.CountLeft] - Calc[m.CountLeft];
            //
            double sum = 0, tmpSum = 0, Esum = 0;
            double E1, E2, E3, C1, C2, C3, CE1, CE2, CE3, CE123, E123;
            int[] Knots = new int[3];
            //
            // сумма по элементам int[(Calc-Exact)^2]dS
            //-----------------------------------------
            // сумма по элементам int[(Exact)^2]dS
            for (int i = 0; i < CountEl; i++)
            {
                Knots = AreaElems[i];
                E1 = Exact[Knots[0]]; E2 = Exact[Knots[1]]; E3 = Exact[Knots[2]];
                C1 = Calc[Knots[0]]+diff; C2 = Calc[Knots[1]]+diff; C3 = Calc[Knots[2]]+diff;
                CE1 = C1 - E1; CE2 = C2 - E2; CE3 = C3 - E3;
                CE123 = CE1 + CE2 + CE3;
                tmpSum = Sk[i] / 12.0 * (CE1 * (CE123 + CE1) + CE2 * (CE123 + CE2) + CE3 * (CE123 + CE3));
                sum += tmpSum;
                E123 = E1 + E2 + E3;
                tmpSum = Sk[i] / 12.0 * (E1 * (E123 + E1) + E2 * (E123 + E2) + E3 * (E123 + E3));
                Esum += tmpSum;
            }
            // находим корень от значения
            double L2_Err = Math.Sqrt(sum / Esum);
            return L2_Err;
        }
        // --- дописать метод сравнения с эксп данными и вычисления погрешности
        public double[][] CalcDiscreapancy(double[] X, double[] Y, double[] u, double[] v, double[] k, double[] tau, double[] ex, double[] ey, double[] eu, double[] ev, double[] ek, double[] etau)
        {
            // -----разбор эскперименальной сетки (1 раз можно сделать)
            double[] Xte = new double[ex.Length]; double[][] Yte;
            double tmpX = 10000000000000;
            int chX = 0; // количество разбиений по x (столбиков)
            // выделяем X координаты
            for (int i = 0; i < ex.Length; i++)
            {
                if (ex[i]!=tmpX)
                {
                    tmpX = ex[i];
                    Xte[chX++] = tmpX;
                }
            }
            double[] Xe = new double[chX];
            for (int i = 0; i < chX; i++)
                Xe[i] = Xte[i];
            Xte = null;
            //выделяем y и номера узлов, чтобы потом определять необходимю функцию 
            Yte = new double[chX][];
            int[][] Numte = new int[chX][];
            int[] chY = new int[chX];
            for (int i = 0; i < chX; i++)
            {
                Yte[i] = new double[100];
                Numte[i] = new int[100];
                for (int j = 0; j < ex.Length; j++)
                {
                    if (Xe[i] == ex[j])
                    {
                        Numte[i][chY[i]] = j;
                        Yte[i][chY[i]++] = ey[j];
                    }
                    else
                        if (chY[i] != 0)
                            break;
                }
            }
            // убираем нулевые элементы в массивах
            double[][] Ye = new double[chX][];
            int[][] Nume = new int[chX][];
            for (int i = 0; i < chX; i++)
            {
                Ye[i] = new double[chY[i]];
                Nume[i] = new int[chY[i]];
                for (int j = 0; j < chY[i]; j++)
                {
                    Ye[i][j] = Yte[i][j];
                    Nume[i][j] = Numte[i][j];
                }
            }
            Numte = null;
            Yte = null;
            //----- разбор расчетной сетки, выделяем те узлы, которые попадают в поле экспериментальных измерений
            double xstart = ex[0]; double xstop = ex[ex.Length - 1];
            double ystart = 0; double ystop = 0;
            int CountsKnots = X.Length; 
            int [] KnotsNumrt = new int[CountsKnots]; // [0] - глобальные номера узлов расчетной сетки
            double[] Xrt = new double[CountsKnots]; // х-координаты точек, попадающих в поле эксп. измерений
            int[] Nrt = new int[CountsKnots]; // количество узлов в столбце
            int chN = 0;  // количество попадающих расчетных узлов
            int n2; int chxr = 0;
            for (int i = 0; i < CountsKnots; i++)
            {
                if ((X[i] >= xstart) && (X[i] <= xstop))
                {
                    for (n2 = 0; n2 < chX-1; n2++)
                    {
                        if ((X[i] >= Xe[n2]) && (X[i] <= Xe[n2+1]))
                        {
                            ystart = Math.Max(Ye[n2][0],Ye[n2+1][0]);
                            ystop = Ye[n2][Ye[n2].Length - 1];
                            break;
                        }
                    }
                    if ((Y[i] >= ystart) && (Y[i] <= ystop))
                    {
                        KnotsNumrt[chN] = i;
                        chN++;
                        //
                        if (Xrt[chxr] != X[i])
                            Xrt[++chxr] = X[i];
                        else
                            Nrt[chxr]++;
                    }
                }
            }
            // убираем нули в массивах
            int[] KnotNumr = new int[chN];
            for (int i = 0; i < chN; i++)
                KnotNumr[i] = KnotsNumrt[i];
            double[] Xr = new double[chxr];
            int[] Nr = new int[chxr];
            for (int i = 1; i < chxr+1; i++)
            {
                Xr[i - 1] = Xrt[i];
                Nr[i - 1] = Nrt[i]+1;
            }
            Nrt = null; Xrt = null; KnotsNumrt = null;
            // --- интерполяция
            // проходим по сетке расчетных узлов (столбиками) и интерполируем из ближайших столбиков эксперименальных данных значения 
            // (учитываем, что эксп. столбцы по Y могут начинаться с разных высот, дно волнистое, надо, чтобы начинались с одной)
            // помещаем расчетные узлы как бы внутрь прямоугольника экспериментальных данных
            double[][] C2 = new double[chxr][]; // массив С2 погрешности
            double eu_max = eu.Max() - eu.Min(); 
            alglib.spline2dinterpolant interp = new alglib.spline2dinterpolant();
            //
            int num = 0; int tmp1=0;
            for (int i = 0; i < chxr; i++)
            {
                tmp1 = 0;
                // определяем столбик расчетных узлов
                double Xp = Xr[i]; // х коррдианата столбика
                double[] Yp = new double[Nr[i]];  // у-координаты столбика
                int[] Np = new int[Nr[i]]; // глобальные номера узлов столбика
                double[] Up = new double[Nr[i]]; // скорость в столбике
                for (int j = 0; j < chN; j++)
                {
                    num = KnotNumr[j];
                    if (X[num] == Xp)
                    {
                        Yp[tmp1] = Y[num];
                        Up[tmp1] = u[num];
                        Np[tmp1++] = num;
                    }
                }
                // находим экспериментальные столбики слева и справа
                double[] Xpe = new double[2]; // х-коорднаты столбиков
                double[] Ype = null; // y-координаты столбиков
                double[,] Upe = null; // значения скорости в столбиках
                for (int j = 0; j < chX-1; j++)
                {
                    if ((Xp>=Xe[j])&&(Xp<=Xe[j+1]))
                    {
                        Xpe[0] = Xe[j];
                        Xpe[1] = Xe[j + 1];
                        // если соседние столбцы одинаковой высоты
                        if (Ye[j].Length == Ye[j + 1].Length)
                        {
                            Ype = Ye[j];
                            Upe = new double[Ye[j].Length, 2];
                            for (int l = 0; l < Ye[j].Length; l++)
                            {
                                Upe[l, 0] = eu[Nume[j][l]];
                                Upe[l, 1] = eu[Nume[j + 1][l]];
                            }
                            break;
                        }
                            // если один начинается выше
                        else
                        {
                            if (Ye[j].Length > Ye[j + 1].Length)
                            {
                                Ype = Ye[j + 1];
                                Upe = new double[Ye[j + 1].Length, 2];
                                for (int l = 0; l < Ye[j + 1].Length; l++)
                                {
                                    Upe[l, 0] = eu[Nume[j][l + 1]];
                                    Upe[l, 1] = eu[Nume[j + 1][l]];
                                }
                                break;
                            }
                            else
                            {
                                Ype = Ye[j];
                                Upe = new double[Ye[j].Length, 2];
                                for (int l = 0; l < Ye[j].Length; l++)
                                {
                                    Upe[l, 0] = eu[Nume[j][l]];
                                    Upe[l, 1] = eu[Nume[j + 1][l + 1]];
                                }
                                break;
                            }
                        }
                    }
                }
                // сама интерполяция
                alglib.spline2dbuildbilinear(Xpe, Ype, Upe, Ype.Length, 2, out interp);
                double[] Upe_calc = new double[Nr[i]];
                // находим интерполяцию экспериментальной функции в узлах расчетной сетки (столбик)
                for (int j = 0; j < Nr[i]; j++)
                    Upe_calc[j] = alglib.spline2dcalc(interp, Xp, Yp[j]);
                //
                // --- выполняем манипуляции по вычислению относительной погрешности (или сохраняем в глобальные массивы)
                C2[i] = new double[Nr[i]];
                for (int j = 0; j < Nr[i]; j++)
                    C2[i][j] = Math.Abs(Up[j] - Upe_calc[j]) / eu_max * 100;
            }
            return C2;

        }

        void calcTauTotal()
        {
            double xstart = (N1 + 1) * StabL + 3 * 0.9;
            double ddx = m.X[m.CountLeft] - m.X[0];
            int xb = (int)(xstart / ddx);
            int xe = (int)((xstart + 0.9) / ddx)+1;
            int Nb = xb*m.CountLeft;
            int Ne = xe*m.CountLeft;
            double[] Taua = wte.CalcTauEverywhere();
            double[] TauT = new double[m.CountLeft];
            double[] Taus = new double[m.CountLeft];
            double[] Counts = new double[m.CountLeft];
            double[] ys = new double[m.CountLeft];
            for (int i = 0; i < ys.Length; i++)
                ys[i] = m.Y[Nb + i];
            //
            int countrows = xe - xb;
            int cknot = 0; double yc = 0;
            for (int k = 0; k < m.CountLeft; k++)
            {
                yc = ys[k];
                for (int i = 0; i<countrows; i++)
                {
                    for (int j = 0; j < m.CountLeft-1; j++)
                    {
                        cknot = Nb + i * m.CountLeft + j;
                        //
                        if ((yc >= m.Y[cknot]) && (yc <= m.Y[cknot + 1]))
                        {
                            Taus[k] += Taua[cknot] + (yc - m.Y[cknot]) * (Taua[cknot+1] - Taua[cknot]) / (m.Y[cknot+1] - m.Y[cknot]);
                            Counts[k]++;
                        }
                        else if (yc < m.Y[cknot])
                            break; 
                    }
                }
                TauT[k] = Taus[k] / Counts[k];

            }
            // расчет Cd по Mendoza 1990 без коэффициента 8
            // 
            double Fp = 0, Fs = 0, ll = 0, zeta12p = 0, zeta12m = 0, ldx=0;
            int rknot = 0, cknotref=0;
            int CountLeft = m.CountLeft;
            int cknotp=0, rknotp=0;
            double rhoU2 = 1000 * 0.62 * 0.62,
                n1 = 0, t1 = 0;
            // Fp = 1/lambda int_0_lambda (p-p_ref) dx
            for (int i = 0; i < countrows; i++)
            {
                cknot = Nb + i * CountLeft;// узлы в 2D сетке
                cknotp = Nb + (i + 1) * CountLeft;
                cknotref = cknot + CountLeft - 1; // значение в том же столбце, но вверзу канала
                rknot = cknot / CountLeft;// узлы в 1D сетке дна
                rknotp = cknotp / CountLeft;
                ldx = Math.Sqrt(ddx * ddx + (rt.Zeta[rknot] - rt.Zeta[rknotp]) * (rt.Zeta[rknot] - rt.Zeta[rknotp]));
                n1 = (rt.Zeta[rknot] - rt.Zeta[rknotp]) / ldx;
                t1 = ddx / ldx;
                Fp -= ddx / 6.0 * ((wte.P[cknot] - wte.P[cknotref]) + 4.0 * (wte.P[cknot] - wte.P[cknotref] + wte.P[cknotp] - wte.P[cknotref + CountLeft]) / 2.0 + (wte.P[cknotp] - wte.P[cknotref + CountLeft])) * n1;
                Fs += ddx / 6.0 * (wte.BTauC[rknot] + 4 * wte.BTau[rknot] + wte.BTauC[rknotp]) * t1;
                ll += ddx;
            }
            Fp /= ll;
            Fs /= ll;
            //
            double tau_s = Fs; // напряжение от трения частиц = 1/L int t_w n_y dS
            double tau_p = Fp; // напряжение от формы дюны = 1/L int (p-p_0) n_x dS
            double tau_t = Fs + Fp; // общее напряжение
            double cp = Fp / rhoU2; // коэффициент сопротивления от формы дюны = 1/ (L rho U^2) int (p-p_0) n_x dS
            double cs = Fs / rhoU2;// коэффициент сопротивления от трения частиц = 1/ (L rho U^2) int t_w n_y dS
            double ct = (Fp + Fs) / rhoU2; // общий коэффицент сопротивления
            double cD = ct * 2 * 0.9 / 0.03; // другой общий коэффициент сопротивления (McLean) = (Fs+Fp)*L/(1/2 rho U^2 h) или = ct*2*L/h
            //
            StreamWriter sw = new StreamWriter("tau_total_calc.txt");
            sw.WriteLine("tau_t_[Pa] y_[m]");
            for (int i = 0; i < m.CountLeft; i++)
                sw.WriteLine(TauT[i].ToString() + " " + ys[i].ToString());
            sw.WriteLine("cp = " + cp.ToString() + ";");
            sw.WriteLine("cs(cf) = " + cs.ToString() + ";");
            sw.WriteLine("ct = " + ct.ToString() + ";");
            sw.WriteLine("cD = " + cD.ToString() + ";");
            sw.Close();
            // расчет неинтегральных величин
            double[] Cp = new double[m.CountBottom];
            double[] Cs = new double[m.CountBottom];
            double[] Cd = new double[m.CountBottom];
            for (int j = 0; j < Nx; j++)
            {
                cknot = j * CountLeft;// узлы в 2D сетке
                cknotref = cknot + CountLeft - 1; // значение в том же столбце, но вверзу канала
                rknot = j;// узлы в 1D сетке дна
                Cp[j] = (wte.P[cknot] - wte.P[cknotref]) / rhoU2;
                Cs[j] = (wte.BTauC[j]) / rhoU2;
                Cd[j] = Cp[j] + Cs[j];
            }
            ChartForm ChartF = new ChartForm();
            //
            double[][] Data;
            int CountLines = 4;
            //
            Data = new double[CountLines][];
            string[] Names = new string[CountLines];
            //
            Data[0] = new double[Nx];
            for (int j = 0; j < Nx; j++)
            {
                Data[0][j] = rt.Zeta[j];
            }

            Data[1] = new double[Nx];
            Data[2] = new double[Nx];
            Data[3] = new double[Nx];
            for (int j = 0; j < Nx; j++)
            {
                Data[1][j] = Cp[j];
                Data[2][j] = Cs[j];
                Data[3][j] = Cd[j];
            }
            ChartF.DataY = Data;
            ChartF.DataX = Xt;
            ChartF.Names = new string[]{"Zeta", "Cp", "Cs", "Ct"};
            ChartF.Text = "taup=" + String.Format("{0:f4}", tau_p) + "; taus=" + String.Format("{0:f4}", tau_s) + "; taut=" + String.Format("{0:f4}", tau_t) + "; cp=" + String.Format("{0:f6}", cp) + "; cs=" + String.Format("{0:f6}", cs) + "; ct=" + String.Format("{0:f6}", ct) + "; cD = " + String.Format("{0:f6}", cD) + ";";
            ChartF.Show();
        }

        private void btnVisual1D_Click(object sender, EventArgs e)
        {
            ChartForm ChartF = new ChartForm();
            //
            double[][] Data;
            int CountLines = 21;
            //
            Data = new double[CountLines][];
            string[] Names = new string[CountLines];
            int ch = 0;
            //
            for (int i = 0; i < 4; i++)
            {
                Data[ch] = new double[Nx];
                for (int j = 0; j < Nx; j++)
                {
                    Data[ch][j] = BuffZeta[i][j];
                }
                Names[ch] = "Zeta " + ch.ToString();
                //Names[ch] = "Дно, " + (TimeFaces[i] * dt / 3600).ToString().Substring(0, 4) + " ч.";
                ch++;

            }
            //
            for (int i = 0; i < 4; i++)
            {
                Data[ch] = new double[Nx];
                for (int j = 0; j < Nx; j++)
                {
                    Data[ch][j] = BuffQb[i][j];
                }
                Names[ch] = "Q " + ch.ToString();
                ch++;

            }
            //   отрисовка пространственных срезов
            int pp = 0;
            for (int i = 0; i < 4; i++)
            {
                Data[ch] = new double[Nx];
                //
                double[] ttau = wte.CalcTauEverywhere();
                for (int j = 0; j < Nx - 1; j++)
                {
                    Data[ch][j] = ttau[j * m.CountLeft + pp + i];
                }
                //
                Data[ch][Nx - 1] = BuffTau[i][Nx - 2];
                Names[ch] = "Tau " + ch.ToString();
                ch++;
            }
            // открисовка временных срезов
            //for (int i = 0; i < 4; i++)
            //{
            //    Data[ch] = new double[Nx];
            //    //
            //    for (int j = 0; j < Nx - 1; j++)
            //    {
            //        Data[ch][j] = BuffTau[i][j];
            //    }
            //    //
            //    Data[ch][Nx - 1] = BuffTau[i][Nx - 2];
            //    Names[ch] = "Tau " + ch.ToString();
            //    ch++;

            //}
            if (BuffTau[0].Max() == 0)
            {
                BuffTau[0] = wte.BTau;
            }
            //
            for (int i = 0; i < 4; i++)
            {
                Data[ch] = new double[Nx];
                for (int j = 0; j < Nx - 1; j++)
                {
                    Data[ch][j] = BuffQs[i][j];
                }
                Data[ch][Nx - 1] = BuffQs[i][Nx - 2];
                Names[ch] = "Qs " + ch.ToString();
                ch++;

            }
            //
            for (int i = 0; i < 4; i++)
            {
                Data[ch] = new double[Nx];
                Data[ch] = wte.GetKBed(pp+i);
                Names[ch] = "kBed " + ch.ToString();
                ch++;

            }
            // вывод Tau_z
            Data[ch] = new double[Nx];
            if (rt.TauC12 != null)
            {
                for (int j = 0; j < Nx - 1; j++)
                {
                    Data[ch][j] = rt.TauC12[j];
                }
                Data[ch][Nx - 1] = rt.TauC12[Nx - 2];
            }
            Names[ch] = "TauC " + ch.ToString();
            ch++;
            //
            double[] xBedExp = null, kBedExp = null, tauBedExp = null;
            //
            if ((kExp==null)||(kExp.x_10==null))
                kExp = new KwollExp(new string[] { "D10.txt", "D20.txt", "D30.txt", "Tuv_av.txt" });
            //
            kg = new KwollGeometry(Nx, 1.5, 1, N1);
            if (angle != 0)
            {
                if (angle == 10)
                {
                    kExp.BedData10(out xBedExp, out tauBedExp, out kBedExp);
                    xBedExp = kExp.SlipBedX(kg.ZeroPoint10 + 0.9 * Math.Round(DuneCount / 2.0, MidpointRounding.ToEven), xBedExp);
                }
                if (angle == 20)
                {
                    kExp.BedData20(out xBedExp, out tauBedExp, out kBedExp);
                    xBedExp = kExp.SlipBedX(kg.ZeroPoint20 + 0.9 * Math.Round(DuneCount / 2.0, MidpointRounding.ToEven), xBedExp);
                }
                if (angle == 30)
                {
                    kExp.BedData30(out xBedExp, out tauBedExp, out kBedExp);
                    xBedExp = kExp.SlipBedX(kg.ZeroPoint30 + 0.9 * Math.Round(DuneCount / 2.0, MidpointRounding.ToEven), xBedExp);
                }
                // по экспериментальным Х-координатам определяют соответствующеие расчетные Y координаты (линейно), в которых и будет линейно аппроксимироваться значение экспериментальной функции
                // yb - это координаты на уровне первого пристеночного узла
                //  ..Y[i]
                //  ..       yb[ch]
                //  ..                Y[i+1]
                //  ..X[i]---xExp-----X[i+1]
                double[][] yb = new double[3][];
                for (int i = 0; i < 3; i++)
                    yb[i] = new double[xBedExp.Length];
                //
                int p = pp-1; // начальная точка, с которой смотрятся срезы tau вверх, 0 - дно
                int chh = 0; double xexp = 0;
                for (int k = 0; k < xBedExp.Length; k++)
                {
                    xexp = xBedExp[k];
                    for (int i = 0; i < m.CountBottom; i++)
                    {
                        if ((xexp >= m.X[m.CountLeft * i]) && (xexp <= m.X[m.CountLeft * (i + 1)]))
                        {
                            yb[0][chh] = m.Y[m.CountLeft * i + 1 + p] + (xexp - m.X[m.CountLeft * i + 1 + p]) * (m.Y[m.CountLeft * (i + 1) + 1 + p] - m.Y[m.CountLeft * i + 1 + p]) / (m.X[m.CountLeft * (i + 1) + 1 + p] - m.X[m.CountLeft * i + 1 + p]);
                            yb[1][chh] = m.Y[m.CountLeft * i + 2 + p] + (xexp - m.X[m.CountLeft * i + 2 + p]) * (m.Y[m.CountLeft * (i + 1) + 2 + p] - m.Y[m.CountLeft * i + 2 + p]) / (m.X[m.CountLeft * (i + 1) + 2 + p] - m.X[m.CountLeft * i + 2 + p]);
                            yb[2][chh] = m.Y[m.CountLeft * i + 3 + p] + (xexp - m.X[m.CountLeft * i + 3 + p]) * (m.Y[m.CountLeft * (i + 1) + 3 + p] - m.Y[m.CountLeft * i + 3 + p]) / (m.X[m.CountLeft * (i + 1) + 3 + p] - m.X[m.CountLeft * i + 3 + p]);
                            chh++;
                            break;
                        }
                    }
                }
                //
                double[][] tauBedExp3 = new double[3][];
                double[][] kBedExp3 = new double[3][];
                for (int i = 0; i < 3; i++)
                {
                    tauBedExp3[i] = new double[xBedExp.Length];
                    kBedExp3[i] = new double[xBedExp.Length];
                }
                    if (angle == 10)
                    {
                        kExp.BedDataHeightCorrection10(yb[0], out tauBedExp, out kBedExp);
                        tauBedExp.CopyTo(tauBedExp3[0], 0);
                        kBedExp.CopyTo(kBedExp3[0], 0);
                        kExp.BedDataHeightCorrection10(yb[1], out tauBedExp, out kBedExp);
                        tauBedExp.CopyTo(tauBedExp3[1], 0);
                        kBedExp.CopyTo(kBedExp3[1], 0);
                        kExp.BedDataHeightCorrection10(yb[2], out tauBedExp, out kBedExp);
                        tauBedExp.CopyTo(tauBedExp3[2], 0);
                        kBedExp.CopyTo(kBedExp3[2], 0);
                    }
                if (angle == 20)
                {
                    kExp.BedDataHeightCorrection20(yb[0], out tauBedExp, out kBedExp);
                    tauBedExp.CopyTo(tauBedExp3[0], 0);
                    kBedExp.CopyTo(kBedExp3[0], 0);
                    kExp.BedDataHeightCorrection20(yb[1], out tauBedExp, out kBedExp);
                    tauBedExp.CopyTo(tauBedExp3[1], 0);
                    kBedExp.CopyTo(kBedExp3[1], 0);
                    kExp.BedDataHeightCorrection20(yb[2], out tauBedExp, out kBedExp);
                    tauBedExp.CopyTo(tauBedExp3[2], 0);
                    kBedExp.CopyTo(kBedExp3[2], 0);
                }
                if (angle == 30)
                {
                    kExp.BedDataHeightCorrection30(yb[0], out tauBedExp, out kBedExp);
                    tauBedExp.CopyTo(tauBedExp3[0], 0);
                    kBedExp.CopyTo(kBedExp3[0], 0);
                    kExp.BedDataHeightCorrection30(yb[1], out tauBedExp, out kBedExp);
                    tauBedExp.CopyTo(tauBedExp3[1], 0);
                    kBedExp.CopyTo(kBedExp3[1], 0);
                    kExp.BedDataHeightCorrection30(yb[2], out tauBedExp, out kBedExp);
                    tauBedExp.CopyTo(tauBedExp3[2], 0);
                    kBedExp.CopyTo(kBedExp3[2], 0);
                }

                //
                //
                //ChartF.ExpDataX = new double[][] { xBedExp, xBedExp };
                //ChartF.ExpDataY = new double[][] { tauBedExp, kBedExp };
                //ChartF.NamesExp = new string[] { "TauExp", "kExp" };
                // без отображения k
                ChartF.ExpDataX = new double[][] { xBedExp, xBedExp, xBedExp };
                ChartF.ExpDataY = tauBedExp3;
                ChartF.NamesExp = new string[] { "TauExp+1", "TauExp+2", "TauExp+3" };
                //
                //ChartF.ExpDataX = new double[][] { xBedExp, xBedExp, xBedExp };
                //ChartF.ExpDataY = kBedExp3;
                //ChartF.NamesExp = new string[] { "kExp+1", "kExp+2", "kExp+3" };
                // без отображения tau
                //ChartF.ExpDataX = new double[][] {  xBedExp };
                //ChartF.ExpDataY = new double[][] {  kBedExp };
                //ChartF.NamesExp = new string[] { "kExp" }; 
                //
            }
            ChartF.DataY = Data; 
            double[] xt = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                xt[i] = Xt[i] * 10;
            }
            //
            ChartF.DataX = xt;
            ChartF.Names = Names;
            ChartF.Show();
            //
            // отрисовка точных напряжений
            //ChartForm chTest = new ChartForm();
            //double[] ExpandedTau = new double[Nx * 2];
            //double[] ExpandedX = new double[Nx * 2];
            ////
            //for (int i = 0; i < Nx - 1; i++)
            //{
            //    ExpandedTau[2 * i] = wte.BTauC[i];
            //    ExpandedX[2 * i] = Xt[i];
            //    //
            //    ExpandedTau[2 * i + 1] = wte.BTau[i];
            //    ExpandedX[2 * i + 1] = Xt[i] + dx / 2.0;
            //}
            ////
            //chTest.DataY = new double[][] { ExpandedTau, wte.BTauC, wte.BTau };
            //chTest.DataXMany = new double[][] { ExpandedX, Xt, Xt };
            //chTest.Names = new string[] { "ExpandedTau", "TauC", "Tau1/2" };
            //chTest.Show();
        }









    }
    public class FPoint
    {
        public float X;
        public float Y;
        public FPoint(float x, float y)
        {
            X = x; Y = y;
        }
    }
}
