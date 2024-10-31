using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    public class KwollExp
    {
        string error = "ok";
        string path = "..\\..\\..\\HydrodynamicLibrary\\obj\\KwollExp_txt\\";
        string[] Names = { "D10.txt", "D20.txt", "D30.txt", "Tuv_av.txt" };
        public string SetExperimentFilesPath
        {
            set { path = value; }
        }
        /// <summary>
        /// Массив имен файлов, содержащих информацию эксперимента ([0]-10 гр. дюны, [1] - 20, [3] - 30, [4] - 10+20+30 tuv_av)
        /// </summary>
        public string[] SetTxtNames
        {
            set
            {
                if (value.Length > 4)
                    Names = value;
                else
                    error = "Массив имен должен содержать более 4х имен файлов";
            }
        }
        #region Dunes10
        /// <summary>
        /// Horizontal distance from dune brink point  [m]
        /// </summary>
        public double[] x_10;
        /// <summary>
        /// Vertical distance above dune trough [m]
        /// </summary>
        public double[] y_10;
        /// <summary>
        /// Time-averaged streamwise velocity [m/s]
        /// </summary>
        public double[] u_t_10;
        /// <summary>
        /// Time-averaged vertical velocity[m/s]
        /// </summary>
        public double[] v_t_10;
        /// <summary>
        /// Intermittency factor of flow reversal
        /// </summary>
        public double[] IF_10;
        /// <summary>
        /// maximum upstream velocity of reversed flow [m/s]
        /// </summary>
        public double[] u_max_10;
        /// <summary>
        /// Vertical gradient in streamwise velocity [1/s]
        /// </summary>
        public double[] u_dy_10;
        /// <summary>
        /// Principal Reynolds Stress [Pa]
        /// </summary>
        public double[] tuv_10;
        /// <summary>
        /// Turbulence production [m2/s3]
        /// </summary>
        public double[] TP_10;
        /// <summary>
        /// Turbulent kinetic energy [Pa/rho]=[m2/s2]
        /// </summary>
        public double[] TKE_10;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 1 using a HoleSize of 2
        /// </summary>
        public double[] Q1_10;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 2 using a HoleSize of 2
        /// </summary>
        public double[] Q2_10;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 3 using a HoleSize of 2
        /// </summary>
        public double[] Q3_10;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 4 using a HoleSize of 2
        /// </summary>
        public double[] Q4_10;
        /// <summary>
        /// Eddy viscosity [m2/s] by Prandtl model
        /// </summary>
        public double[] nuT_10;
        /// <summary>
        /// turbulent energy dissipation by Prandtl model
        /// </summary>
        public double[] e_10;
        #endregion
        //
        #region Dunes20
        /// <summary>
        /// Horizontal distance from dune brink point  [m]
        /// </summary>
        public double[] x_20;
        /// <summary>
        /// Vertical distance above dune trough [m]
        /// </summary>
        public double[] y_20;
        /// <summary>
        /// Time-averaged streamwise velocity [m/s]
        /// </summary>
        public double[] u_t_20;
        /// <summary>
        /// Time-averaged vertical velocity[m/s]
        /// </summary>
        public double[] v_t_20;
        /// <summary>
        /// Intermittency factor of flow reversal
        /// </summary>
        public double[] IF_20;
        /// <summary>
        /// maximum upstream velocity of reversed flow [m/s]
        /// </summary>
        public double[] u_max_20;
        /// <summary>
        /// Vertical gradient in streamwise velocity [1/s]
        /// </summary>
        public double[] u_dy_20;
        /// <summary>
        /// Principal Reynolds Stress [Pa]
        /// </summary>
        public double[] tuv_20;
        /// <summary>
        /// Turbulence production [m2/s3]
        /// </summary>
        public double[] TP_20;
        /// <summary>
        /// Turbulent kinetic energy [Pa]
        /// </summary>
        public double[] TKE_20;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 1 using a HoleSize of 2
        /// </summary>
        public double[] Q1_20;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 2 using a HoleSize of 2
        /// </summary>
        public double[] Q2_20;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 3 using a HoleSize of 2
        /// </summary>
        public double[] Q3_20;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 4 using a HoleSize of 2
        /// </summary>
        public double[] Q4_20;
        /// <summary>
        /// Eddy viscosity [m2/s] by Prandtl model
        /// </summary>
        public double[] nuT_20;
        /// <summary>
        /// turbulent energy dissipation by Prandtl model
        /// </summary>
        public double[] e_20;
        #endregion
        //
        #region Dunes30
        /// <summary>
        /// Horizontal distance from dune brink point  [m]
        /// </summary>
        public double[] x_30;
        /// <summary>
        /// Vertical distance above dune trough [m]
        /// </summary>
        public double[] y_30;
        /// <summary>
        /// Time-averaged streamwise velocity [m/s]
        /// </summary>
        public double[] u_t_30;
        /// <summary>
        /// Time-averaged vertical velocity[m/s]
        /// </summary>
        public double[] v_t_30;
        /// <summary>
        /// Intermittency factor of flow reversal
        /// </summary>
        public double[] IF_30;
        /// <summary>
        /// maximum upstream velocity of reversed flow [m/s]
        /// </summary>
        public double[] u_max_30;
        /// <summary>
        /// Vertical gradient in streamwise velocity [1/s]
        /// </summary>
        public double[] u_dy_30;
        /// <summary>
        /// Principal Reynolds Stress [Pa]
        /// </summary>
        public double[] tuv_30;
        /// <summary>
        /// Turbulence production [m2/s3]
        /// </summary>
        public double[] TP_30;
        /// <summary>
        /// Turbulent kinetic energy [Pa]
        /// </summary>
        public double[] TKE_30;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 1 using a HoleSize of 2
        /// </summary>
        public double[] Q1_30;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 2 using a HoleSize of 2
        /// </summary>
        public double[] Q2_30;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 3 using a HoleSize of 2
        /// </summary>
        public double[] Q3_30;
        /// <summary>
        /// Percentage of velocity measurements falling into Quadrant 4 using a HoleSize of 2
        /// </summary>
        public double[] Q4_30;
        /// <summary>
        /// Eddy viscosity [m2/s] by Prandtl model
        /// </summary>
        public double[] nuT_30;
        /// <summary>
        /// turbulent energy dissipation by Prandtl model
        /// </summary>
        public double[] e_30;
        #endregion
        //
        #region Double-averaged Reynolds shear stress (all dunes)
        /// <summary>
        /// Spatially-averaged ReynoldsStress for 10° dunes
        /// </summary>
        public double[] tuv_10_av;
        /// <summary>
        /// Vertical distance above dune trough for 10° dunes
        /// </summary>
        public double[] y_10_av;
        /// <summary>
        /// Spatially-averaged ReynoldsStress for 20° dunes
        /// </summary>
        public double[] tuv_20_av;
        /// <summary>
        /// Vertical distance above dune trough for 20° dunes
        /// </summary>
        public double[] y_20_av;
        /// <summary>
        /// Spatially-averaged ReynoldsStress for 30° dunes
        /// </summary>
        public double[] tuv_30_av;
        /// <summary>
        /// Vertical distance above dune trough for 30° dunes
        /// </summary>
        public double[] y_30_av;
        #endregion
        //
        public KwollExp()
        {
            Init();
        }
        public KwollExp(string[] names)
        {
            Names = names;
            Init();
        }
        private void Init()
        {
            //if (!Directory.Exists(path) || !File.Exists(path + Names[0]) || !File.Exists(path + Names[1]) || !File.Exists(path + Names[2]) || !File.Exists(path + Names[3]))
            //{
            //    path = "..\\..\\..\\..\\HydrodynamicLibrary\\obj\\KwollExp_txt\\";
            //    if (!Directory.Exists(path) || !File.Exists(path + Names[0]) || !File.Exists(path + Names[1]) || !File.Exists(path + Names[2]) || !File.Exists(path + Names[3]))
            //    {
            //        error = "Directory is not exist. Change the path to files";
            //        return;
            //    }
            //    FillData_102030(out x_10, out y_10, out u_t_10, out v_t_10, out IF_10, out u_max_10, out u_dy_10, out tuv_10, out TP_10, out TKE_10, out Q1_10, out Q2_10, out Q3_10, out Q4_10, Names[0]);
            //    FillData_102030(out x_20, out y_20, out u_t_20, out v_t_20, out IF_20, out u_max_20, out u_dy_20, out tuv_20, out TP_20, out TKE_20, out Q1_20, out Q2_20, out Q3_20, out Q4_20, Names[1]);
            //    FillData_102030(out x_30, out y_30, out u_t_30, out v_t_30, out IF_30, out u_max_30, out u_dy_30, out tuv_30, out TP_30, out TKE_30, out Q1_30, out Q2_30, out Q3_30, out Q4_30, Names[2]);
            //    FillData_Tuv_av(out y_10_av, out y_20_av, out y_30_av, out tuv_10_av, out tuv_20_av, out tuv_30_av, Names[3]);
                
            //}
            try
            {
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(Names[0]);
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(Names[1]);
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(Names[2]);
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(Names[3]);
            }
            catch (Exception exc)
            {
                error += exc.ToString();
                return;
            }
            //
            ReadStream(out x_10, out y_10, out u_t_10, out v_t_10, out IF_10, out u_max_10, out u_dy_10, out tuv_10, out TP_10, out TKE_10, out Q1_10, out Q2_10, out Q3_10, out Q4_10, Names[0]);
            ReadStream(out x_20, out y_20, out u_t_20, out v_t_20, out IF_20, out u_max_20, out u_dy_20, out tuv_20, out TP_20, out TKE_20, out Q1_20, out Q2_20, out Q3_20, out Q4_20, Names[1]);
            ReadStream(out x_30, out y_30, out u_t_30, out v_t_30, out IF_30, out u_max_30, out u_dy_30, out tuv_30, out TP_30, out TKE_30, out Q1_30, out Q2_30, out Q3_30, out Q4_30, Names[2]);
            ReadStreamAv(out y_10_av, out y_20_av, out y_30_av, out tuv_10_av, out tuv_20_av, out tuv_30_av, Names[3]);
            //
            KE_Prandtl(y_10, TKE_10, out nuT_10, out e_10);
            KE_Prandtl(y_20, TKE_20, out nuT_20, out e_20);
            KE_Prandtl(y_30, TKE_30, out nuT_30, out e_30);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[0] - tuv_10_av, [1] - y_20_av, [2] - y_30_av </returns>
        public alglib.spline1dinterpolant[] InterpolateAv()
        {
            alglib.spline1dinterpolant[] s = new alglib.spline1dinterpolant[3];
            //
            alglib.spline1dbuildcubic(y_10_av, tuv_10_av, out s[0]);
            alglib.spline1dbuildcubic(y_20_av, tuv_20_av, out s[1]);
            alglib.spline1dbuildcubic(y_30_av, tuv_30_av, out s[2]);
            //
            return s;
        }
        //
        void ReadStream(out double[] x, out double[] y, out double[] u_t, out double[] v_t, out double[] IF,
            out double[] u_max, out double[] u_dy, out double[] tuv, out double[] TP, out double[] TKE, out double[] Q1,
            out double[] Q2, out double[] Q3, out double[] Q4, string resName)
        {
            string resNamespace = "HydrodynamicLibrary";
            string resource = resNamespace + "." + resName;
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            StreamReader streamReader = new StreamReader(stream);
            int N = 0;
            while (streamReader.ReadLine() != null)
                N++;
            N--;
            streamReader.Close();
            //
            #region Arrays memory allocation
            x = new double[N];
            y = new double[N];
            u_t = new double[N];
            v_t = new double[N];
            IF = new double[N];
            u_max = new double[N];
            u_dy = new double[N];
            tuv = new double[N];
            TP = new double[N];
            TKE = new double[N];
            Q1 = new double[N];
            Q2 = new double[N];
            Q3 = new double[N];
            Q4 = new double[N];
            #endregion
            //
            try
            {
                stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
                streamReader = new StreamReader(stream);
                streamReader.ReadLine();
                string s; string[] ars = new string[14];
                //double x, y, u_t, v_t, IF, u_max, u_dy, tuv, TP, TKE, Q1, Q2, Q3, Q4;
                for (int i = 0; i < N; i++)
                {
                    #region Чтение и запись в массивы
                    s = streamReader.ReadLine();
                    ars = s.Split(' ');
                    //
                    x[i] = double.Parse(ars[0]);
                    y[i] = double.Parse(ars[1]) * 0.2; // /h h=0.2
                    // с учетом основного уклона дна
                    //y[i] = double.Parse(ars[1]) * 0.2 - 0.00132 * (x[i] - x[0]); // /h h=0.2
                    // + обработка от NaN
                    double tmp = double.Parse(ars[2]);
                    if (tmp != double.NaN)
                        u_t[i] = tmp;
                    else
                        u_t[i] = 0;
                    //
                    tmp = double.Parse(ars[3]);
                    if (tmp != double.NaN)
                        v_t[i] = tmp;
                    else
                        v_t[i] = 0;
                    //
                    tmp = double.Parse(ars[4]);
                    if (tmp != double.NaN)
                        IF[i] = tmp;
                    else
                        IF[i] = 0;
                    //
                    tmp = double.Parse(ars[5]);
                    if (tmp != double.NaN)
                        u_max[i] = tmp;
                    else
                        u_max[i] = 0;
                    //
                    tmp = double.Parse(ars[6]);
                    if (tmp != double.NaN)
                        u_dy[i] = tmp;
                    else
                        u_dy[i] = 0;
                    //
                    tmp = double.Parse(ars[7]);
                    if (tmp != double.NaN)
                        tuv[i] = tmp;
                    else
                        tuv[i] = 0;
                    //
                    tmp = double.Parse(ars[8]);
                    if (tmp != double.NaN)
                        TP[i] = tmp;
                    else
                        TP[i] = 0;
                    //
                    tmp = double.Parse(ars[9]);
                    if (tmp != double.NaN)
                        TKE[i] = tmp / 1000.0; // k=u^2+v^2, TKE = 1/2 rho (u^2+v^2)
                    else
                        TKE[i] = 0;
                    //
                    tmp = double.Parse(ars[10]);
                    if (tmp != double.NaN)
                        Q1[i] = tmp;
                    else
                        Q1[i] = 0;
                    //
                    tmp = double.Parse(ars[11]);
                    if (tmp != double.NaN)
                        Q2[i] = tmp;
                    else
                        Q2[i] = 0;
                    //
                    tmp = double.Parse(ars[12]);
                    if (tmp != double.NaN)
                        Q3[i] = tmp;
                    else
                        Q3[i] = 0;
                    //
                    tmp = double.Parse(ars[13]);
                    if (tmp != double.NaN)
                        Q4[i] = tmp;
                    else
                        Q4[i] = 0;
                    #endregion
                }
                //
            }
            catch (Exception exc)
            {
                error += exc.Message;
            }
            //
        }
        private void ReadStreamAv(out double[] y10, out double[] y20, out double[] y30, out double[] tuv10,
            out double[] tuv20, out double[] tuv30, string resName)
        {
            string resNamespace = "HydrodynamicLibrary";
            string resource = resNamespace + "." + resName;
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            StreamReader streamReader = new StreamReader(stream);
            int N = 0;
            while (streamReader.ReadLine() != null)
                N++;
            N--;
            streamReader.Close();
            //
            #region Arrays memory allocation
            y10 = new double[N];
            y20 = new double[N];
            y30 = new double[N];
            tuv10 = new double[N];
            tuv20 = new double[N];
            tuv30 = new double[N];
            #endregion
            //
            try
            {
                stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
                streamReader = new StreamReader(stream);
                streamReader.ReadLine();
                string s; string[] ars = new string[6];
                for (int i = 0; i < N; i++)
                {
                    s = streamReader.ReadLine();
                    ars = s.Split(' ');
                    //
                    tuv10[i] = double.Parse(ars[0]);
                    y10[i] = double.Parse(ars[1]) * 0.2;
                    tuv20[i] = double.Parse(ars[2]);
                    y20[i] = double.Parse(ars[3]) * 0.2;
                    tuv30[i] = double.Parse(ars[4]);
                    y30[i] = double.Parse(ars[5]) * 0.2;
                }
            }
            catch (Exception exc)
            {
                error += exc.Message;
            }
        }
        /////////////////////////// не рабочий код, так как область должна быть прямоугольной для этой библиотеки, а она нет
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns> [0] - u_t_10,[1] - v_t_10, [2] - IF_10, [3] - u_max_10, [4] -  u_dy_10, [5] - tuv_10, [6] - TP_10, [7] -  TKE_10, [8] -  Q1_10, [9] - Q2_10, [10] - Q3_10, [11] - Q4_10</returns>
        //public alglib.spline2dinterpolant[] Interpolate10()
        //{
        //    return Interpolate(x_10, y_10, u_t_10, v_t_10, IF_10, u_max_10, u_dy_10, tuv_10, TP_10, TKE_10, Q1_10, Q2_10, Q3_10, Q4_10);
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns>[0] - u_t_20,[1] - v_t_20, [2] - IF_20, [3] - u_max_20, [4] -  u_dy_20, [5] - tuv_20, [6] - TP_20, [7] -  TKE_20, [8] -  Q1_20, [9] - Q2_20, [20] - Q3_20, [11] - Q4_20</returns>
        //public alglib.spline2dinterpolant[] Interpolate20()
        //{
        //    return Interpolate(x_20, y_20, u_t_20, v_t_20, IF_20, u_max_20, u_dy_20, tuv_20, TP_20, TKE_20, Q1_20, Q2_20, Q3_20, Q4_20);
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns>[0] - u_t_30,[1] - v_t_30, [2] - IF_30, [3] - u_max_30, [4] - u_dy_30, [5] - tuv_30, [6] - TP_30, [7] -  TKE_30, [8] -  Q1_30, [9] - Q2_30, [30] - Q3_30, [11] - Q4_30</returns>
        //public alglib.spline2dinterpolant[] Interpolate30()
        //{
        //    return Interpolate(x_30, y_30, u_t_30, v_t_30, IF_30, u_max_30, u_dy_30, tuv_30, TP_30, TKE_30, Q1_30, Q2_30, Q3_30, Q4_30);
        //}
       
        ///// <summary>
        ///// Метод получения интерполяций всех параметров определенной волны
        ///// </summary>
        ///// <param name="x">х-координаты точек, в которых заданы функции</param>
        ///// <param name="y">y-координаты точек, в которых заданы функции</param>
        ///// <param name="u_t">Time-averaged streamwise velocity [m/s]</param>
        ///// <param name="v_t">Time-averaged vertical velocity[m/s]</param>
        ///// <param name="IF">Intermittency factor of flow reversal</param>
        ///// <param name="u_max">Maximum upstream velocity of reversed flow [m/s]</param>
        ///// <param name="u_dy">Vertical gradient in streamwise velocity [1/s]</param>
        ///// <param name="tuv">Principal Reynolds Stress [Pa]</param>
        ///// <param name="TP">Turbulence production [m2/s3]</param>
        ///// <param name="TKE">Turbulent kinetic energy [Pa]</param>
        ///// <param name="Q1">Percentage of velocity measurements falling into Quadrant 1 using a HoleSize of 2</param>
        ///// <param name="Q2">Percentage of velocity measurements falling into Quadrant 2 using a HoleSize of 2</param>
        ///// <param name="Q3">Percentage of velocity measurements falling into Quadrant 3 using a HoleSize of 2</param>
        ///// <param name="Q4">Percentage of velocity measurements falling into Quadrant 4 using a HoleSize of 2</param>
        ///// <returns> массив сгенерированных интероляций, функции в массиве в том же порядке, что и параметры метода. Получение значения в искомой точке - alglib.spline2dcalc</returns>
        //private alglib.spline2dinterpolant[] Interpolate(double[] x, double[] y, double[] u_t, double[] v_t, double[] IF,
        //    double[] u_max, double[] u_dy, double[] tuv, double[] TP, double[] TKE, double[] Q1, double[] Q2, double[] Q3, double[] Q4)
        //{
        //    //
        //    alglib.spline2dinterpolant[] s = new alglib.spline2dinterpolant[12];
        //    //
        //    int Nx = 0, Ny = 0; int k = 0;
        //    while (y[k + 1] > y[k])
        //    {
        //        Ny++; k++;
        //    }
        //    Ny++; Nx = x_10.Length / Ny;
        //    //
        //    double[] x1 = new double[Nx];
        //    double[] y1 = new double[Ny];
        //    for (int j = 0; j < Nx; j++)
        //        x1[j] = x[j];
        //    for (int j = 0; j < Ny; j++)
        //        y1[j] = y[j];
        //    //
        //    int[][] map = new int[Nx][];
        //    int e = 0;
        //    for (int i = 0; i < Nx; i++)
        //    {
        //        map[i] = new int[Ny];
        //        for (int j = 0; j < Ny; j++)
        //            map[i][j] = e++;
        //    }
        //    //
        //    double[,] _u_t = new double[Ny, Nx];
        //    double[,] _v_t = new double[Ny, Nx];
        //    double[,] _IF = new double[Ny, Nx];
        //    double[,] _u_max = new double[Ny, Nx];
        //    double[,] _u_dy = new double[Ny, Nx];
        //    double[,] _tuv = new double[Ny, Nx];
        //    double[,] _TP = new double[Ny, Nx];
        //    double[,] _TKE = new double[Ny, Nx];
        //    double[,] _Q1 = new double[Ny, Nx];
        //    double[,] _Q2 = new double[Ny, Nx];
        //    double[,] _Q3 = new double[Ny, Nx];
        //    double[,] _Q4 = new double[Ny, Nx];
        //    //
        //    OrderablePartitioner<Tuple<int, int>> rangePartitioner3 = Partitioner.Create(0, Ny);
        //    Parallel.ForEach(rangePartitioner3,
        //            (range, loopState) =>
        //            {
        //                for (int j = range.Item1; j < range.Item2; j++)
        //                {
        //                    for (int i = 0; i < Nx; i++)
        //                    {
        //                        //
        //                        _u_t[j, i] = u_t[map[i][j]];
        //                        _v_t[j, i] = v_t[map[i][j]];
        //                        _IF[j, i] = IF[map[i][j]];
        //                        _u_max[j, i] = u_max[map[i][j]];
        //                        _u_dy[j, i] = u_dy[map[i][j]];
        //                        _tuv[j, i] = tuv[map[i][j]];
        //                        _TP[j, i] = TP[map[i][j]];
        //                        _TKE[j, i] = TKE[map[i][j]];
        //                        _Q1[j, i] = Q1[map[i][j]];
        //                        _Q2[j, i] = Q2[map[i][j]];
        //                        _Q3[j, i] = Q3[map[i][j]];
        //                        _Q4[j, i] = Q4[map[i][j]];
        //                    }
        //                }
        //            });
        //    //
        //    //    Input parameters:
        //    //    X   -   spline abscissas, array[0..N-1]
        //    //    Y   -   spline ordinates, array[0..M-1]
        //    //    F   -   function values, array[0..M-1,0..N-1]
        //    //    M,N -   grid size, M>=2, N>=2
        //    //
        //    //Output parameters:
        //    //    C   -   spline interpolant
        //    //
        //    alglib.spline2dbuildbicubic(x1, y1, _u_t, Ny, Nx, out s[0]);
        //    alglib.spline2dbuildbicubic(x1, y1, _v_t, Ny, Nx, out s[1]);
        //    alglib.spline2dbuildbicubic(x1, y1, _IF, Ny, Nx, out s[2]);
        //    alglib.spline2dbuildbicubic(x1, y1, _u_max, Ny, Nx, out s[3]);
        //    alglib.spline2dbuildbicubic(x1, y1, _u_dy, Ny, Nx, out s[4]);
        //    alglib.spline2dbuildbicubic(x1, y1, _tuv, Ny, Nx, out s[5]);
        //    alglib.spline2dbuildbicubic(x1, y1, _TP, Ny, Nx, out s[6]);
        //    alglib.spline2dbuildbicubic(x1, y1, _TKE, Ny, Nx, out s[7]);
        //    alglib.spline2dbuildbicubic(x1, y1, _Q1, Ny, Nx, out s[8]);
        //    alglib.spline2dbuildbicubic(x1, y1, _Q2, Ny, Nx, out s[9]);
        //    alglib.spline2dbuildbicubic(x1, y1, _Q3, Ny, Nx, out s[10]);
        //    alglib.spline2dbuildbicubic(x1, y1, _Q4, Ny, Nx, out s[11]);
        //    //
        //    return s;
        //}
        private void FillData_102030(out double[] x, out double[] y, out double[] u_t, out double[] v_t, out double[] IF,
            out double[] u_max, out double[] u_dy, out double[] tuv, out double[] TP, out double[] TKE, out double[] Q1,
            out double[] Q2, out double[] Q3, out double[] Q4, string Name)
        {
            StreamReader streamReader = new StreamReader(path + Name);
            int N = 0;
            while (streamReader.ReadLine() != null)
                N++;
            N--;
            streamReader.Close();
            //
            #region Arrays memory allocation
            x = new double[N];
            y = new double[N];
            u_t = new double[N];
            v_t = new double[N];
            IF = new double[N];
            u_max = new double[N];
            u_dy = new double[N];
            tuv = new double[N];
            TP = new double[N];
            TKE = new double[N];
            Q1 = new double[N];
            Q2 = new double[N];
            Q3 = new double[N];
            Q4 = new double[N];
            #endregion
            //
            try
            {
                streamReader = new StreamReader(path + Name);
                streamReader.ReadLine();
                string s; string[] ars = new string[14];
                //double x, y, u_t, v_t, IF, u_max, u_dy, tuv, TP, TKE, Q1, Q2, Q3, Q4;
                for (int i = 0; i < N; i++)
                {
                    #region Чтение и запись в массивы
                    s = streamReader.ReadLine();
                    ars = s.Split(' ');
                    //
                    x[i] = double.Parse(ars[0]);
                    y[i] = double.Parse(ars[1]) * 0.2 - 0.00132*(x[i]-x[0]); // /h h=0.2
                    // + обработка от NaN
                    double tmp = double.Parse(ars[2]);
                    if (tmp != double.NaN)
                        u_t[i] = tmp;
                    else
                        u_t[i] = 0;
                    //
                    tmp = double.Parse(ars[3]);
                    if(tmp!=double.NaN)
                        v_t[i]=tmp;
                    else
                        v_t[i] = 0;
                    //
                    tmp = double.Parse(ars[4]);
                    if (tmp != double.NaN)
                        IF[i] = tmp;
                    else
                        IF[i] = 0;
                    //
                    tmp = double.Parse(ars[5]);
                    if (tmp != double.NaN)
                        u_max[i] = tmp;
                    else
                        u_max[i] = 0;
                    //
                    tmp = double.Parse(ars[6]);
                    if (tmp != double.NaN)
                        u_dy[i] = tmp;
                    else
                        u_dy[i] = 0;
                    //
                    tmp = double.Parse(ars[7]);
                    if (tmp != double.NaN)
                        tuv[i] = tmp;
                    else
                        tuv[i] = 0;
                    //
                    tmp = double.Parse(ars[8]);
                    if (tmp != double.NaN)
                        TP[i] = tmp;
                    else
                        TP[i] = 0;
                    //
                    tmp = double.Parse(ars[9]);
                    if (tmp != double.NaN)
                        TKE[i] = tmp / 1000.0; // k=u^2+v^2, TKE = 1/2 rho (u^2+v^2)
                    else
                        TKE[i] = 0;
                    //
                    tmp = double.Parse(ars[10]);
                    if (tmp != double.NaN)
                        Q1[i] = tmp;
                    else
                        Q1[i] = 0;
                    //
                    tmp = double.Parse(ars[11]);
                    if (tmp != double.NaN)
                        Q2[i] = tmp;
                    else
                        Q2[i] = 0;
                    //
                    tmp = double.Parse(ars[12]);
                    if (tmp != double.NaN)
                        Q3[i] = tmp;
                    else
                        Q3[i] = 0;
                    //
                    tmp = double.Parse(ars[13]);
                    if (tmp != double.NaN)
                        Q4[i] = tmp;
                    else
                        Q4[i] = 0;
                    #endregion
                }
                //
            }
            catch (Exception exc)
            {
                error += exc.Message;
            }
            //
        }
        private void FillData_Tuv_av(out double[] y10, out double[] y20, out double[] y30, out double[] tuv10,
            out double[] tuv20, out double[] tuv30, string Name)
        {
            StreamReader streamReader = new StreamReader(path + Name);
            int N = 0;
            while (streamReader.ReadLine() != null)
                N++;
            N--;
            streamReader.Close();
            //
            #region Arrays memory allocation
            y10 = new double[N];
            y20 = new double[N];
            y30 = new double[N];
            tuv10 = new double[N];
            tuv20 = new double[N];
            tuv30 = new double[N];
            #endregion
            //
            try
            {
                streamReader = new StreamReader(path + Name);
                streamReader.ReadLine();
                string s; string[] ars = new string[6];
                for (int i = 0; i < N; i++)
                {
                    s = streamReader.ReadLine();
                    ars = s.Split(' ');
                    //
                    tuv10[i] = double.Parse(ars[0]);
                    y10[i] = double.Parse(ars[1]) * 0.2;
                    tuv20[i] = double.Parse(ars[2]);
                    y20[i] = double.Parse(ars[3]) * 0.2;
                    tuv30[i] = double.Parse(ars[4]);
                    y30[i] = double.Parse(ars[5]) * 0.2;
                }
            }
            catch (Exception exc)
            {
                error += exc.Message;
            }
        }
        public void SlipData10(double slip10, out double[] x10)
        {
            if (x_10 == null)
                ReadStream(out x_10, out y_10, out u_t_10, out v_t_10, out IF_10, out u_max_10, out u_dy_10, out tuv_10, out TP_10, out TKE_10, out Q1_10, out Q2_10, out Q3_10, out Q4_10, Names[0]);
            x10 = new double[x_10.Length];
            for (int i = 0; i < x10.Length; i++)
                x10[i] = x_10[i] + slip10;
        }
        //
        public void SlipData20(double slip20, out double[] x20)
        {
            if (x_20 == null)
                ReadStream(out x_20, out y_20, out u_t_20, out v_t_20, out IF_20, out u_max_20, out u_dy_20, out tuv_20, out TP_20, out TKE_20, out Q1_20, out Q2_20, out Q3_20, out Q4_20, Names[0]);
            x20 = new double[x_20.Length];
            for (int i = 0; i < x20.Length; i++)
                x20[i] = x_20[i] + slip20;
        }
        //
        public void SlipData30(double slip30, out double[] x30)
        {
            if (x_30 == null)
                ReadStream(out x_30, out y_30, out u_t_30, out v_t_30, out IF_30, out u_max_30, out u_dy_30, out tuv_30, out TP_30, out TKE_30, out Q1_30, out Q2_30, out Q3_30, out Q4_30, Names[0]);
            x30 = new double[x_30.Length];
            for (int i = 0; i < x30.Length; i++)
                x30[i] = x_30[i] + slip30;
        }
        //
        public void BedData_(out double[] x, out double[] tau, out double[] k, double [] x_, double[] tuv_, double [] TKE_)
        {
            double[] tx = new double[x_.Length];
            double[] ttau = new double[x_.Length];
            double[] tk = new double[x_.Length];
            //
            double tmpX=0;
            int ch=0;
            for (int i = 0; i < x_.Length; i++)
            {
                if (x_[i] != tmpX)
                {
                    tmpX = x_[i];
                    tx[ch] = x_[i];
                    if(!double.IsNaN(tuv_[i]))
                        ttau[ch] = tuv_[i];
                    if (!double.IsNaN(TKE_[i]))
                        tk[ch] = TKE_[i];
                    ch++;
                }
            }
            //
            x = new double[ch];
            tau = new double[ch];
            k = new double[ch];
            //
            for (int i = 0; i < ch; i++)
            {
                x[i] = tx[i];
                k[i] = tk[i];
                tau[i] = ttau[i];
            }

        }
        public void BedData10(out double[] x, out double[] tau, out double[] k)
        {
            BedData_(out x, out tau, out k, x_10, tuv_10, TKE_10);
        }
        public void BedData20(out double[] x, out double[] tau, out double[] k)
        {
            BedData_(out x, out tau, out k, x_20, tuv_20, TKE_20);
        }
        public void BedData30(out double[] x, out double[] tau, out double[] k)
        {
            BedData_(out x, out tau, out k, x_30, tuv_30, TKE_30);
        }
        //
        public void BedDataHeightCorrection_(double[] y_bed, out double[] tau, out double[] k, double [] x_, double [] y_, double [] tuv_, double [] TKE_)
        {
            double[] ttau = new double[x_.Length];
            double[] tk = new double[x_.Length];
            //
            double tmpX = 0;
            int ch = 0;
            for (int i = 0; i < x_.Length; i++)
            {
                if (x_[i] != tmpX)
                {
                    tmpX = x_[i];
                    for (int j = 0; j < 100; j++)
                    {
                        if (i + j < x_.Length)
                        {
                            if (x_[i + j] != tmpX)
                                break;
                            if ((y_bed[ch] >= y_[i + j]) && (y_bed[ch] <= y_[i + j + 1]))
                            {
                                if ((!double.IsNaN(tuv_[i + j])) && (!double.IsNaN(tuv_[i + j + 1])))
                                    ttau[ch] = tuv_[i + j] + (y_bed[ch] - y_[i + j]) * (tuv_[i + j + 1] - tuv_[i + j]) / (y_[i + j + 1] - y_[i + j]);
                                if ((!double.IsNaN(TKE_[i + j])) && (!double.IsNaN(TKE_[i + j + 1])))
                                    tk[ch] = TKE_[i + j] + (y_bed[ch] - y_[i + j]) * (TKE_[i + j + 1] - TKE_[i + j]) / (y_[i + j + 1] - y_[i + j]);
                                break;
                            }
                        }
                    }
                    ch++;
                }
            }
            //
            tau = new double[ch];
            k = new double[ch];
            //
            for (int i = 0; i < ch; i++)
            {
                k[i] = tk[i];
                tau[i] = ttau[i];
            }

        }
        //
        public void BedDataHeightCorrection10(double[] y_bed, out double[] tau, out double[] k)
        {
            BedDataHeightCorrection_(y_bed, out tau, out k, x_10, y_10, tuv_10, TKE_10);
        }
        //
        public void BedDataHeightCorrection20(double[] y_bed, out double[] tau, out double[] k)
        {
            BedDataHeightCorrection_(y_bed, out tau, out k, x_20, y_20, tuv_20, TKE_20);
        }
        //
        public void BedDataHeightCorrection30(double[] y_bed, out double[] tau, out double[] k)
        {
            BedDataHeightCorrection_(y_bed, out tau, out k, x_30, y_30, tuv_30, TKE_30);
        }
        //
        public double[] SlipBedX(double slip, double[] xBed)
        {
            double[] slipX = new double[xBed.Length];
            for (int i = 0; i < xBed.Length; i++)
                slipX[i] = xBed[i] + slip;
            //
            return slipX;
        }
        /// <summary>
        /// Расчет e и nuT по k и l по Прандтлю
        /// </summary>
        /// <param name="y">координата y от дна</param>
        /// <param name="k">кинетическая энергия</param>
        /// <param name="nuT">турбулентная вязкость m2/s</param>
        /// <param name="e">турбулентная диссипация</param>
        public void KE_Prandtl(double[] y, double[] k, out double[] nuT, out double[] e)
        {
            int count = y.Length;
            nuT = new double[count];
            e = new double[count];
            //
            double C_D = 0.08; // константа
            double l_mix = 0;// длина пути смешивания
            for (int i = 0; i < count; i++)
            {
                l_mix = 0.41 * y[i];//  должен ли путь смешивания меняться??
                e[i] = C_D * Math.Pow(k[i], 3.0 / 2.0) / l_mix;
                nuT[i] = C_D * k[i] * k[i] / e[i];
            }
        }
        //
        
    }
    //
    public class KwollGeometry
    {
        /// <summary>
        /// Точка brink средней волны 10 градусов, от нее идет отсчет в эксп. данных
        /// </summary>
        public double ZeroPoint10 = 0;
        /// <summary>
        /// Точка brink средней волны 20 градусов, от нее идет отсчет в эксп. данных
        /// </summary>
        public double ZeroPoint20 = 0;
        /// <summary>
        /// Точка brink средней волны 30 градусов, от нее идет отсчет в эксп. данных
        /// </summary>
        public double ZeroPoint30 = 0;
        /// <summary>
        /// Геометрия дна с 10ю 10-ти градусными дюнами и зоной разгона
        /// </summary>
        public double[] Z10;
        /// <summary>
        /// Геометрия дна с 10ю 20-ти градусными дюнами и зоной разгона
        /// </summary>
        public double[] Z20;
        /// <summary>
        /// Геометрия дна с 10ю 30-ти градусными дюнами и зоной разгона
        /// </summary>
        public double[] Z30;
        /// <summary>
        /// Заполнение массивов данными о геометрии дна (Плоская зона FreeArea метров и 10 дюн по 0.9 метров каждая, затем зона установления FreeArea)
        /// </summary>
        /// <param name="Nx">Количество узлов дна</param>
        /// <param name="FreeArea">Зона разгона перед дюнами в метрах</param>
        /// <param name="CountDunes">Количество дюн подряд</param>
        /// <param name="N1">количество дополнительных участков FreeArea перед дюной (если указано 0, значит 1 участок)</param>
        /// <param name="N2">количество дополнительных участков FreeArea после дюны (если указано 0, значит 1 участок)</param>
        public KwollGeometry(int Nx, double FreeArea, int CountDunes, int N1 = 0, int N2 = 0)
        {
            //геометрия 
            //                                     _______  
            //  Inlet |__________freearea_________/  x10  \________freearea______| Outlet
            double L = 2 * FreeArea + CountDunes * 0.9 + ((N1 + N2) * FreeArea);
            double dx = L / (Nx - 1);
            int PointsCount = CountDunes * 3 + 3;
            //
            Z10 = new double[Nx];
            Z20 = new double[Nx];
            Z30 = new double[Nx];
            //координаты опорных узлов для дна с 10 градусными дюнами [0], 20 градусными - [1], 30 градусными - [3]
            double[][] x = new double[3][];
            double[][] y = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                x[i] = new double[PointsCount];
                y[i] = new double[PointsCount];
            }
            double[] x10 = { 0.6164, 0.1418, 0.1418 };
            double[] x20 = { 0.7626, 0.0687, 0.0687 };
            double[] x30 = { 0.8134, 0.0433, 0.0433 };
            double[][] x_all = new double[3][];
            x_all[0] = x10;
            x_all[1] = x20;
            x_all[2] = x30;
            double[] y_all = { 0.03, 0.025, 0 };
            //
            //Dunes
            x[0][1] = (1 + N1) * FreeArea;
            x[1][1] = (1 + N1) * FreeArea;
            x[2][1] = (1 + N1) * FreeArea;
            //
            for (int i = 2; i < PointsCount - 1; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    x[j][i] = x[j][i - 1] + x_all[j][(i - 2) % 3];
                    y[j][i] = y_all[(i - 2) % 3];
                }
            }
            x[0][PointsCount - 1] = x[0][PointsCount - 2] + FreeArea * (1 + N2);
            x[1][PointsCount - 1] = x[1][PointsCount - 2] + FreeArea * (1 + N2);
            x[2][PointsCount - 1] = x[2][PointsCount - 2] + FreeArea * (1 + N2);
            //
            alglib.spline1dinterpolant d10, d20, d30;
            //
            alglib.spline1dbuildlinear(x[0], y[0], out d10);
            alglib.spline1dbuildlinear(x[1], y[1], out d20);
            alglib.spline1dbuildlinear(x[2], y[2], out d30);
            //
            double xl = 0;
            for (int i = 0; i < Nx; i++)
            {
                xl = i * dx;
                Z10[i] = alglib.spline1dcalc(d10, xl);
                Z20[i] = alglib.spline1dcalc(d20, xl);
                Z30[i] = alglib.spline1dcalc(d30, xl);
            }
            double p = Math.Round(CountDunes / 2.0, MidpointRounding.ToEven) * 0.9 + FreeArea * (1 + N1);
            ZeroPoint10 = p - 0.1418;
            ZeroPoint20 = p - 0.0687;
            ZeroPoint30 = p - 0.0433;
        }
    }
}
