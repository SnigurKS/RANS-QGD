using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    public class ReverseShallowWater1D
    {
        /// <summary>
        /// время записи результатов в буфер
        /// </summary>
        double[] TimeLayer = { 0, 0, 0, 0 };
        /// <summary>
        /// расчетное время записи результатов в буфер
        /// </summary>
        public double[] CT = { 0, 1, 2, 3 };
        /// <summary>
        /// Число пространственных узлов
        /// </summary>
        public int CountNodes = 51;
        /// <summary>
        /// шаг схемы по пространству
        /// </summary>
        public double dx;
        #region Искомые величины
        /// <summary>
        /// скорость потока
        /// </summary>
        public double[] U;
        public double[][] TU;
        /// <summary>
        /// глубина потока
        /// </summary>
        public double[] H;
        public double[][] TH;
        /// <summary>
        /// свободная поверхность потока
        /// </summary>
        public double[] Eta;
        /// <summary>
        /// Гидродинамический расход
        /// </summary>
        double Q;
        #endregion

        /// <summary>
        /// Число Фруда
        /// </summary>
        double Fr;
        /// <summary>
        /// средняя скорость и глубина
        /// </summary>
        double U0, H0;
        /// <summary>
        /// шероховатость по Манингу
        /// </summary>
        double n = 0.04;
        double g;
        double rhoW;

        public double Lambda = 0.0010;
        /// <summary>
        /// основной уклон в НУ
        /// </summary>
         double J0;
        //
         double Z0 = 0;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Param">Параметры задачи</param>
        /// <param name="dx">шаг по пространству</param>
        /// <param name="_NX">количество шагов по пространству</param>
        /// <param name="_NT">количество шагов по времени</param>
        /// <param name="_dt">шаг по времени</param>
        /// <param name="J0">основной уклон</param>
        public ReverseShallowWater1D(ReverseShallow1D_Parameter Param, double dx, int _NX, double J0)
        {
            //параметры расчета
            CountNodes = _NX; 
            this.dx = dx;
            // параметры задачи
            H0 = Param.Hout; U0 = Param.Uout; 
            Q = U0 * H0;
            Fr = U0 * U0 / g / H0;
            g = Param.g;
            rhoW = Param.rho_w;
            Lambda = J0 / Fr;
            n = Math.Sqrt(J0) * Math.Pow(H0, 2.0 / 3.0) / U0;
            //n = Param.n;
            this.J0 = J0;
            //
            InitTask();
            //
            U[CountNodes - 1] = U0;
            H[CountNodes - 1] = H0;
        }
        double[] U_old;
        /// <summary>
        /// метод выделения памяти и установки начальных условий
        /// </summary>
        void InitTask()
        {
            #region Выделение памяти под рабочии массивы
            U = new double[CountNodes];
            H = new double[CountNodes];
            Eta = new double[CountNodes];
            U_old = new double[CountNodes];
            for (int i = 0; i < CountNodes; i++)
                U_old[i] = U0;
            #endregion
           
        }
        bool flag = true;
        /// <summary>
        /// метод решения задачи
        /// </summary>
        public double[] TaskSolver(double [] Z, double dt)
        {

            if (flag)
            {
                for (int i = 0; i < CountNodes; i++)
                    Eta[i] = Z[i] + H0;
                flag = false;
            }
            //else
            //{
            //    for (int i = 0; i < CountNodes; i++)
            //        Eta[CountNodes - 1 - i] = (Z[CountNodes - 1] + H0) + i * dx * J0;
            //}

            //
            #region Гидродинамический расчет
            //H[CountNodes - 1] = H[CountNodes - 1] - (Z[CountNodes - 1] - Z0);
            double delta;
            // Расчет глубин
            for (int j = CountNodes - 1; j > 0; j--)
            {
                // текущий Фруд
                Fr = U[j] * U[j] / g / H[j];
                double DZ = Z[j] - Z[j - 1];
                double H13 = Math.Pow(H[j], 1.0 / 3.0);
                double n2 = n * n;
                // расчет приращения
                // по коэффициенту Шези
                Lambda = g * n2 / H13;
                //
                //if(H[j]<1)
                //    H13 = Math.Pow(H[j], 1.5*Math.Sqrt(n)*2);
                //else
                //    H13 = Math.Pow(H[j], 1.3 * Math.Sqrt(n)*2);
                //Lambda = g * n2 / H13;
                //
                //double C = (23 + 1 / n + 0.00155 / J0) / (1 + (23 + 0.00155 / J0) * n / Math.Sqrt(H[j]));
                //низко, но норм
                //double C = Math.Sqrt(g) * 6.67 * Math.Pow((H[j] / 0.00069), 1.0 / 6.0);
                // норм, средне
                //double C = Math.Sqrt(g) * 5.66 * Math.Log10(H[j] / 0.00069) + 6.62;
                //
                //Lambda = g / C / C;
                //
                delta = dx * (DZ / dx + Lambda * Fr) / (1 - Fr * (1 - 3 * Lambda * dx / (2 * H[j])));
                // для НЕстационарного случая
                //delta = dx * (DZ / dx + Lambda * Fr + (Q / H[j] - U_old[j - 1]) / dt) / (1 - Fr * (1 - 3 * Lambda * dx / (2 * H[j])) + dx / dt * Q / H[j] / H[j]);
                if (Math.Abs(delta) / Math.Abs(H[j]) > 0.2)
                {
                    H[j - 1] = H[j] * (1 + 0.1 * delta / Math.Abs(delta));
                }
                else
                {
                    // поправка глубин
                    H[j - 1] = H[j] + delta;// 
                    //H[j - 1] = (Eta[j] - Z[j]) + delta;
                }
                // поправка скоростей
                U[j - 1] = Q / H[j - 1];
                // поправка свободной поверхности
                Eta[j - 1] = Z[j - 1] + H[j - 1];

            }
            Eta[CountNodes - 1] = Z[CountNodes - 1] + H[CountNodes - 1];    //  
            //H[CountNodes - 1] = Eta[CountNodes - 1] - Z[CountNodes - 1];
            H[CountNodes - 1] = 2 * H[CountNodes - 2] - H[CountNodes - 3];  // 
            //H[CountNodes - 1] = H[CountNodes - 1] - (Z[CountNodes - 1] - Z0);
            U[CountNodes - 1] = Q / H[CountNodes - 1]; 
            //
            Z0 = Z[CountNodes - 1];
            //
            // расчет напряжений
            double[] stress = new double[CountNodes - 1];
            for (int i = 0; i < CountNodes - 1; i++)
            {
                double h = 0.5 * (H[i + 1] + H[i]);
                double H13 = Math.Pow(h, 1.0 / 3.0);
                double u = 0.5 * (U[i + 1] + U[i]);
                stress[i] = rhoW * g * n * n * u * u / H13;
            }
            //
            return stress;

            #endregion
        }
               

    }
}
