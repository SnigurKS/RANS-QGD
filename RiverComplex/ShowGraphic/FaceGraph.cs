using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ShowGraphic
{
    public partial class FaceGraph : Form
    {
        /// <summary>
        /// Переменные
        /// </summary>
        double[] U, V, P, E, K, S, Nu, Tau, Pk, X, Y;
        /// <summary>
        /// части из ГУ Донга
        /// </summary>
        double[][] up_parts;
        /// <summary>
        /// физические вкеличины по всему полю
        /// </summary>
        double[] ExpX, ExpY;
        double[][] Exp;
        /// <summary>
        ///  величины в заданном сечении или около него
        /// </summary>
        double[] ExpDataX, ExpDataY;
        double[][] ExpData;
        /// <summary>
        /// Названия точек эксперимента
        /// </summary>
        string[] _NamesExp;
        /// <summary>
        /// Названия точек эксперимента
        /// </summary>
        public string[] NamesExp
        {
            set
            {
                _NamesExp = value;
            }
        }
        //
        public double dx = 0, dy = 0;
        public int Nx = 0, Ny = 0, Nx2 = 0, Ny2 = 0;
        /// <summary>
        /// флаг, показывающий по какой координате делается сечение
        /// </summary>
        bool XY = true;
        //
        double FacePoint = 0;
        bool BackwardStep = false;
        /// <summary>
        ///  цветные линии или черно-белые
        /// </summary>
        bool flagColorLine = true;
        /// <summary>
        /// Показывать ли экспериментальные точки
        /// </summary>
        bool flagExp = false;
        /// <summary>
        /// Размер отрисовываемых линий (не экспериментальных)
        /// </summary>
        int LineSize = 2;
        /// <summary>
        /// размер маркеров отрисовки экспериментальных линий
        /// </summary>
        int PointSize = 8;
        /// <summary>
        /// название грфика
        /// </summary>
        string _GraphName = "";
        /// <summary>
        /// массив коэффициентов обезразмеривания на каждую линию в отдельности
        /// </summary>
        double[] MultCoeff;
        //
        CheckBox[] chBoxes;
        /// <summary>
        /// Флаг логарифмического масштаба по X
        /// </summary>
        bool LogX = false;
        /// <summary>
        /// Флаг Логарифмического масштаба по Y
        /// </summary>
        bool LogY = false;
        /// <summary>
        /// количество линий
        /// </summary>
        int CountLines;
        /// <summary>
        /// флаг зуммирования окна по Y на +-0.01
        /// </summary>
        bool flagZoom = false;
        /// <summary>
        /// Масштаб по осям +/-
        /// </summary>
        double ZoomX = 0, ZoomY = 0;
        /// <summary>
        /// название графика
        /// </summary>
        public string GraphName
        {
            set
            {
                _GraphName = value;
            }
        }
        double[][] DataDimen;
        int[] Markers;
        /// <summary>
        /// Координаты Y линий
        /// </summary>
        double[][] _Data;
        /// <summary>
        /// Координаты Y графиков
        /// </summary>
        public double[][] DataY
        {
            set
            {
                _Data = value;
            }
        }

        //
        //
        double[] _DataX;
        /// <summary>
        /// Координаты X графиков (если привязки к x одинаковые)
        /// </summary>
        public double[] DataX
        {
            set
            {
                _DataX = value;
            }
        }
        double[][] _MultDataX;
        /// <summary>
        /// Координаты X графиков (если привязки к x разные)
        /// </summary>
        public double[][] DataXMany
        {
            set
            {
                _MultDataX = value;
            }
        }
        /// <summary>
        /// Названия линий графика
        /// </summary>
        string[] _Names;
        /// <summary>
        /// название линий графика
        /// </summary>
        public string[] Names
        {
            set
            {
                _Names = value;
            }
        }
        

        public FaceGraph()
        {
            InitializeComponent();
            chPaletteLine.SelectedIndex = 0;
            chWidthLine.SelectedIndex = 1;
        }
        public FaceGraph(double[] U, double[] V, double[] P, int Nx, int Ny, double dx, double dy, double[] e = null, double[] k = null, double[] s = null, double[] nu = null, double[] tau = null, double[] Pk= null, double[] X = null, double[] Y = null)
        {
            InitializeComponent();
            chPaletteLine.SelectedIndex = 0;
            chWidthLine.SelectedIndex = 1;
            //
            CountLines = 3;
            //
            this.U = U;
            this.V = V;
            this.P = P;
            _Names = new string[3];
            _Names[0] = "U";
            _Names[1] = "V";
            _Names[2] = "P";
            if (e != null && k != null && s != null && nu!=null && tau!=null && Pk!=null)
            {
                this.E = e;
                this.K = k;
                this.S = s;
                this.Nu = nu;
                this.Tau = tau;
                this.Pk = Pk;
                CountLines = 9;
                _Names = new string[9];
                _Names[0] = "U";
                _Names[1] = "V";
                _Names[2] = "P";
                _Names[3] = "W/E";
                _Names[4] = "K";
                _Names[5] = "S";
                _Names[6] = "Nu";
                _Names[7] = "Tau";
                _Names[8] = "Pk";
            }
            //
            if ((X != null) && (Y != null))
            {
                this.X = X;
                this.Y = Y;
            }
            this.Nx = Nx;
            this.Ny = Ny;
            this.dx = dx;
            this.dy = dy;
            
            _GraphName = "Срезы значений U, V и P";
            BackwardStep = false;
            //
            CreateData(FacePoint);
        }
        //
        public FaceGraph(double[] U, double[] V, double[] P, double[] p_c, double[] p_k, double[] u_p, double[] u_k, double[] u_c, int Nx, int Ny, int Nx2, int Ny2, double dx, double dy)
        {
            InitializeComponent();
            chPaletteLine.SelectedIndex = 0;
            chWidthLine.SelectedIndex = 1;
            //
            this.U = U;
            this.V = V;
            this.P = P;
            this.Nx = Nx;
            this.Ny = Ny;
            this.Nx2 = Nx2;
            this.Ny2 = Ny2;
            this.dx = dx;
            this.dy = dy;
            up_parts = new double[5][];
            up_parts[0] = p_c;
            up_parts[1] = p_k;
            up_parts[2] = u_p;
            up_parts[3] = u_k;
            up_parts[4] = u_c;
            //
            _Names = new string[3];
            _Names[0] = "U";
            _Names[1] = "V";
            _Names[2] = "P";
            //
            if (up_parts[0] != null)
            {
                _Names = new string[8];
                _Names[0] = "U";
                _Names[1] = "V";
                _Names[2] = "P";
                _Names[3] = "p_conv";
                _Names[4] = "p_kinet";
                _Names[5] = "u_pres";
                _Names[6] = "u_kinet";
                _Names[7] = "u_cont";
            }
            //
            CountLines = 8;
            BackwardStep = true;
            CreateDataBwardStep(FacePoint);
        }

        public void LoadExpData(double[] x, double[] y, double[] U, double[] V, double[] TKE, double [] e, double [] nuT, double [] tau, double [] Pk)
        {
            ExpX = x;
            ExpY = y;
            Exp = new double[7][];
            Exp[0] = U;
            Exp[1] = V;
            Exp[2] = TKE;
            Exp[3] = e;
            Exp[4] = nuT;
            Exp[5] = tau;
            Exp[6] = Pk;

            //
            _NamesExp = new string[7];
            _NamesExp[0] = "U exp";
            _NamesExp[1] = "V exp";
            _NamesExp[2] = "K exp";
            _NamesExp[3] = "E exp";
            _NamesExp[4] = "nuT exp";
            _NamesExp[5] = "Tau exp";
            _NamesExp[6] = "Pk exp";

        }
        private void ChartForm_Resize(object sender, EventArgs e)
        {
            chart1.Width = this.Width - 50;
            chart1.Height = this.Height - 50;
            if (_Data != null)
            {
                //if ((_Data[0].Max() - _Data[0].Min()) > 0.1)
                //    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(-0.1, 0.1);
                chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            }

            //chart1.ChartAreas[0].AxisY.ScaleView.Zoom(-0.1, 0.1);
            //chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
        }

        private void ChartForm_Paint(object sender, PaintEventArgs e)
        {
            #region отрисовка по y разная (y/delta, y+)
            ////переопределение _DataX для отрисовки y/delta2 для эксперимента по Луцкому Weirghard
            //double[] delta2 = new double[Ny];
            //double a1 = 0, a2 = 0, u1=0,u2=0;
            //int N = (int)Math.Round((FacePoint / dx), MidpointRounding.AwayFromZero);
            //double delta = 5 * Math.Sqrt(0.0000165 * 5.0 / 33.0);
            //for (int i = 0; i < Ny-1; i++)
            //{
            //    u1 = U[N * Ny + 1] / 33.0;
            //    a1 = u1 * (1 - u1);
            //    u2 = U[N * (Ny + 1) + 1] / 33.0;
            //    a2 = u2 * (1 - u2);
            //    delta2[i] = 0.5*(a1 + a2) * dy;
            //}
            //delta2[Ny - 1] = delta2[Ny - 2];
            ////
            //for (int i = 0; i < Ny; i++)
            //{
            //    _DataX[i] = dy * i / delta2[i];
            //}
            for (int i = 0; i < Ny; i++)
            {
                // вместо Р передается y_plus в Form.cs Ln333
                _DataX[i] = P[i];
            }
            ////!!!!            //
            #endregion
            //
            chart1.Series.Clear();
            // цветные линии или черные
            if (flagColorLine)
                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            else
            {
                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
                Color[] c = { Color.Black };
                chart1.PaletteCustomColors = c;
            }
            if (LogX)
            {
                if (_DataX[0] <= 0)
                {
                    _DataX[0] = 0.0000001;
                }
            }
            if (LogY)
            {
                if (DataDimen[0][0] <= 0)
                {
                    for (int i = 0; i < chBoxes.Length; i++)
                    {
                        if (chBoxes[i].Checked)
                        {
                            for (int k = Markers[i]; k < Markers[i + 1]; k++)
                            {
                                for (int j = 0; j < DataDimen[0].Length; j++)
                                {
                                    if (DataDimen[k][j] <= 0)
                                        DataDimen[k][j] = 0.000000001;
                                }
                            }
                        }
                    }

                }
            }
            if (chBoxes != null)
            {
                if (_DataX != null)
                {
                    for (int i = 0; i < chBoxes.Length; i++)
                    {
                        if (chBoxes[i].Checked)
                        {
                            for (int k = Markers[i]; k < Markers[i + 1]; k++)
                            {
                                chart1.Series.Add("Series" + k.ToString());
                                chart1.Series["Series" + k.ToString()].Points.DataBindXY(_DataX, DataDimen[k]);
                                chart1.Series["Series" + k.ToString()].ChartType = SeriesChartType.Line;
                                chart1.Series["Series" + k.ToString()].BorderWidth = LineSize;
                                // колонки
                                //chart1.Series["Series" + k.ToString()].ChartType = SeriesChartType.Column;
                                // пунктир
                                //chart1.Series["Series" + k.ToString()].BorderDashStyle = ChartDashStyle.Dash;
                                // должно стоять последним
                                chart1.Series["Series" + k.ToString()].Name = _Names[k];
                            }
                        }
                        //chart1.ChartAreas.Add("chart2");
                        //chart1.ChartAreas[1].
                        chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                        chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                        chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                        chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
                        chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                        chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                        chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
                        //if ((_Data[0].Max() - _Data[0].Min()) > 0.1)
                       
                        chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.001;
                        chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                        chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                        //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.NotSet;
                        //chart1.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.NotSet;

                            chart1.ChartAreas[0].AxisX.IsLogarithmic = LogX;
                            chart1.ChartAreas[0].AxisX.LogarithmBase = 10;
                            chart1.ChartAreas[0].AxisX.IsStartedFromZero = !LogX;
                            chart1.ChartAreas[0].AxisY.IsLogarithmic = LogY;
                            chart1.ChartAreas[0].AxisY.LogarithmBase = 10;
                            chart1.ChartAreas[0].AxisY.IsStartedFromZero = !LogY;

                        //
                        //chart1.ChartAreas[0].AxisY.IsLogarithmic = true;
                        //chart1.ChartAreas[0].AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
                        //chart1.ChartAreas[0].AxisY2.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont;
                        //chart1.ChartAreas[0].AxisY2.Maximum = 0.0001;
                        //chart1.ChartAreas[0].AxisY2.Minimum = -0.0001;
                    }

                }
                // отрисовка данных, если координаты X везде разные
                if (_MultDataX != null)
                {
                    for (int i = 0; i < chBoxes.Length; i++)
                    {
                        if (chBoxes[i].Checked)
                        {
                            for (int k = Markers[i]; k < Markers[i + 1]; k++)
                            {
                                chart1.Series.Add("Series" + k.ToString());
                                chart1.Series["Series" + k.ToString()].Points.DataBindXY(_MultDataX[k], DataDimen[k]);
                                chart1.Series["Series" + k.ToString()].ChartType = SeriesChartType.Line;
                                chart1.Series["Series" + k.ToString()].BorderWidth = LineSize;
                                chart1.Series["Series" + k.ToString()].Name = _Names[k];
                            }
                        }
                    }
                    chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
                    chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                    //
                    chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
                    chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                }
            }
            ////отрисовка экспериментальных данных
            if (flagExp)
            {
                if ((ExpData != null) & (ExpDataX != null))
                {
                    int ch = chart1.Series.Count;
                    for  (int i = 0; i < ExpData.Length; i++)
                    {
                        //if (i <2)
                        {
                            chart1.Series.Add("Series" + ch.ToString());
                            chart1.Series["Series" + ch.ToString()].Points.DataBindXY(ExpDataX, ExpData[i]);
                            chart1.Series[ch].ChartType = SeriesChartType.Point;
                            chart1.Series[ch].Name = _NamesExp[i];
                            chart1.Series[ch].MarkerSize = PointSize;
                            //chart1.Series[ch].BorderWidth = 2;
                            ch++;
                        }
                    }
                    //
                    int idx =cmb_box_C2.SelectedIndex;
                    int idx2 = idx;
                    switch (idx)
                    {
                        case (-1): { idx = 0; idx2 = 0; break; }
                        case (0): { break; }
                        case (1): { break; }
                        case (2): { idx2 = 4; break; }
                        case (3): { break; }
                        case (4): { idx2 = 6; break; }
                        case (5): { idx2 = 7; break; }
                        case (6): { idx2 = 8; break; }
                    }
                    double[] c2 = C2_Discrepancy(ExpDataX, ExpData[idx], _DataX, DataDimen[idx2]);
                    StripStatusLabel_c2.Text = "C2 Discrepancy " + _Names[idx2] + ": Average - " + c2[0].ToString("G4", System.Globalization.CultureInfo.InvariantCulture) +
                        "%, MaxC2 - " + c2[1].ToString("G4", System.Globalization.CultureInfo.InvariantCulture) + "%, MinC2 - " + c2[2].ToString("G4", System.Globalization.CultureInfo.InvariantCulture) + "%.";
                    
                }
                
            }
            // применение параметров к осям
            AxisParameters();
        }

        /// <summary>
        /// Относительная погрешность в сечении
        /// </summary>
        /// <param name="ExpX">Координаты Х экспериментальных точек</param>
        /// <param name="ExpF">Значения функции F в экспериментальных токчах</param>
        /// <param name="X">Расчтеные координаты Х</param>
        /// <param name="F">Расчетные значения функции F</param>
        /// <returns></returns>
        private double[] C2_Discrepancy(double[] ExpX, double[] ExpF, double[] X, double[] F)
        {
            int N = ExpF.Length;
            double[] C2 = new double[N];
            //
            double maxExact = 0, minExact = 10000000;
            double val=0;
            for (int i = 0; i < N; i++)
            {
                val = Math.Abs(ExpF[i]);
                if (maxExact < val)
                    maxExact = val;
                if (minExact > val)
                    minExact = val;
            }
            //
            val = Math.Abs(maxExact - minExact);
            if (val == 0) 
                val = maxExact;
            //интерполируем в экспериментальные точки расчетные значения (линейно)
            double[] CalcF = new double[N];
            double x_curr = 0;
            int XLength = X.Length;
            for (int i = 0; i < N; i++)
            {
                x_curr = ExpX[i];
                for (int j = 0; j < XLength; j++)
                {
                    if ((X[j] <= x_curr) && (X[j+1] >= x_curr))
                    {
                        CalcF[i] = F[j] + (x_curr - X[j]) * (F[j+1] - F[j]) / (X[j+1] - X[j]);
                        break;
                    }
                }
            }
            //
            int ch = 0;
            double minF = 100000, maxF = 0;
            for (int i = 0; i < N; i++)
            {
                if (!double.IsNaN(ExpF[i]))
                {
                    C2[i] = Math.Abs(CalcF[i] - ExpF[i]) / maxExact * 100;
                    ch++;
                    if (C2[i] < minF)
                        minF = C2[i];
                    //
                    if (C2[i] > maxF)
                        maxF = C2[i];
                }
            }
            //
            double[] output = new double[3];
            output[0] = C2.Sum() / ch;
            output[1] = maxF;
            output[2] = minF;
            //
            return output; 
        }
        void AxisParameters()
        {
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = fontDialog1.Font;
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = fontDialog2.Font;
            //
            if (flagZoom && !LogX && !LogY)
            {
                chart1.ChartAreas[0].AxisY.ScaleView.Zoom(-ZoomY, ZoomY);
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(-ZoomX, ZoomX);
            }
            else
            {
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            }
        }
        
        //
        void CreateDataAcc(double FacePoint)
        {
           
            if (XY)
            {
                if (FacePoint <= Nx * dx)
                {
                    _Data = new double[CountLines][];
                    for (int i = 0; i < CountLines; i++)
                        _Data[i] = new double[Ny];
                    _DataX = new double[Ny];
                    int ch = 0; int knot0 = 0, knot1 = 0;
                    for (int j = 0; j < Ny; j++) 
                    {
                        for (int i = 0; i < Nx - 1; i++)
                        {
                            knot0 = j + i * Ny;
                            knot1 = j + (i + 1) * Ny;
                            if ((X[knot0] <= FacePoint) && (X[knot1] > FacePoint))
                            {
                                _Data[0][ch] = U[knot0] + (FacePoint - X[knot0]) * (U[knot1] - U[knot0]) / (X[knot1] - X[knot0]);
                                _Data[1][ch] = V[knot0] + (FacePoint - X[knot0]) * (V[knot1] - V[knot0]) / (X[knot1] - X[knot0]);
                                _Data[2][ch] = P[knot0] + (FacePoint - X[knot0]) * (P[knot1] - P[knot0]) / (X[knot1] - X[knot0]);
                                _DataX[ch] = Y[knot0] + (FacePoint - X[knot0]) * (Y[knot1] - Y[knot0]) / (X[knot1] - X[knot0]);
                                //
                                if (K != null && E != null && S != null && Nu != null)
                                {
                                    _Data[3][ch] = E[knot0] + (FacePoint - X[knot0]) * (E[knot1] - E[knot0]) / (X[knot1] - X[knot0]);
                                    _Data[4][ch] = K[knot0] + (FacePoint - X[knot0]) * (K[knot1] - K[knot0]) / (X[knot1] - X[knot0]);
                                    _Data[5][ch] = S[knot0] + (FacePoint - X[knot0]) * (S[knot1] - S[knot0]) / (X[knot1] - X[knot0]);
                                    _Data[6][ch] = Nu[knot0] + (FacePoint - X[knot0]) * (Nu[knot1] - Nu[knot0]) / (X[knot1] - X[knot0]);
                                    _Data[7][ch] = Tau[knot0] + (FacePoint - X[knot0]) * (Tau[knot1] - Tau[knot0]) / (X[knot1] - X[knot0]);
                                    _Data[8][ch] = Pk[knot0] + (FacePoint - X[knot0]) * (Pk[knot1] - Pk[knot0]) / (X[knot1] - X[knot0]);
                                }
                                //
                                ch++;
                                break;
                            }
                        }

                    }
                    #region  выборка экспериментальных точек
                    //  [b   ...   e)
                    if (flagExp)
                    {
                        int b = -1, e = -1;
                        for (int i = 0; i < ExpX.Length - 1; i++)
                        {
                            if (FacePoint >= ExpX[i] && FacePoint <= ExpX[i + 1])
                            {
                                if (FacePoint - ExpX[i] < ExpX[i + 1] - FacePoint)
                                    e = i + 1;
                                else
                                    b = i + 1;
                                break;
                            }
                        }
                        //
                        if (e == -1 && b != -1)
                        {
                            double Expb = ExpX[b];
                            for (int i = b + 1; i < ExpX.Length; i++)
                            {
                                if (ExpX[i] == Expb)
                                    continue;
                                else
                                {
                                    e = i;
                                    break;
                                }

                            }
                        }
                        else if (b == -1 && e != -1)
                        {
                            double Expe = ExpX[e - 1];
                            for (int i = e - 1; i >= 0; i--)
                            {
                                if (ExpX[i] == Expe)
                                    continue;
                                else
                                {
                                    b = i + 1;
                                    break;
                                }

                            }
                        }
                        int count = e - b;
                        if (count > 0)
                        {
                            ExpDataX = new double[count];
                            ExpData = new double[7][];
                            ExpData[0] = new double[count];
                            ExpData[1] = new double[count];
                            ExpData[2] = new double[count];
                            ExpData[3] = new double[count];
                            ExpData[4] = new double[count];
                            ExpData[5] = new double[count];
                            ExpData[6] = new double[count];
                            //
                            int ch1 = 0;
                            for (int i = b; i < e; i++)
                            {
                                ExpDataX[ch1] = ExpY[i];
                                ExpData[0][ch1] = Exp[0][i];
                                ExpData[1][ch1] = Exp[1][i];
                                ExpData[2][ch1] = Exp[2][i];
                                ExpData[3][ch1] = Exp[3][i];
                                ExpData[4][ch1] = Exp[4][i];
                                ExpData[5][ch1] = Exp[5][i];
                                ExpData[6][ch1] = Exp[6][i];
                                ch1++;
                            }
                        }
                        else
                        {
                            ExpDataX = null;
                            ExpData = null;
                        }
                    }
                    #endregion
                }
                //
                if (_Names == null)
                {
                    _Names = new string[CountLines];
                    _Names[0] = "U";
                    _Names[1] = "V";
                    _Names[2] = "P";
                    if (K != null && E != null && S != null && Nu != null && Tau != null)
                    {
                        _Names[3] = "E";
                        _Names[4] = "K";
                        _Names[5] = "S";
                        _Names[6] = "Nu";
                        _Names[7] = "Tau";
                        _Names[8] = "Pk";
                    }
                }
            }
            else
            {
                if (FacePoint <= Ny * dy)
                {
                    _Data = new double[CountLines][];
                    for (int i = 0; i < CountLines; i++)
                        _Data[i] = new double[Nx];
                    _DataX = new double[Nx];
                    int ch = 0; int knot0 = 0, knot1 = 0;
                    for (int i = 0; i < Nx; i++)
                    {
                        for (int j = 0; j < Ny - 1; j++)
                        {
                            knot0 = i * Ny + j;
                            knot1 = i * Ny + j + 1;
                            if ((Y[knot0] <= FacePoint) && (Y[knot1] >= FacePoint))
                            {
                                _Data[0][ch] = U[knot0] + (FacePoint - Y[knot0]) * (U[knot1] - U[knot0]) / (Y[knot1] - Y[knot0]);
                                _Data[1][ch] = V[knot0] + (FacePoint - Y[knot0]) * (V[knot1] - V[knot0]) / (Y[knot1] - Y[knot0]);
                                _Data[2][ch] = P[knot0] + (FacePoint - Y[knot0]) * (P[knot1] - P[knot0]) / (Y[knot1] - Y[knot0]);
                                _DataX[ch] = X[knot0] + (FacePoint - Y[knot0]) * (X[knot1] - X[knot0]) / (Y[knot1] - Y[knot0]);
                                //
                                if (K != null && E != null && S != null && Nu != null)
                                {
                                    _Data[3][ch] = E[knot0] + (FacePoint - Y[knot0]) * (E[knot1] - E[knot0]) / (Y[knot1] - Y[knot0]);
                                    _Data[4][ch] = K[knot0] + (FacePoint - Y[knot0]) * (K[knot1] - K[knot0]) / (Y[knot1] - Y[knot0]);
                                    _Data[5][ch] = S[knot0] + (FacePoint - Y[knot0]) * (S[knot1] - S[knot0]) / (Y[knot1] - Y[knot0]);
                                    _Data[6][ch] = Nu[knot0] + (FacePoint - Y[knot0]) * (Nu[knot1] - Nu[knot0]) / (Y[knot1] - Y[knot0]);
                                    _Data[7][ch] = Tau[knot0] + (FacePoint - Y[knot0]) * (Tau[knot1] - Tau[knot0]) / (Y[knot1] - Y[knot0]);
                                    _Data[8][ch] = Pk[knot0] + (FacePoint - Y[knot0]) * (Pk[knot1] - Pk[knot0]) / (Y[knot1] - Y[knot0]);
                                }
                                //
                                ch++;
                                break;
                            }
                        }

                    }

                    #region  выборка экспериментальных точек
                    if (flagExp)
                    {
                        int[] Nums = new int[ExpX.Length];
                        int cch = 0;
                        //  [b   ...   e)
                    
                        for (int i = 0; i < ExpY.Length - 1; i++)
                        {
                            if (FacePoint >= ExpY[i] && FacePoint <= ExpY[i + 1])
                            {
                                if (FacePoint - ExpY[i] < ExpY[i + 1] - FacePoint)
                                    Nums[cch++] = i;
                                else
                                    Nums[cch++] = i + 1;
                                
                            }
                        }
                        
                        if (cch != 0)
                        {
                            ExpDataX = new double[cch];
                            ExpData = new double[7][];
                            ExpData[0] = new double[cch];
                            ExpData[1] = new double[cch];
                            ExpData[2] = new double[cch];
                            ExpData[3] = new double[cch];
                            ExpData[4] = new double[cch];
                            ExpData[5] = new double[cch];
                            ExpData[6] = new double[cch];
                            //
                            int ch1 = 0;
                            for (int i = 0; i < cch; i++)
                            {
                                ch1 = Nums[i];
                                ExpDataX[i] = ExpX[ch1];
                                ExpData[0][i] = Exp[0][ch1];
                                ExpData[1][i] = Exp[1][ch1];
                                ExpData[2][i] = Exp[2][ch1];
                                ExpData[3][i] = Exp[3][ch1];
                                ExpData[4][i] = Exp[4][ch1];
                                ExpData[5][i] = Exp[5][ch1];
                                ExpData[6][i] = Exp[6][ch1];
                            }
                        }
                        else
                        {
                            ExpDataX = null;
                            ExpData = null;
                        }
                    }
                    #endregion
                }
            }
            //
            CountLines = _Data.Length;
            DataDimen = new double[CountLines][];
            for (int i = 0; i < CountLines; i++)
            {
                DataDimen[i] = new double[_Data[i].Length];
                for (int j = 0; j < _Data[i].Length; j++)
                    DataDimen[i][j] = _Data[i][j];
            }
        }
        //
        void CreateData(double FacePoint)
        {
            int Count = 0;
            if (XY)
            {
                if (FacePoint <= Nx * dx)
                {
                    int N = (int)Math.Round((FacePoint / dx), MidpointRounding.AwayFromZero);
                    Count = 3;
                    if (K != null && E != null && S != null && Nu != null && Tau != null)
                        Count = 9;
                    if (up_parts != null)
                    {
                        if (up_parts[0] != null)
                            Count += 5;
                    }
                    _Data = new double[Count][];
                    for (int i = 0; i < Count; i++)
                        _Data[i] = new double[Ny];
                    for (int j = 0; j < Ny; j++)
                    {
                        _Data[0][j] = U[N * Ny + j];
                        _Data[1][j] = V[N * Ny + j];
                        _Data[2][j] = P[N * Ny + j];
                    }
                    //
                    if (K != null && E != null && S != null && Nu != null && Tau != null)
                    {
                        for (int j = 0; j < Ny; j++)
                        {
                            _Data[3][j] = E[N * Ny + j];
                            _Data[4][j] = K[N * Ny + j];
                            _Data[5][j] = S[N * Ny + j];
                            _Data[6][j] = Nu[N * Ny + j];
                            _Data[7][j] = Tau[N * Ny + j];
                            _Data[8][j] = Pk[N * Ny + j];
                        }
                    }

                    _DataX = new double[Ny];
                    for (int i = 0; i < Ny; i++)
                        _DataX[i] = dy * i;

                }

                if (up_parts != null)
                {
                    if (up_parts[0] != null)
                    {
                        _Data[3] = up_parts[0];
                        _Data[4] = up_parts[1];
                        _Data[5] = up_parts[2];
                        _Data[6] = up_parts[3];
                        _Data[7] = up_parts[4];
                    }

                }
            }
            else
            {
                if (FacePoint <= Ny * dy)
                {
                    Count = 3;
                    if (K != null && E != null && S != null && Nu != null)
                        Count = 9;
                    int N = (int)(FacePoint / dy);
                    _Data = new double[Count][];
                    for (int i = 0; i < Count; i++)
                        _Data[i] = new double[Nx];
                    for (int j = 0; j < Nx; j++)
                    {
                        _Data[0][j] = U[N + j * Ny];
                        _Data[1][j] = V[N + j * Ny];
                        _Data[2][j] = P[N + j * Ny];
                    }
                    if (K != null && E != null && S != null && Nu != null)
                    {
                        for (int j = 0; j < Nx; j++)
                        {
                            _Data[3][j] = E[N + j * Ny];
                            _Data[4][j] = K[N + j * Ny];
                            _Data[5][j] = S[N + j * Ny];
                            _Data[6][j] = Nu[N + j * Ny];
                            _Data[7][j] = Tau[N + j * Ny];
                            _Data[8][j] = Pk[N + j * Ny];
                        }
                    }
                    _DataX = new double[Nx];
                    for (int i = 0; i < Nx; i++)
                        _DataX[i] = dx * i;
                }
            }
            //
            CountLines = _Data.Length;
            DataDimen = new double[CountLines][];
            for (int i = 0; i < CountLines; i++)
            {
                DataDimen[i] = new double[_Data[i].Length];
                for (int j = 0; j < _Data[i].Length; j++)
                    DataDimen[i][j] = _Data[i][j];
            }
            if (flagExp)
            {
                int b = -1, e = -1;
                for (int i = 0; i < ExpX.Length - 1; i++)
                {
                    if (FacePoint >= ExpX[i] && FacePoint <= ExpX[i + 1])
                    {
                        if (FacePoint - ExpX[i] < ExpX[i + 1] - FacePoint)
                            e = i + 1;
                        else
                            b = i + 1;
                        break;
                    }
                }
                //
                if (e == -1 && b != -1)
                {
                    double Expb = ExpX[b];
                    for (int i = b + 1; i < ExpX.Length; i++)
                    {
                        if (ExpX[i] == Expb)
                            continue;
                        else
                        {
                            e = i;
                            break;
                        }

                    }
                }
                else if (b == -1 && e != -1)
                {
                    double Expe = ExpX[e - 1];
                    for (int i = e - 1; i >= 0; i--)
                    {
                        if (ExpX[i] == Expe)
                            continue;
                        else
                        {
                            b = i + 1;
                            break;
                        }

                    }
                }
                int count = e - b;
                if (count > 0)
                {
                    ExpDataX = new double[count];
                    ExpData = new double[7][];
                    ExpData[0] = new double[count];
                    ExpData[1] = new double[count];
                    ExpData[2] = new double[count];
                    ExpData[3] = new double[count];
                    ExpData[4] = new double[count];
                    ExpData[5] = new double[count];
                    ExpData[6] = new double[count];
                    //
                    int ch1 = 0;
                    for (int i = b; i < e; i++)
                    {
                        ExpDataX[ch1] = ExpY[i];
                        ExpData[0][ch1] = Exp[0][i];
                        ExpData[1][ch1] = Exp[1][i];
                        ExpData[2][ch1] = Exp[2][i];
                        ExpData[3][ch1] = Exp[3][i];
                        ExpData[4][ch1] = Exp[4][i];
                        ExpData[5][ch1] = Exp[5][i];
                        ExpData[6][ch1] = Exp[6][i];
                        ch1++;
                    }
                }
                else
                {
                    ExpDataX = null;
                    ExpData = null;
                }
            }
        }
        void CreateDataBwardStep(double FacePoint)
        {
            if (XY)
            {
                if (FacePoint <= Nx * dx)
                {
                    int N = (int)Math.Round((FacePoint / dx), MidpointRounding.AwayFromZero);
                    int Count = 3;
                    if (up_parts != null)
                    {
                        if(up_parts[0] != null)
                            Count = 8;
                    }
                    if (N < Nx2)
                    {
                        _Data = new double[Count][];
                        for (int i = 0; i < Count; i++)
                            _Data[i] = new double[Ny2];
                        for (int j = 0; j < Ny2; j++)
                        {
                            _Data[0][j] = U[N * Ny2 + j];
                            _Data[1][j] = V[N * Ny2 + j];
                            _Data[2][j] = P[N * Ny2 + j];
                        }
                        double H2 = (Ny - Ny2) * dy;
                        _DataX = new double[Ny2];
                        for (int i = 0; i < Ny2; i++)
                            _DataX[i] = H2 + dy * i;
                    }
                    else
                    {
                        _Data = new double[Count][];
                        for (int i = 0; i < Count; i++)
                            _Data[i] = new double[Ny];
                        int n = (Nx2 - 1) * Ny2 + (N - Nx2 + 1) * Ny;
                        for (int j = 0; j < Ny; j++)
                        {
                            _Data[0][j] = U[n + j];
                            _Data[1][j] = V[n + j];
                            _Data[2][j] = P[n + j];
                        }
                        _DataX = new double[Ny];
                        for (int i = 0; i < Ny; i++)
                            _DataX[i] = dy * i;
                    }
                }
                if (up_parts != null)
                {
                    if (up_parts[0] != null)
                    {
                        _Data[3] = up_parts[0];
                        _Data[4] = up_parts[1];
                        _Data[5] = up_parts[2];
                        _Data[6] = up_parts[3];
                        _Data[7] = up_parts[4];
                    }
                    
                }
            }
            else
            {
                if (FacePoint <= Ny * dy)
                {
                    int N = (int)(FacePoint / dy);
                    int Ny1 = Ny - Ny2;
                    int Nx1 = Nx - Nx2 + 1;
                    if (N < Ny1)
                    {
                        _Data = new double[3][];
                        for (int i = 0; i < 3; i++)
                            _Data[i] = new double[Nx1];
                        int n = (Nx2 - 1) * Ny2 + N - Ny;
                        for (int j = 0; j < Nx1; j++)
                        {
                            _Data[0][j] = U[n + Ny];
                            _Data[1][j] = V[n + Ny];
                            _Data[2][j] = P[n + Ny];
                            n += Ny;
                        }
                        double L2 = (Nx2 - 1) * dx;
                        _DataX = new double[Nx1];
                        for (int i = 0; i < Nx1; i++)
                            _DataX[i] = L2 + dx * i;
                    }
                    else
                    {
                        _Data = new double[3][];
                        for (int i = 0; i < 3; i++)
                            _Data[i] = new double[Nx];
                        int ny = 0; int n = N - Ny1 - Ny2;
                        for (int j = 0; j < Nx; j++)
                        {
                            if (j < (Nx2 - 1))
                                ny = Ny2;
                            else
                                ny = Ny;
                            _Data[0][j] = U[n + ny];
                            _Data[1][j] = V[n + ny];
                            _Data[2][j] = P[n + ny];
                            n += ny;
                        }
                        _DataX = new double[Nx];
                        for (int i = 0; i < Nx; i++)
                            _DataX[i] = dx * i;
                    }
                    
                }
            }
            CountLines = _Data.Length;
            DataDimen = new double[CountLines][];
            for (int i = 0; i < CountLines; i++)
            {
                DataDimen[i] = new double[_Data[i].Length];
                for (int j = 0; j < _Data[i].Length; j++)
                    DataDimen[i][j] = _Data[i][j];
            }
           
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            flagExp = chExp.Checked;
            Invalidate();
        }

        private void chPaletteLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chPaletteLine.SelectedIndex == 0)
                flagColorLine = true;
            else
                flagColorLine = false;
            Invalidate();
        }

        private void chWidthLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LineSize = Convert.ToInt32(chWidthLine.SelectedItem);
                Invalidate();
            }
            catch { }
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog(this);
            Invalidate();
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            fontDialog2.ShowDialog(this);
            Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void txtPointSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                PointSize = Convert.ToInt32(txtPointSize.Text);
                Invalidate();
            }
            catch
            { }
        }

        private void chMult_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtMult.Text = MultCoeff[chMult.SelectedIndex].ToString();
        }

        private void txtMult_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int k = chMult.SelectedIndex;
                MultCoeff[k] = Convert.ToDouble(txtMult.Text);
                //
                for (int i = Markers[k]; i < Markers[k + 1]; i++)
                {
                    for (int j = 0; j < _Data[i].Length; j++)
                    {
                        DataDimen[i][j] = MultCoeff[k] * _Data[i][j];
                    }
                }
                Invalidate();
            }
            catch
            { }
        }

        private void ChartForm_Shown(object sender, EventArgs e)
        {
            // заполнение массивов обезразмеривающих коэффициентов
            // и заполнение массивов обезразмеренных данных
            CountLines = _Data.Length;
            DataDimen = new double[CountLines][];
            for (int i = 0; i < CountLines; i++)
            {
                DataDimen[i] = new double[_Data[i].Length];
                for (int j = 0; j < _Data[i].Length; j++)
                    DataDimen[i][j] = _Data[i][j];
            }
            // определение, какие линии к какой функции относятся
            List<int> markers = new List<int>();
            markers.Add(0);
            char[] ch = { ' ' };
            string name = "", nameOld = _Names[0].Split(ch)[0];
            chMult.Items.Add("Для функции " + nameOld);
            //
            for (int i = 1; i < CountLines; i++)
            {
                name = _Names[i].Split(ch)[0];
                if (name == nameOld)
                    continue;
                else
                {
                    chMult.Items.Add("Для функции " + name);
                    nameOld = name;
                    markers.Add(i);
                }

            }
            markers.Add(CountLines);
            Markers = markers.ToArray();
            //
            MultCoeff = new double[Markers.Length - 1];
            for (int i = 0; i < MultCoeff.Length; i++)
                MultCoeff[i] = 1;
            //
            chBoxes = new CheckBox[CountLines];
            //
            int S = panel1.Size.Width / 3;
            for (int i = 0; i < CountLines; i++)
            {
                chBoxes[i] = new CheckBox();
                chBoxes[i].Name = _Names[i];
                chBoxes[i].Text = _Names[Markers[i]].Split(ch)[0];
                chBoxes[i].Location = new Point(5+(i % 3) * S, 5+15*(i/3));
                chBoxes[i].Size = new System.Drawing.Size(S-10, 15);
                chBoxes[i].CheckedChanged += CheckBox_CheckedChanged;
                panel1.Controls.Add(chBoxes[i]);
            }
            chBoxes[0].Checked = true;
            chBoxes[1].Checked = true;
            chBoxes[2].Checked = true;

        }

        void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Serialize();
        }

        private void Serialize()
        {
            List<Object> fd = new List<Object>();
            fd.Add(fontDialog1.Font);
            fd.Add(fontDialog2.Font);
            fd.Add(MultCoeff.Length);
            for (int i = 0; i < MultCoeff.Length; i++)
                fd.Add(MultCoeff[i]);
            fd.Add(flagColorLine);
            fd.Add(LineSize);
            fd.Add(flagExp);
            fd.Add(PointSize);
            //
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fStream = new FileStream("./ChartFormSettings.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, fd);
            }
        }
        private void DeSerialize()
        {
            int ch = 0;
            List<Object> fd = new List<Object>();
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fStream = File.OpenRead("./ChartFormSettings.dat"))
            {
                fd = (List<Object>)formatter.Deserialize(fStream);
            }
            fontDialog1.Font = (Font)fd[ch++];
            fontDialog2.Font = (Font)fd[ch++];
            AxisParameters();
            if (MultCoeff.Length == (int)fd[ch++])
            {
                for (int i = 0; i < MultCoeff.Length; i++)
                    MultCoeff[i] = Convert.ToDouble(fd[ch++]);
            }
            flagColorLine = Convert.ToBoolean(fd[ch++]);
            LineSize = Convert.ToInt32(fd[ch++]);
            flagExp = Convert.ToBoolean(fd[ch++]);
            PointSize = Convert.ToInt32(fd[ch++]);
            //
            txtMult_TextChanged(this, new EventArgs());
            Invalidate();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DeSerialize();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double n = Convert.ToDouble(textBox1.Text);
                if (n >= 0)
                {
                    FacePoint = n;
                    if (BackwardStep)
                        CreateDataBwardStep(FacePoint);
                    else
                        //CreateData(FacePoint);
                        CreateDataAcc(FacePoint);
                    Invalidate();
                }
            }
            catch
            {
            }
        }

        private void rY_CheckedChanged(object sender, EventArgs e)
        {
            XY = !rY.Checked;
            if (BackwardStep)
                CreateDataBwardStep(FacePoint);
            else
                CreateDataAcc(FacePoint);
                //CreateData(FacePoint);
        }

        private void rX_CheckedChanged(object sender, EventArgs e)
        {
            //XY = rX.Checked;
            //CreateData(XY, FacePoint);
        }

        private void btnVis_Click(object sender, EventArgs e)
        {
            if (btnVis.Text == "<")
            {
                panel1.Visible = true;
                btnVis.Text = ">";
            }
            else
            {
                panel1.Visible = false;
                btnVis.Text = "<";
            }
        }

        private void chLogX_CheckedChanged(object sender, EventArgs e)
        {
            LogX = chLogX.Checked;
            Invalidate();
            
        }

        private void chLogY_CheckedChanged(object sender, EventArgs e)
        {
            LogY = chLogY.Checked;
            Invalidate();
        }

        private void btn_zoom_Click(object sender, EventArgs e)
        {
            flagZoom = !flagZoom;
            if (flagZoom)
            {
                btn_zoom.BackColor = Color.LightBlue;
                ZoomX = Convert.ToDouble(txtZoomX.Text);
                ZoomY = Convert.ToDouble(txtZoomY.Text);
            }
            else
                btn_zoom.BackColor = Color.GhostWhite;
            //
            Invalidate();
        }
    }
}
