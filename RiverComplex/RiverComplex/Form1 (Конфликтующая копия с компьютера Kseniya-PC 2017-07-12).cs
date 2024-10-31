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

namespace RiverComplex
{
    public partial class Form1 : Form
    {
        TimeSpan time = new TimeSpan();
        Stopwatch stopW = new Stopwatch();
        public bool OpenCL = false;
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        //
        int Nx = 0;
        double dx = 0;
        //для сериализации
        int BeginIter = 0;
        int Dy = 200;
        int Dx = 385;
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
        double[] Xt = null;
        double[] Yt = null;
        double BottomLayer = 0, TopLayer = 0;
        Graphics g;
        //
        double[][] BuffZeta;
        double[][] BuffTau;
        double[][] BuffQb;
        int[] TimeFaces ;
        CheckBox OCl;
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
            w = new WElizParameter(800, 0.005, 0.005, 0.001, 0.0001, 0.0001, 0.0369, 1.0, 0.1, 0.0001, false);
            prpMainParameters.SelectedObject = w;
            lblUmax.Text = "Umax = " +  (3.0 / 2.0 * w.Q / H).ToString();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
        }

        void OCl_CheckedChanged(object sender, EventArgs e)
        {
            OpenCL = OCl.Checked;
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
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось Сериализовать решение");
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
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось десериализовать решение");
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
            saveTaskDialog.ShowDialog(this);
        }

        private void saveTaskDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                List<Object> AllTaskObject = new List<object>();
                AllTaskObject.Add(wte);
                AllTaskObject.Add(rt);
                AllTaskObject.Add(BuffTau);
                AllTaskObject.Add(BuffQb);
                AllTaskObject.Add(BuffZeta);
                //
                SaveParameters(AllTaskObject);
                //
                formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                FileStream fStream = new FileStream(saveTaskDialog.FileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(fStream, AllTaskObject);
                fStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить решение" + ex.Message);
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openTaskDialog.ShowDialog(this);
        }

        private void openTaskDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                List<Object> AllTasks = null;
                FileStream fStream = new FileStream(openTaskDialog.FileName, FileMode.Open, FileAccess.Read);
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
                ch = LoadParameters(ch, AllTasks);

                m = wte.Mesh;
                Nx = wte._Nx;
                dx = rt.dx;
                Xt = new double[m.CountBottom];
                for (int i = 0; i < m.CountBottom; i++)
                    Xt[i] = m.X[m.BottomKnots[i]];
                Rec = new RectangleF((float)m.X.Min(), (float)m.Y.Max(), (float)(m.X.Max() - m.X.Min()), (float)(m.Y.Max() - m.Y.Min()));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить решение" + ex.Message);
            }
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            stopW.Restart();
            button1.Enabled = false;
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
            //получение параметров расчетной области
            double[][] coords = areaControl1.GetInterpolatedCoords(Nx);
            double[] Z = coords[0];
            Yt = coords[1];
            Xt = coords[2];
            dx = areaControl1.dx;
            Area ar = new Area(Xt, Yt, Xt, Z, BottomLayer,TopLayer, parts);
            //
            mb = new MeshBuilder(ar, Prs);
            sys.OpenCL = this.OpenCL;
            alg.OpenCL = this.OpenCL;
            mb.OpecCL = this.OpenCL;
            mb.GenerateMesh();
            m = mb.FinalMesh;
            Rec = new RectangleF((float)m.X.Min(), (float)m.Y.Max(), (float)(m.X.Max()-m.X.Min()), (float)(m.Y.Max()-m.Y.Min()));
            //
            r = riverTaskControl1.ParameterObject;
            rt = riverTaskControl1.GetRiverTaskObject();
            RiverTaskLibrary.BoundaryCondition BC = new RiverTaskLibrary.BoundaryCondition(RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed, RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed, 0.0, 0.0);
            rt.ReStartBaseBedLoadTask(BC, r, dx, m.CountBottom, dt);
            rt.SetZeta0(ar.YBottom);
            //
           
            //
            w.time_b = dt;
            wte = new WaterTaskEliz(w, m, sys, alg);
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
                int ch = 0;
                double percentage = 100.0 / (GlobalIteration);
                wte.status = "Running";
                for (int i = BeginIter; i < GlobalIteration; i++)
                {
                    Area ar = new Area(Xt, Yt, Xt, rt.Zeta, BottomLayer, TopLayer, parts);
                    mb.ChangeArea(ar);
                    mb.GenerateMesh();
                    m = mb.FinalMesh;
                    wte.ChangeMesh(m);
                    //
                    wte.Run();
                    if (wte.SerializeNow)
                    {
                        MessageBox.Show("Расчет приостановлен", "Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        wte.status = "Stoped";
                        BeginIter = i;
                        return;
                    }
                    tauPr = 0;double a =0;
                    for (int k = 0; k < wte.BTau.Length; k++)
                    {
                        a = Math.Abs(wte.BTau[k]-wte.BTau[0])/wte.BTau[0];
                        if (a > tauPr)
                            tauPr = (float)a;
                    }
                        //rt.Solve(wte.BTau);
                    if ((wte.err != "ok") || (rt.Message != "ok"))
                    {
                        Exception exc = new Exception(wte.err + rt.Message + "на " + i.ToString() + " итерации");
                        //
                        for (int j = 0; j < Nx - 1; j++)
                        {
                            BuffZeta[ch][j] = rt.Zeta[j];
                            BuffTau[ch][j] = wte.BTau[j];
                            BuffQb[ch][j] = rt.Qb[j];
                        }
                        BuffZeta[ch][Nx - 1] = rt.Zeta[Nx - 1];
                        BuffQb[ch][Nx - 1] = BuffQb[ch][Nx - 2];
                        BuffTau[ch][Nx - 1] = BuffTau[ch][Nx - 2];
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
                            BuffQb[ch][j] = rt.Qb[j];
                        }
                        BuffZeta[ch][Nx - 1] = rt.Zeta[Nx - 1];
                        BuffQb[ch][Nx - 1] = BuffQb[ch][Nx - 2];
                        BuffTau[ch][Nx - 1] = BuffTau[ch][Nx - 2];
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
                MessageBox.Show("Расчет окончен", "Расчет задачи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //
            button1.Enabled = true;
            time = stopW.Elapsed;
            toolStripStatusLabel3.Text += ";  Время:" + time.TotalSeconds;
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = "Количество треугольников = " + wte.CountTriangles.ToString() + ", узлов=" + wte.CountKnots;
            toolStripStatusLabel2.Text = " Q_in=" + wte.Q_in + " Q_out=" + wte.Q_out + " dQ=" + Convert.ToString((wte.Q_in - wte.Q_out) / wte.Q_in * 100) + " %";
            toolStripStatusLabel3.Text = "Количество итераций = " + wte.Iter + " tau' = " + tauPr.ToString() + " dy=" + ((wte.Mesh.Y[1] - wte.Mesh.Y[0])*100).ToString().Substring(0,5)+" см.";
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
            BuffQb = new double[TimeFaces.Length][];
            for (int i = 0; i < TimeFaces.Length; i++)
            {
                BuffZeta[i] = new double[Nx];
                BuffTau[i] = new double[Nx];
                BuffQb[i] = new double[Nx];
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
            MainForm Main2D = new MainForm(wte.U, wte.V, wte.P, m);
            Main2D.Show();
        }

        private void btnVisual1D_Click(object sender, EventArgs e)
        {
            ChartForm ChartF = new ChartForm();
            //
            double[][] Data;
            int CountLines = 12;
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
            //    
            for (int i = 0; i < 4; i++)
            {
                Data[ch] = new double[Nx];
                for (int j = 0; j < Nx - 1; j++)
                {
                    Data[ch][j] = BuffTau[i][j];
                }
                Data[ch][Nx - 1] = BuffTau[i][Nx - 2];
                Names[ch] = "Tau " + ch.ToString();
                ch++;

            }
            ChartF.DataY = Data;
            ChartF.DataX = Xt;
            ChartF.Names = Names;
            ChartF.Show();
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
