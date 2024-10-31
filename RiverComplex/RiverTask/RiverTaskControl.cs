using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RiverTaskLibrary
{
    [Serializable]
    public partial class RiverTaskControl : UserControl
    {
        RiverTaskLibManager RiverManager = new RiverTaskLibManager();
        string[] Names = null;
        BedPhysicsParams Parameters = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public BedPhysicsParams ParameterObject
        {
            get { return Parameters; }
            set 
            { 
                Parameters = value;
                prpModelParameters.SelectedObject = Parameters;
            }
        }
        public int GetSetSelectedIndex
        {
            get { return lstModel.SelectedIndex; }
            set { lstModel.SelectedIndex = value; }
        }
        public RiverTaskControl()
        {
            InitializeComponent();
            Names = RiverManager.GetNamesTasks();
            lstModel.Items.AddRange(Names);
            //
            try
            {
                lstModel.SelectedIndex = 2;
                Parameters = RiverManager.CreateDetermParamChild(0);
                prpModelParameters.SelectedObject = Parameters;
                //
                lstModel.Height = (int)(this.Height * 0.2);
                lstModel.Width = this.Width - 5;
                //
                prpModelParameters.Height = (int)(this.Height * 0.7);
                prpModelParameters.Width = this.Width - 5;
                //
                Point newLocation = new Point(prpModelParameters.Location.X, (int)(this.Height * 0.3));
                prpModelParameters.Location = newLocation;
                lblParam.Location = new Point(newLocation.X, newLocation.Y - 15);
            }
            catch
            { }
        }

        public BaseBedLoadTask GetRiverTaskObject()
        {
            return RiverManager.CreateDetermGenChild(lstModel.SelectedIndex);
        }

        private void RiverTaskControl_Load(object sender, EventArgs e)
        {
           
        }

        private void RiverTaskControl_Resize(object sender, EventArgs e)
        {
            lstModel.Height = (int)(this.Height * 0.2);
            lstModel.Width = this.Width - 5;
            //
            prpModelParameters.Height = (int)(this.Height * 0.7);
            prpModelParameters.Width = this.Width - 5;
            //
            Point newLocation = new Point(prpModelParameters.Location.X, (int)(this.Height * 0.3));
            prpModelParameters.Location = newLocation;
            lblParam.Location = new Point(newLocation.X, newLocation.Y - 10);
            //
        }

        private void lstModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Parameters = RiverManager.CreateDetermParamChild(lstModel.SelectedIndex);
            prpModelParameters.SelectedObject = Parameters;
        }
    }
}
