using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgebraLibrary
{
    [Serializable]
    public partial class AlgebraLibControl : UserControl
    {
        AlgLibManager AlgManager = new AlgLibManager();
        string[][] Names = null;
        //
        public int AlgorythmSelectedIndex
        {
            set { lstAlgorythm.SelectedIndex = value; }
            get { return lstAlgorythm.SelectedIndex; }
        }
        public int SystemSelectedIndex
        {
            set { lstSystemType.SelectedIndex = value; }
            get { return lstSystemType.SelectedIndex; }
        }
        public AlgebraLibControl()
        {
            InitializeComponent();
            Names = AlgManager.GetNamesChilds();
            lstAlgorythm.Items.AddRange(Names[0]);
            lstSystemType.Items.AddRange(Names[1]);
            //
            try
            {
                lstAlgorythm.SelectedIndex = 0;
                lstSystemType.SelectedIndex = 0;
                //
                lstAlgorythm.Height = (int)(this.Height * 0.38);
                lstAlgorythm.Width = this.Width - 5;
                //
                lstSystemType.Height = (int)(this.Height * 0.38);
                lstSystemType.Width = this.Width - 5;
                //
                Point newLocation = new Point(lstSystemType.Location.X, (int)(this.Height * 0.58));
                lstSystemType.Location = newLocation;
                lblSys.Location = new Point(newLocation.X, newLocation.Y - 20);
            }
            catch
            {
            }
        }
        //
        public Algorythm GetAlgorythmObject()
        {
            return AlgManager.CreateDetermChildAlg(lstAlgorythm.SelectedIndex);
        }
        //
        public SSystem GetSysyemObject()
        {
            return AlgManager.CreateDetermChildSys(lstSystemType.SelectedIndex);
        }
        //

        private void AlgebraLibControl_Resize(object sender, EventArgs e)
        {

            lstAlgorythm.Height = (int)(this.Height * 0.38);
            lstAlgorythm.Width = this.Width - 5;
            //
            lstSystemType.Height = (int)(this.Height * 0.38);
            lstSystemType.Width = this.Width - 5;
            //
            Point newLocation = new Point(lstSystemType.Location.X, (int)(this.Height * 0.58));
            lstSystemType.Location = newLocation;
            lblSys.Location = new Point(newLocation.X, newLocation.Y - 20);
        }

        private void AlgebraLibControl_Load(object sender, EventArgs e)
        {
            
        }
    }
}
