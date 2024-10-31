//---------------------------------------------------------------------------
//   Реализация класса буффера для строковой упаковки необходимого 
//           при решаюшении задач алгебраических уравнений
//                              Потапов И.И.
//                        - (C) Copyright 2016 -
//                          ALL RIGHT RESERVED
//                               31.10.16
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
    [Serializable]
    /// <summary>
    /// класс буфферная строка
    /// </summary>
    public class CPackRowBuff
    {
        public string ErrorMessage ="";
        /// <summary>
        /// длина буфера
        /// </summary>
        public int     FN; 
        /// <summary>
        /// значения
        /// </summary>
        public double[]   Elem; 
        /// <summary>
        /// существование
        /// </summary>
        public bool[]     Check;
        /// <summary>
        /// адреса элементов в строке
        /// </summary>
        public int[]     Index;
        /// <summary>
        /// количество элементов в строке
        /// </summary>
        public int      Count; 
        /// <summary>
        /// флаг состояния буфера
        /// </summary>
        public bool       Sort;  
        // создание буффера заданного размера
        public CPackRowBuff(int N=0)
        {
            SetBuff(N);
        }
        public void SetBuff(int N)
        {
            Clear();
            FN = N;
            Elem  = new double[FN];
            Index = new int[FN];
            Check = new bool[FN];
            Free(true);
        }
        public void Clear()
        {
            Elem = null;
            Index = null;
            Check = null;
            FN    = 0;
            Count = 0;
            Sort  = false;
        }
        public void Set(double a,int idx)
        {
            Elem[ idx ] = a;
            Index[ Count++ ] = idx;
            Check[ idx ] = true;
        }
        public void Add(double a,int idx)
        {
            try
            {
                Elem[ idx ] += a;
                if( Check[ idx ] == false )
                {
                    Index[ Count++ ]  = idx;
                    Check[ idx ]      = true;
                    Sort = true;
                }
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message + "idx= "+idx.ToString()+" Count= "+Count.ToString() +
                 " FN= "+FN.ToString()+" a= "+a.ToString() + " Elem= "+ Elem[ idx ].ToString();
                Console.WriteLine(ErrorMessage);
            }
        }
        public void SetDiagOne(int idx)
        {
            Elem[ idx ] = 1;
            Index[ Count++ ] = idx;
            Check[ idx ] = true;
        }
        public int Size() { return FN; }
        /// <summary>
        /// Очистка буфера
        /// </summary>
        /// <param name="Full">Флаг означающий полное стирания (true) или стирание по индексам (false)</param>
        public void Free(bool Full=false)
        {
            if( Full )
            {
                for(int i=0; i<FN; i++) 
                { 
                    Index[i] = 0;
                    Elem[i] = 0;
                    Check[i] = false; 
                }
            }
            else
            {
                for(int i=0; i<Count; i++)
                {
                    Elem[ Index[i] ] = 0;
                    Check[ Index[i] ] = false;
                    Index[i] = 0;
                }
            }
            Count  = 0;
            Sort = false;
        }
        public void FullPrint(int Format=7)
        {
            string SFormat = " {0:F"+Format.ToString() +"} ";
            Console.WriteLine("Count = {0}", Count);
            Console.WriteLine("Size = {0}", FN);
            Console.WriteLine("Elem:");
            for(int i=0; i<FN; i++)
                Console.Write(SFormat, Elem[ i ]);
            Console.WriteLine("Index");
            for(int i=0; i<FN; i++)
                Console.Write(" {0} ", Index[ i ]);
            Console.WriteLine("Check");
            for(int i=0; i<FN; i++)
               Console.Write(" {0} ", Check[ i ]);
       }
    }
}
