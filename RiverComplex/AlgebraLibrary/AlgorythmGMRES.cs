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
    public class AlgorythmGMRES : Algorythm
    {
         public override string Name
         {
             get { return "Метод GMRES"; }
         }
         new public double Discrepancy = 0.0001; //0.0000001;
        int MaxIterations = 10;
        int m = 100;
        bool Preonditioner = false;
        SRowPacked L, U;
        SSquare Lsq, Usq;
        public AlgorythmGMRES()
        { }
        public AlgorythmGMRES(int m, bool preconditioner = false, int MaxIterations = 1000, double discrepancy = 0.0000001)
        {
            this.m = m;
            this.MaxIterations = MaxIterations;
            Discrepancy = discrepancy;
            Preonditioner = preconditioner;
        }
        public override void Solve(SBand A)
        {
            double[] Right = A.GetRight;
            double norm_r0 = 0, norm_w = 0;
            int iterationsCount = m;
            int Size = A.Size;
            //матрица v размера A->N * (m + 1), содержащая 
            //ортонольмальный базис подпространства Крылова
            //хранится транспонированной
            //
            double[] x = new double[Size];
            double[][] v = new double[(m + 1)][];
            for (int i = 0; i < m + 1; i++)
                v[i] = new double[Size];
            double[] w = new double[Size];
            //вектор невязки
            double[] r0 = new double[Size];
            //вспомогательный вектор для произведения А*х
            double[] Ax0 = new double[Size];
            //матрица Хессенберга H размера m*(m + 1)
            double[] H = new double[(m + 1) * m];
            //вектор правой части для системы с матрицей Хессенберга
            double[] eBeta = new double[m + 1];
            //
            if (Preonditioner) 
                ILUDecomp(A, out L, out U);
            //1. Предварительный шаг
            // цикл итерациям метода GMRES
            for (int iter = 0; iter < MaxIterations; iter++)
            {
                // вычисление r[0] и его нормы
                Ax0 = A.MultiplyToMatrix(x);
                //
                
                for (int i = 0; i < Size; i++)
                    r0[i] = Right[i] - Ax0[i];

                if (Preonditioner)
                {
                    double[] buff = L.MultiplyToMatrix(r0);
                    r0 = U.MultiplyToMatrix(buff);
                }
                //
                for (uint l = 0; l < Size; l++)
                {
                    for (uint k = 0; k < m; k++)
                        v[k][l] = 0;
                    w[l] = 0;
                }
                //определение нормы вектора r0
                norm_r0 = 0;
                for (int i = 0; i < r0.Length; i++)
                    norm_r0 += r0[i] * r0[i];
                double beta = Math.Sqrt(norm_r0);
                //проверка на выход из цикла
                if (beta < Discrepancy) 
                    break;
                //вычисление v[i]
                for (int i = 0; i < Size; i++)
                    v[0][i] = r0[i] / beta;
                //2. Основные итерации метода
                for (int j = 0; j < m; j++)
                {
                    //вычисление w[j] = A * v[j]
                    w = A.MultiplyToMatrix(v[j]);
                    //
                    if (Preonditioner)
                    {
                        double[] buf = L.MultiplyToMatrix(w);
                        w = U.MultiplyToMatrix(buf);
                    }
                    //
                    for (int i = 0; i <= j; i++)
                    {
                        //H[i][j] = (w[j], v[i])
                        H[i * m + j] = 0;
                        for (int k = 0; k < Size; k++)
                            H[i * m + j] += w[k] * v[i][k];
                        //
                        //w[j] = w[j] - H[i][j]*v[i]  
                        for (int k = 0; k < Size; k++)
                            w[k] -= H[i * m + j] * v[i][k];
                    }
                    // определеие нормы вектора w
                    norm_w = 0;
                    for (int i = 0; i < Size; i++)
                        norm_w += w[i] * w[i];
                    //H[j + 1][j] = ||w[j]||
                    H[(j + 1) * m + j] = Math.Sqrt(norm_w);
                    //
                    //достигнуто ли точное решение?
                    if (Math.Abs(H[(j + 1) * m + j]) < Discrepancy)
                    {
                        iterationsCount = j;
                        break;
                    }
                    //v[j + 1] = w[j] / H[j + 1][j]
                    double n = H[(j + 1) * m + j];
                    for (int k = 0; k < Size; k++)
                        v[j + 1][k] = w[k] / n;
                }
                //
                //3. Вычисление приближенного решения
                for (uint k = 0; k < iterationsCount + 1; k++)
                    eBeta[k] = beta;
                //выполнение поворотов Гивенса
                double c, s, oldValue;
                //double Hii = H[0];
                for (int i = 0; i < iterationsCount; i++)
                {
                    ////константы с, s
                    c = H[i * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
                    H[(i + 1) * m + i] * H[(i + 1) * m + i]);
                    s = H[(i + 1) * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
                    H[(i + 1) * m + i] * H[(i + 1) * m + i]);
                    //модификация матрицы H
                    for (int j = i; j < iterationsCount; j++)
                    {
                        oldValue = H[i * m + j];
                        H[i * m + j] = oldValue * c + H[(i + 1) * m + j] * s;
                        H[(i + 1) * m + j] =
                        H[(i + 1) * m + j] * c - oldValue * s;
                    }
                    //модификация вектора правой части eBeta
                    double oldBeta = eBeta[i];
                    eBeta[i] = c * oldBeta;
                    eBeta[i + 1] = -oldBeta * s;
                }
                //вычисление y[m] как решения системы 
                //H*y = eBeta без нижней строки
                for (int i = iterationsCount - 1; i >= 0; i--)
                {
                    eBeta[i] = eBeta[i];
                    for (int j = i + 1; j < iterationsCount; j++)
                        eBeta[i] -= eBeta[j] * H[i * m + j];
                    //
                    eBeta[i] /= H[i * m + i];
                }
                //вычисление x[m], y[m] хранится в векторе eBeta
                double product;
                for (int i = 0; i < Size; i++)
                {
                    product = 0.0;
                    for (int j = 0; j < iterationsCount; j++)
                        product += v[j][i] * eBeta[j];
                    //
                    x[i] = 1.0 * x[i] + 1.0 * product;// вместо 1.0 можно поставить коэффициенты релаксации
                }
            }
            A.SetX(x);
            //double[] res = A.MultiplyToMatrix(x);
        }
        public override void Solve(SRowPacked A)
        {
            //
            float[] M;
            int[] Cols;
            int[] RowIndex;
            double[] Right = A.GetRight;
            A.PackMatrixToArrays(out M, out Cols, out RowIndex);
            //
            double norm_r0 = 0, norm_w = 0;
            int iterationsCount = m;
            int Size = A.Size;
            //матрица v размера A->N * (m + 1), содержащая 
            //ортонольмальный базис подпространства Крылова
            //хранится транспонированной
            //
            float[] x = new float[Size];
            float[] v = new float[(m + 1) * Size];
            float[] w = new float[Size];
            //вектор невязки
            double[] r0 = new double[Size];
            //вспомогательный вектор для произведения А*х
            float[] Ax0 = new float[Size];
            //матрица Хессенберга H размера m*(m + 1)
            float[] H = new float[(m + 1) * m];
            //вектор правой части для системы с матрицей Хессенберга
            double[] eBeta = new double[m + 1];
            if (Preonditioner)
                ILUDecomp(M, Cols, RowIndex, out L, out U);
            //----
            //var handle = GCHandle.Alloc(H, GCHandleType.Pinned);
            //var handle2 = GCHandle.Alloc(w, GCHandleType.Pinned);
            // retrieve a raw pointer to pass to the native code:
            //IntPtr ptr = handle.AddrOfPinnedObject();
            //IntPtr ptr2 = handle2.AddrOfPinnedObject();

            // later, possibly in some other method:
            //handle.Free();
            //
            ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
            //Инициализация OpenCl, выбор устройства
            ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
            //Считывание текста программы из файла
            StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\GMRESiteration.cl");
            string[] clSource = new string[2];
            clSource[0] = streamReader.ReadToEnd();
            streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MultiplicationPacked.cl");
            clSource[1] = streamReader.ReadToEnd();
            streamReader.Close();
            //Компиляция программы
            ComputeProgram program = new ComputeProgram(context, clSource);
            program.Build(context.Devices, "", null, IntPtr.Zero);
            //Создание ядра
            ComputeKernel KernelMultiplicationPackedH = program.CreateKernel("MultiplicationPackedH");
            ComputeKernel KernelHUpdate = program.CreateKernel("HUpdate");
            ComputeKernel KernelWUpdare = program.CreateKernel("WUpdare");
            ComputeKernel KernelVUpdare = program.CreateKernel("VUpdare");
            ComputeKernel kernelMultiplicationPacked = program.CreateKernel("MultiplicationPacked");
            //Создание буферов параметров
            //Выделение памяти на device(OpenCl) под переменные
            ComputeBuffer<float> VV = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, v);
            ComputeBuffer<float> WW = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, w);
            ComputeBuffer<float> MM = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, M);
            ComputeBuffer<int> COLS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Cols);
            ComputeBuffer<int> POINTERS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, RowIndex);
            float[] w2 = new float[Size]; // group size
            ComputeBuffer<float> W2 = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, w2);
            ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, x);
            ComputeBuffer<float> YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, Ax0);
            // установка буферов в kernel
            // (__global const float * M, __global const int * cols, __global const int * pointers, __global const float * v, int h, __global float * w)
            KernelMultiplicationPackedH.SetMemoryArgument(0, MM);
            KernelMultiplicationPackedH.SetMemoryArgument(1, COLS);
            KernelMultiplicationPackedH.SetMemoryArgument(2, POINTERS);
            KernelMultiplicationPackedH.SetMemoryArgument(3, VV);
            KernelMultiplicationPackedH.SetMemoryArgument(5, WW);
            //(__global const float* v, __global const float* w, int i, __global float* res)
            KernelHUpdate.SetMemoryArgument(0, VV);
            KernelHUpdate.SetMemoryArgument(1, WW);
            KernelHUpdate.SetMemoryArgument(3, W2);
            //(float H, __global const float* v, __global float* w, int i)
            KernelWUpdare.SetMemoryArgument(1, VV);
            KernelWUpdare.SetMemoryArgument(2, WW);
            //(__global float* v, __global const float* w, int ip1, float n)
            KernelVUpdare.SetMemoryArgument(0, VV);
            KernelVUpdare.SetMemoryArgument(1, WW);
            // MultiplyToMatrix(float[] x, float[] A, int[] Cols, int[] Pointers, int Size)
            kernelMultiplicationPacked.SetMemoryArgument(0, MM);
            kernelMultiplicationPacked.SetMemoryArgument(1, COLS);
            kernelMultiplicationPacked.SetMemoryArgument(2, POINTERS);
            kernelMultiplicationPacked.SetMemoryArgument(3, XX);
            kernelMultiplicationPacked.SetMemoryArgument(4, YY);
            //
            //массив, определяющий размерность расчета (количество потоков в определенном измерении)
            long[] globalSize = new long[1];
            globalSize[0] = Size / 10;
            //
            //1. Предварительный шаг
            // цикл итерациям метода GMRES
            for (int iter = 0; iter < MaxIterations; iter++)
            {
                // вычисление r[0] и его нормы
                if (OpenCL)
                {
                    //Ax0 = MultiplyToMatrix(x, M, Cols, RowIndex, Size);
                    //Вызов ядра
                    commands.Execute(kernelMultiplicationPacked, null, globalSize, null, null);//ok
                    //Ожидание окончания выполнения программы
                    commands.Finish();
                    //commands.ReadFromBuffer(YY, ref Ax0, true, null);
                }
                else
                {
                    float sum = 0;
                    for (int i = 0; i < RowIndex.Length - 1; i++)
                    {
                        int p1 = RowIndex[i];
                        int p2 = RowIndex[i + 1];
                        //
                        sum = 0;
                        //
                        for (int k = p1; k < p2; k++)
                        {
                            sum += M[k] * x[Cols[k]];
                        }
                        Ax0[i] = sum;
                    }
                }

                //
                for (int i = 0; i < Size; i++)
                    r0[i] = Right[i] - Ax0[i];
                //
                if (Preonditioner)
                {
                    double[] buff = L.MultiplyToMatrix(r0);
                    r0 = U.MultiplyToMatrix(buff);
                }
                //
                for (uint l = 0; l < Size; l++)
                {
                    for (uint k = 0; k < m; k++)
                        v[k * Size + l] = 0;
                    w[l] = 0;
                }
                //определение нормы вектора r0
                norm_r0 = 0;
                for (int i = 0; i < r0.Length; i++)
                    norm_r0 += r0[i] * r0[i];
                double beta = Math.Sqrt(norm_r0);
                //проверка на выход из цикла
                if (beta < Discrepancy)
                    break;
                //вычисление v[i]
                for (int i = 0; i < Size; i++)
                    v[i] = (float)(r0[i] / beta);
                //2. Основные итерации метода
                //
                //
                if (OpenCL)
                {
                    //
                    for (int j = 0; j < m; j++)
                    {

                        KernelMultiplicationPackedH.SetValueArgument<int>(4, j);
                        //Вызов ядра
                        commands.Execute(KernelMultiplicationPackedH, null, globalSize, null, null);//ok
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        //commands.ReadFromBuffer(WW, ref w, true, null);
                        //
                        for (int i = 0; i <= j; i++)
                        {
                            //    //
                            KernelHUpdate.SetValueArgument<int>(2, i);
                            //Вызов ядра
                            commands.Execute(KernelHUpdate, null, globalSize, null, null);//ok
                            //Ожидание окончания выполнения программы
                            commands.Finish();
                            //commands.ReadFromBuffer(W2, ref w2, true, null);
                            H[i * m + j] = 0;
                            float Hl = 0;
                            for (int k = 0; k < Size / 10; k++)
                            {
                                Hl += w2[k];
                                w2[k] = 0;
                            }
                            //
                            H[i * m + j] = Hl;
                            //H[i * m + j] = 0;
                            //for (int k = 0; k < Size; k++)
                            //    H[i * m + j] += w[k] * v[i * Size + k];
                            //
                            KernelWUpdare.SetValueArgument(0, Hl);
                            KernelWUpdare.SetValueArgument(3, i);
                            //Вызов ядра
                            commands.Execute(KernelWUpdare, null, globalSize, null, null);// ok
                            //Ожидание окончания выполнения программы
                            commands.Finish();
                            //commands.ReadFromBuffer(WW, ref w, true, null);
                            //
                        }
                        //
                        norm_w = 0;
                        /*
                        globalSize[0] = (long)(Math.Round(Size / 256.0, MidpointRounding.AwayFromZero)*256);
                        Kernelnorm.SetValueArgument(3, Size);
                        //for (int k = 0; k < Size; k++)
                        //{
                        //    w[k] = (float)1;
                        //    w2[k] = 0;
                        //}
                        //
                        //Вызов ядра
                        commands.Execute(Kernelnorm, null, globalSize, new long[] { 256 }, null);//??
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        //
                        int n = 0;
                        while ((w2[n] + w2[n + 1]) != 0)
                        {
                            norm_w += w2[n];
                            w2[n] = 0;
                            n++;
                        }
                        */
                        //commands.ReadFromBuffer(WW, ref w, true, null);
                        for (int i = 0; i < Size; i++)
                            norm_w += w[i] * w[i];
                        //
                        float sqnw = (float)Math.Sqrt(norm_w);
                        H[(j + 1) * m + j] = sqnw;
                        ////
                        //достигнуто ли точное решение?
                        if (Math.Abs(H[(j + 1) * m + j]) < Discrepancy)
                        {
                            iterationsCount = j;
                            break;
                        }
                        //
                        KernelVUpdare.SetValueArgument(2, j + 1);
                        KernelVUpdare.SetValueArgument(3, sqnw);
                        globalSize[0] = Size / 10;
                        //Вызов ядра
                        commands.Execute(KernelVUpdare, null, globalSize, null, null);//ok
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        //commands.ReadFromBuffer(VV, ref v, true, null);
                    }
                    //
                }
                else
                {
                    for (int j = 0; j < m; j++)
                    {
                        //вычисление w[j] = A * v[j]
                        float[] vj = new float[Size];
                        for (int f = 0; f < Size; f++)
                            vj[f] = v[j * Size + f];
                        //
                        w = MultiplyToMatrix(vj, M, Cols, RowIndex, Size);
                        if (Preonditioner)
                        {
                            float[] buf = L.MultiplyToMatrix(w);
                            w = U.MultiplyToMatrix(buf);
                        }
                        //
                        for (int i = 0; i <= j; i++)
                        {
                            //
                            H[i * m + j] = 0;
                            for (int k = 0; k < Size; k++)
                                H[i * m + j] += w[k] * v[i * Size + k];
                            //
                            //  
                            for (int k = 0; k < Size; k++)
                                w[k] -= H[i * m + j] * v[i * Size + k];
                        }
                        //firstOperation(v, w, H, Size, j);
                        // определеие нормы вектора w
                        norm_w = 0;
                        for (int i = 0; i < Size; i++)
                            norm_w += w[i] * w[i];
                        //H[j + 1][j] = ||w[j]||
                        H[(j + 1) * m + j] = (float)Math.Sqrt(norm_w);
                        //
                        //достигнуто ли точное решение?
                        if (Math.Abs(H[(j + 1) * m + j]) < Discrepancy)
                        {
                            iterationsCount = j;
                            break;
                        }
                        //v[j + 1] = w[j] / H[j + 1][j]
                        double n = H[(j + 1) * m + j];
                        for (int k = 0; k < Size; k++)
                            v[(j + 1) * Size + k] = (float)(w[k] / n);
                    }
                }
                //
                //3. Вычисление приближенного решения
                for (uint k = 0; k < iterationsCount + 1; k++)
                    eBeta[k] = beta;
                //выполнение поворотов Гивенса
                double c, s, oldValue;
                //double Hii = H[0];
                for (int i = 0; i < iterationsCount; i++)
                {
                    ////константы с, s
                    c = H[i * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
                    H[(i + 1) * m + i] * H[(i + 1) * m + i]);
                    s = H[(i + 1) * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
                    H[(i + 1) * m + i] * H[(i + 1) * m + i]);
                    //модификация матрицы H
                    for (int j = i; j < iterationsCount; j++)
                    {
                        oldValue = H[i * m + j];
                        H[i * m + j] = (float)(oldValue * c + H[(i + 1) * m + j] * s);
                        H[(i + 1) * m + j] = (float)(H[(i + 1) * m + j] * c - oldValue * s);
                    }
                    //модификация вектора правой части eBeta
                    double oldBeta = eBeta[i];
                    eBeta[i] = c * oldBeta;
                    eBeta[i + 1] = -oldBeta * s;
                }
                //вычисление y[m] как решения системы 
                //H*y = eBeta без нижней строки
                for (int i = iterationsCount - 1; i >= 0; i--)
                {
                    eBeta[i] = eBeta[i];
                    for (int j = i + 1; j < iterationsCount; j++)
                        eBeta[i] -= eBeta[j] * H[i * m + j];
                    //
                    eBeta[i] /= H[i * m + i];
                }
                //вычисление x[m], y[m] хранится в векторе eBeta
                //commands.ReadFromBuffer(VV, ref v, true, null);
                float product;
                for (int i = 0; i < Size; i++)
                {
                    product = 0.0f;
                    for (int j = 0; j < iterationsCount; j++)
                        product += (float)(v[j * Size + i] * eBeta[j]);
                    //
                    x[i] = 1.0f * x[i] + 1.0f * product;// вместо 1.0 можно поставить коэффициенты релаксации
                }
            }
            //
            W2.Dispose();
            POINTERS.Dispose();
            COLS.Dispose();
            MM.Dispose();
            WW.Dispose();
            VV.Dispose();
            //Kernelnorm.Dispose();
            KernelVUpdare.Dispose();
            KernelWUpdare.Dispose();
            KernelHUpdate.Dispose();
            KernelMultiplicationPackedH.Dispose();
            program.Dispose();
            commands.Dispose();
            context.Dispose();
            //
            //handle.Free();
            //handle2.Free();
            //
            double[] Xd = new double[x.Length];
            for (int i = 0; i < Xd.Length; i++)
                Xd[i] = (double)x[i];
            //
            A.SetX(Xd);
            //double[] res = MultiplyToMatrix(x, M, Cols, RowIndex, Size);
        }
        //public override void Solve(SRowPacked A)
        //{
        //    float[] M;
        //    int[] Cols;
        //    int[] RowIndex;
        //    double[] Right = A.GetRight;
        //    A.PackMatrixToArrays(out M, out Cols, out RowIndex);
        //    //
        //    double norm_r0 = 0, norm_w = 0;
        //    int iterationsCount = m;
        //    int Size = A.Size;
        //    //матрица v размера A->N * (m + 1), содержащая 
        //    //ортонольмальный базис подпространства Крылова
        //    //хранится транспонированной
        //    //
        //    float[] x = new float[Size];
        //    float[][] v = new float[(m + 1)][];
        //    for (int i = 0; i < m + 1; i++)
        //        v[i] = new float[Size];
        //    float[] w = new float[Size];
        //    //вектор невязки
        //    double[] r0 = new double[Size];
        //    //вспомогательный вектор для произведения А*х
        //    float[] Ax0 = new float[Size];
        //    //матрица Хессенберга H размера m*(m + 1)
        //    float[] H = new float[(m + 1) * m];
        //    //вектор правой части для системы с матрицей Хессенберга
        //    double[] eBeta = new double[m + 1];
        //    if (Preonditioner)
        //        ILUDecomp(M, Cols, RowIndex, out L, out U);
        //    //----
        //    //1. Предварительный шаг
        //    // цикл итерациям метода GMRES
        //    for (int iter = 0; iter < MaxIterations; iter++)
        //    {
        //        // вычисление r[0] и его нормы
        //        Ax0 = MultiplyToMatrix(x, M, Cols, RowIndex, Size);
        //        //
        //        for (int i = 0; i < Size; i++)
        //            r0[i] = Right[i] - Ax0[i];
        //        //
        //        if (Preonditioner)
        //        {
        //            double[] buff = L.MultiplyToMatrix(r0);
        //            r0 = U.MultiplyToMatrix(buff);
        //        }
        //        //
        //        for (uint l = 0; l < Size; l++)
        //        {
        //            for (uint k = 0; k < m; k++)
        //                v[k][l] = 0;
        //            w[l] = 0;
        //        }
        //        //определение нормы вектора r0
        //        norm_r0 = 0;
        //        for (int i = 0; i < r0.Length; i++)
        //            norm_r0 += r0[i] * r0[i];
        //        double beta = Math.Sqrt(norm_r0);
        //        //проверка на выход из цикла
        //        if (beta < Discrepancy)
        //            break;
        //        //вычисление v[i]
        //        for (int i = 0; i < Size; i++)
        //            v[0][i] = (float)(r0[i] / beta);
        //        //2. Основные итерации метода
        //        for (int j = 0; j < m; j++)
        //        {
        //            //вычисление w[j] = A * v[j]
        //            w = MultiplyToMatrix(v[j], M, Cols, RowIndex, Size);
        //            if (Preonditioner)
        //            {
        //                float[] buf = L.MultiplyToMatrix(w);
        //                w = U.MultiplyToMatrix(buf);
        //                buf = null;
        //            }
        //            //
        //            VectorSum(H, v, w, j, Size);
        //            //for (int i = 0; i <= j; i++)
        //            //{
        //            //    //H[i][j] = (w[j], v[i])
        //            //    H[i * m + j] = 0;
        //            //    for (int k = 0; k < Size; k++)
        //            //        H[i * m + j] += w[k] * v[i][k];
        //            //    //
        //            //    //w[j] = w[j] - H[i][j]*v[i]  
        //            //    for (int k = 0; k < Size; k++)
        //            //        w[k] -= H[i * m + j] * v[i][k];
        //            //}
        //            // определеие нормы вектора w
        //            norm_w = 0;
        //            for (int i = 0; i < Size; i++)
        //                norm_w += w[i] * w[i];
        //            //H[j + 1][j] = ||w[j]||
        //            H[(j + 1) * m + j] = (float)Math.Sqrt(norm_w);
        //            //
        //            //достигнуто ли точное решение?
        //            if (Math.Abs(H[(j + 1) * m + j]) < Discrepancy)
        //            {
        //                iterationsCount = j;
        //                break;
        //            }
        //            //v[j + 1] = w[j] / H[j + 1][j]
        //            double n = H[(j + 1) * m + j];
        //            for (int k = 0; k < Size; k++)
        //                v[j + 1][k] = (float)(w[k] / n);
        //        }
        //        //
        //        //3. Вычисление приближенного решения
        //        for (uint k = 0; k < iterationsCount + 1; k++)
        //            eBeta[k] = beta;
        //        //выполнение поворотов Гивенса
        //        double c, s, oldValue;
        //        //double Hii = H[0];
        //        for (int i = 0; i < iterationsCount; i++)
        //        {
        //            ////константы с, s
        //            c = H[i * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
        //            H[(i + 1) * m + i] * H[(i + 1) * m + i]);
        //            s = H[(i + 1) * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
        //            H[(i + 1) * m + i] * H[(i + 1) * m + i]);
        //            //модификация матрицы H
        //            for (int j = i; j < iterationsCount; j++)
        //            {
        //                oldValue = H[i * m + j];
        //                H[i * m + j] = (float)(oldValue * c + H[(i + 1) * m + j] * s);
        //                H[(i + 1) * m + j] = (float)(H[(i + 1) * m + j] * c - oldValue * s);
        //            }
        //            //модификация вектора правой части eBeta
        //            double oldBeta = eBeta[i];
        //            eBeta[i] = c * oldBeta;
        //            eBeta[i + 1] = -oldBeta * s;
        //        }
        //        //вычисление y[m] как решения системы 
        //        //H*y = eBeta без нижней строки
        //        for (int i = iterationsCount - 1; i >= 0; i--)
        //        {
        //            eBeta[i] = eBeta[i];
        //            for (int j = i + 1; j < iterationsCount; j++)
        //                eBeta[i] -= eBeta[j] * H[i * m + j];
        //            //
        //            eBeta[i] /= H[i * m + i];
        //        }
        //        //вычисление x[m], y[m] хранится в векторе eBeta
        //        float product;
        //        for (int i = 0; i < Size; i++)
        //        {
        //            product = 0.0f;
        //            for (int j = 0; j < iterationsCount; j++)
        //                product += (float)(v[j][i] * eBeta[j]);
        //            //
        //            x[i] = 1.0f * x[i] + 1.0f * product;// вместо 1.0 можно поставить коэффициенты релаксации
        //        }
        //    }
        //    double[] Xd = new double[x.Length];
        //    for (int i = 0; i < Xd.Length; i++)
        //        Xd[i] = (double)x[i];
        //    //
        //    A.SetX(Xd);
        //    //double[] res = MultiplyToMatrix(x, M, Cols, RowIndex, Size);
        //}

        private void VectorSum(float[] H, float[][] v, float[] w, int j, int Size)
        {
            if (OpenCL)
            {
                float[] bufH = new float[1];
                float[] res = new float[Size];
                try
                {
                    //Выбор платформы расчета, создание контекста
                    ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
                    ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
                    //Инициализация OpenCl, выбор устройства
                    ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
                    //Считывание текста программы из файла
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\VectorSum.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("VH");
                    ComputeKernel kernel2 = program.CreateKernel("VW");
                    //Создание буферов параметров
                    // (__global float* res, __global const float* v, __global float* w)
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> WW = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, w);
                    ComputeBuffer<float> RR = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.CopyHostPointer, res);
                    ComputeBuffer<float> VV; ComputeBuffer<float> H1;
                    //
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[1];
                    globalSize[0] = Size;
                    for (int i = 0; i < j + 1; i++)
                    {
                        //
                        VV = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, v[i]);
                        kernel.SetMemoryArgument(0, RR);
                        kernel.SetMemoryArgument(1, VV);
                        kernel.SetMemoryArgument(2, WW);
                        //Вызов ядра
                        commands.Execute(kernel, null, globalSize, null, null);
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        //
                        commands.ReadFromBuffer(RR, ref res, true, null);
                        //
                        H[i * m + j] = 0;
                        float Hl = 0;
                        for (int k = 0; k < Size; k++)
                            Hl += res[k];
                        //
                        H[i * m + j] = Hl;
                        //
                        res = new float[Size];
                        //
                        //__global float* H1, __global const float* v, __global float* w
                        bufH[0] = Hl;
                        H1 = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, bufH);
                        kernel2.SetMemoryArgument(0, H1);
                        kernel2.SetMemoryArgument(1, VV);
                        kernel2.SetMemoryArgument(2, WW);
                        //Вызов ядра
                        commands.Execute(kernel2, null, globalSize, null, null);
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        commands.ReadFromBuffer(WW, ref w, true, null);
                        H1.Dispose();
                        VV.Dispose();
                    }
                    // чтение искомой функции из буфера kernel-а
                    
                    //
                    kernel.Dispose();
                    kernel2.Dispose();
                    RR.Dispose();
                    WW.Dispose();
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
                for (int i = 0; i <= j; i++)
                {
                    //
                    H[i * m + j] = 0;
                    for (int k = 0; k < Size; k++)
                        H[i * m + j] += w[k] * v[i][k];
                    //
                    //  
                    for (int k = 0; k < Size; k++)
                        w[k] -= H[i * m + j] * v[i][k];
                }
            }
        }
        public override void Solve(SSquare A)
        {
            double[] Right = A.GetRight;
            double norm_r0 = 0, norm_w = 0;
            int iterationsCount = m;
            int Size = A.Size;
            //матрица v размера A->N * (m + 1), содержащая 
            //ортонольмальный базис подпространства Крылова
            //хранится транспонированной
            //
            double[] x = new double[Size];
            double[][] v = new double[(m + 1)][];
            for (int i = 0; i < m + 1; i++)
                v[i] = new double[Size];
            double[] w = new double[Size];
            //вектор невязки
            double[] r0 = new double[Size];
            //вспомогательный вектор для произведения А*х
            double[] Ax0 = new double[Size];
            //матрица Хессенберга H размера m*(m + 1)
            double[] H = new double[(m + 1) * m];
            //вектор правой части для системы с матрицей Хессенберга
            double[] eBeta = new double[m + 1];
            if (Preonditioner)
                ILUDecomp(A, out Lsq, out Usq);
            //1. Предварительный шаг
            // цикл итерациям метода GMRES
            for (int iter = 0; iter < MaxIterations; iter++)
            {
                // вычисление r[0] и его нормы
                Ax0 = A.MultiplyToMatrix(x);
                //
                for (int i = 0; i < Size; i++)
                    r0[i] = Right[i] - Ax0[i];
                //
                if (Preonditioner)
                {
                    double[] buff = Lsq.MultiplyToMatrix(r0);
                    r0 = Usq.MultiplyToMatrix(buff);
                }
                //
                for (uint l = 0; l < Size; l++)
                {
                    for (uint k = 0; k < m; k++)
                        v[k][l] = 0;
                    w[l] = 0;
                }
                //определение нормы вектора r0
                norm_r0 = 0;
                for (int i = 0; i < r0.Length; i++)
                    norm_r0 += r0[i] * r0[i];
                double beta = Math.Sqrt(norm_r0);
                //проверка на выход из цикла
                if (beta < Discrepancy) 
                    break;
                //вычисление v[i]
                for (int i = 0; i < Size; i++)
                    v[0][i] = r0[i] / beta;
                //2. Основные итерации метода
                for (int j = 0; j < m; j++)
                {
                    //вычисление w[j] = A * v[j]
                    w = A.MultiplyToMatrix(v[j]);
                    //
                    if (Preonditioner)
                    {
                        double[] buf = Lsq.MultiplyToMatrix(w);
                        w = Usq.MultiplyToMatrix(buf);
                    }
                    //
                    for (int i = 0; i <= j; i++)
                    {
                        //H[i][j] = (w[j], v[i])
                        H[i * m + j] = 0;
                        for (int k = 0; k < Size; k++)
                            H[i * m + j] += w[k] * v[i][k];
                        //
                        //w[j] = w[j] - H[i][j]*v[i]  
                        for (int k = 0; k < Size; k++)
                            w[k] -= H[i * m + j] * v[i][k];
                    }
                    // определеие нормы вектора w
                    norm_w = 0;
                    for (int i = 0; i < Size; i++)
                        norm_w += w[i] * w[i];
                    //H[j + 1][j] = ||w[j]||
                    H[(j + 1) * m + j] = Math.Sqrt(norm_w);
                    //
                    //достигнуто ли точное решение?
                    if (Math.Abs(H[(j + 1) * m + j]) < Discrepancy)
                    {
                        iterationsCount = j;
                        break;
                    }
                    //v[j + 1] = w[j] / H[j + 1][j]
                    double n = H[(j + 1) * m + j];
                    for (int k = 0; k < Size; k++)
                        v[j + 1][k] = w[k] / n;
                }
                //
                //3. Вычисление приближенного решения
                for (uint k = 0; k < iterationsCount + 1; k++)
                    eBeta[k] = beta;
                //выполнение поворотов Гивенса
                double c, s, oldValue;
                //double Hii = H[0];
                for (int i = 0; i < iterationsCount; i++)
                {
                    ////константы с, s
                    c = H[i * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
                    H[(i + 1) * m + i] * H[(i + 1) * m + i]);
                    s = H[(i + 1) * m + i] / Math.Sqrt(H[i * m + i] * H[i * m + i] +
                    H[(i + 1) * m + i] * H[(i + 1) * m + i]);
                    //модификация матрицы H
                    for (int j = i; j < iterationsCount; j++)
                    {
                        oldValue = H[i * m + j];
                        H[i * m + j] = oldValue * c + H[(i + 1) * m + j] * s;
                        H[(i + 1) * m + j] =
                        H[(i + 1) * m + j] * c - oldValue * s;
                    }
                    //модификация вектора правой части eBeta
                    double oldBeta = eBeta[i];
                    eBeta[i] = c * oldBeta;
                    eBeta[i + 1] = -oldBeta * s;
                }
                //вычисление y[m] как решения системы 
                //H*y = eBeta без нижней строки
                for (int i = iterationsCount - 1; i >= 0; i--)
                {
                    eBeta[i] = eBeta[i];
                    for (int j = i + 1; j < iterationsCount; j++)
                        eBeta[i] -= eBeta[j] * H[i * m + j];
                    //
                    eBeta[i] /= H[i * m + i];
                }
                //вычисление x[m], y[m] хранится в векторе eBeta
                double product;
                for (int i = 0; i < Size; i++)
                {
                    product = 0.0;
                    for (int j = 0; j < iterationsCount; j++)
                        product += v[j][i] * eBeta[j];
                    //
                    x[i] = 1.0 * x[i] + 1.0 * product;// вместо 1.0 можно поставить коэффициенты релаксации
                }
            }
             A.SetX(x);
            double[] res = A.MultiplyToMatrix(x);
        }
        //
        void firstOperation(float[] V, float[] W, float []H, int Size, int j)
        {
            //for (int i = 0; i <= j; i++)
            //{
            //
            //    H[i * m + j] = 0;
            //    for (int k = 0; k < Size; k++)+
            //        H[i * m + j] += w[k] * v[i * Size + k];+
            // 
            //    for (int k = 0; k < Size; k++)
            //        w[k] -= H[i * m + j] * v[i * Size + k];
            //}
            if (OpenCL)
             {
                 float[] res = new float[Size/4];
                try
                {
                    //Выбор платформы расчета, создание контекста
                    ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
                    ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
                    //Инициализация OpenCl, выбор устройства
                    ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
                    //Считывание текста программы из файла
                    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\FirstOperation.cl");
                    string clSource = streamReader.ReadToEnd();
                    streamReader.Close();
                    //Компиляция программы
                    ComputeProgram program = new ComputeProgram(context, clSource);
                    program.Build(context.Devices, "", null, IntPtr.Zero);
                    //Создание ядра
                    ComputeKernel kernel = program.CreateKernel("FirstOperation1");
                    ComputeKernel kernel2 = program.CreateKernel("FirstOperation2");
                    //Создание буферов параметров
                    // (__global const float* v, __global const float* w, int i, __global float* res)
                    //Выделение памяти на device(OpenCl) под переменные
                    ComputeBuffer<float> VV = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, V);
                    ComputeBuffer<float> WW = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, W);
                    ComputeBuffer<float> RR = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.CopyHostPointer, res);
                    // установка буферов в kernel
                    kernel.SetMemoryArgument(0, VV);
                    kernel.SetMemoryArgument(1, WW);
                    kernel.SetMemoryArgument(3, RR);
                    //
                    kernel2.SetMemoryArgument(1, VV);
                    kernel2.SetMemoryArgument(2, WW);
                    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
                    long[] globalSize = new long[1];
                    
                    for (int i = 0; i < j + 1; i++)
                    {
                        globalSize[0] = Size / 4;
                        kernel.SetValueArgument<int>(2, i);
                        //Вызов ядра
                        commands.Execute(kernel, null, globalSize, null, null);
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        commands.ReadFromBuffer(RR, ref res, true, null);
                        //
                        H[i * m + j] = 0;
                        float Hl = 0;
                        for (int k = 0; k < Size/4; k++)
                            Hl += res[k];
                        //
                        H[i * m + j] = Hl;
                        globalSize[0] = Size;
                        //
                        ComputeBuffer<float> Hh = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, new float[] { Hl });
                        //очищение памяти
                        //__kernel void FirstOperation2 (float H, __global const float* v, __global float* w, int i)
                        kernel2.SetMemoryArgument(0, Hh);
                        kernel2.SetValueArgument<int>(3, i);
                        //Вызов ядра
                        commands.Execute(kernel2, null, globalSize, null, null);
                        //Ожидание окончания выполнения программы
                        commands.Finish();
                        commands.ReadFromBuffer(WW, ref W, true, null);
                        Hh.Dispose();
                    }
                    //
                    kernel2.Dispose();
                    kernel.Dispose();
                    RR.Dispose();
                    WW.Dispose();
                    VV.Dispose();
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
                for (int i = 0; i <= j; i++)
                {
                    //
                    H[i * m + j] = 0;
                    for (int k = 0; k < Size; k++)
                        H[i * m + j] += W[k] * V[i * Size + k];
                    //
                    //  
                    for (int k = 0; k < Size; k++)
                        W[k] -= H[i * m + j] * V[i * Size + k];
                }
            }
        }
        public float[] MultiplyToMatrix(float[] x, float[] A, int[] Cols, int[] Pointers, int Size)
        {
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
                    globalSize[0] = Size/10;
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
            //float[] y = new float[Size];
            //try
            //{
            //    //Выбор платформы расчета, создание контекста
            //    ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[1]);
            //    ComputeContext context = new ComputeContext(ComputeDeviceTypes.All, properties, null, IntPtr.Zero);
            //    //Инициализация OpenCl, выбор устройства
            //    ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
            //    //Считывание текста программы из файла
            //    string s = AppDomain.CurrentDomain.BaseDirectory;
            //    StreamReader streamReader = new StreamReader("..\\..\\..\\AlgebraLibrary\\OpenCL\\MultiplicationPacked.cl");
            //    string clSource = streamReader.ReadToEnd();
            //    streamReader.Close();
            //    //Компиляция программы
            //    ComputeProgram program = new ComputeProgram(context, clSource);
            //    program.Build(context.Devices, "", null, IntPtr.Zero);
            //    //Создание ядра
            //    ComputeKernel kernel = program.CreateKernel("MultiplicationPacked");
            //    //Создание буферов параметров
            //    // (__local const float * a, __local const int * cols, __local const int * pointers, __local const float * x, __global float * y)
            //    //Выделение памяти на device(OpenCl) под переменные
            //    ComputeBuffer<float> AA = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, A);
            //    ComputeBuffer<float> XX = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, x);
            //    ComputeBuffer<float> YY = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, y);
            //    ComputeBuffer<int> COLS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Cols);
            //    ComputeBuffer<int> POINTERS = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, Pointers);
            //    // установка буферов в kernel
            //    kernel.SetMemoryArgument(0, AA);
            //    kernel.SetMemoryArgument(1, COLS);
            //    kernel.SetMemoryArgument(2, POINTERS);
            //    kernel.SetMemoryArgument(3, XX);
            //    kernel.SetMemoryArgument(4, YY);
            //    //массив, определяющий размерность расчета (количество потоков в определенном измерении)
            //    long[] globalSize = new long[1];
            //    globalSize[0] = Size;
            //    //Вызов ядра
            //    commands.Execute(kernel, null, globalSize, null, null);
            //    //Ожидание окончания выполнения программы
            //    commands.Finish();
            //    // чтение искомой функции из буфера kernel-а
            //    commands.ReadFromBuffer(YY, ref y, true, null);
            //    //очищение памяти
            //    POINTERS.Dispose();
            //    COLS.Dispose();
            //    YY.Dispose();
            //    XX.Dispose();
            //    AA.Dispose();
            //    kernel.Dispose();
            //    //
            //    program.Dispose();
            //    commands.Dispose();
            //    context.Dispose();
            //}
            //catch (Exception ee)
            //{
            //    error += ee.Message.ToString();
            //}
            ////
            //double[] yd = new double[Size];
            //for (int i = 0; i < yd.Length; i++)
            //    yd[i] = y[i];
            ////
            //return yd;
        }

        // Вычисляем неполное LU разложение предобуславливателя
        void ILUDecomp(SSquare A, out SSquare LM, out SSquare UM)
        {
            LM = new SSquare();
            UM = new SSquare();
            //
            int FN = A.Size;
            double[][] Matrix = A.GetMatrix;
            double[][] L_Matrix = new double[FN][];
            double[][] U_Matrix = new double[FN][];
            for (int i = 0; i < FN; i++)
            {
                L_Matrix[i] = new double[FN];
                U_Matrix[i] = new double[FN];
            }
            //
            for (int k = 0; k < FN; k++)
            {
                for (int j = 0; j < k; j++)
                {
                    if (Math.Abs(Matrix[k][j]) > 0)
                    {
                        double SumLU = 0;
                        for (int i = 0; i < j; i++)
                            SumLU += L_Matrix[k][i] * U_Matrix[i][j];
                        L_Matrix[k][j] = (Matrix[k][j] - SumLU) / U_Matrix[j][j];
                    }
                    else
                        L_Matrix[k][j] = 0;
                }
                L_Matrix[k][k] = 1;
                for (int j = 0; j < FN; j++)
                {
                    if (Math.Abs(Matrix[k][j]) > 0)
                    {
                        double SumLU = 0;
                        for (int i = 0; i < k; i++)
                            SumLU += L_Matrix[k][i] * U_Matrix[i][j];
                        U_Matrix[k][j] = Matrix[k][j] - SumLU;
                    }
                    else
                        U_Matrix[k][j] = 0;
                }
            }
            //
            LM.SetSystem(FN);
            UM.SetSystem(FN);
            //
            LM.BuildMatrix(L_Matrix);
            UM.BuildMatrix(U_Matrix);
               
        }

        void ILUDecomp(SBand A, out SRowPacked LM, out SRowPacked UM)
        {
            LM = new SRowPacked();
            UM = new SRowPacked();
            //
            int FN = A.Size;
            int FH = A.FH;
            double[][] Matrix = A.GetMatrix;
            double[][] L_Matrix = new double[FN][];
            double[][] U_Matrix = new double[FN][];
            for (int i = 0; i < FN; i++)
            {
                L_Matrix[i] = new double[FN];
                U_Matrix[i] = new double[FN];
            }
            int FM = 0;
            int FHH = 0;
            //
            for (int k = 0; k < FN; k++)
            {
                int sdvig = 0;
                FHH = FH;
                //ограничение на хвост ленты
                if (FN - k < FH)
                    FHH = FN - k;
                //ограничение на начало ленты
                FM = FH - 1;
                if (k < FM)
                    FM = k;
                else
                    sdvig = k - FM;
                //
                int k2 = 0, j2 = 0, ch = 0;
                if (k < FM)
                    k2 = k;
                else
                {
                    k2 = FM;
                    j2 = k - FM;
                }
                //
                for (int j = 0; j < FM + FHH; j++)
                {
                    if (j+sdvig < k)
                    {
                        //слева от главной диагонали
                        if (sdvig + j <= FM)
                        {
                            if (Math.Abs(Matrix[j2][k2]) > 0)
                            {
                                double SumLU = 0;
                                for (int i = 0; i < j + sdvig; i++)
                                    SumLU += L_Matrix[k][i+sdvig] * U_Matrix[i+sdvig][sdvig + j];
                                L_Matrix[k][sdvig + j] = (Matrix[j2][k2] - SumLU) / U_Matrix[sdvig + j][sdvig + j];
                            }
                            else
                                L_Matrix[k][j+sdvig] = 0;
                            k2--; j2++; ch++;
                        }
                        else
                        {
                            if (Math.Abs(Matrix[k][j-ch]) > 0)
                            {
                                double SumLU = 0;
                                for (int i = 0; i < j-sdvig; i++)
                                    SumLU += L_Matrix[k][i+sdvig] * U_Matrix[i+sdvig][sdvig + j];
                                L_Matrix[k][sdvig + j] = (Matrix[k][j - ch] - SumLU) / U_Matrix[sdvig + j][sdvig + j];
                            }
                            else
                                L_Matrix[k][sdvig + j] = 0;
                        }
                    }
                }
                L_Matrix[k][k] = 1;
                //
                k2 = 0; j2 = 0; ch = 0;
                if (k < FM)
                    k2 = k;
                else
                {
                    k2 = FM;
                    j2 = k - FM;
                }
                //
                for (int j = 0; j < FM + FHH; j++)
                {
                    if (j < FM)
                    {
                        if (Math.Abs(Matrix[j2][k2]) > 0)
                        {
                            double SumLU = 0;
                            for (int i = 0; i + sdvig < k; i++)
                                SumLU += L_Matrix[k][i + sdvig] * U_Matrix[i + sdvig][j + sdvig];
                            U_Matrix[k][j + sdvig] = Matrix[j2][k2] - SumLU;
                        }
                        else
                            U_Matrix[k][j + sdvig] = 0;
                        k2--; j2++; ch++;
                    }
                    else
                    {
                        if (Math.Abs(Matrix[k][j-ch]) > 0)
                        {
                            double SumLU = 0;
                            for (int i = 0; i+sdvig < k; i++)
                                SumLU += L_Matrix[k][i+sdvig] * U_Matrix[i+sdvig][j+sdvig];
                            U_Matrix[k][j+sdvig] = Matrix[k][j-ch] - SumLU;
                        }
                        else
                            U_Matrix[k][j+sdvig] = 0;
                    }
                }
            }
            //
            LM.SetSystem(FN);
            UM.SetSystem(FN);
            //
            LM.BuildMatrix(L_Matrix);
            UM.BuildMatrix(U_Matrix);

        }
        //
        void ILUDecomp(float [] M, int[] Cols, int[] Pointers, out SRowPacked LM, out SRowPacked UM)
        {
            LM = new SRowPacked();
            UM = new SRowPacked();
            //
            int FN = Pointers.Length-1;
            double[][] L_Matrix = new double[FN][];
            double[][] U_Matrix = new double[FN][];
            for (int i = 0; i < FN; i++)
            {
                L_Matrix[i] = new double[FN];
                U_Matrix[i] = new double[FN];
            }
            //
            int p1 = 0, p2 = 0;
            for (int k = 0; k < FN; k++)
            {
                p1 = Pointers[k];
                p2 = Pointers[k + 1];
                //
                for (int j = p1; j < p2; j++) // for (int j = 0; j < k; j++)
                {
                    if (Cols[j] < k)
                    {
                        if (Math.Abs(M[j]) > 0)
                        {
                            double SumLU = 0;
                            for (int i = p1; i < j; i++) //  for (int i = 0; i < j; i++)
                                SumLU += L_Matrix[k][Cols[i]] * U_Matrix[Cols[i]][Cols[j]];
                            L_Matrix[k][Cols[j]] = (M[j] - SumLU) / U_Matrix[Cols[j]][Cols[j]];
                        }
                        else
                            L_Matrix[k][Cols[j]] = 0;
                    }
                    //
                }
                L_Matrix[k][k] = 1;
                //
                //
                for (int j = p1; j < p2; j++) // for (int j = 0; j < FN; j++)
                {
                    if (Math.Abs(M[j]) > 0)
                    {
                        double SumLU = 0;
                        for (int i = p1; i < p2; i++) // for (int i = 0; i < k; i++)
                        {
                            if (Cols[i]<k)
                                SumLU += L_Matrix[k][Cols[i]] * U_Matrix[Cols[i]][Cols[j]];
                        }
                        U_Matrix[k][Cols[j]] = M[j] - SumLU;
                    }
                    else
                        U_Matrix[k][Cols[j]] = 0;
                }
            }
            //
            LM.SetSystem(FN);
            UM.SetSystem(FN);
            //
            LM.BuildMatrix(L_Matrix);
            UM.BuildMatrix(U_Matrix);
        }

    }
}
