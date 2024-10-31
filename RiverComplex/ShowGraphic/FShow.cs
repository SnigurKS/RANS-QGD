using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShowGraphic
{
    public partial class FShow : Form
    {
        // вид графика
        bool FlagGraph = false;
        // количество узлов
        public int CountLine=0;
        // количество кривых
        public int CountCurve=0;
        // массив графиков
        public double[][] Data = null;
        // массив точек эксперимента [i][0] - x, [i][1] - y
        public List<double[][]> Exp_Data = null;
        // рабочий масштаб по Х
        public double WLengthX = 1.0;
        // рабочий масштаб по Y
        public double WLengthY = 0.4;
        // наименование графиков
        public string GraphName="";
        string[] Names = null;
        /// <summary>
        /// Смещение нуля графика по Y
        /// </summary>
        double dY = 0;
        public FShow()
        {
            InitializeComponent();
        }

        
        private string ScaleGraphY(double Length, int idx)
        {
            int i = idx % 11;
            // деления на координатных осях
            string[] scale = { "0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1.0" };
            double s = double.Parse(scale[i]);
            double value = (1 - s) * dY + s * Length;
            string SLengthF = (value).ToString();
            string[] SS = SLengthF.Split('.');
            string SLength;
            if (SS.Length == 2)
            {
                int LS = SS[1].Length;
                if (LS > 4) LS = 4;
                SLength = SS[0] + "." + SS[1].Substring(0, LS);
                  }
            else
                SLength = SS[0];
            return SLength;
        }


        private string ScaleGraphL(double Length, int idx)
        {
            int i = idx % 11;
            // деления на координатных осях
            string[] scale = { "0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1.0" };
            double s = double.Parse(scale[i]);
            double value = s * Length;
            string SLengthF = (value).ToString();
            string[] SS = SLengthF.Split('.');
            string SLength;
            if (SS.Length == 2)
            {
                int LS = SS[1].Length;
                if (LS > 4) LS = 4;
                SLength = SS[0] + "." + SS[1].Substring(0, LS);
            }
            else
                SLength = SS[0];
            return SLength;
        }
        //private string ScaleGraphH(double Length, int idx)
        //{
        //    int i = idx % 5;
        //    // деления на координатных осях
        //    string[] scaleF = { "0", "0.25", "0.5", "0.75", "1.0" };
        //    string[] scaleT = { "-1.0", "-0.5", "0", "0.75", "1.0" };
        //    string[] scale;
        //    if(FlagGraph == true)
        //        scale = scaleT;
        //    else
        //        scale = scaleF;
        //    string SLengthF = (double.Parse(scale[i]) * Length).ToString();
        //    string[] SS = SLengthF.Split('.');
        //    string SLength;// = scale[i];
        //    if (SS.Length == 2)
        //    {
        //        int LS = SS[1].Length;
        //        if (LS > 3) LS = 3;
        //        SLength = SS[0] + "." + SS[1].Substring(0, LS);
        //    }
        //    else
        //        SLength = SS[0];
        //    return SLength;
        //}
        private void button1_Click_1(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void FShow_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                Pen penRed = new Pen(Color.Red, 2);
                Pen[] CPens = new Pen[13];
                Pen penGreen = new Pen(Color.Green, 2);
                Pen penBlack = new Pen(Color.Black, 2);
                Pen penGray = new Pen(Color.Gray, 2);
                CPens[0] = new Pen(Color.Black, 2);
                CPens[1] = new Pen(Color.Green, 2);
                CPens[2] = new Pen(Color.Gray, 2);
                CPens[3] = new Pen(Color.HotPink, 2);
                CPens[4] = new Pen(Color.Pink, 2);
                CPens[5] = new Pen(Color.Plum, 2);
                CPens[6] = new Pen(Color.Aqua, 2);
                CPens[7] = new Pen(Color.Purple, 2);
                CPens[8] = new Pen(Color.RoyalBlue, 2);
                CPens[9] = new Pen(Color.Teal, 2);
                CPens[10] = new Pen(Color.Violet, 2);
                CPens[11] = new Pen(Color.Tan, 2);
                CPens[12] = new Pen(Color.Salmon, 2);

                Pen[] pens = new Pen[4];
                pens[0] = penBlack;
                pens[1] = penGreen;
                pens[2] = penRed;

                g.DrawString(GraphName, new Font("Arial", 14), Brushes.Black, 160, 15);

                // привязки системы координат
                int W = this.Width - 120;
                int H = this.Height - 50;
                if (FlagGraph == true)
                    H = (this.Height - 50) / 2;

                int DWTop = 50;
                int DWBotton = H - 50;
                int DWTopBotton = 2 * DWBotton - DWTop;

                int DWLeft = 70;
                int DWRight = W - 50;

                int CountStart = 0;
                // координатные оси
                Point a = new Point(DWLeft, DWBotton);
                Point b = new Point(DWRight + 10, DWBotton);
                Point c = new Point(DWLeft, DWTop - 10);
                Point mc = new Point(DWLeft, DWTopBotton);
                g.DrawLine(penBlack, a, b);
                if (FlagGraph == true)
                {
                    g.DrawLine(penBlack, c, mc);
                    CountStart = 1;
                }
                else
                    g.DrawLine(penBlack, a, c);

                // горизонтальные
                int LenghtX = DWRight - DWLeft;
                int DeltX = (int)(LenghtX / 4);
                int DeltXG = (int)(LenghtX / 10);

                for (int i = CountStart; i < 11; i++)
                {
                    int x = DWLeft + DeltXG * i;
                    a = new Point(x, DWBotton);
                    b = new Point(x, DWBotton + 5);
                    g.DrawLine(penBlack, a, b);
                    string SLength = ScaleGraphL(WLengthX, i);
                    g.DrawString(SLength, new Font("Arial", 12), Brushes.Black, x - 10, DWBotton + 10);
                }
                // вертикальные
                int LenghtY = DWBotton - DWTop;
                int DeltY = (int)(LenghtY / 10);
                //0
                int CountPoint = 11;
                for (int i = 0; i < CountPoint; i++)
                {
                    int y = DWBotton - DeltY * i;
                    a = new Point(DWLeft - 5, y);
                    b = new Point(DWLeft, y);
                    g.DrawLine(penBlack, a, b);
                    string SLength = ScaleGraphY(WLengthY, i);
                    g.DrawString(SLength, new Font("Arial", 12), Brushes.Black, DWLeft - 65, y - 10);
                }
                if (FlagGraph == true)
                {
                    for (int i = 1; i < 11; i++)
                    {
                        int y = DWBotton + DeltY * i;
                        a = new Point(DWLeft - 5, y);
                        b = new Point(DWLeft, y);
                        g.DrawLine(penBlack, a, b);
                        string SLength = ScaleGraphY(WLengthY, i);
                        g.DrawString("-" + SLength, new Font("Arial", 12), Brushes.Black, DWLeft - 65, y - 10);
                    }
                }

                // отрисовка графиков
                int xa,xb;
                if (Data != null)
                {
                    double dx = WLengthX / (CountLine - 1);
                    // количество кривых
                    for (int idx = 0; idx < CountCurve; idx++)
                    {
                        CheckState Flag = cListBoxFiltr.GetItemCheckState(idx);
                        if (Flag == CheckState.Unchecked)
                            continue;
                        // количество узлов
                        for (uint i = 0; i < Data[idx].Length-1; i++)
                        {
                            if (cbRevers.Checked)
                            {
                                xa = DWLeft + (int)(LenghtX * (1-(dx * i) / WLengthX));
                                xb = DWLeft + (int)(LenghtX * (1-(dx * i + dx) / WLengthX));
                            }
                            else 
                            {
                                xa = DWLeft + (int)(LenghtX * (dx * i) / WLengthX);
                                xb = DWLeft + (int)(LenghtX * (dx * i + dx) / WLengthX);
                            }

                            int ya = DWTop + (int)(LenghtY * (1 - (Data[idx][i]-dY) / WLengthY));
                            int yb = DWTop + (int)(LenghtY * (1 - (Data[idx][i + 1]-dY) / WLengthY));

                            a = new Point(xa, ya);
                            b = new Point(xb, yb);
                            CPens[0] = new Pen(Color.Black, 3.0f);
                            if (checkBox1.Checked == true)
                                g.DrawLine(CPens[0], a, b);
                            else
                                g.DrawLine(CPens[idx % 13], a, b);
                        }
                    }
                }
                // отрисовка экспериментальных точек
                    xa = 0; xb = 0;
                   
                    //Exp_Data[0][0] = 1; Exp_Data[1][0] = 12 / (WLengthX / (CountLine - 1));
                    //Exp_Data[0][1] = 0.2; Exp_Data[1][1] = 0.04;
                if ((Exp_Data != null)&&(ch_Ex.Checked))
                {
                    double dx = WLengthX / (CountLine - 1);
                    double CountCurve = Exp_Data.Count;
                    Brush[] br = {Brushes.Green, Brushes.DarkSlateGray, Brushes.DeepPink };
                    // количество кривых
                    for (int idx = 0; idx < CountCurve; idx ++)
                    {
                        double[][] exp = Exp_Data[idx];
                        for (int i = 0; i < Exp_Data[0].Length; i++)
                        {
                            //
                            if (cbRevers.Checked)
                                xa = DWLeft + (int)(LenghtX * (1 - (exp[i][0]) / WLengthX));
                            else
                                xa = DWLeft + (int)(LenghtX * (exp[i][0]) / WLengthX);
                            //
                            int ya = DWTop + (int)(LenghtY * (1 - (exp[i][1] - dY) / WLengthY));
                            //
                            if (checkBox1.Checked == true)
                                g.FillEllipse(br[idx], (float)xa, (float)ya, 8, 8);
                            else
                                g.FillEllipse(br[idx], (float)xa, (float)ya, 8, 8);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Name = ee.Message;
            }
        }

   
        public void SetcListBoxFiltr(string[] Names)
        {
            cListBoxFiltr.Items.Clear();
            for (int i = 0; i < CountCurve; i++)
                cListBoxFiltr.Items.Add(Names[i], true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dY = double.Parse(tbdY.Text);
                WLengthY = double.Parse(topdY.Text) - dY;
                Invalidate();
            }
            catch(Exception ee)
            {
                Name = ee.Message;
            }
        }

        private void cListBoxFiltr_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void FShow_Load(object sender, EventArgs e)
        {
            //topdY.Text = (WLengthY+dY).ToString();
            dY = double.Parse(tbdY.Text);
            WLengthY = double.Parse(topdY.Text) - dY;
        }

        private void cb2K_CheckedChanged(object sender, EventArgs e)
        {
            FlagGraph = cb2K.Checked;
            Invalidate();
        }

        private void cbRevers_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
