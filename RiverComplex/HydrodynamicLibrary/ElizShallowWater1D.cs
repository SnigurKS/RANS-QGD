using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    /// <summary>
    /// Решение одномерного уравгнения мелкой воды
    ///     30 12 11 Потапов Игорь Иванович
    ///     алгоритм Елизарова Т.Г., Злотник А.А., Никитина О.В.
    ///     
    /// </summary>
    public class ElizShallowWater1D
    {
        BoundaryCondition Boundary;
        /// <summary>
        /// Вязкость потока
        /// </summary>
        double Mu;
        /// <summary>
        /// скорость потока на выходе в область
        /// </summary>
        double U0;
        /// <summary>
        /// глубина потока на выходе
        /// </summary>
        double H0;
        /// <summary>
        /// плотность воды кг/м^3
        /// </summary>
        double rho_w = 1000;
        /// <summary>
        /// шероховатость по Манингу
        /// </summary>
        double n = 0.03;
        /// <summary>
        /// гравитационная постоянная (м/с/с)
        /// </summary>
        double g = 9.8;
        double g23 = 0;
        /// <summary>
        /// длина области (м)
        /// </summary>
        public double L;
        /// <summary>
        /// шаг по пространству (м)
        /// </summary>
        public double dx;
        /// <summary>
        /// шаг по времени (с)
        /// </summary>
        public double dt;
        //
        double Time;
        /// <summary>
        /// количество узлов в области
        /// </summary>
        int CountKnots;
        /// <summary>
        /// количество элементов
        /// </summary>
        int CountElems;
        
        #region Основные неизвестные в узлах сетки CountKnots
        /// <summary>
        /// средняя скорость
        /// </summary>
        public double[] U;
        /// <summary>
        /// средняя глубина потока
        /// </summary>
        public double[] H;
        /// <summary>
        /// внешние силы (ветровые нагрузки)
        /// </summary>
        public double[] Tau;
        /// <summary>
        /// уровень свободной поверхности
        /// </summary>
        public double[] Eta;
        /// <summary>
        /// коэффициент релаксации
        /// </summary>
        public double[] tau;
        /// <summary>
        /// фиктивная турбулентная вязкость
        /// </summary>
        public double[] mu;
        /// <summary>
        /// аналог скорости звука
        /// </summary>
        public double[] c;
        #endregion
        #region Вспомогательные неизвестные в центрах тяжести межузловых элементов
        /// <summary>
        /// средняя скорость
        /// </summary>
        double[] mU;
        /// <summary>
        /// средняя глубина потока 
        /// </summary>
        double[] mH;
        /// <summary>
        /// внешние силы(ветровые нагрузки)
        /// </summary>
        double[] mTau;
        /// <summary>
        /// уровень дна
        /// </summary>
        double[] mZeta;
        /// <summary>
        /// коэффициент релаксации
        /// </summary>
        double[] mtau;
        /// <summary>
        /// поток
        /// </summary>
        double[] Jm;
        /// <summary>
        /// поправка скорости
        /// </summary>
        double[] W;
        /// <summary>
        /// приведенное напряжение
        /// </summary>
        double[] PI;
        #endregion
        /// <summary>
        /// параметр Курранта
        /// </summary>
        double betta = 0.1; // 0 < betta < 1
        double alpha = 0.6;
        double delta = 0.1;
        /// <summary>
        /// минимальная глубина "мокрого" потока 
        /// </summary>
        double errH = 0.001;
        /// <summary>
        /// параметры схемы
        /// </summary>
        double Cmax, Hmax, gamma;
        public string Message = "Ok";


       
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_L">длина области (м)</param>
        /// <param name="_T">длина расчетного периода (с)</param>
        /// <param name="_U">скорость потока на входе в область</param>
        /// <param name="_H">глубина потока на входе в область</param>
        /// <param name="_J">средний уклон дна</param>
        /// <param name="_Mu">Вязкость жидкости</param>
        /// <param name="_N">количество узлов в области</param>
        public ElizShallowWater1D(ElizShallow1D_Parameter Parameter,double L, int Nx, double Time)
        {
            try
            {
                this.L = L;
                Mu = Parameter.Mu;
                U0 = Parameter.Uout;
                H0 = Parameter.Hout;
                CountKnots = Nx;
                CountElems = Nx - 1;
                g23 = 2.0 / 3.0 * g;
                n = Parameter.n;
                // шаг сетки
                dx = L / CountElems;
                this.Time = Time;
                //Boundary = _bc;
                ////вычисление среднего уклона
                //for (int i = 0; i < CountElems; i++)
                //    J += (Zeta[i] - Zeta[i + 1]) / dx;
                //J /= CountElems;
                // память
                U = new double[CountKnots];
                H = new double[CountKnots];
                mu = new double[CountKnots];
                
                tau = new double[CountKnots];
                Eta = new double[CountKnots];
                c = new double[CountKnots];
                Jm = new double[CountElems];
                W = new double[CountElems];
                PI = new double[CountElems];
                mU = new double[CountElems];
                mH = new double[CountElems];
                mZeta = new double[CountElems];
                mtau = new double[CountElems];
                mTau = new double[CountElems];
                //
                for (int i = 0; i < CountKnots; i++)
                {
                    U[i] = U0;
                    H[i] = H0;
                    mu[i] = Mu;
                }
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }
        /// <summary>
        /// Метод решения задачи гидродинамики
        /// </summary>
        /// <param name="Zeta">Уровень дна</param>
        /// <param name="Tau">Ветровые нагрузки</param>
        /// <returns></returns>
        public double[] Solver(double[] Zeta, double[] Tau = null)
        {
            if (Tau == null)
                Tau = new double[CountKnots];
            double dU, dH, dZeta, mMu;
            double U2p = 0, U2 = 0;
            double H2p = 0, H2 = 0;
            double pi0, pi1, pi2;
            double w0, w1;
            double _H, q0, q1, q2;
            //
            double time = 0;
            int CountTime = 0;
            try
            {
                #region Решение задачи мелкой воды
                // расчет шага по времени dt коэффициента вязкости mu[i] 
                // и параметра регуляризации tau[i] с учетом возможного сухого дна потока 
                do
                {
                    for (int i = 0; i < c.Length; i++)
                        c[i] = Math.Sqrt(g * H[i]);
                    Cmax = c.Max();
                    Hmax = H.Max();
                    if (Hmax / 10 > H0) throw new Exception("Бяда Hmax = " + Hmax.ToString());
                    if (Hmax >= errH)
                        dt = betta * dx / (Cmax);
                    else
                    {
                        gamma = delta * Hmax;
                        // шаг по времени
                        dt = betta * dx / (Cmax + gamma);
                    }
                    for (int i = 0; i < c.Length; i++)
                    {
                        // время релаксации
                        if (Math.Abs(H[i]) > errH)
                            tau[i] = alpha * dx / c[i];
                        else
                            tau[i] = alpha * dx / (c[i] + gamma);
                        // отрелаксированная вязкость потока
                        mu[i] = g23 * tau[i] * H[i] * H[i] + Mu;
                    }
                    // Расчет основных параметров (потоков, приведенного напряжения и
                    // скорости релаксации в центрах элементов
                    for (int i = 0; i < Jm.Length; i++)
                    {
                        // расчет основных неизвестных в центрах элементов
                        mU[i] = 0.5 * (U[i + 1] + U[i]);
                        mH[i] = 0.5 * (H[i + 1] + H[i]);
                        mZeta[i] = 0.5 * (Zeta[i + 1] + Zeta[i]);
                        mTau[i] = 0.5 * (Tau[i + 1] + Tau[i]);
                        mtau[i] = 0.5 * (tau[i + 1] + tau[i]);
                        mMu = 0.5 * (mu[i + 1] + mu[i]);
                        // квадраты функций
                        U2 = U[i] * U[i]; U2p = U[i + 1] * U[i + 1];
                        H2 = H[i] * H[i]; H2p = H[i + 1] * H[i + 1];
                        // расчет основных производных в центрах элементов
                        dU = (U[i + 1] - U[i]) / dx;
                        dH = (H[i + 1] - H[i]) / dx;
                        dZeta = (Zeta[i + 1] - Zeta[i]) / dx;
                        // расчет приведенного напряжения
                        pi0 = mH[i] * mtau[i];
                        pi1 = mMu * dU + pi0 * mU[i] * (g * dH + 0.5 * (U2p - U2) / dx + g * dZeta - mTau[i]);
                        pi2 = pi0 * g * (H[i + 1] * U[i + 1] - H[i] * U[i]) / dx;
                        PI[i] = pi1 + pi2;
                        // расчет скорости релаксации
                        w0 = g * 0.5 * (H2p - H2) / dx + (H[i + 1] * U[i + 1] * U[i + 1] - H[i] * U[i] * U[i]) / dx;
                        w1 = g * mH[i] * dZeta - mH[i] * mTau[i];
                        W[i] = mtau[i] / mH[i] * (w0 + w1);
                        // отрелаксированный поток
                        Jm[i] = mH[i] * (mU[i] - W[i]);
                    }
                    // вычислени глубины и скорости на новом временном слое
                    double oldH, oldU;
                    for (int i = 1; i < H.Length - 1; i++)
                    {
                        oldH = H[i];
                        // глубина но новом слое по времени
                        H[i] = oldH - dt / dx * (Jm[i] - Jm[i - 1]);
                        _H = 0.5 * (mH[i] + mH[i - 1]) - tau[i] * (mH[i] * mU[i] + mH[i - 1] * mU[i - 1]) / dx;
                        q0 = (mU[i] * Jm[i] - mU[i - 1] * Jm[i - 1]) / dx;
                        q1 = 0.5 * g * (mH[i] * mH[i] - mH[i - 1] * mH[i - 1]) / dx;
                        q2 = (PI[i] - PI[i - 1]) / dx + _H * (Tau[i] - g * (mZeta[i] - mZeta[i - 1]) / dx);
                        // расход на новом слое по времени
                        U[i] = U[i] * oldH - dt * (q0 + q1 - q2);
                        // вычисление скорости потока с учетом сухого дна
                        if (Math.Abs(H[i]) < errH)
                            U[i] = 0;
                        else
                            U[i] = U[i] / H[i];
                    }

                    // добавление в граничное условие 
                    //U[0] = U0;
                    //H[H.Length - 1] = 2 * H[H.Length - 2] - H[H.Length - 3];

                    H[0] = H[1];
                    U[0] = H0 * U0 / H[0];
                    U[CountKnots - 1] = U[CountKnots - 2];
                    //
                    time += dt;
                    CountTime++;
                }
                while (time < Time);
                //
                
               
                //
                for (int i = 0; i < CountKnots; i++)
                    Eta[i] = Zeta[i] + H[i];
                #endregion
                // расчет напряжений
                double[] stress = new double[CountKnots-1];
                for (int i = 0; i < H.Length-1; i++)
                {
                    double h = 0.5 * (H[i + 1] + H[i]);
                    double H13 = Math.Pow(h, 1.0 / 3.0);
                    double u = 0.5 * (U[i + 1] + U[i]);
                    stress[i] = rho_w * g * n * n * u * u / H13;
                }
                //
                return stress;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return null;
            }
        }
        //
        protected void CalkBCondition(ref double[] Func)
        {
            double q_in = Boundary.InletValue;
            // граничные условыия на входе
            switch (Boundary.Inlet)
            {
                case TypeBoundaryCondition.Transit_Feed:
                    // Транзитный_поток - ровное дно
                    Func[0] = Func[1];
                    break;
                case TypeBoundaryCondition.Dirichlet_boundary_conditions:
                    {
                        Func[0] = q_in;
                    }
                    break;
                case TypeBoundaryCondition.Neumann_boundary_conditions:
                    {
                        Func[0] = Func[1] - dx * q_in;
                        break;
                    }
            }
            double q_out = Boundary.OutletValue;
            // граничные условыия на выходе
            switch (Boundary.Outlet)
            {
                case TypeBoundaryCondition.Transit_Feed:
                    // Транзитный_поток - ровное дно
                    Func[Func.Length - 1] = Func[Func.Length - 2];
                    break;
                case TypeBoundaryCondition.Dirichlet_boundary_conditions:
                    {
                        Func[Func.Length-1] = q_out;
                    }
                    break;
                case TypeBoundaryCondition.Neumann_boundary_conditions:
                    Func[Func.Length - 1] = Func[Func.Length - 2] - dx * q_out;
                    break;
            }
        }
    }
}
