using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    public class Shallow1DBase_Parameter
    {
        /// <summary>
        /// постоянный расход жидкости
        /// </summary>
        protected double _Q = 0.018495;
        /// <summary>
        /// скорость потока на входе в область
        /// </summary>
        protected double _U0 = 0.45;
        /// <summary>
        /// глубина потокана выходе
        /// </summary>
        protected double _H0 = 0.0411;
        /// <summary>
        /// гравитационная постоянная (м/с/с)
        /// </summary>
        protected double _g = 9.8;
        /// <summary>
        /// шероховатость по Манингу
        /// </summary>
        double _n = 0.03;
        /// <summary>
        /// плотность воды кг/м^3
        /// </summary>
        double _rho_w = 1000;
        //
        //[Description("Ускорение свободного падения"), Category("Гидродинамические параметры"), DisplayName("g")]
        [Browsable(false)]
        public double g
        {
            get { return _g; }
            set { _g = value; }
        }
        //
        [Description("Расход потока"), Category("Гидродинамические параметры"), DisplayName("Q")]
        public double Q
        {
            get { return _H0 * _U0; }
        }
        [Description("Шероховатость по Маннингу"), Category("Гидродинамические параметры"), DisplayName("n")]
        public double n
        {
            get { return _n; }
            set { _n = value; }
        }
        //[Description("Плотность воды, кг/м3"), Category("Гидродинамические параметры"), DisplayName("rho_w")]
        [Browsable(false)]
        public double rho_w
        {
            get { return _rho_w; }
            set { _rho_w = value; }
        }
    }
}
