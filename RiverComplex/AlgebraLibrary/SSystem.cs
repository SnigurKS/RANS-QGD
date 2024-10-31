using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
    [Serializable]
    public abstract class SSystem
    {
        public bool OpenCL = false;
        public string error = "";
        public int Size;
        public abstract string Name{ get;  }
        /// <summary>
        /// Правая часть
        /// </summary>
        protected double[] Right;
        public double[] GetRight
        {
            get
            {
                return Right;
            }
        }
        /// <summary>
        /// Вектор неизвестных
        /// </summary>
        protected double[] X;
        public virtual void SetX(double[] x)
        {
            X = x;
        }
        public double[] GetX { get { return X; } }
        /// <summary>
        /// Установка размера системы
        /// </summary>
        /// <param name="Size">Ранг</param>
        public abstract void SetSystem(int Size, int BSize=0);
        /// <summary>
        /// Очистка системы
        /// </summary>
        public abstract void ClearSystem();
        /// <summary>
        /// Построение правой части
        /// </summary>
        /// <param name="Values">Значения</param>
        /// <param name="Indexes">Индексы ячеек</param>
        public abstract void BuildRight(double[] Values, int[] Indexes);
        /// <summary>
        /// Построение матрицы
        /// </summary>
        /// <param name="Values">Значения</param>
        /// <param name="Indexes">Индексы ячеек</param>
        public abstract void BuildMatrix(double[][] Values, int[] Indexes);
        /// <summary>
        /// Установка граничных условий
        /// </summary>
        /// <param name="Boundary">Значения ГУ</param>
        /// <param name="Indexes">Индексы ячеек</param>
        public abstract void SetBoundary(double[] Boundary, int[] Indexes);
        //
        public abstract double[] MultiplyToMatrix(double[] Vector);
        
        public abstract void Accept(Algorythm Alg, int[] Ar = null, int flag = 0);
        //
        public double[] Subtraction(double[] V1, double[] V2)
        {
            double[] tmp = new double[V1.Length];
            try
            {
                // вычисляются локальные матрицы жесткости и производится сборка глобальной матрицы жесткости
                OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, V1.Length);
                Parallel.ForEach(rangePartitioner,
                      (range, loopState) =>
                      {
                          for (int i = range.Item1; i < range.Item2; i++)
                          //for (int fe = 0; fe < Mesh.CountElem; fe++)
                          {
                              tmp[i] = V1[i] - V2[i];
                          }
                      });
                //
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
            return tmp;
        }
        /// <summary>
        /// Метод перемножения векторов с использованием Parallel.ForEach
        /// </summary>
        /// <param name="V1T">Вектор-строка</param>
        /// <param name="V2">Вектор-столбец</param>
        /// <returns>Результат умножения векторов</returns>
        public double Multiplucation(double[] V1T, double[] V2)
        {
            double result = 0;
            double[] tmp = new double[V1T.Length];
            try
            {
                // вычисляем локальные матрицы жесткости и производим сборку глобальной матрицы жесткости
                OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, V1T.Length);
                Parallel.ForEach(rangePartitioner,
                      (range, loopState) =>
                      {
                          for (int i = range.Item1; i < range.Item2; i++)
                          //for (int fe = 0; fe < Mesh.CountElem; fe++)
                          {
                              tmp[i] = V1T[i] * V2[i];
                          }
                      });
                //
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
            for (int i = 0; i < tmp.Length; i++)
                result += tmp[i];

            return result;
        }
        //
        //public void Example()
        //{
        //    //--Тестирование OpenCL кода MatrixToVector
        //    AlgSRowPacked al = new AlgSRowPacked();// AlgBiCG; // AlgBiStableCG;
        //    double[][] m = new double[3][];
        //    for (int i = 0; i < 3; i++)
        //        m[i] = new double[4];
        //    //
        //    m[0][0] = 1.5; m[1][0] = 0.7; m[2][0] = 0.6; m[3][0] = 1.0;
        //    m[0][1] = 0.5; m[1][1] = 1.5; m[2][1] = 0.6; m[3][1] = 1.0;
        //    m[0][2] = 0.5; m[1][2] = 0.7; m[2][2] = 1.5; m[3][2] = 1.0;
        //    m[0][3] = 0.5; m[1][3] = 0.7; m[2][3] = 0.6; m[3][3] = 1.5;
        //    //
        //    double[] vector = new double[4];
        //    vector[0] = 1.0; vector[1] = 3.0; vector[2] = 2.0; vector[3] = 5.0;
        //    // X = -0.68320; 0.80441; -0.02204; // 3.26722
        //    al.SetSystem(vector.Length);
        //    al.BuildMatrix(m, null);
        //    al.BuildRight(vector);
        //    al.Solve();
        //}
    }
}
