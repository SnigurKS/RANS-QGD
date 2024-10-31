using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    [Serializable]
    public class OrthoParameter : DiffParameter
    {
        /// <summary>
        /// Коэффициент релаксации ортотропности
        /// </summary>
        double _RelaxOrtho = 0.9;
        /// <summary>
        /// Коэффициент релаксации ортотропности
        /// </summary>
        double _Tay = 0.9;
        //
        [DisplayName("Коэффициент релаксации ортотропности"), Category("Ортотропные параметры разбиения")]
        public double RelaxOrtho
        {
            get { return _RelaxOrtho; }
            set { _RelaxOrtho = value; }
        }
        [DisplayName("Коэффициент релаксации ортотропности общий"), Category("Ортотропные параметры разбиения")]
        public double Tay
        {
            get { return _Tay; }
            set { _Tay = value; }
        }
        //
        public OrthoParameter()
        { }
        /// <summary>
        /// Параметры для ортотропного разбиения расчетной области
        /// </summary>
        /// <param name="Nx">Количество узлов по x</param>
        /// <param name="Ny">Количество узлов по y</param>
        /// <param name="index">Тип сетки: 0 - треугольная, 1 - смешанная, 2 - четырехугольная</param>
        /// <param name="flagFirst">Флаг первичности сетки</param>
        /// <param name="RelaxOrtho">Коэффициент релаксации</param>
        /// <param name="Tay">Коэффициент релаксации</param>
        public OrthoParameter(int Nx, int Ny, int index, double P, double Q, double RelaxOrtho, double Tay)
        {
            this.Nx = Nx;
            this.Ny = Ny;
            this.index = index;
            this.P = P;
            this.Q = Q;
            this.RelaxOrtho = RelaxOrtho;
            this.Tay = Tay;
        }

    }
}
