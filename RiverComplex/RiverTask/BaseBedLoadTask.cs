//-----------------------------------------------------------------------------------
//                 Реализация абстрактного класса задачи BaseBedLoadTask 
//      В классе определены общие рабочие массивы необходимые для расчета донных деформаций
//      реализация алгоритмов расчета донных деформаций по различным русловым моделям 
//      должна реализовываться в абстрактных методах класса
//-----------------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//-----------------------------------------------------------------------------------
//            Модуль BedLoadLibrary для расчета донных деформаций 
//                (учет движения только влекомых наносов)
//                              Потапов И.И.
//                              Снигур К. С.
//                        - (C) Copyright 2017 -
//                          ALL RIGHT RESERVED
//                               13.01.17
//-----------------------------------------------------------------------------------
using System;
//-----------------------------------------------------------------------------------
namespace RiverTaskLibrary 
{
    /// <summary>
    /// Базисный класс задачи расчета донных деформаций
    /// </summary>
    [Serializable]
    public abstract class BaseBedLoadTask
    {
        protected BedPhysicsParams Parameter;
        /// <summary>
        /// Наименование задачи
        /// </summary>
        public abstract string Name{get;}

        /// <summary>
        /// Метод решения системы алгебраических уравнений
        /// true  - скользящий счет
        /// false - прогонка Томаса
        /// </summary>
        public bool flagSys = false;
        /// <summary>
        /// Флаг отладки
        /// </summary>
        public int debug = 0;
        /// <summary>
        /// Поле сообщений о состоянии задачи
        /// </summary>
        public string Message = "ok";
        //
        protected int otstup = 0;
        protected double kappa2, kappa, g, d, cx;
        protected double rhoS, rhoW, tanphi, epsilon;
        protected double tau0, q0, Fa0, gamma;

        #region Рабочие массивы
        /// <summary>
        /// массив донных отметок
        /// </summary>
        public double[] Zeta = null;
        /// <summary>
        /// массив донных отметок на предыдущем слое по времени
        /// </summary>
        public double[] Zeta0 = null;
        /// <summary>
        ///  косинус гамма угол между нормалью к дну и вертикальной осью
        /// </summary>
        public double[] CosGamma;
        ///// <summary>
        ///// полный расход объемный наносов
        ///// </summary>
        public double[] Qb = null;
        ///// <summary>
        ///// расход объемный наносов на ровном дне
        ///// </summary>
        public double[] QbTau = null;
        ///// <summary>
        ///// расход объемный наносов от уклона дна
        ///// </summary>
        public double[] QbZeta = null;
        ///// <summary>
        ///// расход объемный наносов от давления
        ///// </summary>
        public double[] QbPress = null;
        ///// <summary>
        ///// полный расход объемный наносов
        ///// </summary>
        public double[] Velocity = null;
        ///// <summary>
        ///// расход объемный наносов на ровном дне
        ///// </summary>
        public double[] VelocityTau = null;
        ///// <summary>
        ///// расход объемный наносов от уклона дна
        ///// </summary>
        public double[] VelocityZeta = null;
        public double[] VelocityZetaQ = null;
        ///// <summary>
        ///// расход объемный наносов от давления
        ///// </summary>
        public double[] VelocityPress = null;

        #endregion

        #region Краевые условия

        /// <summary>
        /// тип задаваемых ГУ
        /// </summary>
        public BoundaryCondition BCBed;

        #endregion

        #region Служебные переменные
        /// <summary>
        /// Количество расчетных узлов для дна
        /// </summary>
        public int Count;
        /// <summary>
        /// Количество расчетных подобластей
        /// </summary>
        public int N;
        /// <summary>
        /// шаг дискретной схемы по х
        /// </summary>
        public double dx;
        /// <summary>
        /// текущее время расчета 
        /// </summary>
        public double time = 0;
        /// <summary>
        /// текущая итерация по времени 
        /// </summary>
        public int CountTime = 0;
        /// <summary>
        /// количество узлов по времени
        /// </summary>
        public int LengthTime = 200000;
        /// <summary>
        /// относительная точность при вычислении изменения донной поверхности
        /// </summary>
        protected double eZeta = 0.0000000001;
        /// <summary>
        /// расчетный период времени, сек 
        /// </summary>
        public double T;
        /// <summary>
        /// шаг по времени
        /// </summary>
        public double dt;
        /// <summary>
        /// расход наносов на входе
        /// </summary>
        // public double q_in;
        /// <summary>
        /// расход наносов на выходе
        /// </summary>
        // public double q_out;
        /// <summary>
        /// Уровень дна в 0 узле
        /// </summary>
        public double Z0 = 1;
        /// <summary>
        /// Уровень дна в N узле
        /// </summary>
        public double ZN = 1;
        /// <summary>
        /// количество итераций по нелинейности
        /// </summary>
        public int MaxIterNoLine = 1000000;
        protected double b = 0, S_c = 0, S_p = 0;
        /// <summary>
        /// массовые расходы 
        /// </summary>
        protected double F_e = 0, F_w = 0, F_n = 0, F_s = 0;
        /// <summary>
        /// проводимости
        /// </summary>
        protected double D_e = 0, D_w = 0, D_n = 0, D_s = 0;
        /// <summary>
        /// числа Пекле
        /// </summary>
        protected double P_e = 0, P_w = 0, P_n = 0, P_s = 0;
        /// <summary>
        /// коэффициенты дискретных аналогов 
        /// </summary>
        protected double a_E = 0, a_W = 0, a_N = 0, a_S = 0;
        protected double Zeta_E = 0, Zeta_W = 0, Zeta_P = 0;
        protected double a0_P = 0, a_P = 0, a_Pp = 0;

        protected double A, B, C, qA;

        public double[] Ae = null;
        public double[] Ap = null;
        public double[] Aw = null;
        public double[] Sc = null;
        //
        [NonSerialized]
        public double[] TauC12 = null;

        #endregion
        /// <summary>
        /// Конструктор по умолчанию/тестовый
        /// </summary>
        public BaseBedLoadTask()
        {
        }
        /// <summary>
        /// Метод перезапуска задачи с новыми параметрами
        /// </summary>
        /// <param name="BCBed">Тип граничных условий</param>
        /// <param name="param">Параметры расчета русловой задачи</param>
        /// <param name="dx">Шаг по дну</param>
        /// <param name="Nx">Количество узлов по дну</param>
        /// <param name="dt">Шаг по времени</param>
        public virtual void ReStartBaseBedLoadTask(BoundaryCondition BCBed, BedPhysicsParams param, double dx, int Nx, double dt)
        {
            Parameter = param;
            SetBedLoadPhysics();
            Count = Nx;
            this.dx = dx;
            this.dt = dt;
            this.N = Count - 1;
            this.BCBed = BCBed;
            Zeta = new double[Count];
            Zeta0 = new double[Count];
            Qb = new double[N];
            QbTau = new double[N];
            QbZeta = new double[N];
            QbPress = new double[N];
            Velocity = new double[Count];
            VelocityTau = new double[Count];
            VelocityZeta = new double[Count];
            VelocityZetaQ = new double[Count];
            VelocityPress = new double[Count];
            CosGamma = new double[N];
            // Коэффициенты системы
            Ae = new double[N];
            Ap = new double[N];
            Aw = new double[N];
            Sc = new double[N];
        }
        public virtual void ReStartBaseBedLoadTask(BedPhysicsParams param, double dx, int Nx, double dt)
        {
            Parameter = param;
            this.dx = dx;
            this.dt = dt;
            this.N = Nx - 1;
            SetBedLoadPhysics();
        }
        private void SetBedLoadPhysics()
        {
            otstup = Parameter.otstup;
            kappa2 = Parameter.kappa * Parameter.kappa;
            rhoS = Parameter.rhoS;
            rhoW = Parameter.rhoW;
            tanphi = Parameter.tf;
            g = Parameter.g;
            d = Parameter.d;
            cx = Parameter.cx;
            kappa = Parameter.kappa;
            epsilon = Parameter.eps;
            // коэффициент сухого трения
            Fa0 = tanphi * (rhoS - rhoW) * g;
            // критические напряжения на ровном дне
            tau0 = 3.0 / 8.0 * kappa * kappa * d * Fa0 / cx;
            // константа расхода влекомых наносов
            q0 = 4.0 / (3.0 * kappa * Math.Sqrt(rhoW) * Fa0 * (1 - epsilon));
            // относительная полотность
            gamma = (rhoS - rhoW) / rhoW;     

        
        }
        /// <summary>
        /// Переустановка текущих граничных условий 
        /// </summary>
        /// <param name="typeBCBed">Тип граничных условий</param>
        public void ReStartBCBedTask(BoundaryCondition BCBed)
        {
            this.BCBed = BCBed;
        }
        /// <summary>
        /// Установка начального профиля дна
        /// </summary>
        /// <param name="typeBCBed">Тип граничных условий</param>
        public void SetZeta0(double[] Zeta0)
        {
            if (Zeta0.Length == Count)
            {
                this.Zeta0 = Zeta0;
                for (int i = 0; i < Zeta0.Length; i++)
                    Zeta[i] = Zeta0[i];
            }
            else
                Message = "Несогласованность размерностей при установке начальных условий";
        }
        /// <summary>
        /// Тестовая печать поля
        /// </summary>
        /// <param name="Name">имя поля</param>
        /// <param name="mas">массив пля</param>
        /// <param name="FP">точность печати</param>
        public void PrintMas(string Name, double[] mas, int FP = 8)
        {
            string Format = " {0:F6}";
            if (FP != 6)
                Format = " {0:F" + FP.ToString() + "}";

            Console.WriteLine(Name);
            for (int i = 0; i < mas.Length; i++)
            {
                Console.Write(Format, mas[i]);
            }
            Console.WriteLine();
        }
        #region Абстрактные методы класса
        /// <summary>
        /// Задание граничных условий
        /// </summary>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="P">придонное давление</param>
        protected abstract void CalkBCondition(double[] tau, double[] ps = null);
        /// <summary>
        /// Вычисление расхода по значению придонного касательного напряжения
        /// </summary>
        /// <param name="Tau">Критическое напряжение</param>
        /// <param name="J">Уклон дна</param>
        /// <param name="dp">Градиент давления</param>
        /// <returns></returns>
        public abstract double CalculateRate(double Tau, double J, double dp=0);
        /// <summary>
        /// Вычисление изменений формы донной поверхности на 1 шаге по времени 
        /// Патанкар (Полностью неявный дискретный аналог ст 40 ф.3.40 - 3.41)
        /// среднее гармоническое величиние коэффициентов проводимости ke,kw
        /// </summary>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="P">придонное давление или свободная поверхность потока</param>
        /// <param name="Q0">правая часть нулевой член разложения по зета</param>
        /// <param name="Q1">правая часть первый член разложения по зета</param
        /// <returns>новая форма дна</returns>
        public abstract double[] Solve(double[] tau, double[] P = null, double[] Q0=null, double[] Q1=null);
        /// <summary>
        /// Мой метод решения уравнения Экснера по статье КИМ 2019
        /// </summary>
        /// <param name="Tau">напряжение в целых узлах</param>
        /// <param name="Nt">количество итераций по подзадаче для дна (обычно одна)</param>
        /// <param name="RS">рсход взвешенных наносов = alpha*W(Sz-S[i])/H</param>
        /// <param name="Pbed">придонное давление</param>
        /// 
        /// <returns>новая форма дна</returns>
        public virtual double[] MySolve(double[] Tau, double[] RS = null, double [] Pbed = null, int Nt = 1)
        { return null; }
        #endregion
    }
}
