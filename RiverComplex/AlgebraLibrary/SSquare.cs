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
    public class SSquare: SSystem
    {
        /// <summary>
        /// матрица системы
        /// </summary>
        double[][] Matrix;
        // одномерное представление для умножения
        float[] A = null;
        int[] tmpi;
        double[] ci;
        bool[] flags;
        public double[][] GetMatrix { get { return Matrix; }}
        // [1] Определение ранга САУ
        public override void SetSystem(int N, int N2=0)
        {
            Size = N;
            tmpi = new int[Size];
            ci = new double[Size];
            flags = new bool[Size];
            Right  = new double[Size];
            X = new double[Size];
            Matrix = new double[Size][];
            for (int i = 0; i < Size; i++)
            {
                Matrix[i] = new double[Size];
            }
            A = null;
        }
        // [2] Очистка системы
        public override void ClearSystem()
        {
            for (int i = 0; i < Size; i++)
            {
                Right[i] = 0;
                for (int j = 0; j < Size; j++)
                {
                    Matrix[i][j] = 0;
                }
            }
            A = null;
        }
       
        // [3] Формирование матрицы системы
        public override void BuildMatrix(double[][] LMartix, int[] Adress=null)
        {
           try
           {
               if (Adress != null)
               {
                   for (int a = 0; a < Adress.Length; a++)
                       for (int b = 0; b < Adress.Length; b++)
                           Matrix[Adress[a]][Adress[b]] += LMartix[a][b];
               }
               else
               {
                   for (int a = 0; a < LMartix.Length; a++)
                       for (int b = 0; b < LMartix[0].Length; b++)
                           Matrix[a][b] += LMartix[a][b];
               }
           }
           catch(Exception e)
           {
              error += e.Message.ToString();
           }
        }
        // [4] Формирование правой части
        public override void BuildRight(double[] LRight, int[] Adress = null)
        {
            try
            {
                if (Adress != null)
                {
                    for (int a = 0; a < Adress.Length; a++)
                        Right[Adress[a]] += LRight[a];
                }
                else
                {
                    for (int a = 0; a < LRight.Length; a++)
                        Right[a] += LRight[a];
                }
            }
            catch(Exception e)
            {
                error += e.Message.ToString();
            }
        }
        // [5] Выполнение граничных условий
        public override void SetBoundary(double[] Conditions/*[in]*/,int[] Adress/*[in]*/)
        {
            try
            {
                int ad,curKnot;
                int Count = Adress.Length;
                for(curKnot=0; curKnot<Count; curKnot++)
                {
                    ad = Adress[curKnot];   // адрес граничной неизвестной
                    for(int i=0; i<Size; i++)
                    {
                        // Обрабатываем столбец
                        Right[i] -= Matrix[i][ad]*Conditions[curKnot];
                        Matrix[i][ad] = 0;
                        // стираем строку системы
                        Matrix[ad][i] = 0;
                    }
                    // ставим 1 на главной диагонали
                    Matrix[ad][ad] = 1.0f;
                    // ставим значение ГУ по адресу (ad) в правую чать
                    Right[ad] = Conditions[curKnot];
                    //
                    //InitialX[ad] = 0;
                }
            }
            catch(Exception e)
            {
                error += e.Message.ToString();
            }
        }
        // Сборка САУ по строкам !!!
        public void AddStringSystem(double[] ColElems, int[] ColAdress,
         int IndexRow, double R)
        {
            try
            {
                for (int i = 0; i < ColAdress.Length; i++)
                    Matrix[IndexRow][ColAdress[i]] = ColElems[i];
                Right[IndexRow] = R;
            }
            catch(Exception e)
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
        //
        public override double[] MultiplyToMatrix(double[] V)
        {
            int p = 4;// M[0].Length должно быть кратное p
            int N = Matrix.Length * Matrix[0].Length;
            int e = 0;
            if (A == null)
            {
                A = new float[N];
                for (int i = 0; i < Matrix[0].Length; i++)
                    for (int j = 0; j < Matrix.Length; j++)
                        A[e++] = (float)Matrix[j][i];
            }
            //
            float[] x = new float[V.Length];
            for (int i = 0; i < x.Length; i++)
                x[i] = (float)V[i];
            //
            float[] y = new float[Matrix.Length * p];
            double[] yd = new double[Matrix.Length];
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
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MatrixToVector.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("MatrixToVector");
                    //Создание буферов параметров
                    int m = Matrix.Length;
                    int n = Matrix[0].Length;
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> AA = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, A);
                    ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, x);
                    ComputeBuffer<float> YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
                    // установка буферов в kernel
                    kernel.SetMemoryArgument(0, AA);
                    kernel.SetMemoryArgument(1, XX);
                    kernel.SetMemoryArgument(2, YY);
                    kernel.SetLocalArgument(3, m * sizeof(float) * p);
                    kernel.SetValueArgument(4, m);
                    kernel.SetValueArgument(5, n);
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[2];
                    globalSize[0] = m;
                    globalSize[1] = p;
                    //Вызов ядра
                    commands.Execute(kernel, null, globalSize, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(YY, ref y, true, null);
                    //очищение памяти
                    XX.Dispose();
                    AA.Dispose();
                    kernel.Dispose();
                    //
                    //второй kernel
                    ComputeKernel kernel2 = program.CreateKernel("reduce_rows");
                    //установка аргументов
                    YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
                    kernel2.SetMemoryArgument(0, YY);
                    kernel2.SetValueArgument(1, m);
                    kernel2.SetValueArgument(2, p);
                    //
                    long[] globalSize2 = new long[1];
                    globalSize2[0] = m;
                    //Вызов ядра
                    commands.Execute(kernel2, null, globalSize2, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(YY, ref y, true, null);
                    //очищение памяти в порядке, обратном созданию
                    YY.Dispose();
                    program.Dispose();
                    commands.Dispose();
                    context.Dispose();
                }
                catch (Exception ee)
                {
                    error += ee.Message.ToString();
                }
                for (int i = 0; i < yd.Length; i++)
                    yd[i] = y[i];
            }
            else
            {
                for (int i=0;i<Matrix.Length;i++)
                    for (int j = 0; j < Matrix[0].Length; j++)
                        yd[i] += Matrix[i][j] * V[j];
            }
            //
            return yd;
        }
        public static double[] MultiplyToMatrix(float[] x, float[] ColMatrix, int m, int n)
        {
            int p = 4;// M[0].Length должно быть кратное p
            //
            float[] y = new float[m * p];
            double[] yd = new double[m];
            if (true)
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
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MatrixToVector.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("MatrixToVector");
                    //Создание буферов параметров
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> AA = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, ColMatrix);
                    ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, x);
                    ComputeBuffer<float> YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
                    // установка буферов в kernel
                    kernel.SetMemoryArgument(0, AA);
                    kernel.SetMemoryArgument(1, XX);
                    kernel.SetMemoryArgument(2, YY);
                    kernel.SetLocalArgument(3, m * sizeof(float) * p);
                    kernel.SetValueArgument(4, m);
                    kernel.SetValueArgument(5, n);
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[2];
                    globalSize[0] = m;
                    globalSize[1] = p;
                    //Вызов ядра
                    commands.Execute(kernel, null, globalSize, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(YY, ref y, true, null);
                    //очищение памяти
                    XX.Dispose();
                    AA.Dispose();
                    kernel.Dispose();
                    //
                    //второй kernel
                    ComputeKernel kernel2 = program.CreateKernel("reduce_rows");
                    //установка аргументов
                    YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
                    kernel2.SetMemoryArgument(0, YY);
                    kernel2.SetValueArgument(1, m);
                    kernel2.SetValueArgument(2, p);
                    //
                    long[] globalSize2 = new long[1];
                    globalSize2[0] = m;
                    //Вызов ядра
                    commands.Execute(kernel2, null, globalSize2, null, null);
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    // чтение искомой функции из буфера kernel-а
                    commands.ReadFromBuffer(YY, ref y, true, null);
                    //очищение памяти в порядке, обратном созданию
                    YY.Dispose();
                    program.Dispose();
                    commands.Dispose();
                    context.Dispose();
                }
                catch (Exception ee)
                {
                   ee.Message.ToString();
                }
                for (int i = 0; i < yd.Length; i++)
                    yd[i] = y[i];
            }
            else
            {
                for (int i = 0; i < m; i++)
                    for (int j = 0; j < n; j++)
                        yd[i] += ColMatrix[i*n+n] * x[j];
            }
            //
            return yd;
        }
        public override string  Name
        {
            get
            {
                return "Квадратная матрица";
            }
        }
        // Операция умножения вектора на матрицу
        public void Mult_Vector(double[] X, double[] R, int IsRight)
        {
            try
            {
                for (int i = 0; i < Size; i++)
                {
                    R[i] = 0;
                    for (int j = 0; j < Size; j++)
                        R[i] += Matrix[i][j] * X[j];
                    R[i] -= IsRight * Right[i];
                }
            }
            catch(Exception e)
            {
                error += e.Message.ToString();
            }
        }
        // [10]
        // public override   void  PrintSystem(string BufSizeame/*[in]*/);
        // [11] конструктор
        public SSquare() { }
        // [12] Установка правой части
        protected void Calibrate(int  k, double C)
        {
           double CC = 1/C;
           for(int i=k; i<Size; i++)
           {
              for(int j=k; j<Size; j++) Matrix[i][j]*=CC;
              Right[i] *=CC;
           }
        }
        //
        
        
        public void BuildOnlyRight(double[] Right1, int[] Adress)
        {
            for (int i = 0; i < Right1.Length; i++)
                Right[Adress[i]] = Right1[i];
        }
        //public void Examples()
        //{
        //    //
        //    //  Способ использования для разностных схем
        //    //
        //    // коэффициенты матрицы
        //    double[] A1 = { 2, -1, 0, 0 };
        //    double[] A2 = { -1, 2, -1, 0 };
        //    double[] A3 = { 0, -1, 2, -1 };
        //    double[] A4 = { 0, 0, -1, 2 };
        //    // адреса коэффициентов в матрице
        //    int[] C1 = { 0, 1, 2, 3 };
        //    // вектор правой части
        //    double[] R = { 1, 0, 0, 1 };
        //    AlgSSquare a = new AlgSSquare();
        //    // порядок системы можно определить так (верхний треугольник)
        //    // a.SetSystem(4);
        //    // но экономичней выбрать ленту с шириной 2
        //    a.SetSystem(4);
        //    //// формируем систему
        //    //a.AddStringSystem(A1, C1, 0, 1);
        //    //a.AddStringSystem(A2, C1, 1, 0);
        //    //a.AddStringSystem(A3, C1, 2, 0);
        //    //a.AddStringSystem(A4, C1, 3, 1);
        //    // формируем систему
        //    a.AddStringSystem(A1, C1, 0, R[0]);
        //    a.AddStringSystem(A2, C1, 1, R[1]);
        //    a.AddStringSystem(A3, C1, 2, R[2]);
        //    a.AddStringSystem(A4, C1, 3, R[3]);
        //    //
        //    int[] badr = { 0, 1, 2, 3, 4, 7, 8, 11, 12, 13, 14, 15 };
        //    a.Forwarding(badr);
        //    //
        //    // решаем систему
        //    a.BackSolve();
        //    // смотрим ответ
        //    Console.WriteLine("{0} {1} {2} {3} ", a.X[0], a.X[1], a.X[2], a.X[3]);
        //    Console.ReadLine();
        //    a.ClearSystem();
        //    //
        //    //  Способ использования для конечно элементных схем
        //    // Локальная матрица жесткости
        //    double[][] K = new double[2][];
        //    double[] K1 = { 1, -1 };
        //    double[] K2 = { -1, 1 };
        //    K[0] = K1; K[1] = K2;
        //    // Массивы связности
        //    int[] L1 = { 0, 1 };
        //    int[] L2 = { 1, 2 };
        //    int[] L3 = { 2, 3 };
        //    int[] L4 = { 3, 4 };
        //    // локальная правая часть
        //    double[] P = { 0, 0 };
        //    //
        //    double[] BC = { 5, 1 };
        //    int[] LC = { 0, 4 };
        //    double[] X1 = { 0, 0, 0, 0, 0 };
        //    AlgSSquare b = new AlgSSquare();
        //    // порядок системы
        //    b.SetSystem(5);
        //    // Формирование ГМЖ системы
        //    b.BuildMatrix(K, L1);
        //    b.BuildMatrix(K, L2);
        //    b.BuildMatrix(K, L3);
        //    b.BuildMatrix(K, L4);
        //    // Формирование правой части
        //    b.BuildRight(P, L1);
        //    b.BuildRight(P, L2);
        //    b.BuildRight(P, L3);
        //    b.BuildRight(P, L4);
        //    // Выполнение граничных условий
        //    b.SetBoundary(BC, LC);
        //    // решаем систему
        //    b.Solve();
        //    // смотрим ответ
        //    Console.WriteLine("{0} {1} {2} {3} {4} ", b.X[0], b.X[1], b.X[2], b.X[3], b.X[4]);
        //    Console.ReadLine();
        //    //
        //    // коэффициенты матрицы
        //    double[] D1 = { 2, -1, 0, 0 };
        //    double[] D2 = { -1, 2, -1, 0 };
        //    double[] D3 = { 0, -1, 2, -1 };
        //    double[] D4 = { 0, 0, -1, 2 };
        //    // адреса коэффициентов в матрице
        //    int[] V1 = { 0, 1, 2, 3 };
        //    // вектор правой части
        //    double[] RR = { 0, 0, 2, 1 };
        //    AlgSSquare aaa = new AlgSSquare();
        //    // порядок системы можно определить так (верхний треугольник)
        //    // a.SetSystem(4);
        //    // но экономичней выбрать ленту с шириной 2
        //    aaa.SetSystem(4);
        //    // формируем систему
        //    //a.AddStringSystem(A1, C1, 0, 1);
        //    //a.AddStringSystem(A2, C1, 1, 0);
        //    //a.AddStringSystem(A3, C1, 2, 0);
        //    //a.AddStringSystem(A4, C1, 3, 1);
        //    aaa.AddStringSystem(D1, V1, 0, R[0]);
        //    aaa.AddStringSystem(D2, V1, 1, R[1]);
        //    aaa.AddStringSystem(D3, V1, 2, R[2]);
        //    aaa.AddStringSystem(D4, V1, 3, R[3]);
        //    aaa.Forwarding(badr); //изначально было без массива в аргументе
        //    // решаем систему
        //    aaa.BackSolve();
        //    // смотрим ответ
        //    Console.WriteLine("{0} {1} {2} {3} ", aaa.X[0], aaa.X[1], aaa.X[2], aaa.X[3]);
        //    Console.ReadLine();
        //    a.ClearSystem();
        //}
    }
}
