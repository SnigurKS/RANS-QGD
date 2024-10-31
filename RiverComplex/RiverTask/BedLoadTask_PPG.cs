//-----------------------------------------------------------------------------------
//                      Реализация класса BedLoadTask_PPG 
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
//                               16.01.17
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
    /// Модель движения влекомых наносов  Петрова П.Г.
    /// </summary>
    public class BedLoadTask_PPG : BaseBedLoadTask
    {
        public override string Name
        {
            get
            {
                return "Модель базовая 1991";
            }
        }
        public BedLoadTask_PPG()
            : base()
        {
        }
        //
        public override void ReStartBaseBedLoadTask(BoundaryCondition BCBed, BedPhysicsParams param, double dx, int Nx, double dt)
        {
            base.ReStartBaseBedLoadTask(BCBed, param, dx, Nx, dt);
        }
        //
        public override void ReStartBaseBedLoadTask(BedPhysicsParams param, double dx, int Nx, double dt)
        {
            base.ReStartBaseBedLoadTask(param, dx, Nx, dt);
        }
        /// <summary>
        /// Мой метод решения уравнения Экснера по статье КИМ 2019
        /// </summary>
        /// <param name="Tau">напряжение в целых узлах</param>
        /// <param name="Nt">количество итераций по подзадаче для дна (обычно одна)</param>
        /// <param name="RS">рсход взвешенных наносов = alpha*W(Sz-S[i])/H</param>
        /// <returns>новая форма дна</returns>
        public override double[] MySolve(double[] Tau, double[] RS=null, double[] P = null, int Nt=1)
        {
            int Nx = Tau.Length;
            double[] dZ12 = new double[Nx];
            double[] Tau12 = new double[Nx];
            TauC12 = new double[Nx];
            double[] AbsTau12 = new double[Nx];
            double[] B12 = new double[Nx];
            double[] A12 = new double[Nx];
            //
            try
            {
                double DZ = 0, cosgam = 0, Fa = 0, q012 = 0, xi = 0;
                for (int k = 0; k < Nt; k++)
                {
                    for (int m = 0; m < MaxIterNoLine; m++)
                    {
                        // вычисление Tau и DZ в полуцелых узлах |--.--|--.--|
                        //                                       -> 0     1
                        for (int i = otstup - 1; i < Nx - 1; i++)
                        {
                            dZ12[i] = Zeta[i + 1] - Zeta[i];
                            Tau12[i] = 2.0 * Tau[i] * Tau[i + 1] / (Tau[i] + Tau[i + 1]); // (Tau[i] + Tau[i + 1]) / 2.0;//
                            AbsTau12[i] = Math.Abs(Tau12[i]);
                        }
                        // вычисление B в полуцелых узлах (+ вычисление tauc12)
                        for (int i = otstup - 1; i < Nx; i++)
                        {
                            DZ = dZ12[i];
                            cosgam = dx / Math.Sqrt(dx * dx + DZ * DZ);
                            Fa = (rhoS - rhoW) * g * tanphi;
                            TauC12[i] = 9.0 / 8.0 * kappa2 * Fa * d / cx * (1 + 1.0 / tanphi * DZ / dx);
                            q012 = 4.0 / 3.0 * rhoS / Math.Sqrt(rhoW) / kappa / Fa/ cosgam;
                            if (TauC12[i] < AbsTau12[i])
                                A12[i] = q012 * Tau12[i] * (Math.Sqrt(AbsTau12[i]) - Math.Sqrt(TauC12[i])) / rhoS / (1 - epsilon);
                            else 
                                A12[i] = 0;
                            // B всегда не равно нулю
                            B12[i] = q012 * AbsTau12[i] * (Math.Sqrt(AbsTau12[i]) - Math.Sqrt(TauC12[i]) / 2.0) / tanphi / rhoS / (1 - epsilon);
                        }
                        //// определение TauC неразмываемое на участке [0, otstup] (с 10% запасом)
                        //double tauCotstup = TauC12[otstup - 1] * (1 + 1.0 / tanphi * Math.Sign(TauC12[otstup - 1]) * dZ12[otstup - 1] / dx) * 1.1;
                        //double tauotstup = Tau[otstup - 1] * (1 + 1.0 / tanphi * Math.Sign(Tau[otstup - 1]) * dZ12[otstup - 1] / dx) * 1.1;
                        //if (tauotstup > tauCotstup)
                        //    tauCotstup = tauotstup;
                        //// установка TauC неразмываемое на участке [0, otstup]
                        //TauC12[otstup - 1] = tauCotstup;
                        //TauC12[otstup] = tauCotstup;
                        //
                        // вычисление Zeta в целых узлах, начиная со второго |--.--|--.--|--.--|
                        //                                                      kw P  ke
                        double ke = 0, kw = 0, a0p = 0, Ae = 0, Aw = 0, Ap = 0, TauC = 0, q0 = 0, sq = 0, dsq = 0, dTau = 0, RR = 0, x = 0, OldZ = 0, maxerror = 0;
                        double AbsTau_i = 0, Tauz = 0;
                        for (int i = otstup - 1; i < Nx - 1; i++)
                        {
                            AbsTau_i = Math.Abs(Tau[i]);
                            // 
                            ke = B12[i];
                            kw = B12[i - 1];
                            //
                            a0p = dx / dt;
                            Ae = ke / dx;
                            Aw = kw / dx;
                            Ap = Ae + Aw + a0p;
                            //расчет R и RR
                            DZ = 2.0 * Zeta[i] * Zeta[i + 1] / (Zeta[i] + Zeta[i + 1]) - 2.0 * Zeta[i - 1] * Zeta[i] / (Zeta[i - 1] + Zeta[i]);
                            cosgam = dx / Math.Sqrt(dx * dx + DZ * DZ);
                            Fa = (rhoS - rhoW) * g * tanphi;
                            TauC = 9.0 / 8.0 * kappa2 * Fa * d / cx;
                            //
                            if (RS != null)
                                RR = RS[i] / (rhoS) / (1 - epsilon);
                            else
                                RR = 0;
                            //
                            Tauz = TauC * (1 + 1.0 / tanphi * DZ / dx);
                            // как параболическое уравнение
                            if (AbsTau_i <= Tauz)
                                Sc[i] = -RR;
                            else
                                Sc[i] = -(A12[i] - A12[i - 1]) / dx - RR;
                            //
                            x = Sc[i] * dx + a0p * Zeta0[i];
                            // расчет Zeta
                            OldZ = Zeta[i];
                            Zeta[i] = (Ae * Zeta[i + 1] + Aw * Zeta[i - 1] + x) / Ap;
                            //
                            xi = Math.Sqrt(TauC / AbsTau_i);
                            q0 = 4.0 / 3.0 * rhoS / Math.Sqrt(rhoW) / kappa / Fa / cosgam;
                            Qb[i] = Math.Sign(Tau[i]) * q0 * Math.Sqrt(AbsTau_i * AbsTau_i * AbsTau_i) * (1 - xi - 1.0 / tanphi / cosgam * (1 - xi / 2.0) * ((Zeta[i] + Zeta[i - 1]) / 2.0 - (Zeta[i + 1] + Zeta[i]) / 2.0) / dx);
                            //
                            if (Zeta[i] != 0)
                                maxerror = Math.Max(Math.Abs(OldZ - Zeta[i]) / Zeta[i], maxerror);
                        }// Nx
                        //Zeta[0] = 2 * Zeta[1] - Zeta[2];
                        {
                            // расчет по Петрову
                            xi = Math.Sqrt(TauC12[Nx - 2] / AbsTau12[Nx - 2]);
                            double qq0 = 4.0 / 3.0 * Math.Sqrt(Math.Abs(Tau[Nx - 2] * Tau[Nx - 2] * Tau[Nx - 2])) / Math.Sqrt(rhoW) / (rhoS - rhoW) / kappa / g / tanphi / cosgam / (1 - epsilon);
                            double BB = qq0 / tanphi / cosgam * (1 - xi / 2.0);
                            double AA = qq0 * Math.Sign(Tau[Nx - 2]) * (1 - xi) / cosgam;
                            //
                            double QA = AA;
                            double QB = -BB * (Zeta[Nx - 1] - Zeta[Nx - 2]) / dx;
                            double QT = QA + QB;
                            //
                            // расчет по P
                            //double f = 0.01;
                            //double s = f * rhoS;
                            //double qq0 = 4.0 / 3.0 * Math.Sqrt(Math.Abs(Tau[Nx - 1] * Tau[Nx - 1] * Tau[Nx - 1])) / Math.Sqrt(rhoW) / (rhoS - rhoW) / kappa / g / tanphi / cosgam / (1 - epsilon);
                            //double BB = qq0 / tanphi / cosgam * (xi / 2.0 + (1 - xi) * (1 + s) / s);
                            //double AA = qq0 * Math.Sign(Tau[Nx - 1]) * (1 - xi);
                            //double CC = qq0 * (1 - xi) / s / tanphi;
                            ////
                            //double QA = Math.Sign(Tau[Nx - 2]) * AA;
                            //double QB = -BB * (Zeta[Nx - 2] - Zeta[Nx - 3]) / dx;
                            //double QC = CC / cosgam * (P[Nx - 2] - P[Nx - 3]) / dx;
                            //double QT = QA + QB + QC;
                            //
                            if (xi > 1)
                            {
                                AA = 0;
                                BB = 0;
                            }
                            //Zeta[Nx - 1] = Zeta[Nx - 2] + dx / BB * AA; // dqdx = 0 - идет вверх хвост
                            Zeta[Nx - 1] = Zeta[Nx - 2] + dx / BB * (AA - QT); // q = qtrans

                            //DZ = (Zeta[Nx - 1] - Zeta[Nx - 2]);
                            //cosgam = dx / Math.Sqrt(dx * dx + DZ * DZ);
                            //xi = Math.Sqrt(TauC12[Nx - 2] / Math.Abs(Tau12[Nx - 2]));
                            //double qq1 = 4.0 / 3.0 * Math.Sqrt(Math.Abs(Tau12[Nx - 2] * Tau12[Nx - 2] * Tau12[Nx - 2])) / Math.Sqrt(rhoW) / (rhoS - rhoW) / kappa / g / tanphi / cosgam / (1 - epsilon);
                            //double AA1p = qq1 * Math.Sign(Tau12[Nx - 2]) * (1 - xi);
                            //double AA1 = qq1 * Math.Sign(Tau12[Nx - 3]) * (1 - Math.Sqrt(TauC12[Nx - 3] / Math.Abs(Tau12[Nx - 3])));
                            //// 
                            //Zeta[Nx - 1] = Zeta[Nx - 2] + dx / B12[Nx - 2] * (AA1p + B12[Nx - 3] * (Zeta[Nx - 2] - Zeta[Nx - 3]) / dx - AA1 - Qtrans * dx * dx); // dqdx=qtrans - идет вниз
                            ////


                        }
                        //
                        if (maxerror < 1.0e-6)
                            m = MaxIterNoLine;
                    }// Nm // MaxIterNoLine
                    
                    //// Сглаживание дна по лавинной модели
                    Avalanche.Lavina(Zeta, tanphi, dx, 0.6, 0);

                    //переопределение начального значения zeta для следующего шага по времени
                    for (int j = 0; j < Zeta.Length; j++)
                        Zeta0[j] = Zeta[j];
                }// Nt
            }
            catch (Exception e)
            {
                Message = e.Message;
                for (int j = 0; j < Zeta.Length; j++)
                    Zeta[j] = Zeta0[j];
            }
            //
            return Zeta;
        }
        /// Модель Петрова П.Г.
        /// </summary>
        /// <param name="tau">Напряжение</param>
        /// <param name="A">A</param>
        /// <param name="B">B</param>
        /// <param name="qA">qA</param>
        private void CalkABC(double tau, out double A, out double B, out double qA)
        {
            double chi = Math.Sqrt(tau0 / Math.Abs(tau));
            if (chi < 1)
            {
                A = 1 - chi;
                B = (1 - chi / 2.0) / tanphi;
            }
            else
            {
                A = 0;
                B = 0;
            }
            qA = q0 * tau * Math.Sqrt(Math.Abs(tau));
        }
        /// <summary>
        /// Задание граничных условий
        /// </summary>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="P">придонное давление в модели не используется</param>
        protected override void CalkBCondition(double[] tau, double[] ps = null)
        {
            double dz;
            double q_in = BCBed.InletValue;
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
                    {
                        CalkABC(tau[0], out A, out B, out qA);
                        //dz = -dt / dx * (qA * (A - B * (Zeta[1] - Zeta[0]) / dx) - q_in);
                        dz = dx / B * (q_in / qA - A);
                        Zeta[0] = Zeta[0] + dz;
                    }
                    break;
                case TypeBoundaryCondition.Periodic_boundary_conditions:
                    for (int j = 0; j < 2; j++)
                    {
                        CalkABC(tau[0], out A, out B, out qA);
                        Zeta[0] = Zeta[1] + dx / B * (q_in / qA - A);
                        Zeta[Zeta.Length - 1] = Zeta[Zeta.Length - 2];
                    }
                    break;
            }
            double q_out = BCBed.OutletValue;
            switch (BCBed.Outlet)
            {
                case TypeBoundaryCondition.Transit_Feed:
                    // Транзитный_поток - ровное дно
                   // Zeta[Zeta.Length - 1] = Zeta[Zeta.Length - 2];
                    break;
                case TypeBoundaryCondition.Dirichlet_boundary_conditions:
                    {
                        ZN = q_out;
                        Zeta[N] = ZN;
                    }
                    break;
                case TypeBoundaryCondition.Neumann_boundary_conditions:
                    {
                        CalkABC(tau[N - 1], out A, out B, out qA);
                        //dz = dt / dx * (q_out - qA * (A - B * (Zeta[N] - Zeta[N-1]) / dx));
                        dz = dx / B * (q_out / qA - A);
                        Zeta[N] = Zeta0[N - 1] - dz;
                    }
                    break;
            }
        }
        //
        public override double CalculateRate(double Tau, double J, double dp = 0)
        {
            // расчет коэффициентов 
            CalkABC(Tau, out A, out B, out qA); ;
            // косинус гамма
            double CosGamma = Math.Sqrt(1 / (1 + J * J));
            //
            qA /= CosGamma;
            // Расход наносов
            return qA * (A - B * J / CosGamma);
        }
        //
        bool flag = true;
        /// <summary>
        /// Вычисление изменений формы донной поверхности на 1 шаге по времени 
        /// Модель движения донных наносов Петрова П.Г.
        /// </summary>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="P">придонное давление</param>
        /// <returns>новая форма дна</returns>
        public override double[] Solve(double[] tau, double[] P=null, double[] Q0=null, double[] Q1=null)
        {
            
            try
            {
                double maxerr = 0;
                double maxZeta = 0;
                double dz;
                double[] mA = new double[N];
                double[] mB = new double[N];
                double[] S = new double[Count];
                
                for (int i = 0; i < N; i++)
                {
                    // расчет коэффициентов 
                    CalkABC(tau[i], out A, out B, out qA);
                    if (i < otstup)
                    {
                        A = 0;
                        B = 0;
                        qA = 0;
                    }
                    // производная (dzeta/dx)_i+1/2 
                    dz = (Zeta0[i + 1] - Zeta0[i]) / dx;
                    // косинус гамма
                    CosGamma[i] = Math.Sqrt(1 / (1 + dz * dz));
                    //
                    qA /= CosGamma[i];
                    mA[i] = qA * A;
                    mB[i] = Math.Abs(qA * B / dx);
                    QbTau[i] = qA * A;
                    QbZeta[i] = -qA * B * dz / CosGamma[i];
                    // Расход наносов
                    Qb[i] = qA * (A - B * dz / CosGamma[i]); //* rhoS * (1 - epsilon);
                }
                // Расчет скоростей c n- й итерации
                for (int i = 1; i < N - 1; i++)
                {
                    VelocityPress[i] = 0;
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
                    S[i + 1] = -mA[i + 1] + mA[i];
                }
                // Контроль сходимости
                double D = mB.Max();
                double dt_control = 0.5 * dx * dx / D;
                if (dt > dt_control)
                {
                    dt = dt_control * 0.5;
                    //throw new Exception("Шаг по времени не удовлетворяет требованию устойчивости схемы dt = " + dt.ToString() + "требуемый dt = " + dt_control.ToString());
                }
                //////через q
                //double[] Qq = new double[Count - 1];
                //Qq = Qb;
                //double E = 1.0; //(1 - epsilon) / rhoS;
                //for (int i = otstup; i < Count - 1; i++)
                //{
                //    double dQ = (Qq[i] - Qq[i - 1]);
                //    double dZ = dt / dx * dQ * E;
                //    //if (Math.Abs(dZ) < 1.0E-16)
                //    //    dZ = 0;
                //    // Схема!
                //    Zeta[i] = Zeta[i] - dZ;
                //}
                //CalkBCondition(tau);
                //Zeta[0] = 2 * Zeta[1] - Zeta[2];
                //Zeta[N] = 2 * Zeta[N - 1] - Zeta[N - 2]; // -- работает нормально
                ////Zeta[N] = Zeta[N - 1] - 0.00416 * dx; //-- значительная просадка решения
  //--??             
                // граничные условия
                CalkBCondition(tau);
                
                //Zeta[0] = 2 * Zeta[1] - Zeta[2];
                //Zeta[N] = 2 * Zeta[N - 1] - Zeta[N - 2];
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
                        //
                        if (!flag)
                        {
                            if (me / maxZeta < eZeta)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (indexZeta == MaxIterNoLine - 1)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (indexZeta == MaxIterNoLine - 1)
                            break;
                    }
                }
                else
                {
                    double ap0 = dx / dt;
                    for (int i = 1; i < N; i++)
                    {
                        Ae[i] = mB[i];
                        Aw[i] = mB[i - 1];
                        Ap[i] = Ae[i] + Aw[i] + ap0;
                        Sc[i] = ap0 * Zeta0[i] + S[i];
                    }
                    TSolver solver = new TSolver(N);
                    solver.Solver(Aw, Ap, Ae, Sc, Zeta);
                }
                //// Сглаживание дна по лавинной моделе
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
        public double[] SSolve(double[] tau, double[] P = null, double[] Q0 = null, double[] Q1 = null)
        {
            double qIn = BCBed.InletValue;
            double qOut = BCBed.OutletValue;
            try
            {
                double maxerr = 0;
                double maxZeta = 0;
                double dz;
                double[] mA = new double[N];
                double[] mB = new double[N];
                double[] S = new double[Count];

                for (int i = 0; i < N; i++)
                {
                    // расчет коэффициентов 
                    CalkABC(tau[i], out A, out B, out qA);
                    // производная (dzeta/dx)_i+1/2 
                    dz = (Zeta0[i + 1] - Zeta0[i]) / dx;
                    // косинус гамма
                    CosGamma[i] = Math.Sqrt(1 / (1 + dz * dz));
                    //
                    qA /= CosGamma[i];
                    mA[i] = qA * A;
                    mB[i] = Math.Abs(qA * B / dx);
                    QbTau[i] = qA * A;
                    QbZeta[i] = -qA * B * dz / CosGamma[i];
                    // Расход наносов
                    Qb[i] = qA * (A - B * dz / CosGamma[i]);
                }
                // Расчет скоростей c n- й итерации
                for (int i = 1; i < N - 1; i++)
                {
                    VelocityPress[i] = 0;
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
                    S[i + 1] = -mA[i + 1] + mA[i];
                }
                // Контроль сходимости
                double D = mB.Max();
                double dt_control = 0.5 * dx * dx / D;
                if (dt > dt_control)
                {
                    // dt = dt_control;
                    throw new Exception("Шаг по времени не удовлетворяет требованию устойчивости схемы dt = " + dt.ToString() + "требуемый dt = " + dt_control.ToString());
                }
                // граничные условия
                CalkBCondition(tau);
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
                        //
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
                    double ap0 = dx / dt;
                    for (int i = 1; i < N; i++)
                    {
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
