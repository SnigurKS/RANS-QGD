using MeshLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualizationLibrary
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
            double[] U = new double[50 * 30];
            double[] V = new double[50 * 30];
            double[] P = new double[50 * 30];
            for (int i = 0; i < 50; i++)
                for (int j = 0; j < 30; j++)
                {
                    U[i * 30 + j] = j;
                    V[i * 30 + j] = i;
                    P[i * 30 + j] = (j + i) / 2.0;
                }
            Area area = new Area(0, 2, 5, 0, 50, 1);
            AlgParameter AlgP = new AlgParameter(50, 30, 0, 1, 1);
            Parameter[] param = new Parameter[1];
            param[0] = AlgP;
            MeshBuilder mb = new MeshBuilder(area, param, false);
            mb.GenerateMesh();
            Mesh M = mb.FinalMesh;
            Application.Run(new MainForm(U, V, P, M));
        }
    }
}
