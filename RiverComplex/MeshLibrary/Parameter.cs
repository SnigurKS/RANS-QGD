using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    [Serializable]
    public abstract class Parameter
    {
        public int Method = 0;
        /// <summary>
        /// Количество узлов по горизонтали
        /// </summary>
        int _Nx = 100;
        /// <summary>
        /// Количество узлов по вертикали
        /// </summary>
        int _Ny = 20;
        /// <summary>
        /// Параметр разбиения сетки: 0 - треугольная, 1 - прямоугольная, 2 - смешанная
        /// </summary>
        int _index = 0;
        /// <summary>
        /// Регулятор наклона. Сгущение на верхнюю границу --> 0, сгущение на нижнюю границу --> 2
        /// </summary>
        double _P = 1;
        /// <summary>
        /// Демпфирующий параметр
        /// </summary>
        double _Q = 1;
        //
        [Description("Узлы по x"), Category("Параметры сетки"), DisplayName("Nx")]
        public int Nx
        {
            get { return _Nx; }
            set { _Nx = value; }
        }
        [Description("Узлы по y"), Category("Параметры сетки"), DisplayName("Ny")]
        public int Ny
        {
            get { return _Ny; }
            set { _Ny = value; }
        }
        [Description("Тип сетки: 0 - треугольная, 1 - смешанная, 2 - четырехугольная"), Category("Параметры сетки"), DisplayName("Тип сетки")]
        public int index
        {
            get { return _index; }
            set { _index = value % 3; }
        }
        //
        [Description("Регулятор наклона (0;2). Сгущение на верхнюю границу --> 0, сгущение на нижнюю границу --> 2"), Category("Параметры сетки"), DisplayName("P")]
        public double P
        {
            get { return _P; }
            set { _P = value; }
        }
        [Description("Демпфирующий параметр"), Category("Параметры сетки"), DisplayName("Q")]
        public double Q
        {
            get { return _Q; }
            set { _Q = value; }
        }
        //
        public Parameter()
        { }
        /// <summary>
        /// Основные параметры для разбиения расчетной области
        /// </summary>
        /// <param name="Nx">Количество узлов по x</param>
        /// <param name="Ny">Количество узлов по y</param>
        /// <param name="index">Тип сетки: 0 - треугольная, 1 - смешанная, 2 - четырехугольная</param>
        public Parameter(int Nx, int Ny, int index, double P, double Q)
        {
            this.Nx = Nx;
            this.Ny = Ny;
            this.index = index;
            this.P = P;
            this.Q = Q;
        }
    }
}
