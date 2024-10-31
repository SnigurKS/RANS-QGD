using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    public abstract class MeshGenerator
    {
        public bool OpenCL = false;
        public bool Cuda = false;
        string exception = "";
        protected double[] X, Y;
        protected double P = 1, Q = 1;
        public Parameter p;
        protected System.Diagnostics.Stopwatch stopW1 = new System.Diagnostics.Stopwatch();
        protected System.Diagnostics.Stopwatch stopCalc = new System.Diagnostics.Stopwatch();
        protected System.Diagnostics.Stopwatch stopAll = new System.Diagnostics.Stopwatch();
        public TimeSpan timeTransrort;
        public TimeSpan timeCalculate;
        public TimeSpan timeAll;
        public abstract string Name { get; }
        public Mesh Generate(SimpleArea sArea)
        {
            GenerateCoords(sArea);
            p.Nx = sArea.Nx;
            //
            if (p.index == 0)
                return GenerateTriangleMesh();
            else if (p.index == 1)
                return GenerateQuadroMesh();
            else
                return GenerateMixedMesh();
        }
        abstract protected void GenerateCoords(SimpleArea sArea);
        protected Mesh GenerateMixedMesh()
        {
            return null;
        }
        protected Mesh GenerateQuadroMesh()
        {
            return null;
        }
        //
        //
        /// <summary>
        /// функция растяжения линии
        /// </summary>
        /// <param name="P">регулятор наклона</param>
        /// <param name="Q">демпфирующий параметр</param>
        /// <returns>массив от 0 до 1 с распределением точек</returns>
        protected double[] Stretch(double P, double Q, int CountPoints)
        {
            double[] S = new double[CountPoints];
            int AN = CountPoints - 1;
            double DETA = 1.0f / AN;
            double TQI = (double)(1.0f / Math.Tanh(Q));
            double AL, DUM, ETA;
            for (int i = 0; i < CountPoints; i++)
            {
                AL = i;
                ETA = AL * DETA;
                DUM = Q * (1 - ETA);
                DUM = 1 - Math.Tanh(DUM) * TQI;
                S[i] = P * ETA + (1 - P) * DUM;
            }
            return S;
        }
        /// <summary>
        /// метод генерации треугольной сетки (важно, чтобы координаты шли от левого верхнего угла к правому нижнему)
        /// </summary>
        /// <returns>сгенерированная сетка</returns>
        protected Mesh GenerateTriangleMesh()
        {
            try
            {
                int NX = p.Nx;
                int NY = p.Ny;
                // количество элементов
                int NE = (NY - 1) * (NX - 1) * 2;
                // создание карты
                int[][] map = new int[NY][];
                int e = 0;
                for (int i = 0; i < NY; i++)
                {
                    map[i] = new int[NX];
                    for (int j = 0; j < NX; j++)
                        map[i][j] = e++;
                }
                
                //Генерация сетки в зависимости от ее структуры StructureIndex
                Mesh m = new Mesh();
                // генерация треугольной сетки
                m.AreaElems = new int[NE][]; // [NE, 3];
                for (int i = 0; i < NE; i++)
                    m.AreaElems[i] = new int[3];
                //
                m.CountKnots = NX * NY;
                m.CountElems = NE;
                m.X = X;
                m.Y = Y;
                //m.X = new double[m.CountKnots];
                //X.CopyTo(m.X,0);
                //m.Y = new double[m.CountKnots];
                //Y.CopyTo(m.Y, 0);
                e = 0;
                int a, b, c, d;
                //for (int i = 0; i < NY - 3; i++)
                //!! не забыть закомменить флаг неизменности структуры сетки в Form BackgroundWork
                for (int i = 0; i < NY - 1; i++)
                    for (int j = 0; j < NX - 1; j++)
                    {
                        a = map[i][j];
                        b = map[i][j + 1];
                        c = map[i + 1][j + 1];
                        d = map[i + 1][j];

                        //double Lac = (X[a] - X[c]) * (X[a] - X[c]) +
                        //                (Y[a] - Y[c]) * (Y[a] - Y[c]);

                        //double Lbd = (X[b] - X[d]) * (X[b] - X[d]) +
                        //                (Y[b] - Y[d]) * (Y[b] - Y[d]);
                        ////
                        //if ((Lac / Lbd > 0.9999) && (Lac / Lbd < 1.0001))
                        //{
                        //if (j % 2 == j % 2 * 2)
                        //{
                        //    m.AreaElems[e][0] = a;
                        //    m.AreaElems[e][1] = c;
                        //    m.AreaElems[e][2] = b;
                        //    e++;
                        //    //
                        //    m.AreaElems[e][0] = a;
                        //    m.AreaElems[e][1] = d;
                        //    m.AreaElems[e][2] = c;
                        //    e++;
                        //}
                        //else
                        //{
                        //    m.AreaElems[e][0] = a;
                        //    m.AreaElems[e][1] = d;
                        //    m.AreaElems[e][2] = b;
                        //    e++;
                        //    //
                        //    m.AreaElems[e][0] = b;
                        //    m.AreaElems[e][1] = d;
                        //    m.AreaElems[e][2] = c;
                        //    e++;
                        //}
                        //}
                        //else if (Lac < Lbd)
                        //{
                        //    m.AreaElems[e][0] = a;
                        //    m.AreaElems[e][1] = c;
                        //    m.AreaElems[e][2] = b;
                        //    e++;
                        //    //
                        //    m.AreaElems[e][0] = a;
                        //    m.AreaElems[e][1] = d;
                        //    m.AreaElems[e][2] = c;
                        //    e++;
                        //}
                        //else
                        //{
                        m.AreaElems[e][0] = a;
                        m.AreaElems[e][1] = d;
                        m.AreaElems[e][2] = b;
                        e++;
                        //
                        m.AreaElems[e][0] = b;
                        m.AreaElems[e][1] = d;
                        m.AreaElems[e][2] = c;
                        e++;
                        //}
                    }
                //// сетка с регулярными КО по дну
                //for (int i = NY - 3; i < NY - 2; i++)
                //    for (int j = 0; j < NX - 1; j++)
                //    {
                //        a = map[i][j];
                //        b = map[i][j + 1]; ;
                //        c = map[i + 1][j + 1];
                //        d = map[i + 1][j];
                //        //
                //        m.AreaElems[e][0] = a;
                //        m.AreaElems[e][1] = c;
                //        m.AreaElems[e][2] = b;
                //        e++;
                //        //
                //        m.AreaElems[e][0] = a;
                //        m.AreaElems[e][1] = d;
                //        m.AreaElems[e][2] = c;
                //        e++;

                //    }
                //for (int i = NY - 2; i < NY - 1; i++)
                //    for (int j = 0; j < NX - 1; j++)
                //    {
                //        a = map[i][j];
                //        b = map[i][j + 1]; ;
                //        c = map[i + 1][j + 1];
                //        d = map[i + 1][j];
                //        //
                //        m.AreaElems[e][0] = a;
                //        m.AreaElems[e][1] = d;
                //        m.AreaElems[e][2] = b;
                //        e++;
                //        //
                //        m.AreaElems[e][0] = b;
                //        m.AreaElems[e][1] = d;
                //        m.AreaElems[e][2] = c;
                //        e++;

                //    }
                // генерация массивов граничных узлов
                m.LeftKnots = new int[NY];
                m.RightKnots = new int[NY];
                m.TopKnots = new int[NX];
                m.BottomKnots = new int[NX];
                m.CountLeft = NY;
                m.CountRight = NY;
                m.CountTop = NX;
                m.CountBottom = NX;
                for (int i = 0; i < NY; i++)
                {
                    m.LeftKnots[i] = map[i][0];
                    m.RightKnots[NY - i - 1] = map[i][NX - 1];
                }
                for (int j = 0; j < NX; j++)
                {
                    m.TopKnots[NX - j - 1] = map[0][j];
                    m.BottomKnots[j] = map[NY - 1][j];
                }
                //
                map = null;
                return m;
            }
            catch (Exception ex)
            {
                exception += ex.Message.ToString();
                return null;
            }

        }
    }
}
