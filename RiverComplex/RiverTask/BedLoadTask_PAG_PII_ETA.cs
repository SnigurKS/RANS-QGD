//-----------------------------------------------------------------------------------
//                  Реализация класса BedLoadTask_PAG_PII
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
//                               18.01.17
//-----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
//-----------------------------------------------------------------------------------
namespace RiverTaskLibrary
{
    [Serializable]
    /// <summary>
    /// Класс задачи о расчете донных деформаций 
    /// Модель движения наносов Петрова А.Г., Потапова И.И. 
    /// </summary>
    public class BedLoadTask_PAG_PII_ETA : BaseBedLoadTask
    {
        double s;
        public override string Name
        {
            get
            {
                return "Модель cо свободной поверхностью 2011";
            }
        }
        /// <summary>
        /// Конструктор 
        /// </summary>
        public BedLoadTask_PAG_PII_ETA()
            : base() {  }
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="L">Длина области</param>
        /// <param name="BCBed">Граничные условия</param>
        /// <param name="dt">Шаг по времени</param>
        /// <param name="Count">Количество узлов в области</param>
        /// </summary>
        public BedLoadTask_PAG_PII_ETA(BoundaryCondition BCBed, BedPhysicsParams param, double dx, int Nx, double dt)
        {
            base.ReStartBaseBedLoadTask(BCBed, param, dx, Nx, dt);
            BedPhysicsParams_PAG_PII_ETA i = (BedPhysicsParams_PAG_PII_ETA)param;
            s = i.f * gamma;
        }
        public override void ReStartBaseBedLoadTask(BoundaryCondition BCBed, BedPhysicsParams param, double dx, int Nx, double dt)
        {
            base.ReStartBaseBedLoadTask(BCBed, param, dx, Nx, dt);
            BedPhysicsParams_PAG_PII i = (BedPhysicsParams_PAG_PII)param;
            s = i.f * gamma;
        }
        /// <summary>
        /// Модель Петрова А.Г. и Потапова И.И.
        /// </summary>
        /// <param name="tau">Напряжение</param>
        /// <param name="A">A</param>
        /// <param name="B">B</param>
        /// <param name="C">C</param>
        /// <param name="qA">qA</param>
        /// </summary>
        private void CalkABC(double tau, out double A, out double B, out double C, out double qA)
        {
            double chi = Math.Sqrt(tau0 / Math.Abs(tau));
            if (chi < 1)
            {
                A = 1 - chi;
                B =(chi / 2 + (1 - chi) * (1 + s) / s) / tanphi;
                C = A / s / tanphi;
                // поправка для свободной поверхности
                B = B - C;
            }
            else
            {
                A = 0;
                B = 0;
                C = 0;
            }
            
            qA = q0 * tau * Math.Sqrt(Math.Abs(tau));
        }
        /// <summary>
        /// Задание граничных условий
        /// </summary>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="eta">придонное давление в модели не используется</param>
        /// </summary>
        protected override void CalkBCondition(double[] tau, double[] ps = null)
        {
            double dz;
            double q_in = BCBed.InletValue;
            // граничные условыия на входе
            switch (BCBed.Inlet)
            {
                case TypeBoundaryCondition.Transit_Feed:
                    // Транзитный_поток - ровное дно
                    Zeta[0] = Zeta[1];
                    break;
                case TypeBoundaryCondition.Dirichlet_boundary_conditions:
                    {
                        Z0 = q_in;
                        Zeta[0] = Z0;
                    }
                    break;
                case TypeBoundaryCondition.Neumann_boundary_conditions:
                    //if (tau0 > Math.Abs(tau[0]))
                    //    dz = 0;
                    //else
                    //{
                        CalkABC(tau[0], out A, out B, out C, out qA);
                        //dz = dx / B * (q_in / qA - A + C * (ps[N] - ps[N - 1]));
                        dz = -dt / dx * (qA * (A - B * (Zeta[1] - Zeta[0]) / dx - C * (ps[1] - ps[0]) / dx) - q_in);
                    //}
                    Zeta[0] = Zeta[1] + dz;
                    break;
                case TypeBoundaryCondition.Periodic_boundary_conditions:
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            CalkABC(tau[0], out A, out B, out C, out qA);
                            Zeta[0] = Zeta[1] + dx / B * (q_in / qA - A + C * (ps[N] - ps[N - 1]));
                            Zeta[Zeta.Length - 1] = Zeta[Zeta.Length - 2];
                        }
                    }
                    break;
            }
            double q_out = BCBed.OutletValue;
            // граничные условыия на выходе
            switch (BCBed.Outlet)
            {
                case TypeBoundaryCondition.Transit_Feed:
                    // Транзитный_поток - ровное дно
                    Zeta[Zeta.Length - 1] = Zeta[Zeta.Length - 2];
                    break;
                case TypeBoundaryCondition.Dirichlet_boundary_conditions:
                    {
                        ZN = q_out;
                        Zeta[N] = ZN;
                    }
                    break;
                case TypeBoundaryCondition.Neumann_boundary_conditions:
                    //if (tau0 > Math.Abs(tau[N - 1]))
                    //    dz = 0;
                    //else
                    //{
                        CalkABC(tau[N - 1], out A, out B, out C, out qA);
                        //dz = dx / B * (q_out / qA - A + C * (ps[N] - ps[N - 1]));
                        dz = dt / dx * (q_out - qA * (A - B * (Zeta[N] - Zeta[N - 1]) / dx - C * (ps[N] - ps[N - 1]) / dx));
                    //}
                    Zeta[N] = Zeta0[N - 1] - dz;
                    break;
            }
        }
        public override double CalculateRate(double Tau, double J, double dp = 0)
        {
            // расчет коэффициентов 
            CalkABC(Tau, out A, out B, out C, out qA);
            // косинус гамма
            double CosGamma = Math.Sqrt(1 / (1 + J * J));
            //
            qA /= CosGamma;
            // Расход наносов
            return qA * (A - B * J / CosGamma - C * dp / CosGamma);
        }
        /// <summary>
        /// Вычисление изменений формы донной поверхности на 1 шаге по времени 
        /// Патанкар (Полностью неявный дискретный аналог ст 40 ф.3.40 - 3.41)
        /// среднее гармоническое величиние коэффициентов проводимости ke,kw
        /// </summary>
        /// <param name="Zeta0">исходная форма дна</param>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="eta">свободная поверхность потока</param>
        /// <returns>новая форма дна</returns>
        /// </summary>
        public override double[] Solve(double[] tau, double[] eta, double[] Q0, double[] Q1)
        {
            try
            {
                double maxerr = 0;
                double maxZeta = 0;
                double dz, dp;
                double[] mA = new double[N];
                double[] mB = new double[N];
                double[] mC = new double[N];
                double[] ps = new double[Count];
                double[] S = new double[Count];
                // Свободная поверхность потока в узлах Zeta,  Zeta0
                for (int j = 1; j < N; j++)
                    ps[j] = 0.5 * (eta[j] + eta[j - 1]);
                // линейная интерполяция плоха!!! нужна квадратичная
                ps[0] = (2 * eta[1] - eta[2]) ;
                ps[N] = (2 * eta[N - 1] - eta[N - 2]);
                //
                for (int i = 0; i < N; i++)
                {
                    // расчет коэффициентов 
                    CalkABC(tau[i], out A, out B, out C, out qA);
                    if (i < otstup)
                    {
                        A = 0;
                        B = 0;
                        C = 0;
                    }
                    // производная (dzeta/dx)_i+1/2 
                    dz = (Zeta0[i + 1] - Zeta0[i]) / dx;
                    dp = (ps[i + 1] - ps[i]) / dx;
                    // косинус гамма
                    CosGamma[i] = Math.Sqrt(1 / (1 + dz * dz));
                    //
                    qA /= CosGamma[i];
                    mA[i] = qA * A;
                    mB[i] = Math.Abs(qA * B / dx);
                    mC[i] = qA * C / dx;
                    QbTau[i] = qA * A;
                    QbZeta[i] = -qA * B * dz / CosGamma[i];
                    // Расход наносов !!! на границах расход наносов плох при ГГУ нуна думать
                    QbPress[i] = -qA * C * dp / CosGamma[i];
                    // Расход наносов
                    Qb[i] = qA * (A - B * dz / CosGamma[i] - C * dp / CosGamma[i]);
                }
                // Расчет скоростей c n- й итерации
                for (int i = 1; i < N - 1; i++)
                {
                    double cw = mC[i];
                    double ce = mC[i + 1];
                    double cp = ce + cw;
                    VelocityPress[i] = (ce * ps[i + 2] - cp * ps[i + 1] + cw * ps[i]) / dx;
                    double ae = mB[i];
                    double aw = mB[i - 1];
                    double ap = ae + aw;
                    double ZetaOld = Zeta[i];
                    VelocityZeta[i] = (ae * Zeta0[i + 2] - ap * Zeta0[i + 1] + aw * Zeta0[i]) / dx;
                    VelocityZetaQ[i] = -(QbZeta[(i + 1) % N] - QbZeta[i % N]) / dx;
                    VelocityTau[i] = (-mA[i + 1] + mA[i]) / dx;
                    Velocity[i] = VelocityPress[i] + VelocityZeta[i] + VelocityTau[i];
                }
                // правая часть
                for (int i = 0; i < N - 1; i++)
                {
                    double cw = mC[i];
                    double ce = mC[i + 1];
                    double cp = ce + cw;
                    S[i + 1] = ce * ps[i + 2] - cp * ps[i + 1] + cw * ps[i] - mA[i + 1] + mA[i];
                }
                // Контроль сходимости
                double D = mB.Max();
                double dt_control = (0.5 * dx * dx / D) / dx;
                if (dt > dt_control)
                {
                    // dt = dt_control;
                    throw new Exception("Шаг по времени не удовлетворяет требованию устойчивости схемы dt = " + dt.ToString() + "требуемый dt = " + dt_control.ToString());
                }
                // граничные условия
                CalkBCondition(tau, ps);
                Zeta[N] = 2 * Zeta[N - 1] - Zeta[N - 2];
                // Метод решения системы
                if (flagSys)
                {
                    //  решение уравнения методом скользящего счета для неявной схемы
                    int indexZeta = 0;
                    for (indexZeta = 0; indexZeta < MaxIterNoLine; indexZeta++)
                    {
                        maxerr = 0;
                        // ход в право
                        for (int i = 1; i < N; i++)
                        {
                            // среднее гармоническое величиние коэффициентов проводимости ke,kw
                            double ae = mB[i];
                            double aw = mB[i - 1];
                            double ap0 = dx / dt;
                            double ap = ae + aw + ap0;
                            double ZetaOld = Zeta[i];
                            Zeta[i] = (ae * Zeta[i + 1] + aw * Zeta[i - 1] + ap0 * Zeta0[i] + S[i]) / ap;
                            double error = Math.Abs(Zeta[i] - ZetaOld);
                            if (error > maxerr)
                                maxerr = error;
                        }
                        //  ход в лево
                        for (int i = N - 1; i > 1; i--)
                        {
                            // среднее гармоническое величиние коэффициентов проводимости ke,kw
                            double ae = mB[i];
                            double aw = mB[i - 1];
                            double ap0 = dx / dt;
                            double ap = ae + aw + ap0;
                            double ZetaOld = Zeta[i];
                            Zeta[i] = (ae * Zeta[i + 1] + aw * Zeta[i - 1] + ap0 * Zeta0[i] + S[i]) / ap;
                            double error = Math.Abs(Zeta[i] - ZetaOld);
                            if (error > maxerr)
                                maxerr = error;
                        }
                        maxZeta = Zeta.Max() - Zeta.Min() + 0.0001;
                        double me = Math.Abs(maxerr / maxZeta * dt);
                        if (double.IsNaN(me) == true)
                            throw new Exception("Отсутствие сходимости метода");
                        if (me / maxZeta < eZeta)
                        {
                            break;
                        }
                        if (indexZeta == MaxIterNoLine - 1)
                            break;
                    }
                }
                else
                {
                    // Прогонка
                    double ap0 = dx / dt;
                    for (int i = 1; i < N; i++)
                    {
                        // среднее гармоническое величиние коэффициентов проводимости ke,kw
                        Ae[i] = mB[i];
                        Aw[i] = mB[i - 1];
                        Ap[i] = Ae[i] + Aw[i] + ap0;
                        Sc[i] = ap0 * Zeta0[i] + S[i];
                    }
                    TSolver solver = new TSolver(N);
                    solver.Solver(Aw, Ap, Ae, Sc, Zeta);
                }
                // Сглаживание дна по лавинной моделе
                Avalanche.Lavina(Zeta, tanphi, dx, 0.6, 0);

                //переопределение начального значения zeta для следующего шага по времени
                for (int j = 0; j < Zeta.Length; j++)
                    Zeta0[j] = Zeta[j];
            }
            catch (Exception e)
            {
                Message = e.Message;
                for (int j = 0; j < Zeta.Length; j++)
                    Zeta[j] = Zeta0[j];
            }
            return Zeta;
        }
    }
}
