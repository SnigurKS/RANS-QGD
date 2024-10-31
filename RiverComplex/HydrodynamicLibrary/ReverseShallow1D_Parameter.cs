using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicLibrary
{
    public class ReverseShallow1D_Parameter : Shallow1DBase_Parameter
    {
        [Description("Глубина потока на выходе из расчетной области"), Category("Гидродинамические параметры"), DisplayName("H_out")]
        public double Hout
        {
            get { return _H0; }
            set { _H0 = value; }

        }
        //
        [Description("Скорость потока на выходе из расчетной области"), Category("Гидродинамические параметры"), DisplayName("U_out")]
        public double Uout
        {
            get { return _U0; }
            set { _U0 = value; }
        }
    }
}
