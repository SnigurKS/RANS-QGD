using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    public class ElizShallow1D_Parameter : Shallow1DBase_Parameter
    {
        #region Основные параметры
        /// <summary>
        /// Вязкость потока
        /// </summary>
        double _mu = 0.05;
       
        
        #endregion
        // 
        [Description("Динамическая вязкость Па·с"), Category("Гидродинамические параметры"), DisplayName("Mu")]
        public double Mu
        {
            get { return _mu; }
            set { _mu = value; }
        }
        //
        [Description("Глубина потока на выходе из расчетной области"), Category("Гидродинамические параметры"), DisplayName("H_out")]
        public double Hout
        {
            get { return _H0; }
            set { _H0 = value; }

        }
        //
        [Description("Скорость потока на выходе из расчетной области"), Category("Гидродинамические параметры"), DisplayName("U_out")]
        //[Description("Скорость потока на входе в расчетную область"), Category("Гидродинамические параметры"), DisplayName("U_in")]
        public double Uout
        {
            get { return _U0; }
            set { _U0 = value; }
        }
        //
       
        //
       
    }
}
