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
    public partial class ChartForm : Form
    {
       // System.Windows.Forms.DataVisualization.Charting.LabelStyle AxisXStyle = new System.Windows.Forms.DataVisualization.Charting.LabelStyle();
       // System.Windows.Forms.DataVisualization.Charting.LabelStyle AxisYStyle = new System.Windows.Forms.DataVisualization.Charting.LabelStyle();
        /// <summary>
        ///  цветные линии или черно-белые
        /// </summary>
        bool flagColorLine = true;
        /// <summary>
        /// Показывать ли экспериментальные точки
        /// </summary>
        bool flagExp = true;
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
        /// <summary>
        /// Флаг увеличения масштаба по осям
        /// </summary>
        bool flagZoom = false;
        /// <summary>
        /// Масштаб по осям +/-
        /// </summary>
        double ZoomX = 0, ZoomY = 0;
        //
        SeriesChartType LineType = SeriesChartType.Line;
        //
        CheckBox[] chBoxes;
        /// <summary>
        /// количество линий
        /// </summary>
        int CountLines;
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
        /// <summary>
        /// Координаты Y экспериментальных точек
        /// </summary>
        double[][] _ExpDataY;
        /// <summary>
        /// Координаты Y экспериментальных точек
        /// </summary>
        public double[][] ExpDataY
        {
            set 
            {
                _ExpDataY = value;
            }
        }
        /// <summary>
        /// Координаты X экспериментальных точек
        /// </summary>
        double[][] _ExpDataX;
        /// <summary>
        /// Координаты X экспериментальных точек
        /// </summary>
        public double[][] ExpDataX
        {
            set
            {
                _ExpDataX = value;
            }
        }
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
        
        public ChartForm()
        {
            InitializeComponent();
            chPaletteLine.SelectedIndex = 0;
            chWidthLine.SelectedIndex = 1;
            chLineType.DataSource = Enum.GetValues(typeof(SeriesChartType));
            chLineType.SelectedItem = SeriesChartType.Line;
           
            
        }

        private void ChartForm_Resize(object sender, EventArgs e)
        {
            chart1.Width = this.Width - 50;
            chart1.Height = this.Height - 50;
            //
            chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            //
            if (flagZoom)
            {
                chart1.ChartAreas[0].AxisY.ScaleView.Zoom(-ZoomY, ZoomY);
                chart1.ChartAreas[0].AxisX.ScaleView.Zoom(-ZoomX, ZoomX);
            }

            
            //chart1.ChartAreas[0].AxisY.ScaleView.Zoom(-0.1, 0.1);
            //chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
        }

        private void ChartForm_Paint(object sender, PaintEventArgs e)
        {
            
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
                                chart1.Series["Series" + k.ToString()].ChartType = LineType;//SeriesChartType.Line;
                                chart1.Series["Series" + k.ToString()].BorderWidth = LineSize;
                                //пунктир
                                //chart1.Series["Series" + k.ToString()].BorderDashStyle = ChartDashStyle.Dash;
                                //
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
                        chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
                        chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                        chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                        //
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
                                chart1.Series["Series" + k.ToString()].ChartType = LineType;// SeriesChartType.Point;//SeriesChartType.Line;
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
                    chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 0.01;
                    chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                }
            }
            //отрисовка экспериментальных данных
            if (flagExp)
            {
                if ((_ExpDataY != null) && (_ExpDataX != null))
                {
                    int ch = chart1.Series.Count;
                    for (int i = 0; i < _ExpDataY.Length; i++)
                    {
                        chart1.Series.Add("Series" + ch.ToString());
                        chart1.Series["Series" + ch.ToString()].Points.DataBindXY(_ExpDataX[i], _ExpDataY[i]);
                        chart1.Series[ch].ChartType = SeriesChartType.Point;
                        chart1.Series[ch].Name = _NamesExp[i];
                        chart1.Series[ch].MarkerSize = PointSize;
                        //chart1.Series[ch].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Triangle;
                        //chart1.Series[ch].BorderWidth = 2;
                        ch++;
                    }
                    double[] c2 = C2_Discrepancy(_ExpDataX[0], _ExpDataY[0], _DataX, DataDimen[8]);
                    StripStatuslbl_C2.Text = "C2 Discrepancy " + _Names[0] + ": Average - " + c2[0].ToString("G4", System.Globalization.CultureInfo.InvariantCulture) +
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
            double val = 0;
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
                    if ((X[j] <= x_curr) && (X[j + 1] >= x_curr))
                    {
                        CalcF[i] = F[j] + (x_curr - X[j]) * (F[j + 1] - F[j]) / (X[j + 1] - X[j]);
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
            if (flagZoom)
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
                for (int i = Markers[k]; i < Markers[k+1]; i++)
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
            MultCoeff = new double[Markers.Length-1];
            for (int i = 0; i < MultCoeff.Length; i++)
                MultCoeff[i] = 1;
            //
            chBoxes = new CheckBox[Markers.Length - 1];
            //
            for (int i = 0; i < Markers.Length - 1; i++)
            {
                chBoxes[i] = new CheckBox();
                chBoxes[i].Text = _Names[Markers[i]].Split(ch)[0];
                if (i < 3)
                    chBoxes[i].Location = new Point(5 + i * 75, 5);
                else
                    chBoxes[i].Location = new Point(5 + (i - 3) * 75, 20);
                chBoxes[i].Size = new System.Drawing.Size(65, 15);
                chBoxes[i].CheckedChanged += CheckBox_CheckedChanged;
                panel1.Controls.Add(chBoxes[i]);
                
            }
            chBoxes[0].Checked = true;

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
            int ch=0;
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
            LineSize = (int)(fd[ch++]);
            flagExp = Convert.ToBoolean(fd[ch++]);
            PointSize = (int)(fd[ch++]);
            //
            txtMult_TextChanged(this, new EventArgs());
            Invalidate();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DeSerialize();
        }

        private void btnZoom_Click(object sender, EventArgs e)
        {
            flagZoom = !flagZoom;
            if (flagZoom)
            {
                btnZoom.BackColor = Color.LightBlue;
                ZoomX = Convert.ToDouble(txtZoomX.Text);
                ZoomY = Convert.ToDouble(txtZoomY.Text);
            }
            else
                btnZoom.BackColor = Color.GhostWhite;
            Invalidate();
        }

        private void chLineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LineType = (SeriesChartType)chLineType.SelectedItem;
            Invalidate();
        }
    }
}
