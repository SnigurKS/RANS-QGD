using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
    [Serializable]
    public class AlgorythmGauss: Algorythm
    {
        public override string Name
        {
            get { return "Метод Гаусса"; }
        }
        new public double Discrepancy =  0.0000000000001;
        public override void Solve(SSquare A)
        {
            int Size = A.Size;
            double[][] Matrix = A.GetMatrix;
            double[] Right = A.GetRight;
            double[] X = new double[Size];
            try
            {
                // сообщение о проделанной  работе :)
                int i;
                int k, tmp;
                double c;
                for (i = 0; i < Size; i++)
                {
                    // поиск максимального индекса в столбце
                    c = Math.Abs(Matrix[i][i]); tmp = i;
                    for (int j = i + 1; j < Size; j++)
                    {
                        if (Math.Abs(Matrix[j][i]) > c)
                        {
                            c = Math.Abs(Matrix[j][i]); tmp = j;
                        }
                    }
                    // если найден коэф. больше чем на главной диагонали то
                    if (tmp > i)
                    {
                        for (k = 0; k < Size; k++)  // сохраняем tmp - ю строку
                            X[k] = Matrix[tmp][k];
                        for (k = 0; k < Size; k++)  // заменяем tmp - ю строку на i - ю
                            Matrix[tmp][k] = Matrix[i][k];
                        for (k = 0; k < Size; k++)  // заменяем tmp - ю строку на i - ю
                            Matrix[i][k] = X[k];
                        // изменяем правую часть
                        c = Right[tmp]; Right[tmp] = Right[i]; Right[i] = c;
                    }
                    // Балансировка ведомой САУ
                    Calibrate(i, c, Size, ref Matrix,ref Right);
                    // проводим прямой ход исключения по Гауссу
                    for (int j = i + 1; j < Size; j++)
                    {
                        if (Math.Abs(Matrix[i][i]) < Discrepancy)
                        {
                            error+="Матрица вырождена !";
                            return;
                        }
                        c = Matrix[j][i] / Matrix[i][i];
                        if (Math.Abs(c) > Discrepancy)
                        {
                            for (k = i; k < Size; k++)
                                Matrix[j][k] = Matrix[j][k] - c * Matrix[i][k];
                            Right[j] = Right[j] - c * Right[i];
                        }
                    }
                }
                // выполняем обратный ход
                for (i = Size - 1; i > -1; i--)
                {
                    //FSendRateMessage(Size-i);
                    c = Right[i];
                    for (int j = i + 1; j < Size; j++)
                        c = c - Matrix[i][j] * X[j];
                    X[i] = c / Matrix[i][i];
                }
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += "AlgorythmGauss" + e.Message.ToString();
            }
        }
        public override void Solve(SBand A)
        {
            double[][] Matrix = A.GetMatrix;
            double[] Right = A.GetRight;
            int Size = A.Size;
            int FH = A.FH;
            //
            double[] X = new double[Size];
            try
            {
                int Key = 0; // влаг для могократного обратного хода (не используется в данной версии)
                double c, ai;
                int i;
                int m, j, l, k;
                if (Key == 0)
                {
                    /* ------ прямой ход -------- */
                    for (i = 0; i < Size; i++)
                    {
                        ai = Matrix[i][0]; m = FH;
                        if (Size - i < FH)
                            m = Size - i;
                        for (j = 1; j < m; j++)
                        {
                            c = Matrix[i][j] / ai;
                            l = i + j;
                            for (k = 0; k < m - j; k++)
                            {
                                Matrix[l][k] -= c * Matrix[i][k + j];
                            }
                        }
                    }
                }
                // повторный прямой ход
                for (i = 0; i < Size; i++)
                {
                    ai = Matrix[i][0];
                    if (Size - i < FH)
                        m = Size - i;
                    else m = FH;
                    for (j = 1; j < m; j++)
                    {
                        c = Matrix[i][j] / ai;
                        l = i + j;
                        Right[l] -= c * Right[i];
                    }
                }
                // Обратный ход
                for (i = Size - 1; i >= 0; i--)
                {
                    ai = Matrix[i][0];
                    if (Size - i < FH)
                        m = Size - i;
                    else
                        m = FH;
                    c = 0;
                    for (j = 1; j < m; j++)
                        c += Matrix[i][j] * X[i + j];

                    X[i] = (Right[i] - c) / ai;
                }
                A.SetX(X);
            }
            catch (Exception e)
            {
                error = error + "AlgorythmGauss" + e.Message;
            }
        }
        public override void Solve(SRowPacked A)
        {
            int i, j, k, kp, Count;
            double a = 0, b = 0;
            try
            {
                CPackRow[] Matrix = A.GetMatrix;
                CPackRowBuff Buff = A.Buff;
                double[] Right = A.GetRight;
                int FN = A.Size;
                //
                double[] X = new double[FN];
                //  метод решения разряженной СЛУ с плотной
                //  упаковкой методом исключения Гаусса по строкам,
                // очистка буфера
                Buff.Free(true);
                //Прямой ход - полный выбор
                for (i = 0; i < FN; i++)
                {
                    // распаковка sl строки
                    Count = Matrix[i].Size();
                    // -------------------------
                    Matrix[i].UnPackRow(Buff);
                    // -------------------------
                    // обработка строк на исключение
                    for (j = Matrix[i].Index[0]; j < i; j++)
                    {
                        // если элемент уже равен нулю пропускаем
                        if (Math.Abs(Buff.Elem[j]) < CPackRow.DEPS)
                            continue;
                        // буфер для i
                        a = Buff.Elem[j] / Matrix[j].Elem[0];
                        // процесс исключения элемента из строки
                        for (k = 0; k < Matrix[j].Size(); k++)
                        {
                            kp = Matrix[j].Index[k];
                            // вычитаем из буфера текущую строку
                            b = Matrix[j].Elem[k];
                            Buff.Add(-a * b, kp);
                        }
                        // обработка правой части
                        Right[i] -= Right[j] * a;
                    }
                    // упаковка строки в случае ее изменения
                    if (Matrix[i].Index[0] < i)
                        Matrix[i].SetBufPackRow(Buff);
                    else
                        Buff.Free(true);
                }
                for (i = FN - 1; i > -1; i--)
                {
                    a = Right[i];
                    int numElem = Matrix[i].Size(); // количество ненулевых элементов в строке
                    for (j = numElem - 1; j > 0; j--)
                    {
                        int jp = Matrix[i].Index[j];
                        a -= Matrix[i].Elem[j] * X[jp];
                    }
                    X[i] = a / Matrix[i].Elem[0];
                }
                A.SetX(X);
            }
            catch (Exception ee)
            {
                error = "AlgorythmGauss for PackedSysytem";
            }
        }
        //
        int[] tmpi;
        double[] ci;
        bool[] flags;
        public override void Forwarding(SSquare A, int[] BoundAdresses)
        {
            int Size = A.Size;
            double[][] Matrix = A.GetMatrix;
            //
            tmpi = new int[Size];
            ci = new double[Size];
            flags = new bool[Size];
            //
            int i;
            int k, tmp;
            double c;
            for (i = 0; i < Size; i++)
            {
                if (!BoundAdresses.Any(item => item == i))
                {
                    // поиск максимального индекса в столбце
                    c = Math.Abs(Matrix[i][i]); tmp = i;
                    for (int j = i + 1; j < Size; j++)
                    {
                        if (Math.Abs(Matrix[j][i]) > c)
                        {
                            c = Math.Abs(Matrix[j][i]); tmp = j;
                        }
                    }
                    double[] m = new double[Size];
                    // если найден коэф. больше чем на главной диагонали то
                    if (tmp > i)
                    {
                        for (k = 0; k < Size; k++)  // сохраняем tmp - ю строку
                            m[k] = Matrix[tmp][k];
                        for (k = 0; k < Size; k++)  // заменяем tmp - ю строку на i - ю
                            Matrix[tmp][k] = Matrix[i][k];
                        for (k = 0; k < Size; k++)  // заменяем tmp - ю строку на i - ю
                            Matrix[i][k] = m[k];
                        // изменяем правую часть
                        //c = Right[tmp]; Right[tmp] = Right[i]; Right[i] = c;
                        flags[i] = true;
                    }
                    ci[i] = c;
                    tmpi[i] = tmp;
                }
            }
        }
        //
        public override void BackSolve(SSquare A)
        {
            try
            {
                int Size = A.Size;
                double[][] Matrix = A.GetMatrix;
                double[] Right = A.GetRight;
                //
                double[] X = new double[Size];
                //
                int i;
                int k;
                double c;
                for (i = 0; i < Size; i++)
                {
                    c = ci[i];
                    if (flags[i])
                    {
                        c = Right[tmpi[i]]; Right[tmpi[i]] = Right[i]; Right[i] = ci[i];
                    }
                    // проводим прямой ход исключения по Гауссу
                    for (int j = i + 1; j < Size; j++)
                    {
                        if (Math.Abs(Matrix[i][i]) < Discrepancy)
                        {
                            error += "Матрица вырождена !";
                            return;
                        }
                        c = Matrix[j][i] / Matrix[i][i];
                        if (Math.Abs(c) > Discrepancy)
                        {
                            for (k = i; k < Size; k++)
                                Matrix[j][k] = Matrix[j][k] - c * Matrix[i][k];
                            Right[j] = Right[j] - c * Right[i];
                        }
                    }
                }
                // выполняем обратный ход
                for (i = (int)Size - 1; i > -1; i--)
                {
                    c = Right[i];
                    for (int j = i + 1; j < Size; j++)
                        c = c - Matrix[i][j] * X[j];
                    X[i] = c / Matrix[i][i];
                }
                A.SetX(X);
            }
            catch (Exception e)
            {
                error += e.Message.ToString();
            }
        }
        //
        public override void Forwarding(SBand A, int[] BoundAdresses=null)
        {
            try
            {
                double[][] Matrix = A.GetMatrix;
                int Size = A.Size;
                int FH = A.FH;
                //
                double[] tempM;
                double c, ai;
                int i;
                int m, j, l, k;
                /* ------ прямой ход -------- */
                for (i = 0; i < Size; i++)
                {
                    tempM = Matrix[i];
                    ai = tempM[0]; m = FH;
                    if (Size - i < FH)
                        m = Size - i;
                    for (j = 1; j < m; j++)
                    {
                        c = tempM[j] / ai;
                        l = i + j;
                        for (k = 0; k < m - j; k++)
                        {
                            Matrix[l][k] -= c * tempM[k + j];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = error + "AlgorythmGauss.Forwarding" + ex.Message;
            }
        }
        public override void BackSolve(SBand A)
        {
            try
            {
                double[][] Matrix = A.GetMatrix;
                double[] Right = A.GetRight;
                int Size = A.Size;
                int FH = A.FH;
                //
                double[] X = new double[Size];
                //
                double c, ai, ri;
                int i;
                int m, j, l;
                // повторный прямой ход
                for (i = 0; i < Size; i++)
                {
                    ai = Matrix[i][0];
                    ri = Right[i];
                    if (Size - i < FH)
                        m = Size - i;
                    else 
                        m = FH;
                    for (j = 1; j < m; j++)
                    {
                        c = Matrix[i][j] / ai;
                        l = i + j;
                        Right[l] -= c * ri;
                    }
                }
                // Обратный ход
                for (i = Size - 1; i >= 0; i--)
                {
                    ai = Matrix[i][0];
                    ri = Right[i];
                    if (Size - i < FH)
                        m = Size - i;
                    else
                        m = FH;
                    c = 0;
                    for (j = 1; j < m; j++)
                        c += Matrix[i][j] * X[i + j];

                    X[i] = (ri - c) / ai;

                }
                A.SetX(X);
            }

            catch (Exception e)
            {
                error = error + "AlgorythmGauss.BackSolve" + e.Message;
            }
        }
        protected void Calibrate(int k, double C, int Size, ref double[][] Matrix, ref double[] Right)
        {
            double CC = 1 / C;
            for (int i = k; i < Size; i++)
            {
                for (int j = k; j < Size; j++) Matrix[i][j] *= CC;
                Right[i] *= CC;
            }
        }
    }
}
