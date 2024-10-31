using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeshLibrary
{
    public class MeshLibManager
    {
        /// <summary>
        /// список потомков базового класса
        /// </summary>
        private Type[] ListGenerators;
        private Type[] ListParameters;
        /// <summary>
        /// конструктор и метод создания списка типов базового класса
        /// </summary>
        /// <param name="ChildType">Тип класса-потомка (использовать операцию typeof)</param>
        /// <param name="LibName">Название библиотеки(this.GetType().FullName взять первое до точки)</param>
        public MeshLibManager()
        {
            string NameSpace = Assembly.GetExecutingAssembly().GetName().Name;
            Type baseType = typeof(MeshGenerator);
            //
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(NameSpace);
            ListGenerators = Array.FindAll(assembly.GetTypes(),
                    delegate(Type type)
                    {
                        return (baseType != type) && baseType.IsAssignableFrom(type);
                    }
                );
            //
            baseType = typeof(Parameter);
            //
            assembly = System.Reflection.Assembly.Load(NameSpace);
            ListParameters = Array.FindAll(assembly.GetTypes(),
                    delegate(Type type)
                    {
                        return (baseType != type) && baseType.IsAssignableFrom(type);
                    }
                );
            ListGenerators = ListGenerators.OrderBy(s => s.Name).ToArray();
            ListParameters = ListParameters.OrderBy(s => s.Name).ToArray();
        }
        /// <summary>
        /// Получить список потомков
        /// </summary>
        /// <returns>список потомков</returns>
        public string[] GetNamesGenerators()
        {
            string[] Names = new string[ListGenerators.Length];
            for (int i = 0; i < ListGenerators.Length; i++)
            {
                PropertyInfo s = ListGenerators[i].GetProperty("Name");
                object obj = ListGenerators[i].GetConstructor(new Type[] { }).Invoke(new object[] { });
                Names[i] = (string)s.GetValue(obj);
                //Names[i] = ListGenerators[i].Name;
            }
            return Names;
        }
        /// <summary>
        /// Получить список потомков
        /// </summary>
        /// <returns>список потомков</returns>
        public string[] GetNamesParameters()
        {
            string[] Names = new string[ListParameters.Length];
            //
            for (int i = 0; i < ListParameters.Length; i++)
                Names[i] = ListParameters[i].Name;
            return Names;
        }
        /// <summary>
        /// получить экземпляр указанного класса
        /// </summary>
        /// <param name="Index">порядковый номер в списке потомков базового класса</param>
        /// <returns></returns>
        public MeshGenerator CreateDetermGenChild(int Index)
        {
            try
            {
                return (MeshGenerator)ListGenerators[Index].GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch 
            { 
                return null; 
            }
        }
        //
        public Parameter CreateDetermParamChild(int Index)
        {
            try
            {
                return (Parameter)ListParameters[Index].GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Метод возвращает Свойства полей наследника класса Parameter
        /// </summary>
        /// <param name="childParameter"> Объект Parameter</param>
        /// <param name="Names">Возвращаемый массив имен параметров</param>
        /// <param name="Types">Возвращаемый массив типов параметров</param>
        public static void Par(Parameter childParameter, out string[] Names, out Type[] Types)
        {
            PropertyInfo[] baseInfo = childParameter.GetType().BaseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Public);
            PropertyInfo[] childInfo = childParameter.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Public);
            Names = new string[baseInfo.Length + childInfo.Length];
            Types = new Type[Names.Length];
            int k = 0;
            for (int i = 0; i < baseInfo.Length; i++)
            {
                Names[k] = baseInfo[i].Name;
                Types[k++] = baseInfo[i].PropertyType;
            }
            //
            for (int i = 0; i < childInfo.Length; i++)
            {
                Names[k] = childInfo[i].Name;
                Types[k++] = childInfo[i].PropertyType;
            }
        }
    }
}
