using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using MeshLibrary;
using AlgebraLibrary;
using RiverTaskLibrary;

namespace RiverComplex
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //// Тест Open CL кода методов Algebra
            //Random rnd = new Random(0);
            //double[][] M = new double[100][];
            //double[] V = new double[100];
            //for (int i = 0; i < 100; i++)
            //{
            //    M[i] = new double[100];
            //    for (int j = 0; j < 100; j++)
            //    {
            //        M[i][j] = rnd.NextDouble();
            //        if (i == j)
            //            M[i][j] = 2.0;
            //    }
            //    V[i] = rnd.Next(1, 10);
            //}
            ////
            //SRowPacked system2 = new SRowPacked();
            //system2.SetSystem(100);
            //system2.BuildRight(V);
            //system2.BuildMatrix(M);
            //AlgorythmGMRES Alg2 = new AlgorythmGMRES();
            //// - если закомментирвать 2 строки, то ошибка с памятью и указателями, массив H и w исчезают при iter=1, i=0, j=64
            //system2.Accept(Alg2);
            //double[] x = system2.GetX;
            ////
            //Alg2.OpenCL = true;
            //system2.Accept(Alg2);
            //double[] x1 = system2.GetX;
            ////
            //SSquare system = new SSquare();
            //system.SetSystem(100);
            //system.BuildRight(V);
            //system.BuildMatrix(M);
            //AlgorythmGauss Alg = new AlgorythmGauss();
            //system.Accept(Alg);
            //double[] x2 = system.GetX;

            //
            //
            //--Тестирование OpenCL кода MatrixToVector
            //double[][] m = new double[4][];
            //for (int i = 0; i < 4; i++)
            //    m[i] = new double[4];
            //
            //m[0][0] = 1.5; m[1][0] = 0.7; m[2][0] = 0.6; m[3][0] = 1.0;
            //m[0][1] = 0.5; m[1][1] = 1.5; m[2][1] = 0.6; m[3][1] = 1.0;
            //m[0][2] = 0.5; m[1][2] = 0.7; m[2][2] = 1.5; m[3][2] = 1.0;
            //m[0][3] = 0.5; m[1][3] = 0.7; m[2][3] = 0.6; m[3][3] = 1.5;
            //X -0.6832; 0.8044; -0.0220; 3.26722
            //m[0][0] = 2; m[1][0] = 1; m[2][0] = 1; m[3][0] = 1;
            //m[0][1] = 1; m[1][1] = 2; m[2][1] = 1; m[3][1] = 1;
            //m[0][2] = 1; m[1][2] = 1; m[2][2] = 2; m[3][2] = 1;
            //m[0][3] = 1; m[1][3] = 1; m[2][3] = 1; m[3][3] = 2;
            ////
            //m[0][0] = 10; m[1][0] = 2; m[2][0] = 3; m[3][0] = 0;
            //m[0][1] = 2; m[1][1] = 10; m[2][1] = 5; m[3][1] = 6;
            //m[0][2] = 3; m[1][2] = 5; m[2][2] = 10; m[3][2] = 7;
            //m[0][3] = 0; m[1][3] = 6; m[2][3] = 7; m[3][3] = 10;
            //
            // для Band
            //double[][] m = new double[4][];
            //m[0] = new double[3];
            //m[1] = new double[3];
            //m[2] = new double[2];
            //m[3] = new double[1];
            ////
            //m[0][0] = 2;
            //m[0][1] = 1; m[1][0] = 2;
            //m[0][2] = 1; m[1][1] = 1; m[2][0] = 2;
            //             m[1][2] = 1; m[2][1] = 1; m[3][0] = 2;
            //
            //double[] vector = new double[4];
            //vector[0] = 0; vector[1] = 3.0; vector[2] = 2.0; vector[3] = 5.0;
            ////// X = -2; 1; 0; 3
            ////vector[0] = 1.0; vector[1] = 3.0; vector[2] = 2.0; vector[3] = 5.0;
            ////
            ////// X = -0.68320; 0.80441; -0.02204; // 3.26722
            ////SRowPacked system = new SRowPacked();
            //SSquare system = new SSquare();
            ////SBand system = new SBand();
            ////system.SetSystem(4, 3);
            //system.SetSystem(4);
            //system.BuildRight(vector);
            //system.BuildMatrix(m);
            
            
            //AlgorythmGMRES Alg = new AlgorythmGMRES(10);
            //double[] x = new double[4];
            //system.Accept(Alg);
            //Alg.GMResMethod(system);
            
            //al.SetSystem(vector.Length);
            //al.BuildMatrix(m);
            //al.BuildRight(vector);
            //al.Solve();
            //
            //double[] Xb = new double[10];
            //double[] Yb = new double[10];
            //double[] Xt = new double[10];
            //double[] Yt = new double[10];
            //for (int i = 0; i < 10; i++)
            //{
            //    Xb[i] = i * 0.5;
            //    Xt[i] = i * 0.5;
            //    //
            //    Yb[i] = 0;
            //    Yt[i] = 2;
            //}
            ////
            //Area ar = new Area(Xt, Yt, Xb, Yb, 0.1, 0, 2);
            //int[] Method = new int[2];
            //Method[0] = 2;
            //int[] Structure = new int[1];
            //MeshBuilder mb = new MeshBuilder(ar,Method);
            //Parameter[] Prs = new Parameter[2];
            ////Prs[0] = new DiffParameter(5, 10, 0, true);
            //Prs[0] = new OrthoParameter(5, 10, 0, true, 0.9,0.9);
            //Prs[1] = new AlgParameter(5, 10, 0, true);
            //mb.GenerateMesh(Prs);
            //Mesh m = mb.FinalMesh;
            //WElizParameter p = new WElizParameter(800, 0.9,0.9, 10, 0.001, 0.0001, 0.1, 0.9, 0.05, 0.0001, false);
            //SRowPacked Alg = new SRowPacked();
            //WaterTaskEliz wte = new WaterTaskEliz(p, m, Alg);

        }
           
    }
}





public class FieldInfoClass
{
    public AlgParameter a;
    public int myField1 = 0;
    protected string myField2 = null;
    public static void Main2()
    {

        FieldInfo[] myFieldInfo;
        Type myType = typeof(FieldInfoClass);
        // Get the type and fields of FieldInfoClass.
        myFieldInfo = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Public);
        Console.WriteLine("\nThe fields of " +
            "FieldInfoClass are \n");
        // Display the field information of FieldInfoClass.
        //myFieldInfo[0].Attributes
        for (int i = 0; i < myFieldInfo.Length; i++)
        {
            Console.WriteLine("\nName            : {0}", myFieldInfo[i].Name);
            Console.WriteLine("Declaring Type  : {0}", myFieldInfo[i].DeclaringType);
            Console.WriteLine("IsPublic        : {0}", myFieldInfo[i].IsPublic);
            Console.WriteLine("MemberType      : {0}", myFieldInfo[i].MemberType);
            Console.WriteLine("FieldType       : {0}", myFieldInfo[i].FieldType);
            Console.WriteLine("IsFamily        : {0}", myFieldInfo[i].IsFamily);
            Console.ReadKey();
        }
        //
    }
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