using Cloo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
    [Serializable]
    public class SBand: SSystem
    {
        //public string error = "";
        /// <summary>
        /// Ширина ленты ГМЖ
        /// </summary>
        public int FH;
        int[] tmpi;
        double[][] ci;
        /// <summary>
        /// Матрица данного метода
        /// </summary>
        double[][] Matrix;
        // одномерное представление матрицы для умножения
        float[] M = null;
        public double[][] GetMatrix { get { return Matrix; } }
        public override string Name
        {
            get { return "Ленточная матрица"; }
        }
        public SBand()
        {

        }
        public SBand(int MatrixSize, int BandSize)
        {
            SetSystem(MatrixSize, BandSize);
        }
        /// <summary>
        /// Установка размера системы
        /// </summary>
        /// <param name="Size">Ранг</param>
        public override void SetSystem(int N, int H)
        {
            //
            Size = N; FH = H;
            tmpi = new int[Size];
            ci = new double[Size][];
            Right = new double[Size];
            X = new double[Size];
            Matrix = new double[Size][];
            for (int i = 0; i < Size; i++)
            {
                Matrix[i] = new double[FH];
                ci[i] = new double[FH];
            }
            //Clear();
        }
        /// <summary>
        /// Очистка системы
        /// </summary>
        public override void ClearSystem()
        {
            for (int i = 0; i < Size; i++)
            {
                Right[i] = 0;
                for (int j = 0; j < FH; j++)
                {
                    Matrix[i][j] = 0;
                }
            }
        }
        public void ClearRight()
        {
            for (int i = 0; i < Size; i++)
            {
                Right[i] = 0;
            }
        }
        //
        /// <summary>
        /// Построение правой части
        /// </summary>
        /// <param name="Values">Значения</param>
        /// <param name="Indexes">Индексы ячеек</param>
        public override void BuildRight(double[] Values, int[] Indexes=null)
        {
            try
            {
                if (Indexes != null)
                {
                    for (int a = 0; a < Indexes.Length; a++)
                        Right[Indexes[a]] += Values[a];
                }
                else
                {
                    for (int a = 0; a < Values.Length; a++)
                        Right[a] += Values[a];
                }
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }
        /// <summary>
        /// Построение матрицы
        /// </summary>
        /// <param name="Values">Значения</param>
        /// <param name="Indexes">Индексы ячеек</param>
        public override void BuildMatrix(double[][] Values, int[] Indexes=null)
        {
            try
            {
                if (Indexes != null)
                {
                    int i, j, nv, nh;
                    for (i = 0; i < Indexes.Length; i++)
                    {
                        nv = Indexes[i];
                        for (j = 0; j < Indexes.Length; j++)
                        {
                            nh = Indexes[j] - nv;
                            if (nh >= 0)
                                Matrix[nv][nh] += Values[i][j];
                        }
                    }
                }
                else
                {
                    int i, j;
                    for (i = 0; i < Values.Length; i++)                 
                        for (j = 0; j < Values[i].Length; j++)
                                Matrix[i][j] += Values[i][j];
                }
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }
        /// <summary>
        /// Установка граничных условий
        /// </summary>
        /// <param name="Boundary">Значения ГУ</param>
        /// <param name="Indexes">Индексы ячеек</param>
        public override void SetBoundary(double[] Boundary, int[] Indexes)
        {
            try 
            {
                double Ves = 1.0e30;
                int ad, curKnot;
                int Count = Indexes.Length;
                for (curKnot = 0; curKnot < Count; curKnot++)
                {
                    ad = Indexes[curKnot];   // адрес граничной неизвестной
                    Right[ad] = Boundary[curKnot] * Ves;
                    Matrix[ad][0] = Ves;
                }
            }
            catch (Exception e)
            {
                error = error + "Algebra.SetBoundary" + e.Message;
            }
        }
        //
        public void BoundConditionsRight(double[] Conditions/*[in]*/, int[] Adress/*[in]*/)
        {
            try
            {
                double Ves = 1.0e30;
                int ad, curKnot;
                int Count = Adress.Length;
                for (curKnot = 0; curKnot < Count; curKnot++)
                {
                    ad = Adress[curKnot];   // адрес граничной неизвестной
                    Right[ad] = Conditions[curKnot] * Ves;
                }
            }
            catch (Exception e)
            {
                error = error + "Algebra.BoundCondition" + e.Message;
            }
        }
        public void SetBoundaryWithoutMatrix(double[] Boundary, int[] Indexes)
        {
            try
            {
                double Ves = 1.0e30;
                int ad, curKnot;
                int Count = Indexes.Length;
                for (curKnot = 0; curKnot < Count; curKnot++)
                {
                    ad = Indexes[curKnot];   // адрес граничной неизвестной
                    Right[ad] = Boundary[curKnot] * Ves;
                }
            }
            catch (Exception e)
            {
                error = error + "Algebra.SetBoundaryWithoutMatrix" + e.Message;
            }
        }
        /// <summary>
        /// Рассчитать
        /// </summary>
        /// <param name="Alg">Алгоритм расчета</param>
        /// <param name="Ar">Опционально массив данных</param>
        /// <param name="flag">Флаг расчета (0 - обычный, 1 - прямой ход, 2 - обратный ход, )</param>
        public override void Accept(Algorythm Alg, int[] Ar = null, int flag = 0)
        {
            string s = Alg.GetType().Name;
            if (s == "AlgorythmGauss")
            {
                if (flag == 0)
                    Alg.Solve(this);
                else
                    if (flag == 2)
                        Alg.BackSolve(this);
                    else if (flag == 1)
                        Alg.Forwarding(this, Ar);
            }
            else
                Alg.Solve(this);
        }
        //----- Тестить, не всегда правильно выдает последнюю строку! при Size=FH
        public override double[] MultiplyToMatrix(double[] Vector)
        {
            //// Тест
            //Size = 5;
            //FH = 3;
            //Matrix = new double[Size][];
            //for (int i = 0; i < Size; i++)
            //{
            //    Matrix[i] = new double[FH];
            //    Matrix[i][0] = 1;
            //    Matrix[i][1] = i + 2;
            //    Matrix[i][2] = 1;
            //}
            //Matrix[3][2] = 0;
            //Matrix[4][1] = 0;
            //Matrix[4][2] = 0;
            //Vector = new double[Size];
            //for (int i = 0; i < Size; i++)
            //    Vector[i] = 1.0;
            // //X = 4;7;10;11;7; при V = 1;1;1;1;1

            if (M == null)
            {
                // перевод ленты в одномерный массив
                M = new float[Size * FH];
                int k = 0; int m = 0;
                for (int i = 0; i < Size; i++)
                {
                    if (Size - i < FH)
                        m = Size - i;
                    else
                        m = FH;
                    for (int j = 0; j < m; j++)
                        M[k++] = (float)Matrix[i][j];
                }
            }
            //
            float[] X = new float[Size];
            float[] V = new float[Size];
            for (int i = 0; i < Size; i++)
                V[i] = (float)Vector[i];
            //
            if (!OpenCL)
            {
                for (int i = 0; i < Size; i++)
                {
                    float sum = X[i];
                    int m = 0; int idx = 0;
                    if (Size - i < FH)
                    {
                        m = Size - i;
                        idx = FH * i - (FH - m - 1);
                    }
                    else
                    {
                        m = FH;
                        idx = FH * i;
                    }
                    //
                    sum += M[idx] * V[i];
                    for (int j = 1; j < m; j++)
                    {
                        sum += M[idx + j] * V[i + j];
                        X[i + j] += M[idx + j] * V[i];
                    }
                    X[i] = sum;
                }
            }
            else
            {
                //
                try
                {
                    //Выбор платформы расчета, создание контекста
                    ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
                    ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
                    //Инициализация OpenCl, выбор устройства
                    ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
                    //Считывание текста программы из файла
                    string s = AppDomain.CurrentDomain.BaseDirectory;
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MultiplicationBand.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("MultiplicationBand");
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> MM = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, M);
                    ComputeBuffer<float> VV = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, V);
                    ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, X);
                    // установка буферов в kernel
                    kernel.SetMemoryArgument(0, MM);
                    kernel.SetMemoryArgument(1, VV);
                    kernel.SetMemoryArgument(2, XX);
                    kernel.SetValueArgument(3, FH);
                    kernel.SetValueArgument(4, Size);
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[1];
                    globalSize[0] = Size;
                    //Вызов ядра
                    commands.Execute(kernel, null, globalSize, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(XX, ref X, true, null);
                    //очищение памяти
                    XX.Dispose();
                    VV.Dispose();
                    MM.Dispose();
                    //
                    kernel.Dispose();
                    program.Dispose();
                    commands.Dispose();
                    context.Dispose();
                }
                catch (Exception ee)
                {
                    error += ee.Message.ToString();
                }
            }
            //
            double[] Result = new double[Size];
            // перевод в double
            for (int i = 0; i < Size; i++)
                Result[i] = X[i];
            //
            return Result;
        }
        // тест
        public double[] MultiplyToMatrixSharp(double[] Vector)
        {
            Size = 5;
            FH = 3;
            Matrix = new double[Size][];
            for (int i = 0; i < Size; i++)
            {
                Matrix[i] = new double[FH];
                Matrix[i][0] = 1;
                Matrix[i][1] = i + 2;
                Matrix[i][2] = 1;
            }
            Matrix[3][2] = 0;
            Matrix[4][1] = 0;
            Matrix[4][2] = 0;
            double[] Result = new double[Size];
            int m;
            //
            for (int i = 0; i < Size; i++)
            {
                double sum = Result[i];
                if (Size - i < FH)
                    m = Size - i;
                else
                    m = FH;
                //
                sum += Matrix[i][0] * Vector[i];
                for (int j = 1; j < m; j++)
                {
                    sum += Matrix[i][j] * Vector[i + j];
                    Result[i + j] += Matrix[i][j] * Vector[i];
                }
                Result[i] = sum;
            }
            return Result;
            //
            //Debugging OpenCL version
            //X = new float[Size];
            //for (int i = 0; i < Size; i++)
            //{
            //    float sum = X[i];
            //    m = 0;
            //    int idx = 0;
            //    if (Size - i < FH)
            //    {
            //        m = Size - i;
            //        idx = FH * i - (FH - m - 1);
            //    }
            //    else
            //    {
            //        m = FH;
            //        idx = FH * i;
            //    }
            //    //
            //    sum += M[idx] * V[i];
            //    for (int j = 1; j < m; j++)
            //    {
            //        sum += M[idx + j] * V[i + j];
            //        X[i + j] += M[idx + j] * V[i];
            //    }
            //    X[i] = sum;
            //    //
            //}
        }
    }
}
