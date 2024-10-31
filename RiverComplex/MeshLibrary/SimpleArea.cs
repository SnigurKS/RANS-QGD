using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshLibrary
{
    public class SimpleArea
    {
        protected int _Nx;
        public int Nx { get { return _Nx; } }
        /// <summary>
        /// X-координаты верхней границы  
        /// </summary>
        public double[] XTop;
        /// <summary>
        /// Y-координаты верхней границы
        /// </summary>
        public double[] YTop;
        /// <summary>
        /// X-координаты нижней границы  
        /// </summary>
        public double[] XBottom;
        /// <summary>
        /// Y-координаты нижней границы
        /// </summary>
        public double[] YBottom;
        //
        public SimpleArea()
        { }
        public SimpleArea(double[] xTop, double[] yTop, double[] xBottom, double[] yBottom) 
        {
            _Nx = xBottom.Length;
            this.XTop = new double[_Nx];
            this.YTop = new double[_Nx];
            this.XBottom = new double[_Nx];
            this.YBottom = new double[_Nx];
            for (int k = 0; k < _Nx; k++)
            {
                this.XTop[k] = xTop[k];
                this.YTop[k] = yTop[k];
                this.XBottom[k] = xBottom[k];
                this.YBottom[k] = yBottom[k];
            }
            _Nx = xBottom.Length;
        }
    }
}
