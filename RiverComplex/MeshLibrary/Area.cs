using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    [Serializable]
    public class Area : SimpleArea
    {
        public int ID;
        //           T
        //    0 ------>----
        //     |----X1-----|
        // L  \|/         /|\    R
        //     |           |
        //     |----X2-----|
        //     ------>------
        //           B
        // Есть в родительском классе
        ///// <summary>
        ///// X-координаты верхней границы  
        ///// </summary>
        //double[] XTop;
        ///// <summary>
        ///// Y-координаты верхней границы
        ///// </summary>
        //double[] YTop;
        ///// <summary>
        ///// X-координаты нижней границы  
        ///// </summary>
        //double[] XBottom;
        ///// <summary>
        ///// Y-координаты нижней границы
        ///// </summary>
        //double[] YBottom;
        /// <summary>
        /// X-координаты первой промежуточной границы  
        /// </summary>
        public double[] X1;
        /// <summary>
        /// Y-координаты первой промежуточной границы
        /// </summary>
        public double[] Y1;
        /// <summary>
        /// X-координаты второй промежуточной границы  
        /// </summary>
        public double[] X2;
        /// <summary>
        /// Y-координаты второй промежуточной границы
        /// </summary>
        public double[] Y2;
        /// <summary>
        /// Толщина слоя сгущения на верхней границе расчетной области
        /// </summary>
        public double topLayer = 0;
        /// <summary>
        /// Толщина слоя сгущения на нижней границе расчетной области
        /// </summary>
        public double bottomLayer = 0;
        /// <summary>
        /// Количество горизонтальных подобластей расчетной области
        /// </summary>
        public int partsCount = 1;
        public int PartsCount
        {
            get { return partsCount; }
            set { partsCount = value; }
        }
        //
        public Area(double[] xTop, double[] yTop, double[] xBottom, double[] yBottom)
            :base(xTop, yTop, xBottom, yBottom) 
        { }
        //
        /// <summary>
        /// Задание расчетной области с 1 или 2-мя подслоями, с возможностью указания разбиения по горизонтали
        /// </summary>
        /// <param name="XTop">X-координаты верхней границы</param>
        /// <param name="YTop">Y-координаты верхней границы</param>
        /// <param name="XBottom">X-координаты нижней границы</param>
        /// <param name="YBottom">Y-координаты нижней границы</param>
        /// <param name="TopLayer">Толщина верхнего подслоя</param>
        /// <param name="BottomLayer">Толщина нижнейго подслоя</param>
        /// <param name="N">Количество подобластей по горизонтали</param>
        public Area(double[] XTop, double[] YTop, double[] XBottom, double[] YBottom, double BottomLayer, double TopLayer = 0, int N = 1) 
        {
            _Nx = XBottom.Length;
            partsCount = N;
            //
            this.XTop = new double[_Nx];
            this.YTop = new double[_Nx];
            this.XBottom = new double[_Nx];
            this.YBottom = new double[_Nx];
            for (int k = 0; k < _Nx; k++)
            {
                this.XTop[k] = XTop[k];
                this.YTop[k] = YTop[k];
                this.XBottom[k] = XBottom[k];
                this.YBottom[k] = YBottom[k];
            }
            //
            this.bottomLayer = BottomLayer;
            this.topLayer = TopLayer;
            //
            if (BottomLayer != 0)
            {
                X2 = new double[_Nx];
                Y2 = new double[_Nx];
                for (int i = 0; i < _Nx; i++)
                {
                    X2[i] = XBottom[i];
                    Y2[i] = YBottom[i] + bottomLayer;
                }
            }
            //
            if (topLayer != 0)
            {
                X1 = new double[_Nx];
                Y1 = new double[_Nx];
                for (int i = 0; i < _Nx; i++)
                {
                    X1[i] = XTop[i];
                    Y1[i] = YTop[i] - topLayer;
                }
            }

        }
        /// <summary>
        /// Геренация прямоугольной области
        /// </summary>
        /// <param name="LeftTopCornerX">X-координата левого верхнейго угла</param>
        /// <param name="LeftTopCornerY">Y-координата левого верхнейго угла</param>
        /// <param name="RightBottomCornerX">X-координата правого нижнего угла</param>
        /// <param name="RightBottomCornerY">Y-координата правого нижнейго угла</param>
        /// <param name="Nx">Количество узлов по x</param>
        /// <param name="N">Количество секций по горизонтали</param>
        public Area(double LeftTopCornerX, double LeftTopCornerY, double RightBottomCornerX, double RightBottomCornerY, int Nx, int N)
        {
            partsCount = N;
            this.XTop = new double[Nx];
            this.YTop = new double[Nx];
            this.XBottom = new double[Nx];
            this.YBottom = new double[Nx];
            //
            double dx = (RightBottomCornerX- LeftTopCornerX)/(Nx-1);
            for (int i = 0; i < Nx; i++)
            {
                XTop[i] = dx * i;//RightBottomCornerX - dx * i;
                YTop[i] = LeftTopCornerY;
                XBottom[i] = dx * i;
                YBottom[i] = RightBottomCornerY;
            }
            _Nx = Nx;
        }
        //
        /// <summary>
        /// Метод генерирует координаты промежуточных поверхностей (или одной поверхности) по методу Флетчера
        /// </summary>
        void GenerateMiddleSurfaces()
        { }
        //
        public SimpleArea[] GenerateSubAreas()
        {
            return null;
        }
    }
}
