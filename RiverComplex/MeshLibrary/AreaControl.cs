using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MeshLibrary
{
    public partial class AreaControl : UserControl
    {
        alglib.spline1dinterpolant interp = new alglib.spline1dinterpolant();
        DataTable top, bottom;
        DataSet Coords;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public DataSet GetSetDataSet
        {
            get { return Coords; }
            set
            {
                Coords = value;
                if ((Coords.Tables[0] != null) && (Coords.Tables[1] != null))
                {
                    bottom = Coords.Tables[0];
                    top = Coords.Tables[1];
                    dataGridView1.DataSource = bottom;
                }
            }
        }
        //
        double _dx = 0;
        public double dx
        {
            get { return _dx; }
        }
        double L = 2, H = 0.1;
        int[] SelectedIndexes = new int[2];
        public int[] GetSetSelectedIndexes
        {
            get { return SelectedIndexes; }
            set 
            {
                SelectedIndexes = value;
                if (rbtnBottom.Checked)
                    lstSpline.SelectedIndex = SelectedIndexes[0];
                if(rbtnTop.Checked)
                    lstSpline.SelectedIndex = SelectedIndexes[1];
            }
        }
        public double GetSetL
        {
            get { return L; }
            set 
            {
                L = value;
                txtL.Text = L.ToString();
            }
        }
        public double GetSetH
        {
            get { return H; }
            set 
            {
                H = value;
                txtH.Text = H.ToString();
            }
        }
        //
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public DataTable[] GetSetXYCoords
        {
            get
            {
                DataTable[] dt = new DataTable[2];
                dt[0] = Coords.Tables[0];
                dt[1] = Coords.Tables[1];
                return dt;
            }
            set
            {
                Coords.Tables.Clear();
                bottom = Coords.Tables.Add("Bottom");
                top = Coords.Tables.Add("Top");
                //
                DataColumn XB = bottom.Columns.Add("X", typeof(Decimal));
                DataColumn YB = bottom.Columns.Add("Y", typeof(Decimal));
                //
                DataColumn XT = top.Columns.Add("X", typeof(Decimal));
                DataColumn YT = top.Columns.Add("Y", typeof(Decimal));
                //
                DataRow dr;
                for (int i = 0; i < value[0].Rows.Count; i++)
                {
                    dr = bottom.NewRow();
                    dr.BeginEdit();
                    dr.SetField<Decimal>(XB, (decimal)(value[0].Rows[i].ItemArray[0]));
                    dr.SetField<Decimal>(YB, (decimal)(value[0].Rows[i].ItemArray[1]));
                    bottom.Rows.Add(dr);
                }
                //
                for (int i = 0; i < value[0].Rows.Count; i++)
                {
                    dr = top.NewRow();
                    dr.BeginEdit();
                    dr.SetField<Decimal>(XT, (decimal)(value[1].Rows[i].ItemArray[0]));
                    dr.SetField<Decimal>(YT, (decimal)(value[1].Rows[i].ItemArray[1]));
                    top.Rows.Add(dr);
                }
                //
                if (rbtnBottom.Checked)
                    dataGridView1.DataSource = bottom;
                if (rbtnTop.Checked)
                    dataGridView1.DataSource = top;
            }
        }
        public AreaControl()
        {
            InitializeComponent();
            lstSpline.SelectedIndex = 0;
            //
            FillDataSetTable();
            //dataGridView1.Columns[0].Width = 75;
            //dataGridView1.Columns[1].Width = 75;
            //
            Invalidate();
        }

        private void FillDataSetTable()
        {
            Coords = new DataSet("Coordinates");
            //
            bottom = Coords.Tables.Add("Bottom");
            top = Coords.Tables.Add("Top");
            //
            DataColumn XB = bottom.Columns.Add("X", typeof(Decimal));
            DataColumn YB = bottom.Columns.Add("Y", typeof(Decimal));
            //
            DataColumn XT = top.Columns.Add("X", typeof(Decimal));
            DataColumn YT = top.Columns.Add("Y", typeof(Decimal));
            //
            DataRow dr = bottom.NewRow();
            dr.BeginEdit();
            dr.SetField<Decimal>(XB, 0);
            dr.SetField<Decimal>(YB, 0);
            bottom.Rows.Add(dr);
            dr = bottom.NewRow();
            dr.SetField<Decimal>(XB, (decimal)L);
            dr.SetField<Decimal>(YB, 0);
            bottom.Rows.Add(dr);
            dr.EndEdit();
            //
            DataRow dr2 = top.NewRow();
            dr2.BeginEdit();
            dr2.SetField<Decimal>(XT, 0);
            dr2.SetField<Decimal>(YT, (decimal)H);
            top.Rows.Add(dr2);
            dr2 = top.NewRow();
            dr2.SetField<Decimal>(XT, (decimal)L);
            dr2.SetField<Decimal>(YT, (decimal)H);
            top.Rows.Add(dr2);
            dr2.EndEdit();
            //dr.AcceptChanges();
            //dr2.AcceptChanges();
            //
            if(rbtnTop.Checked)
                dataGridView1.DataSource = Coords.Tables[1];
            else
                dataGridView1.DataSource = Coords.Tables[0];

        }
        /// <summary>
        /// Получить массив координат нижней и верхней границы
        /// </summary>
        /// <param name="Nx">количество узлов по X</param>
        /// <returns>Массив, где [0] - BottomY, [1] - TopY, [2] - Bottom and Top X (as they are the same)</returns>
        public double[][] GetInterpolatedCoords(int Nx)
        {
            _dx = L / (Nx - 1);
            //
            //
            double[][] BottomTopYX = new double[3][];
            BottomTopYX[0] = new double[Nx];
            BottomTopYX[1] = new double[Nx];
            BottomTopYX[2] = new double[Nx];
            // x-координаты одинаковые у верхней и нижней границы
            for (int i = 0; i < Nx; i++)
                BottomTopYX[2][i] = _dx * i;
            // y-координаты нижней и верхней границ 
            for (int ch = 0; ch < 2; ch++)
            {
                //получение из таблиц с формы массивов координат
                int RowsCount = Coords.Tables[ch].Rows.Count;
                double[][] xyb = new double[2][];
                xyb[0] = new double[RowsCount];//x
                xyb[1] = new double[RowsCount];//y
                //
                for (int i = 0; i < RowsCount; i++)
                {
                    xyb[0][i] = Convert.ToDouble(Coords.Tables[ch].Rows[i][0]);
                    xyb[1][i] = Convert.ToDouble(Coords.Tables[ch].Rows[i][1]);
                }
                
                //построение сплайна
                switch (SelectedIndexes[ch])
                {
                    case 0:
                        {
                            // если задана прямая линия, то не усложняем ее построение линейным сплайном
                            if (xyb[ch].Length == 2)
                            {
                                //if (ch == 0)
                                //{
                                //    for (int i = 0; i < Nx; i++)
                                //        //BottomTopYX[ch][i] = (0.05 * H - i * 0.05 * H / (Nx - 1)) * Math.Cos((i * dx * 8 * Math.PI) / (xyb[0][0] - xyb[0][1]));
                                //        BottomTopYX[ch][i] = 0.05 * H * Math.Cos((i * dx * 2 * Math.PI) / (xyb[0][0] - xyb[0][1]));
                                //}
                                //else
                                //{
                                //    for (int i = 0; i < Nx; i++)
                                //        BottomTopYX[ch][i] = xyb[1][0];
                                //}
                                for (int i = 0; i < Nx; i++)
                                    BottomTopYX[ch][i] = xyb[ch][0];
                                continue;
                            }
                            break;
                        }
                    case 1://линейный
                        alglib.spline1dbuildlinear(xyb[0], xyb[1], out interp);
                        break;
                    case 2://кубический
                        alglib.spline1dbuildcubic(xyb[0], xyb[1], out interp);
                        break;
                    case 3:// Катмулл-Рома (Эрмитов) 
                        alglib.spline1dbuildcatmullrom(xyb[0], xyb[1], out interp);
                        break;
                    case 4:// Акима
                        alglib.spline1dbuildakima(xyb[0], xyb[1], out interp);
                        break;

                }
                //интерполяция значений в необходимых точках по сплайну
                double x = 0;
                for (int i = 0; i < BottomTopYX[0].Length; i++)
                {
                    BottomTopYX[ch][i] = alglib.spline1dcalc(interp, x);
                    x += _dx;
                }
                
            }
            //
            return BottomTopYX;
        }

        private void rbtnBottom_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Coords.Tables[0];
            lstSpline.SelectedIndex = SelectedIndexes[0];
        }

        private void rbtnTop_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Coords.Tables[1];
            lstSpline.SelectedIndex = SelectedIndexes[1];
        }

        private void lstSpline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbtnBottom.Checked)
                SelectedIndexes[0] = lstSpline.SelectedIndex;
            if (rbtnTop.Checked)
                SelectedIndexes[1] = lstSpline.SelectedIndex;

        }

        private void txtH_TextChanged(object sender, EventArgs e)
        {
            try
            {
                H = Convert.ToDouble(txtH.Text);
                if (lstSpline.SelectedIndex == 0)
                    FillDataSetTable();
            }
            catch
            { }
        }

        private void txtL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                L = Convert.ToDouble(txtL.Text);
                if(lstSpline.SelectedIndex==0)
                    FillDataSetTable();
            }
            catch { }
        }

        private void AreaControl_Resize(object sender, EventArgs e)
        {
            //
            dataGridView1.Width = this.Width - 22;
            dataGridView1.Height = (int) (this.Height-160);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
