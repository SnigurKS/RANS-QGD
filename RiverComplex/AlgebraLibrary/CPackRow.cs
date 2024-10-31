//---------------------------------------------------------------------------
//   Реализация класса упаковки для строки САУ необходимого 
//        при решаюшении задач алгебраических уравнений
//                              Потапов И.И.
//                        - (C) Copyright 2016 -
//                          ALL RIGHT RESERVED
//                               1.11.16
//---------------------------------------------------------------------------
//                  Источник ПРОЕКТ ___MixTasker 28 06 06
//                              Потапов И.И.
//                        - (C) Copyright 2003
//                          ALL RIGHT RESERVED
//             Адаптирован на С++ 20003 проект MixTasker
//             Адаптирован на С#  2016 г. русловые расчеты
//---------------------------------------------------------------------------
//   Реализация библиотеки методов решения систем алгебраических уравнений
//---------------------------------------------------------------------------
using System;

namespace AlgebraLibrary
{
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //const int Reserv = 16;
    //---------------------------------------------------------------------
    // Класс упакованная строка
    [Serializable]
    public class CPackRow
    {
        static public double DEPS = 1e-6;
        public int MaxCount = 0;
        public int Count = 0;
        /// <summary>
        /// элементы упакованной строки
        /// </summary>
        public double[] Elem = null;
        /// <summary>
        /// индексы элементов в строке
        /// </summary>
        public int[] Index = null;
        //
        public CPackRow() { }
        //
        public CPackRow(int N)
        {
            SetPackRow(N);
        }
        public CPackRow(CPackRow e)
        {
            SetPackRow(e);
        }
        public CPackRow(CPackRowBuff Buf, bool Sort = false)
        {
            SetBufPackRow(Buf, Sort);
        }
        // количество элементов
        public int Size() { return Count; }
        // Очистка строки
        public void Clear()
        {
            Elem = null;
            Index = null;
            Count = 0;
            MaxCount = 0;
        }
        // Распределение памяти под строку
        public void SetPackRow(int N)
        {
            if (N > MaxCount)
            {
                Count = N;
                MaxCount = N;
                Elem = new double[MaxCount];
                Index = new int[MaxCount];
            }
            else
            {
                Free();
                Count = N;
            }
        }
        // Распределение памяти под строку
        public void SetPackRowFree(int N)
        {
            SetPackRow(N);
            Free();
            //memset( Elem,0,sizeof(double)*Count );
            //memset( Index,0,sizeof(int)*Count );
        }
        // установка
        public void SetPackRow(CPackRow e)
        {
            SetPackRow(e.Count);
            for (int i = 0; i < Count; i++)
            {
                Elem[i] = e.Elem[i];
                Index[i] = e.Index[i];
            }
            //memcpy( Elem,e->Elem, sizeof(double)*Count );
            //memcpy( Index,e->Index, sizeof(int)*Count );
        }
        // оператор копирования
        //public CPackRow  operator = (CPackRow e)
        //{
        //   SetPackRow( e );
        //   return *this;
        //}
        /// 
        //public CPackRow& __fastcall operator <= ( CPackRow& e)
        public CPackRow ExChange(CPackRow e)
        {
            double[] tmpElem = Elem;
            int[] tmpIndex = Index;
            int tmpFCount = Count;
            int tmpMaxCount = MaxCount;

            Elem = e.Elem;
            Index = e.Index;
            Count = e.Count;
            MaxCount = e.MaxCount;

            e.Elem = tmpElem;
            e.Index = tmpIndex;
            e.Count = tmpFCount;
            e.MaxCount = tmpMaxCount;
            return this;
        }
        // получение элемента
        public double this[int idx]
        {
            get
            {
                if (idx < Index[0] || idx > Index[Count - 1])
                    return 0;
                for (int i = 0; i < Count; i++)
                    if (Index[i] == idx)
                        return Elem[i];
                return 0;
            }
        }
        // значение максимального элемента в строке
        public double MaxElem()
        {
            int k = 0;
            for (int i = 1; i < Count; i++)
                if (Math.Abs(Elem[k]) < Math.Abs(Elem[i])) k = i;
            return Elem[k];
        }
        /// <summary>
        /// индекс и значение максимального элемента в строке элемента
        /// </summary>
        /// <param name="IdxMax"></param>
        /// <param name="IdxReal"></param>
        /// <param name="ElemMax"></param>
        public void GetMaxElem(ref int IdxMax, ref int IdxReal, ref double ElemMax)
        {
            int k = 0;
            for (int i = 1; i < Count; i++)
                if (Math.Abs(Elem[k]) < Math.Abs(Elem[i])) k = i;
            ElemMax = Elem[k];
            IdxReal = (int)Index[k];
            IdxMax = (int)k;
        }
        public void DiagOne(int ad)
        {
            SetPackRow(1);
            Index[0] = ad;
            Elem[0] = 1;
            // Count       =  1;
        }
        // формирование строки по вектору
        public void CreateVector(double[] V, int[] Adress)
        {
            SetPackRow(Adress.Length);
            // memcpy( Elem, V, sizeof(double)*Count );
            //memcpy( Index,Adress, sizeof(int)*Count );
            // заплата
            for (int i = 0; i < Adress.Length; i++)
            {
                Elem[i] = V[i];
                Index[i] = Adress[i];
            }
        }

        int Reserv = 1000;
        /// <summary>
        /// установкf элемента строку
        /// </summary>
        /// <param name="e"></param>
        /// <param name="Idx"></param>
        public void PushBack(double e, int Idx)
        {
            // исключение нулевых элементов
            if (Math.Abs(e) > DEPS)
            {
                if (Count > 0)
                {
                    if (Count == MaxCount)
                    {
                        double[] tmpElem = Elem;
                        int[] tmpIndex = Index;
                        int tmpMaxCount = MaxCount;
                        MaxCount += Reserv;
                        Elem = new double[MaxCount];
                        Index = new int[MaxCount];
                        for (int i = 0; i < Count; i++)
                        {
                            Elem[i] = tmpElem[i];
                            Index[i] = tmpIndex[i];
                        }
                    }
                }
                else
                {
                    SetPackRow(Reserv);
                    Count = 0;
                }
                Index[Count] = Idx;
                Elem[Count] = e;
                Count++;
            }
        }
        /// <summary>
        /// установка элементов строки
        /// </summary>
        /// <param name="Buf"></param>
        /// <param name="Sort"></param>
        public void SetBufPackRow(CPackRowBuff Buf, bool Sort = false)
        {
            SetPackRow(Buf.Count);
            if (Sort == true || Buf.Sort == true)
            {
                Count = 0;
                int BCount = Buf.Size();
                for (int i = 0; i < BCount; i++)
                {
                    if (Buf.Check[i] == true)
                    {
                        // исключение нулевых элементов
                        if (Math.Abs(Buf.Elem[i]) > DEPS)
                        {
                            Index[Count] = i;
                            Elem[Count] = Buf.Elem[i];
                            Count++;
                        }
                        Buf.Elem[i] = 0;
                        Buf.Check[i] = false;
                    }
                }
            }
            else
            {
                Count = 0;
                for (int i = 0; i < Buf.Count; i++)
                {
                    int id = Buf.Index[i];
                    Buf.Check[id] = false;
                    // исключение нулевых элементов
                    if (Math.Abs(Buf.Elem[id]) < DEPS)
                    {
                        Buf.Elem[id] = 0;
                        continue;
                    }
                    Index[Count] = id;
                    Elem[Count] = Buf.Elem[id];
                    Buf.Elem[id] = 0;
                    Buf.Check[id] = false;
                    Count++;
                }
                Buf.Count = 0;
            }
        }
        /// <summary>
        /// умножение строки на число
        /// </summary>
        /// <param name="a"></param>
        public void Mult(double a)
        {
            for (int i = 0; i < Count; i++) Elem[i] *= a;
        }
        /// <summary>
        /// скалярное умножение двух упакованных сортированных строк
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        //public double operator *(CPackRow e)
        public double Multiplay(CPackRow e)
        {
            double sum = 0;
            int j = 0;
            if (e.Count == 0) return sum;
            for (int i = 0; i < Count; i++)
            {
                for (; j < e.Count; )
                {
                    if (Index[i] == e.Index[j])
                    {
                        sum += Elem[i] * e.Elem[j];
                        j++;
                        break;
                    }
                    if (Index[i] < e.Index[j])
                        break;
                    if (++j == e.Count) return sum;
                }
            }
            return sum;
        }
        /// <summary>
        /// Удаление элемента по индексу
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public double ClearElem(int idx)
        {
            double El = 0;
            if (idx < Count)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (idx == Index[i])
                    {
                        El = Elem[i];
                        Count--;
                        for (int k = i; k < Count; k++)
                        {
                            Elem[k] = Elem[k + 1];
                            Index[k] = Index[k + 1];
                        }
                        Elem[Count] = 0;
                        Index[Count] = 0;
                        break;
                    }
                }
            }
            return El;
        }
        /// <summary>
        /// Распаковка строки в буффер
        /// </summary>
        /// <param name="Buf"></param>
        /// <returns></returns>
        public int UnPackRow(CPackRowBuff Buf)
        {
            // количество элементов в строке
            for (int id = 0; id < Count; id++)
            {
                Buf.Elem[Index[id]] = Elem[id];
                Buf.Check[Index[id]] = true;
                Buf.Index[id] = Index[id];
            }
            Buf.Count = Count;
            return Count;
        }
        /// <summary>
        /// умножение строки на расширенный вектор
        /// </summary>
        /// <param name="Vec"></param>
        /// <returns></returns>
        public double MultVec(double[] Vec)
        {
            double sum = 0;
            for (int i = 0; i < Count; i++)
                sum += Elem[i] * Vec[Index[i]];
            return sum;
        }
        /// <summary>
        /// Печать строки
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="Format"></param>

        public void FullPrint(int Size, int Format = 7)
        {
            int idx = 0;
            string SFormat = " {0:F" + Format.ToString() + "} ";
            Console.WriteLine("Size = {0}", Size);
            Console.WriteLine("Elem:");
            for (int i = 0; i < Size; i++)
            {
                if (Count > 0 && idx < Count && i == Index[idx])
                    Console.Write(SFormat, Elem[idx++]);
                else
                    Console.Write("0  ");
            }
        }
        /// <summary>
        /// прямая сортировка
        /// </summary>
        public void Sorting()
        {
            double E;
            int m, ID;
            if (Count == 1) return;
            for (int i = 0; i < Count; i++)
            {
                m = Index[i];
                // находим минимальный адрес
                for (int j = i + 1; j < Count; j++)
                    if (m > Index[j])
                        m = Index[j];
                if (m == i) continue;
                // перестановка
                E = Elem[i];
                ID = Index[i];
                Elem[i] = Elem[m];
                Index[i] = Index[m];
                Elem[m] = E;
                Index[m] = ID;
            }
        }
        public void Free()
        {
            for (int i = 0; i < Count; i++)
            {
                Elem[i] = 0;
                Index[i] = 0;
            }
        }
    }
}
