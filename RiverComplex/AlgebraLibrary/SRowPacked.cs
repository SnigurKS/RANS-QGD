using Cloo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
    [Serializable]
    public class SRowPacked: SSystem
    {
        protected CPackRow[] Matrix;
        float[] A = null;
        int[] Cols = null;
        int[] Pointers = null;
        public CPackRow[] GetMatrix { get { return Matrix; } }

        public CPackRowBuff Buff;
        protected int Count = 0;
        //
        public override void SetX(double[] x)
        { X = x; }
        public override string Name
        {
            get { return "Плотная упаковка по строкам"; }
        }
        public SRowPacked()
        {

        }
        public override void SetSystem(int Size, int Size2 = 0)
        {
            this.Size = Size;
            Matrix = new CPackRow[Size];
            for (int i = 0; i < Size; i++)
                Matrix[i] = new CPackRow();
                Buff = new CPackRowBuff(Size);
            //
            Right = new double[Size];
            X = new double[Size];
            A = null;
        }
        public override void ClearSystem()
        {
            for (int i = 0; i < Size; i++)
                Matrix[i].Free();
            Buff.Free();
            A = null;
        }
        //
        public override void BuildMatrix(double[][] Values, int[] Indexes=null)
        {
            if (Indexes != null)
            {
                // цикл по неизвестным
                for (int a = 0; a < Indexes.Length; a++)
                {
                    int k = Indexes[a];
                    // распаковка k - й строки
                    Matrix[k].UnPackRow(Buff);
                    // добавление элементов в строку
                    for (int b = 0; b < Indexes.Length; b++)
                        Buff.Add(Values[a][b], Indexes[b]);
                    // запись буфера в матрицу
                    Matrix[k].SetBufPackRow(Buff, true);
                }
            }
            else
            {
                for (int a = 0; a < Values.Length; a++)
                {
                    Buff.Free();
                    // добавление элементов в строку
                    for (int b = 0; b < Values[a].Length; b++)
                        if(Values[a][b]!=0)
                            Buff.Add(Values[a][b], b);
                    // запись буфера в матрицу
                    Matrix[a].SetBufPackRow(Buff, true);
                }
            }
        }
        public override void SetBoundary(double[] Boundary, int[] Indexes)
        {
            try
            {
                // сообщение о проделанной  работе
                // FSentReportMessage("выполнение граничных условий");
                int id, Knot;
                for (Knot = 0; Knot < Indexes.Length; Knot++)
                {
                    // FSendRateMessage(Knot,Count);
                    id = Indexes[Knot];   // адрес граничной неизвестной
                    // Обрабатываем столбец
                    for (int i = 0; i < Size; i++)
                        Right[i] -= Matrix[i].ClearElem(id) * Boundary[Knot];
                    // Обрабатываем строку устанавливаем на главной диагонали 1
                    Matrix[id].DiagOne(id);
                    // ставим значение ГУ по адресу (id) в правую чать
                    Right[id] = Boundary[Knot];
                }
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }
        /// <summary>
        /// Выделение памяти для матрицы
        /// </summary>
        /// <param name="MatrixSize">Порядок матрицы</param>
        /// <param name="NotNullCount">Количество ненулевых элементов</param>
        public override double[] MultiplyToMatrix(double[] V)
        {
            if (A==null)
                //конвертация матрицы из сжатых строк в массивы
                PackMatrixToArrays(out A, out Cols, out Pointers);
            //
            float[] x = new float[Size];
            for (int i = 0; i < Size; i++)
                x[i] = (float)V[i];
            //
            float[] y = new float[Size];
            double[] yd = new double[Size];
            if (OpenCL)
            {
                try
                {
                    //Выбор платформы расчета, создание контекста
                    ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
                    ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
                    //Инициализация OpenCl, выбор устройства
                    ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
                    //Считывание текста программы из файла
                    string s = AppDomain.CurrentDomain.BaseDirectory;
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MultiplicationPacked.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("MultiplicationPacked");
                    //Создание буферов параметров
                    // (__local const float * a, __local const int * cols, __local const int * pointers, __local const float * x, __global float * y)
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> AA = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, A);
                    ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, x);
                    ComputeBuffer<float> YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
                    ComputeBuffer<int> COLS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Cols);
                    ComputeBuffer<int> POINTERS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Pointers);
                    // установка буферов в kernel
                    kernel.SetMemoryArgument(0, AA);
                    kernel.SetMemoryArgument(1, COLS);
                    kernel.SetMemoryArgument(2, POINTERS);
                    kernel.SetMemoryArgument(3, XX);
                    kernel.SetMemoryArgument(4, YY);
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[1];
                    globalSize[0] = Size;
                    //Вызов ядра
                    commands.Execute(kernel, null, globalSize, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(YY, ref y, true, null);
                    //очищение памяти
                    POINTERS.Dispose();
                    COLS.Dispose();
                    YY.Dispose();
                    XX.Dispose();
                    AA.Dispose();
                    kernel.Dispose();
                    //
                    program.Dispose();
                    commands.Dispose();
                    context.Dispose();
                }
                catch (Exception ee)
                {
                    error += ee.Message.ToString();
                }
                //
                for (int i = 0; i < yd.Length; i++)
                    yd[i] = y[i];
            }
            else
            {
                double sum = 0;
                for (int i = 0; i < Pointers.Length - 1; i++)
                {
                    int p1 = Pointers[i];
                    int p2 = Pointers[i + 1];
                    //
                    sum = 0;
                    //
                    for (int k = p1; k < p2; k++)
                    {
                        sum += A[k] * x[Cols[k]];
                    }
                    yd[i] = sum;
                }
            }
            //
            return yd;
        }
        /// <summary>
        /// Выделение памяти для матрицы
        /// </summary>
        /// <param name="MatrixSize">Порядок матрицы</param>
        /// <param name="NotNullCount">Количество ненулевых элементов</param>
        public float[] MultiplyToMatrix(float[] x)
        {
            if (A == null)
                //конвертация матрицы из сжатых строк в массивы
                PackMatrixToArrays(out A, out Cols, out Pointers);
            //
            //
            float[] y = new float[Size];
            if (OpenCL)
            {
                try
                {
                    //Выбор платформы расчета, создание контекста
                    ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
                    ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
                    //Инициализация OpenCl, выбор устройства
                    ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
                    //Считывание текста программы из файла
                    string s = AppDomain.CurrentDomain.BaseDirectory;
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MultiplicationPacked.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("MultiplicationPacked");
                    //Создание буферов параметров
                    // (__local const float * a, __local const int * cols, __local const int * pointers, __local const float * x, __global float * y)
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> AA = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, A);
                    ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, x);
                    ComputeBuffer<float> YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
                    ComputeBuffer<int> COLS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Cols);
                    ComputeBuffer<int> POINTERS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Pointers);
                    // установка буферов в kernel
                    kernel.SetMemoryArgument(0, AA);
                    kernel.SetMemoryArgument(1, COLS);
                    kernel.SetMemoryArgument(2, POINTERS);
                    kernel.SetMemoryArgument(3, XX);
                    kernel.SetMemoryArgument(4, YY);
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[1];
                    globalSize[0] = Size;
                    //Вызов ядра
                    commands.Execute(kernel, null, globalSize, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(YY, ref y, true, null);
                    //очищение памяти
                    POINTERS.Dispose();
                    COLS.Dispose();
                    YY.Dispose();
                    XX.Dispose();
                    AA.Dispose();
                    kernel.Dispose();
                    //
                    program.Dispose();
                    commands.Dispose();
                    context.Dispose();
                }
                catch (Exception ee)
                {
                    error += ee.Message.ToString();
                }
                //
            }
            else
            {
                float sum = 0;
                for (int i = 0; i < Pointers.Length - 1; i++)
                {
                    int p1 = Pointers[i];
                    int p2 = Pointers[i + 1];
                    //
                    sum = 0;
                    //
                    for (int k = p1; k < p2; k++)
                    {
                        sum += A[k] * x[Cols[k]];
                    }
                    y[i] = sum;
                }
            }
            //
            return y;
        }
        /// <summary>
        /// Метод переводит матрицу в вид одномерного массива значений, массива значений слолбцов и указателей на начало строки
        /// </summary>
        /// <param name="MatrixPacked">Значения матрицы</param>
        /// <param name="Cols">Значения столбцов элементов MatrixPacked</param>
        /// <param name="Pointers">Индексы элементов в MatrixPacked, с которых начинается новая строка</param>
        public void PackMatrixToArrays(out float[] MatrixPacked, out int[] Cols, out int[] Pointers)
        {
            // Tecт
            //Size = 4;
            //Matrix = new CPackRow[4];
            //CPackRowBuff buff = new CPackRowBuff(4);
            //buff.Add(1, 2);
            //Matrix[0] = new CPackRow(buff);
            //buff.Free(true);
            //buff.Add(1, 0); buff.Add(2, 1); buff.Add(3, 2); buff.Add(4,3);
            //Matrix[1] = new CPackRow(buff);
            //buff.Free(true);
            //buff.Add(2, 0);
            //Matrix[2] = new CPackRow(buff);
            //buff.Free(true);
            //buff.Add(2, 0);
            //Matrix[3] = new CPackRow(buff);
            // количество ненулевых элементов в матрице
            int count = 0;
            for (int i = 0; i < Size; i++)
                count += Matrix[i].Count;
            // выделение памяти для массивов
            MatrixPacked = new float[count]; // сразу переводим во float для дальнейшего расчета на OpenCL
            Cols = new int[count];
            Pointers = new int[Size+1];
            //
            int ch=0; int row_count=0;
            for (int i = 0; i < Size; i++)
            {
                // количество элементов в текущей строке
                row_count = Matrix[i].Count;
                // копируем элементы и их индексы в массивы
                for (int j = 0; j < row_count; j++)
                {
                    MatrixPacked[ch] = (float)Matrix[i].Elem[j];
                    Cols[ch++] = Matrix[i].Index[j];
                }
                // обозначаем, что со следующего элемента начнется новая строка
                Pointers[i+1] = Pointers[i] + row_count;
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
                if (Indexes == null)
                {
                    for (int i = 0; i < Values.Length; i++)
                        Right[i] = Values[i];
                }
                else
                {
                    for (int i = 0; i < Values.Length; i++)
                        Right[Indexes[i]] += Values[i];
                }
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
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
       
        //public double[][] Transpose(double[][] M)
        //{
        //    double[][] MT = new double[M[0].Length][];
        //     for (int i=0;i<MT.Length;i++)
        //         MT[i] = new double[M.Length];
        //     try
        //     {
        //         // Выполняем построчный перенос столбцов M в строки MT
        //         OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, M.Length);
        //         Parallel.ForEach(rangePartitioner,
        //               (range, loopState) =>
        //               {
        //                   for (int i = range.Item1; i < range.Item2; i++)
        //                   {
        //                       for (int j = 0; j < M[0].Length; j++)
        //                           MT[j][i] = M[i][j];
        //                   }
        //               });
        //         //
        //     }
        //     catch (Exception e)
        //     {
        //         error += e.Message.ToString();
        //     }
        //    return MT;
        //}
    }
}
