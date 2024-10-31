using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydrodynamicLibrary
{
    static class Program
    {
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TestForm t = new TestForm();
            Application.Run(t);
        }
    }
}
