//-----------------------------------------------------------------------------------
//       Реализация класса BedPhysicsParams определяющего физические параметры задачи
//                                  интерфейсный класс моделя
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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace RiverTaskLibrary
{
    [Serializable]
    /// <summary>
    /// Класс параметров для задачи о донных деформациях,
    /// предназначенный для работы с визуальными интерфейсами
    /// </summary>
    public class BedPhysicsParams
    {
        #region                             River task parameters
        /// <summary>
        /// плотность воды
        /// </summary>
        private double _rhoW = 1000;
        /// <summary>
        /// плотность песка
        /// </summary>
        private double _rhoS = 2650;
        /// <summary>
        /// тангенс угла внутреннего трения
        /// </summary>
        private double _tf = 0.3299356669; // phi = 32
        /// <summary>
        /// пористость донного материала
        /// </summary>
        private double _eps = 0.5f;
        /// <summary>
        /// средний диамер частиц (м)
        /// </summary>
        private double _d = 0.00016;
        /// <summary>
        /// коэффициент лобового сопротивления частиц
        /// </summary>
        private double _cx = 0.5f;
        /// <summary>
        /// постоянная Кармана
        /// </summary>
        private double _kappa = 0.2516187710;
        /// <summary>
        /// Гравитационная постоянная
        /// </summary>
        private double _g = 9.8;
        /// <summary>
        /// Количество неразмываемых узлов 
        /// </summary>
        private int _otstup = 100;
       
        #endregion
        //
        #region                                 River parameters properties
        [Description("Ускорение свбодного падения"), Category("Русловые параметры"), DisplayName("g")]
        public double g
        {
            get { return _g; }
            set { _g = value; }
        }
        [Description("Плотность воды, кг/м3"), Category("Русловые параметры"), DisplayName("rho_w")]
        public double rhoW
        {
            get { return _rhoW; }
            set { _rhoW = value; }
        }
        [Description("Плотность грунта, кг/м3"), Category("Русловые параметры"), DisplayName("rho_s")]
        public double rhoS
        {
            get { return _rhoS; }
            set { _rhoS = value; }
        }
        [Description("Тангенс phi"), Category("Русловые параметры"), DisplayName("tg(phi)")]
        public double tf
        {
            get { return _tf; }
            set { _tf = Math.Tan(value / 180 * Math.PI); }
        }
        [Description("Пористость грунта"), Category("Русловые параметры"), DisplayName("epsilon")]
        public double eps
        {
            get { return _eps; }
            set { _eps = value; }
        }
        [Description("Диаметр частиц, м"), Category("Русловые параметры"), DisplayName("d")]
        public double d
        {
            get { return _d; }
            set { _d = value; }
        }
        [Description("Лобовое сопротивление"), Category("Русловые параметры"), DisplayName("cx")]
        public double cx
        {
            get { return _cx; }
            set { _cx = value; }
        }
        [Description("Постоянная Кармана"), Category("Русловые параметры"), DisplayName("kappa")]
        public double kappa
        {
            get { return _kappa; }
            set { _kappa = value; }
        }
        [DisplayName("Неразмываемые узлы"), Category("Русловые параметры")]
        public int otstup
        {
            get { return _otstup; }
            set { _otstup = value; }
        }
        //
        #endregion
    }

    
}
