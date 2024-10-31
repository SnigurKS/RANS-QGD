using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    [Serializable]
    public class WElizParameter
    {
        #region                             Hydrodynamic task parameters
        ///
        /// <summary>
        /// число Рейнольдса
        /// </summary>
        double _Re = 723.5f;
        /// штраф неразрывности в давлении
        /// </summary>
        double _alpha_n = 0.005f;
        /// <summary>
        /// штраф регуляризационной части в давлении
        /// </summary>
        double _alpha_r = 0.005f;
        ///// <summary>
        ///// штраф фиктивного источника на правой границе
        ///// </summary>
        //double _alpha_p = 0.005f;
        /// <summary>
        /// Временной шаг для расчета гидродинамки
        /// </summary>
        double _dt = 0.001f;
        /// <summary>
        /// Время размыва дна = время расчета подзадачи гидродинамики
        /// </summary>
        double _dtGlobal = 0.001f;
        /// <summary>
        /// Параметр регуляризации
        /// </summary>
        double _tau = 0.0001f;
        /// <summary>
        /// Расход жидкости на входе
        /// </summary>
        double _Q = 0.0369f;
        /// <summary>
        /// Релаксация давления
        /// </summary>
        double _relaxP = 1;
        /// <summary>
        /// Динамическая вязкость жидкости
        /// </summary>
        double _mu = 0.051f;
        /// <summary>
        /// Кинематическая вязкость жидкости
        /// </summary>
        double _nu_m = 0.051f / 1000f;
        /// <summary>
        /// Максимальная допустимая погрешность по давлению
        /// </summary>
        double _errP = 0.001f;
        /// <summary>
        /// Свободная поверхность (есть или нет)
        /// </summary>
        bool _surf_flag = false;
        /// <summary>
        /// Коэффициент регуляции поступления кинетической энергии через правую границу
        /// </summary>
        double _delta = 0.05;
        #endregion
        //
        #region  Hydrodynamic parameters properties
        [DisplayName ("Q"), Description("Расход потока, м2/с"), Category("Гидродинамические параметры")]
        public double Q
        {
            get { return _Q; }
            set { _Q = value; }
        }
        [DisplayName("alpha_n"), Description("Релаксация неразрывности"), Category("Гидродинамические параметры")]
        public double alpha_n
        {
            get { return _alpha_n; }
            set { _alpha_n = value; }
        }
        [DisplayName("alpha_r"), Description("Релаксация регуляризации"), Category("Гидродинамические параметры")]
        public double alpha_r
        {
            get { return _alpha_r; }
            set { _alpha_r = value; }
        }
        //[DisplayName("alpha_p"), Description("Регуляция фиктивного источника на правой границе"), Category("Гидродинамические параметры")]
        //public double alpha_p
        //{
        //    get { return _alpha_p; }
        //    set { _alpha_p = value; }
        //}
        [Browsable(false)]
        public double time_b
        {
            get { return _dtGlobal; }
            set { _dtGlobal = value; }
        }
        [DisplayName("HydroIteration"), Description("Количество итераций"), Category("Гидродинамические параметры")]
        public int iter
        {
            get { return Convert.ToInt32(time_b / _dt); }
        }
        [DisplayName("dt_local"), Description("Локальный шаг по времени, с"), Category("Гидродинамические параметры")]
        public double dt_local
        {
            get { return _dt; }
            set { _dt = value; }
        }
        [DisplayName("tau"), Description("Параметр регуляризации, с. \nЕсли ввести в поле -dx, то посчитается значение по формуле alpha*dx/cs, где alpha=1, сs - скорость звука в воде 1500 м/с"), Category("Гидродинамические параметры")]
        public double tau
        {
            get { return _tau; }
            set {
                if (value < 0)
                {
                    _tau = -value / 1500.0;
                }
                else
                    _tau = value; 
            }
        }
        [DisplayName("Re"), Description("Рейнольдс"), Category("Гидродинамические параметры"), Browsable(false)]
        public double Re
        {
            get { return _Re; }
            set { _Re = value; }
        }
        [DisplayName("Relaxation P"), Description("Релаксация давления"), Category("Гидродинамические параметры")]
        public double relaxP
        {
            get { return _relaxP; }
            set { _relaxP = value; }
        }
        [DisplayName("Mu"), Description("Динамическая вязкость Па·с"), Category("Гидродинамические параметры")]
        public double mu
        {
            get { return _mu; }
            set { _mu = value; }
        }
        [DisplayName("Nu"), Description("Кинематическая вязкость, м2/с"), Category("Гидродинамические параметры")]
        public double nu_m
        {
            get { return _mu / 1000; }
        }
        [DisplayName("ErrorP"), Description("Погрешность по давлению"), Category("Гидродинамические параметры")]
        public double errP
        {
            get { return _errP; }
            set { _errP = value; }
        }
        [DisplayName("Free surface"), Description("Свободная поверхность (есть или нет)"), Category("Гидродинамические параметры")]
        public bool surf_flag
        {
            get { return _surf_flag; }
            set { _surf_flag = value; }
        }
        [DisplayName("delta"), Description("Коэффициент поступления кинетической энергии через границу выхода"), Category("Гидродинамические параметры")]
        public double delta
        {
            get { return _delta; }
            set { _delta = value; }
        }
        //
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Re">Рейнольдс</param>
        /// <param name="alpha_n">Релаксация неразрывности</param>
        /// <param name="alpha_r">Релаксация регуляризации</param>
        /// <param name="time_b">Время размыва</param>
        /// <param name="dt">Шаг по времени, с</param>
        /// <param name="tau">Параметр регуляризации, с</param>
        /// <param name="Q">Расход потока, м2/с</param>
        /// <param name="relaxP">Релаксация давления</param>
        /// <param name="mu">Динамическая вязкость Па·с</param>
        /// <param name="errP">Погрешность по давлению</param>
        /// <param name="surf_flag">Свободная поверхность (есть или нет)</param>
        /// <param name="delta">Коэффициент поступления кинетической энергии через границу выхода (delta --> 0)</param>
        public WElizParameter(double Re, double alpha_n, double alpha_r, double time_b, double dt, double tau, double Q, double relaxP, double mu, double errP, bool surf_flag, double delta)
        {
            _Re = Re;
            _alpha_n = alpha_n;
            _alpha_r = alpha_r;
            _dtGlobal = time_b;
            _tau = tau;
            _Q = Q;
            _relaxP = relaxP;
            _mu = mu;
            _errP = errP;
            _surf_flag = surf_flag;
            _delta = delta;
        }
    }
}
