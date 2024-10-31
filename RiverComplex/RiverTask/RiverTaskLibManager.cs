//-----------------------------------------------------------------------------------
//                Реализация шаблонного класса задачи TaskLibManager 
//          Класс предназначен для управления коллекциями задач и их параметов 
//-----------------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//-----------------------------------------------------------------------------------
//            Модуль BedLoadLibrary для расчета донных деформаций 
//                (учет движения только влекомых наносов)
//                              Потапов И.И.
//                              Снигур К. С.
//                        - (C) Copyright 2017 -
//                          ALL RIGHT RESERVED
//                               20.01.17
//-----------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
//-----------------------------------------------------------------------------------
namespace RiverTaskLibrary
{
    /// <summary>
    /// Класс для управления коллекцией задач порожденных от базового класса
    /// </summary>
    public class RiverTaskLibManager
    {
        public string ErrorMessage = "Ok";
        /// <summary>
        /// список потомков базового класса
        /// </summary>
        private Type[] ListModels;
        private Type[] ListParameters;
        string NameSpace = "BedLoadLibrary";
        /// <summary>
        /// Конструктор класс для управления коллекцией задач порожденных от базового класса
        /// </summary>
        /// <param name="NameSpace">статическая строка с имененм пространства имен в котором находится требуемая иерархия классов</param>
        public RiverTaskLibManager()
        {
            try
            {
                // получение имени пространстваS
                //Type sdd = typeof(TaskLibManager);
                //NameSpace = sdd.Namespace.ToString();
                NameSpace = Assembly.GetExecutingAssembly().GetName().Name;
                //
                System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(NameSpace);

                Type baseType = typeof(BaseBedLoadTask);

                ListModels = Array.FindAll
                    (
                        assembly.GetTypes(), delegate(Type type)
                        {
                            return (baseType != type) && baseType.IsAssignableFrom(type);
                        }
                    );
                //
                baseType = typeof(BedPhysicsParams);
                //
                assembly = System.Reflection.Assembly.Load(NameSpace);

                ListParameters = Array.FindAll
                    (
                        assembly.GetTypes(),
                        delegate(Type type)
                        {
                            return (baseType != type) && baseType.IsAssignableFrom(type);
                        }
                    );

                ListModels = ListModels.OrderBy(s => s.Name).ToArray();
                ListParameters = ListParameters.OrderBy(s => s.Name).ToArray();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
        /// <summary>
        /// Получить список потомков
        /// </summary>
        /// <returns>список потомков</returns>
        public string[] GetNamesTasks()
        {
            string[] Names = new string[ListModels.Length];
            //
            for (int i = 0; i < ListModels.Length; i++)
            {
                PropertyInfo pInfo = ListModels[i].GetProperty("Name");
                object obj = ListModels[i].GetConstructor(new Type[] { }).Invoke(new object[] { });
                Names[i] = Convert.ToString(pInfo.GetValue(obj, null));
                //Names[i] = ListModels[i].Name;
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
        public BaseBedLoadTask CreateDetermGenChild(int Index)
        {
            return (BaseBedLoadTask)ListModels[Index % ListModels.Length].GetConstructor(new Type[] { }).Invoke(new object[] { });
        }
        /// <summary>
        /// получить экземпляр указанного класса
        /// </summary>
        /// <param name="Index">порядковый номер в списке потомков базового класса</param>
        /// <returns></returns>
        public BedPhysicsParams CreateDetermParamChild(int Index)
        {
            return (BedPhysicsParams)ListParameters[Index % ListParameters.Length].GetConstructor(new Type[] { }).Invoke(new object[] { });
        }
    }
}
