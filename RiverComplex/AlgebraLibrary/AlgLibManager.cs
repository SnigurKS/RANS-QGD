using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AlgebraLibrary
{
    public class AlgLibManager
    {
        /// <summary>
        /// список потомков базового класса
        /// </summary>
        private Type[] ListAlg;
        private Type[] ListSystem;
        /// <summary>
        /// конструктор и метод создания списка типов базового класса
        /// </summary>
        /// <param name="ChildType">Тип класса-потомка (использовать операцию typeof)</param>
        /// <param name="LibName">Название библиотеки(this.GetType().FullName взять первое до точки)</param>
        public AlgLibManager()
        {
            string NameSpace = Assembly.GetExecutingAssembly().GetName().Name;
            Type baseType = typeof(SSystem);
            //
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(NameSpace);
            ListSystem = Array.FindAll(assembly.GetTypes(),
                    delegate(Type type)
                    {
                        return (baseType != type) && baseType.IsAssignableFrom(type);
                    }
                );
            //
            baseType = typeof(Algorythm);
            //
            assembly = System.Reflection.Assembly.Load(NameSpace);
            ListAlg = Array.FindAll(assembly.GetTypes(),
                    delegate(Type type)
                    {
                        return (baseType != type) && baseType.IsAssignableFrom(type);
                    }
                );
        }
        /// <summary>
        /// Получить список потомков: 0 - [Alrorythm]; 1 - [SSytem]
        /// </summary>
        /// <returns>список потомков: 0 - [Alrorythm]; 1 - [SSytem] </returns>
        public string[][] GetNamesChilds()
        {
            string[][] Names = new string[2][];
            Names[0] = new string[ListAlg.Length];
            Names[1] = new string[ListSystem.Length];
            //
            for (int i = 0; i < ListAlg.Length; i++)
            {
                PropertyInfo s = ListAlg[i].GetProperty("Name");
                object obj = ListAlg[i].GetConstructor(new Type[] { }).Invoke(new object[] { });
                Names[0][i] = (string)s.GetValue(obj);
                //Names[0][i] = ListAlg[i].Name;
            }
            //
            for (int i = 0; i < ListSystem.Length; i++)
            {
                PropertyInfo s = ListSystem[i].GetProperty("Name");
                object obj = ListSystem[i].GetConstructor(new Type[] { }).Invoke(new object[] { });
                Names[1][i] = (string)s.GetValue(obj);
                //Names[1][i] = ListSystem[i].Name;
            }
            return Names;
        }
        /// <summary>
        /// получить экземпляр указанного класса
        /// </summary>
        /// <param name="Index">порядковый номер в списке потомков базового класса</param>
        /// <returns></returns>
        public SSystem CreateDetermChildSys(int Index)
        {
            try
            {
                return (SSystem)ListSystem[Index].GetConstructor(new Type[] { }).Invoke(new object[] { }); 
            }
            catch 
            { 
                return null; 
            }
        }
        //
        public Algorythm CreateDetermChildAlg(int Index)
        {
            try
            {
                return (Algorythm)ListAlg[Index].GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return null;
            }
        }
    }
}
