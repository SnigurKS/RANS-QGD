using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydrodynamicLibrary
{
    public partial class TestForm : Form
    {
        ReverseShallow1D_Parameter p;
        ReverseShallowWater1D shWR;
        //
        ElizShallowWater1D shEliz;
        ElizShallow1D_Parameter pEliz;
        //
        Graphics g;
        RectangleF Rec;
        double[] Z0 = null;
        int WS, HS;
        int Nx = 50;
        double dx;
        double Time = 3 * 3600;
        double dt;
        double L = 1;
        int otstup=0;
        double preArea = 1.0;
        double[][] Z;
        double[][] Eta;
        double[][] Tau;
        double[][] U;
        double[][] H;
        double[][] Q;
        //
        int CountCurve;
        double[][] Data;
        string[] Names;
        List<double[][]> exp_data;
        public TestForm()
        {
            InitializeComponent();
            p = new ReverseShallow1D_Parameter();
            //p.Hout = 2;
            //p.Uout = 2.21;
           
            propertyGrid1.SelectedObject = p;
            g = this.CreateGraphics();
        }

        private void TestForm_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font(FontFamily.GenericSansSerif,10.0f);
            WS = this.Width - 300;
            HS = this.Height - 300;
            //
            try
            {
                //
                if (shWR != null)
                {
                    float LUpX = 0;
                    float LUpY = (float)shWR.Eta.Max();
                    float Height = (float)(shWR.Eta.Max() - Z0.Min());
                    Rec = new RectangleF(0, LUpX, (float)L, Height);
                    //
                    //g.DrawString("U", font, Brushes.Black, new PointF(20, 50));
                    //g.DrawString("H", font, Brushes.Black, new PointF(20, 100));
                    //g.DrawString("Z", font, Brushes.Black, new PointF(20, 150));
                    //g.DrawString("Eta", font, Brushes.Black, new PointF(20, 200));
                    //for (int i = 0; i < Nx; i++)
                    //{
                    //    g.DrawString(String.Format("{0:F5}",shWR.U[i]), font, Brushes.Black, new PointF(50 + i * 65, 50));
                    //    g.DrawString(String.Format("{0:F5}", shWR.H[i]), font, Brushes.Black, new PointF(50 + i * 65, 100));
                    //    g.DrawString(String.Format("{0:F5}", Z0[i]), font, Brushes.Black, new PointF(50 + i * 65, 150));
                    //    g.DrawString(String.Format("{0:F5}", shWR.Eta[i]), font, Brushes.Black, new PointF(50 + i * 65, 200));
                    //}
                    //
                    Pen pen = new Pen(Brushes.Brown);
                    Pen pen1 = new Pen(Brushes.Blue);
                    for (int k = 0; k < 4; k++)
                        for (int i = 0; i < Nx - 1; i++)
                        {
                            float x1 = (float)(i * dx);
                            float x2 = (float)((i + 1) * dx);
                            g.DrawLine(pen, Mashtabing(new PointF(x1, (float)Z[k][i])), Mashtabing(new PointF(x2, (float)Z[k][i + 1])));
                            g.DrawLine(pen1, Mashtabing(new PointF(x1, (float)Eta[k][i])), Mashtabing(new PointF(x2, (float)Eta[k][i + 1])));
                        }
                }
                if (shEliz != null)
                {
                    float LUpX = 0;
                    float LUpY = (float)shEliz.Eta.Max();
                    float Height = (float)(shEliz.Eta.Max() - Z0.Min());
                    Rec = new RectangleF(0, LUpX, (float)L, Height);
                    //
                    //g.DrawString("U", font, Brushes.Black, new PointF(20, 50));
                    //g.DrawString("H", font, Brushes.Black, new PointF(20, 100));
                    //g.DrawString("Z", font, Brushes.Black, new PointF(20, 150));
                    //g.DrawString("Eta", font, Brushes.Black, new PointF(20, 200));
                    //for (int i = 0; i < Nx; i++)
                    //{
                    //    g.DrawString(String.Format("{0:F5}",shWR.U[i]), font, Brushes.Black, new PointF(50 + i * 65, 50));
                    //    g.DrawString(String.Format("{0:F5}", shWR.H[i]), font, Brushes.Black, new PointF(50 + i * 65, 100));
                    //    g.DrawString(String.Format("{0:F5}", Z0[i]), font, Brushes.Black, new PointF(50 + i * 65, 150));
                    //    g.DrawString(String.Format("{0:F5}", shWR.Eta[i]), font, Brushes.Black, new PointF(50 + i * 65, 200));
                    //}
                    //
                    Pen pen = new Pen(Brushes.Brown);
                    Pen pen1 = new Pen(Brushes.Blue);
                    for (int k = 0; k < 4; k++)
                        for (int i = 0; i < Nx - 1; i++)
                        {
                            float x1 = (float)(i * dx);
                            float x2 = (float)((i + 1) * dx);
                            g.DrawLine(pen, Mashtabing(new PointF(x1, (float)Z[k][i])), Mashtabing(new PointF(x2, (float)Z[k][i + 1])));
                            g.DrawLine(pen1, Mashtabing(new PointF(x1, (float)Eta[k][i])), Mashtabing(new PointF(x2, (float)Eta[k][i + 1])));
                        }
                }

            }
            catch 
            { }
        }
        protected Point Mashtabing(PointF p)
        {
            float X = p.X;
            float Y = p.Y;
            int x = (int)((X - Rec.X) / Rec.Width * WS) + 10;
            int y = (int)((1 - (Y - Rec.Y) / Rec.Height) * HS) + 50;
            return new Point(x, y);
        }

        private void TestForm_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Time = 24 * 3600;
            //
            dt = 0.5;
            //L = 200;
            
            L = (11.2 + preArea);
            //Nx = 400*80;
            Nx = 200+50;
            dx = L / (Nx - 1);
            otstup = (int)(preArea / dx);
            double J = 0.00416;
            //массивы для хранения буфера
            Z = new double[4][];
            Eta = new double[4][];
            Tau = new double[4][];
            U = new double[4][];
            H = new double[4][];
            Q = new double[4][];
            //начальный уровень дна
            Z[0] = new double[Nx];
            Eta[0] = new double[Nx];
            U[0] = new double[Nx];
            H[0] = new double[Nx];
            Q[0] = new double[Nx];
            double[] Q1 = new double[Nx];
            double[] Q2 = new double[Nx];
            Z0 = new double[Nx];
            for (int i = otstup; i < Nx; i++)
            {
                //Z0[i] = (Nx - 1 - i) * dx * J;
                Z0[i] = (11.2) * J - J * dx * (i-otstup);
                Z[0][i] = Z0[i];
                
            }
            //синхронизация параметров
            //BedPhysicsParams P = taskControl1.ParameterObject;
            //P.otstup = this.otstup;
            //for (int i = otstup-1; i >=otstup/2; i--)
            //{
            //    Z0[i] = Z0[i + 1] - P.tf*dx;
            //    Z[0][i] = Z0[i];
            //}
            for (int i = otstup; i >= 0; i--)
            {
                // бетон - ровное дно
                //Z0[i] = Z0[i + 1];
                // бетон - под уклоном J
                Z0[i] = Z0[i + 1] + J * dx;
                Z[0][i] = Z0[i];
            }
            Z0[otstup - 1] = (Z0[otstup + 1] + Z0[otstup - 2]) / 2;
            for (int i = 0; i < Nx; i++)
            {
                Eta[0][i] = Z0[i] + p.Hout;
                H[0][i] = p.Hout;
                U[0][i] = p.Uout;
            }
            //
            //p.rho_w = P.rhoW;
            //p.g = P.g;
            shWR = new ReverseShallowWater1D(p, dx, Nx, J);
            //получение объекта задачи дна
            //BaseBedLoadTask BTask = taskControl1.GetRiverTaskObject();
            //// установка граничных условий
            //RiverTaskLibrary.BoundaryCondition BCondition = new RiverTaskLibrary.BoundaryCondition();
            ////BCondition.Inlet = BedLoadLibrary.TypeBoundaryCondition.Neumann_boundary_conditions;
            //BCondition.Inlet = RiverTaskLibrary.TypeBoundaryCondition.Dirichlet_boundary_conditions;
            //BCondition.InletValue = Z0[0];
            //BCondition.Outlet = RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed;// Neumann_boundary_conditions;
            ////установка ГУ в решение
            //BTask.ReStartBaseBedLoadTask(BCondition, P, dx, Nx, dt);
            //// финт для трансзитного расхода на выходе (вычисляется по параметрам, которые вводятся вместе с ГУ)
            ////BCondition.OutletValue = BTask.CalculateRate(shWR.TaskSolver(Z0)[Nx - 2], J);
            //// установка начального дна в решение
            //BTask.SetZeta0(Z0);
            //for (int i = 0; i < Nx; i++)
            //{
            //    Q[0][i] = BCondition.OutletValue;
            //}
            ////
            int ch=1;
            Int64 Nt = Convert.ToInt64(Time / dt);
            double[] stress;
            //Tau[0] = new double[Nx];
            //расчет
            for (int i = 0; i < Nt; i++)
            {
                //гидродинамика, получаем напряжения
                stress = shWR.TaskSolver(Z0, dt);
                // дно, получем новый уровень дна
                //Z0 = BTask.Solve(stress, shWR.Eta, Q1, Q2);
                // буферизация каждый час
                //if (((i * dt) % 3600 == 0)||(i==Nt-1))
                if (((i * dt) / 3600 == 1) || ((i * dt) / 3600 == 4) || (i == Nt-1))
                {
                    Z[ch] = new double[Nx];
                    Eta[ch] = new double[Nx];
                    Tau[ch] = new double[Nx-1];
                    U[ch] = new double[Nx];
                    H[ch] = new double[Nx];
                    Q[ch] = new double[Nx-1];
                    for (int j = 0; j < Nx; j++)
                    {
                        Z[ch][j] = Z0[j];
                        Eta[ch][j] = shWR.Eta[j];
                        U[ch][j] = shWR.U[j];
                        H[ch][j] = shWR.H[j];
                        
                    }
                    for (int j = 0; j < Nx - 1; j++)
                    {
                        Tau[ch][j] = stress[j];
                        //Q[ch][j] = BTask.Qb[j];
                    }
                    ch++;
                }
            }
            //
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                

                DataPrepearingForGraphic(out CountCurve, out Data, out Names, out exp_data);
                //
                ShowGraphic.FShow FShow = new ShowGraphic.FShow();
                FShow.Data = Data;
                FShow.Exp_Data = exp_data;
                FShow.CountCurve = CountCurve;
                FShow.CountLine = Nx;
                FShow.GraphName = "Размыв 24 часа";
                FShow.WLengthX =L;
                FShow.SetcListBoxFiltr(Names);
                FShow.Show();
            }
            catch 
            { 
            }
        }

        private void DataPrepearingForGraphic(out int CountCurve, out double[][] Data, out string[] Names, out List<double[][]> exp_data)
        {
            #region Данные для отрисовки
            bool flagTau = ch_Tau.Checked;
            bool flagEta = chEta.Checked;
            bool flagH = chH.Checked;
            bool flagU = chU.Checked;
            bool flagQ = chQ.Checked;
            CountCurve = 4;
            if (flagTau)
                CountCurve += 4;
            if (flagEta)
                CountCurve += 4;
            if (flagH)
                CountCurve += 4;
            if (flagU)
                CountCurve += 4;
            if (flagQ)
                CountCurve += 4;
            //
            
            Data = new double[CountCurve][];
            Names = new string[CountCurve];
            try
            {
                int ch = 0;
                for (int k = 0; k < 4; k++)
                {
                    Data[ch] = new double[Nx - otstup];
                    for (int i = otstup; i < Nx; i++)
                        Data[ch][i-otstup] = Z[k][i];
                    Names[ch] = "Zeta " + ch.ToString();
                    ch++;
                }
                //
                if (flagTau)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx - otstup - 1];
                        for (int i = otstup; i < Nx - 1; i++)
                            Data[ch][i-otstup] = Tau[k][i];
                        Names[ch] = "Tau " + ch.ToString();
                        ch++;
                    }
                }
                if (flagEta)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx - otstup];
                        for (int i = otstup; i < Nx; i++)
                            Data[ch][i - otstup] = Eta[k][i];
                        Names[ch] = "Eta " + ch.ToString();
                        ch++;
                    }
                }
                //
                if (flagU)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx - otstup];
                        for (int i = otstup; i < Nx; i++)
                            Data[ch][i - otstup] = U[k][i];
                        Names[ch] = "U " + ch.ToString();
                        ch++;
                    }
                }
                //
                if (flagH)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx - otstup];
                        for (int i = otstup; i < Nx; i++)
                            Data[ch][i - otstup] = H[k][i];
                        Names[ch] = "H " + ch.ToString();
                        ch++;
                    }
                }
                //
                if (flagQ)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx - otstup];
                        for (int i = otstup; i < Nx - 1; i++)
                            Data[ch][i - otstup] = Q[k][i];
                        Names[ch] = "Q " + ch.ToString();
                        ch++;
                    }
                }
        #endregion
                //
                #region Экспериментальные данные 1,2,3 часа -- A Non-Equilibrium Sediment Transport Model for Coastal Inlets and Navigation Channels Alejandro Sánchez and Weiming Wu
                //List<double[][]> exp_data = new List<double[][]>(); //лист кривых эксперимента
                //double[][] exp1 = new double[18][]; //первая кривая эксперимента через 1 час
                //for (int i = 0; i < exp1.Length; i++)
                //    exp1[i] = new double[2];//[0] -x, [1] -y;
                //exp1[0][1] = 3.25;
                //exp1[1][1] = 3.25;
                //exp1[2][1] = 3.24;
                //exp1[3][1] = 3.15;
                //exp1[4][1] = 3.11;
                //exp1[5][1] = 3.05;
                //exp1[6][1] = 2.85;
                //exp1[7][1] = 2.8;
                //exp1[8][1] = 2.75;
                //exp1[9][1] = 2.7;
                //exp1[10][1] = 2.47;
                //exp1[11][1] = 2.25;
                //exp1[12][1] = 2.05;
                //exp1[13][1] = 1.8;
                //exp1[14][1] = 1.6;
                //exp1[15][1] = 1.47;
                //exp1[16][1] = 1.25;
                //exp1[17][1] = 1;
                ////

                //////х через каждые 0,5 м
                ////for (int i = 0; i < exp1.Length; i++)
                ////{
                ////    exp1[i][0] -= 0.2;
                ////    exp1[i][1] = 0.5 + 0.5*i;
                ////}
                //exp_data.Add(exp1);
                ////
                //double[][] exp2 = new double[18][]; //первая кривая эксперимента через 2 часа
                //for (int i = 0; i < exp1.Length; i++)
                //    exp2[i] = new double[2];//[0] -x, [1] -y;
                //exp2[0][1] = 2.8;
                //exp2[1][1] = 2.75;
                //exp2[2][1] = 2.7;
                //exp2[3][1] = 2.6;
                //exp2[4][1] = 2.58;
                //exp2[5][1] = 2.52;
                //exp2[6][1] = 2.5;
                //exp2[7][1] = 2.45;
                //exp2[8][1] = 2.3;
                //exp2[9][1] = 2.2;
                //exp2[10][1] = 2.1;
                //exp2[11][1] = 1.97;
                //exp2[12][1] = 1.8;
                //exp2[13][1] = 1.68;
                //exp2[14][1] = 1.51;
                //exp2[15][1] = 1.45;
                //exp2[16][1] = 1.2;
                //exp2[17][1] = 1;
                ////
                //exp_data.Add(exp2);
                ////
                //double[][] exp3 = new double[18][]; //первая кривая эксперимента через 3 часа
                //for (int i = 0; i < exp1.Length; i++)
                //    exp3[i] = new double[2];//[0] -x, [1] -y;
                //exp3[0][1] = 2.48;
                //exp3[1][1] = 2.47;
                //exp3[2][1] = 2.46;
                //exp3[3][1] = 2.43;
                //exp3[4][1] = 2.3;
                //exp3[5][1] = 2.25;
                //exp3[6][1] = 2.15;
                //exp3[7][1] = 2.1;
                //exp3[8][1] = 2.05;
                //exp3[9][1] = 1.95;
                //exp3[10][1] = 1.8;
                //exp3[11][1] = 1.75;
                //exp3[12][1] = 1.58;
                //exp3[13][1] = 1.5;
                //exp3[14][1] = 1.3;
                //exp3[15][1] = 1.22;
                //exp3[16][1] = 1.11;
                //exp3[17][1] = 0.9;
                ////
                //exp_data.Add(exp3);
                ////х через каждые 0,5 м
                //for (int i = 0; i < exp1.Length; i++)
                //{
                //    //exp1[i][1] *= 0.1-0.02;
                //    exp1[i][1] = exp1[i][1] * 0.01 - 0.0055; // + 0.2;
                //    exp2[i][1] = exp2[i][1] * 0.01 - 0.0055; // +0.2;
                //    exp3[i][1] = exp3[i][1] * 0.01 - 0.0055; // +0.2;
                //    exp1[i][0] = 0.5 + 0.5 * i;
                //    exp2[i][0] = 0.5 + 0.5 * i;
                //    exp3[i][0] = 0.5 + 0.5 * i;
                //}
                ////
                #endregion
            }
            catch
            { }
                #region Экспериментальные данные 1, 4, 24 часа --- Jodeau, Wu (CCHE1D)
                exp_data = new List<double[][]>(); //лист кривых эксперимента
                double[][] exp1 = new double[14][]; //первая кривая эксперимента через 1 час
                for (int i = 0; i < exp1.Length; i++)
                    exp1[i] = new double[2];//[0] -x, [1] -y;
                exp1[0][1] = -0.015;
                exp1[1][1] = -0.0155;
                exp1[2][1] = -0.016;
                exp1[3][1] = -0.0165;
                exp1[4][1] = -0.017;
                exp1[5][1] = -0.018;
                exp1[6][1] = -0.0195;
                exp1[7][1] = -0.021;
                exp1[8][1] = -0.023;
                exp1[9][1] = -0.026;
                exp1[10][1] = -0.028;
                exp1[11][1] = -0.0315;
                exp1[12][1] = -0.033;
                exp1[13][1] = -0.036;
                //
                exp_data.Add(exp1);
                //
                double[][] exp2 = new double[14][]; //первая кривая эксперимента через 4 часа
                for (int i = 0; i < exp1.Length; i++)
                    exp2[i] = new double[2];//[0] -x, [1] -y;
                exp2[0][1] = -0.025;
                exp2[1][1] = -0.0255;
                exp2[2][1] = -0.026;
                exp2[3][1] = -0.0269;
                exp2[4][1] = -0.027;
                exp2[5][1] = -0.0278;
                exp2[6][1] = -0.0276;
                exp2[7][1] = -0.03;
                exp2[8][1] = -0.0315;
                exp2[9][1] = -0.033;
                exp2[10][1] = -0.0345;
                exp2[11][1] = -0.0365;
                exp2[12][1] = -0.037;
                exp2[13][1] = -0.039;
                //
                exp_data.Add(exp2);
                //
                double[][] exp3 = new double[14][]; //первая кривая эксперимента через 24 часа
                for (int i = 0; i < exp1.Length; i++)
                    exp3[i] = new double[2];//[0] -x, [1] -y;
                exp3[0][1] = -0.04;
                exp3[1][1] = -0.0402;
                exp3[2][1] = -0.0405;
                exp3[3][1] = -0.041;
                exp3[4][1] = -0.0414;
                exp3[5][1] = -0.0418;
                exp3[6][1] = -0.0422;
                exp3[7][1] = -0.043;
                exp3[8][1] = -0.0432;
                exp3[9][1] = -0.044;
                exp3[10][1] = -0.0445;
                exp3[11][1] = -0.0449;
                exp3[12][1] = -0.0457;
                exp3[13][1] = -0.0459;
                //
                exp_data.Add(exp3);
                //х через каждые 0,5 м
                for (int i = 0; i < exp1.Length; i++)
                {
                    //exp1[i][1] *= 0.1-0.02;
                    exp1[i][1] += 0.047;
                    exp2[i][1] += 0.047;
                    exp3[i][1] += 0.047;
                    exp1[i][0] = 0.6 + 0.6 * i;
                    exp2[i][0] = 0.6 + 0.6 * i;
                    exp3[i][0] = 0.6 + 0.6 * i;
                }
                #endregion
            
            
        }
        private void DataPrepearingForGraphicWithPreArea(out int CountCurve, out double[][] Data, out string[] Names, out List<double[][]> exp_data)
        {
            #region Данные для отрисовки
            bool flagTau = ch_Tau.Checked;
            bool flagEta = chEta.Checked;
            bool flagH = chH.Checked;
            bool flagU = chU.Checked;
            bool flagQ = chQ.Checked;
            CountCurve = 4;
            if (flagTau)
                CountCurve += 4;
            if (flagEta)
                CountCurve += 4;
            if (flagH)
                CountCurve += 4;
            if (flagU)
                CountCurve += 4;
            if (flagQ)
                CountCurve += 4;
            //

            Data = new double[CountCurve][];
            Names = new string[CountCurve];
            try
            {
                int ch = 0;
                for (int k = 0; k < 4; k++)
                {
                    Data[ch] = new double[Nx];
                    for (int i = 0; i < Nx; i++)
                        Data[ch][i] = Z[k][i];
                    Names[ch] = "Zeta " + ch.ToString();
                    ch++;
                }
                //
                if (flagTau)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx];
                        for (int i = 0; i < Nx - 1; i++)
                            Data[ch][i] = Tau[k][i];
                        Names[ch] = "Tau " + ch.ToString();
                        ch++;
                    }
                }
                if (flagEta)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx];
                        for (int i = 0; i < Nx; i++)
                            Data[ch][i] = Eta[k][i];
                        Names[ch] = "Eta " + ch.ToString();
                        ch++;
                    }
                }
                //
                if (flagU)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx];
                        for (int i = 0; i < Nx; i++)
                            Data[ch][i] = U[k][i];
                        Names[ch] = "U " + ch.ToString();
                        ch++;
                    }
                }
                //
                if (flagH)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx];
                        for (int i = 0; i < Nx; i++)
                            Data[ch][i] = H[k][i];
                        Names[ch] = "H " + ch.ToString();
                        ch++;
                    }
                }
                //
                if (flagQ)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Data[ch] = new double[Nx];
                        for (int i = 0; i < Nx - 1; i++)
                            Data[ch][i] = Q[k][i];
                        Names[ch] = "Q " + ch.ToString();
                        ch++;
                    }
                }
            #endregion
                //
                #region Экспериментальные данные 1,2,3 часа -- A Non-Equilibrium Sediment Transport Model for Coastal Inlets and Navigation Channels Alejandro Sánchez and Weiming Wu
                //List<double[][]> exp_data = new List<double[][]>(); //лист кривых эксперимента
                //double[][] exp1 = new double[18][]; //первая кривая эксперимента через 1 час
                //for (int i = 0; i < exp1.Length; i++)
                //    exp1[i] = new double[2];//[0] -x, [1] -y;
                //exp1[0][1] = 3.25;
                //exp1[1][1] = 3.25;
                //exp1[2][1] = 3.24;
                //exp1[3][1] = 3.15;
                //exp1[4][1] = 3.11;
                //exp1[5][1] = 3.05;
                //exp1[6][1] = 2.85;
                //exp1[7][1] = 2.8;
                //exp1[8][1] = 2.75;
                //exp1[9][1] = 2.7;
                //exp1[10][1] = 2.47;
                //exp1[11][1] = 2.25;
                //exp1[12][1] = 2.05;
                //exp1[13][1] = 1.8;
                //exp1[14][1] = 1.6;
                //exp1[15][1] = 1.47;
                //exp1[16][1] = 1.25;
                //exp1[17][1] = 1;
                ////

                //////х через каждые 0,5 м
                ////for (int i = 0; i < exp1.Length; i++)
                ////{
                ////    exp1[i][0] -= 0.2;
                ////    exp1[i][1] = 0.5 + 0.5*i;
                ////}
                //exp_data.Add(exp1);
                ////
                //double[][] exp2 = new double[18][]; //первая кривая эксперимента через 2 часа
                //for (int i = 0; i < exp1.Length; i++)
                //    exp2[i] = new double[2];//[0] -x, [1] -y;
                //exp2[0][1] = 2.8;
                //exp2[1][1] = 2.75;
                //exp2[2][1] = 2.7;
                //exp2[3][1] = 2.6;
                //exp2[4][1] = 2.58;
                //exp2[5][1] = 2.52;
                //exp2[6][1] = 2.5;
                //exp2[7][1] = 2.45;
                //exp2[8][1] = 2.3;
                //exp2[9][1] = 2.2;
                //exp2[10][1] = 2.1;
                //exp2[11][1] = 1.97;
                //exp2[12][1] = 1.8;
                //exp2[13][1] = 1.68;
                //exp2[14][1] = 1.51;
                //exp2[15][1] = 1.45;
                //exp2[16][1] = 1.2;
                //exp2[17][1] = 1;
                ////
                //exp_data.Add(exp2);
                ////
                //double[][] exp3 = new double[18][]; //первая кривая эксперимента через 3 часа
                //for (int i = 0; i < exp1.Length; i++)
                //    exp3[i] = new double[2];//[0] -x, [1] -y;
                //exp3[0][1] = 2.48;
                //exp3[1][1] = 2.47;
                //exp3[2][1] = 2.46;
                //exp3[3][1] = 2.43;
                //exp3[4][1] = 2.3;
                //exp3[5][1] = 2.25;
                //exp3[6][1] = 2.15;
                //exp3[7][1] = 2.1;
                //exp3[8][1] = 2.05;
                //exp3[9][1] = 1.95;
                //exp3[10][1] = 1.8;
                //exp3[11][1] = 1.75;
                //exp3[12][1] = 1.58;
                //exp3[13][1] = 1.5;
                //exp3[14][1] = 1.3;
                //exp3[15][1] = 1.22;
                //exp3[16][1] = 1.11;
                //exp3[17][1] = 0.9;
                ////
                //exp_data.Add(exp3);
                ////х через каждые 0,5 м
                //for (int i = 0; i < exp1.Length; i++)
                //{
                //    //exp1[i][1] *= 0.1-0.02;
                //    exp1[i][1] = exp1[i][1] * 0.01 - 0.0055; // + 0.2;
                //    exp2[i][1] = exp2[i][1] * 0.01 - 0.0055; // +0.2;
                //    exp3[i][1] = exp3[i][1] * 0.01 - 0.0055; // +0.2;
                //    exp1[i][0] = 0.5 + 0.5 * i;
                //    exp2[i][0] = 0.5 + 0.5 * i;
                //    exp3[i][0] = 0.5 + 0.5 * i;
                //}
                ////
                #endregion
            }
            catch
            { }
            #region Экспериментальные данные 1, 4, 24 часа --- Jodeau, Wu (CCHE1D)
            exp_data = new List<double[][]>(); //лист кривых эксперимента
            double[][] exp1 = new double[14][]; //первая кривая эксперимента через 1 час
            for (int i = 0; i < exp1.Length; i++)
                exp1[i] = new double[2];//[0] -x, [1] -y;
            exp1[0][1] = -0.015;
            exp1[1][1] = -0.0155;
            exp1[2][1] = -0.016;
            exp1[3][1] = -0.0165;
            exp1[4][1] = -0.017;
            exp1[5][1] = -0.018;
            exp1[6][1] = -0.0195;
            exp1[7][1] = -0.021;
            exp1[8][1] = -0.023;
            exp1[9][1] = -0.026;
            exp1[10][1] = -0.028;
            exp1[11][1] = -0.0315;
            exp1[12][1] = -0.033;
            exp1[13][1] = -0.036;
            //
            exp_data.Add(exp1);
            //
            double[][] exp2 = new double[14][]; //первая кривая эксперимента через 4 часа
            for (int i = 0; i < exp1.Length; i++)
                exp2[i] = new double[2];//[0] -x, [1] -y;
            exp2[0][1] = -0.025;
            exp2[1][1] = -0.0255;
            exp2[2][1] = -0.026;
            exp2[3][1] = -0.0269;
            exp2[4][1] = -0.027;
            exp2[5][1] = -0.0278;
            exp2[6][1] = -0.0276;
            exp2[7][1] = -0.03;
            exp2[8][1] = -0.0315;
            exp2[9][1] = -0.033;
            exp2[10][1] = -0.0345;
            exp2[11][1] = -0.0365;
            exp2[12][1] = -0.037;
            exp2[13][1] = -0.039;
            //
            exp_data.Add(exp2);
            //
            double[][] exp3 = new double[14][]; //первая кривая эксперимента через 24 часа
            for (int i = 0; i < exp1.Length; i++)
                exp3[i] = new double[2];//[0] -x, [1] -y;
            exp3[0][1] = -0.04;
            exp3[1][1] = -0.0402;
            exp3[2][1] = -0.0405;
            exp3[3][1] = -0.041;
            exp3[4][1] = -0.0414;
            exp3[5][1] = -0.0418;
            exp3[6][1] = -0.0422;
            exp3[7][1] = -0.043;
            exp3[8][1] = -0.0432;
            exp3[9][1] = -0.044;
            exp3[10][1] = -0.0445;
            exp3[11][1] = -0.0449;
            exp3[12][1] = -0.0457;
            exp3[13][1] = -0.0459;
            //
            exp_data.Add(exp3);
            //х через каждые 0,5 м
            for (int i = 0; i < exp1.Length; i++)
            {
                //exp1[i][1] *= 0.1-0.02;
                exp1[i][1] += 0.047;
                exp2[i][1] += 0.047;
                exp3[i][1] += 0.047;
                exp1[i][0] = 0.6 + 0.6 * i;
                exp2[i][0] = 0.6 + 0.6 * i;
                exp3[i][0] = 0.6 + 0.6 * i;
            }
            #endregion


        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool flag = false;
            double[] DataX;
            if (flag)
            {
                DataPrepearingForGraphic(out CountCurve, out Data, out Names, out exp_data);
                //
                DataX = new double[Nx - otstup];
                for (int i = otstup; i < Nx; i++)
                    DataX[i - otstup] = i * dx - preArea;
            }
            else
            {
                DataPrepearingForGraphicWithPreArea(out CountCurve, out Data, out Names, out exp_data);
                //
                DataX = new double[Nx];
                for (int i = 0; i < Nx; i++)
                    DataX[i] = i * dx - preArea;
            }
            double[][][] Exp = exp_data.ToArray();
            double[][] expY = new double[3][];
            expY[0] = new double[Exp[0].Length];
            expY[1] = new double[Exp[0].Length];
            expY[2] = new double[Exp[0].Length];
            for (int i = 0; i < Exp[0].Length; i++)
            {
                expY[0][i] = Exp[0][i][1];
                expY[1][i] = Exp[1][i][1];
                expY[2][i] = Exp[2][i][1];
            }
            double[][] expX = new double[3][];
            expX[0] = new double[Exp[0].Length];
            expX[1] = new double[Exp[0].Length];
            expX[2] = new double[Exp[0].Length];
            for (int i = 0; i < Exp[0].Length; i++)
            {
                expX[0][i] = Exp[0][i][0];
                expX[1][i] = Exp[1][i][0];
                expX[2][i] = Exp[2][i][0];
            }
            string[] expNames = { "exp 1 ч", "exp 4 ч", "exp 24 ч" };
            //
            ShowGraphic.ChartForm FShow = new ShowGraphic.ChartForm();
            FShow.DataY = Data;
            FShow.DataX = DataX;
            FShow.ExpDataY = expY;
            FShow.ExpDataX = expX;
            FShow.GraphName = "Размыв 24 часа";
            FShow.Names = Names;
            FShow.NamesExp = expNames; 
            FShow.Show();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            pEliz = new ElizShallow1D_Parameter();
            pEliz.Hout = 2;
            pEliz.Uout = 2.21;
            //
            Time = 200;
            //
            dt = 0.01;
            L = 25;
            Nx = 500;
            dx = L / (Nx - 1);
            //массивы для хранения буфера
            Z = new double[4][];
            Eta = new double[4][];
            Tau = new double[4][];
            U = new double[4][];
            H = new double[4][];
            Q = new double[4][];
            //начальный уровень дна
            Z[0] = new double[Nx];
            Eta[0] = new double[Nx];
            U[0] = new double[Nx];
            H[0] = new double[Nx];
            Q[0] = new double[Nx];
            double[] Q1 = new double[Nx];
            double[] Q2 = new double[Nx];
            Z0 = new double[Nx];
            double x = 0;
            for (int i = 0; i < Nx; i++)
            {
                x = i * dx;
                if ((x > 8) && (x < 12))
                    Z0[i] = 0.2 - 0.05 * (x - 10) * (x - 10);
                Z[0][i] = Z0[i];
                Eta[0][i] = pEliz.Hout;
                H[0][i] = pEliz.Hout;
                U[0][i] = pEliz.Uout;
            }
            //синхронизация параметров
            //BedPhysicsParams P = taskControl1.ParameterObject;
            //pEliz.rho_w = P.rhoW;
            //pEliz.g = P.g;
            //shEliz = new ElizShallowWater1D(pEliz, L, Nx, dt);
            ////shWR = new ReverseShallowWater1D(p, dx, Nx, 0);
            ////получение объекта задачи дна
            //BaseBedLoadTask BTask = taskControl1.GetRiverTaskObject();
            //// установка граничных условий
            //RiverTaskLibrary.BoundaryCondition BCondition = new RiverTaskLibrary.BoundaryCondition();
            //BCondition.Inlet = RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed;
            //BCondition.Outlet = RiverTaskLibrary.TypeBoundaryCondition.Transit_Feed;// Neumann_boundary_conditions;
            ////установка ГУ в решение
            //BTask.ReStartBaseBedLoadTask(BCondition, P, dx, Nx, dt);
            //// финт для трансзитного расхода на выходе (вычисляется по параметрам, которые вводятся вместе с ГУ)
            ////BCondition.OutletValue = BTask.CalculateRate(shWR.TaskSolver(Z0)[Nx - 2], J);
            //// установка начального дна в решение
            //BTask.SetZeta0(Z0);
            //for (int i = 0; i < Nx; i++)
            //    Q[0][i] = BCondition.OutletValue;
            ////
            int ch = 1;
            Int64 Nt = Convert.ToInt64(Time / dt);
            double[] stress;
            Tau[0] = new double[Nx];
            //расчет
            for (int i = 0; i < Nt; i++)
            {
                //гидродинамика, получаем напряжения
                stress = shEliz.Solver(Z0);
                // дно, получем новый уровень дна
                //Z0 = BTask.Solve(stress, shEliz.Eta, Q1, Q2);
                // буферизация 
                if (((i * dt) == 50) || ((i * dt) == 100) || (i == Nt - 1))
                {
                    Z[ch] = new double[Nx];
                    Eta[ch] = new double[Nx];
                    Tau[ch] = new double[Nx - 1];
                    U[ch] = new double[Nx];
                    H[ch] = new double[Nx];
                    Q[ch] = new double[Nx - 1];
                    for (int j = 0; j < Nx; j++)
                    {
                        Z[ch][j] = Z0[j];
                        Eta[ch][j] = shEliz.Eta[j];
                        U[ch][j] = shEliz.U[j];
                        H[ch][j] = shEliz.H[j];

                    }
                    for (int j = 0; j < Nx - 1; j++)
                    {
                        Tau[ch][j] = stress[j];
                        //Q[ch][j] = BTask.Qb[j];
                    }
                    ch++;
                }
            }  
        }

        
    }
}
