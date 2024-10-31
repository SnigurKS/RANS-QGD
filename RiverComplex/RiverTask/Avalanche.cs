//---------------------------------------------------------------------------
//      Реализация класса Avalancheвычисляющего осыпание склона из несвязного 
//      материала. Осыпание склона происходит при превышении уклона склона угла 
//      внутреннего трения для материала формирующего склон.
//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//            Модуль BedLoadLibrary для расчета донных деформаций 
//                (учет движения только влекомых наносов)
//                              Потапов И.И.
//                              Снигур К. С.
//                        - (C) Copyright 2017 -
//                          ALL RIGHT RESERVED
//                               16.01.17
//---------------------------------------------------------------------------
using System;
//---------------------------------------------------------------------------
namespace RiverTaskLibrary
{
    class Avalanche
    {
        /// <summary>
        /// Метод лавинного обрушения правого склонов 
        /// </summary>
        /// <param name="Z">массив донных отметок</param>
        /// <param name="tf">тангенс внутреннено трения **</param>
        /// <param name="ds">шаг сетки</param>
        /// <param name="Step">шаг остановки процесса, при 0 лавина проходит полностью</param>
        public static void Lavina(double[] Z, double tf, double ds, double Relax, int Step)
        {
            LavinaRight(Z, tf, ds, Relax, Step);
            LavinaLeft(Z, tf, ds, Relax, Step);
        }
        /// <summary>
        /// Метод лавинного обрушения правого склона, при ходе с правой стороны
        /// </summary>
        /// <param name="Z">массив донных отметок</param>
        /// <param name="tf">тангенс внутреннено трения **</param>
        /// <param name="ds">шаг сетки</param>
        /// <param name="Step">шаг остановки процесса, при 0 лавина проходит полностью</param>
        public static void LavinaRight(double[] Z, double tf, double ds, double Relax, int Step)
        {
            double dh = ds * tf;
            int idx = 0;
            for (int i = 1; i < Z.Length; i++)
            {
                double dz = Z[i] - Z[i - 1];
                if (dz < 0)
                {
                    if (-dz > dh)
                    {
                        double delta = (dh + dz) * Relax;
                        Z[i] -= delta;
                        Z[i - 1] += delta;
                        idx++;
                        if (idx == Step)
                            break;
                    }

                }
            }
        }
        /// <summary>
        /// Метод лавинного обрушения правого склона, при ходе с левой стороны
        /// </summary>
        /// <param name="Z">массив донных отметок</param>
        /// <param name="tf">тангенс внутреннено трения **</param>
        /// <param name="ds">шаг сетки</param>
        /// <param name="Step">шаг остановки процесса, при 0 лавина проходит полностью</param>
        public static void LavinaLeft(double[] Z, double tf, double ds, double Relax, int Step)
        {
            double dh = ds * tf;
            int idx = 0;
            for (int i = 1; i < Z.Length; i++)
            {
                int k = Z.Length - i - 1;
                int kp = Z.Length - i;
                double dz = Z[kp] - Z[k];
                if (dz > 0)
                {
                    if (dz > dh)
                    {
                        double delta = (dz - dh) * Relax;
                        Z[k] += delta;
                        Z[kp] -= delta;
                        idx++;
                        if (idx == Step)
                            break;
                    }

                }
            }
        }
    }
}
