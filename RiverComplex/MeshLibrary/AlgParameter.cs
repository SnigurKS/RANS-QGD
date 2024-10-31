using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    [Serializable]
    public class AlgParameter : Parameter
    {

        public AlgParameter()
        {
        }
        /// <summary>
        /// Параметры для алгебраического разбиения расчетной области
        /// </summary>
        /// <param name="Nx">Количество узлов по x</param>
        /// <param name="Ny">Количество узлов по y</param>
        /// <param name="index">Тип сетки: 0 - треугольная, 1 - смешанная, 2 - четырехугольная</param>
        /// <param name="flagFirst">Флаг первичности сетки</param>
        public AlgParameter(int Nx, int Ny, int index, double P, double Q)
        {
            this.Nx = Nx;
            this.Ny = Ny;
            this.index = index;
            this.P = P;
            this.Q = Q;
        }
    }
    //
}
