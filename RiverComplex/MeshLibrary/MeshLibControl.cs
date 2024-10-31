using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshLibrary
{
    [Serializable]
    public partial class MeshLibControl : UserControl
    {
        //выбранная область: 0 - Top, 1 - Middle, 2 - Bottom
        double BottomLayer = 0;
        double TopLayer = 0;
        int parts = 1;
        //
        public double GetSetBottomLayer
        {
            set
            {
                BottomLayer = value;
                txtBottomLayer.Text = BottomLayer.ToString();
            }
            get { return BottomLayer; }
        }
        public double GetSetTopLayer
        {
            set 
            { 
                TopLayer = value;
                txtTopLayer.Text = TopLayer.ToString();
            }
            get { return TopLayer; }
        }
        public int GetSetParts
        {
            set 
            {
                parts = value;
                nudParts.Value = parts;
            }
            get { return parts; }
        }
        int SelectedArea = 1;
        int[] SavedIndexes = { -1, 0, -1 };
        public int[] GetSetSelectedIndexes
        {
            get 
            {
                return SavedIndexes;
            }
            set
            {
                SavedIndexes = value;
                lstMesh.SelectedIndex = SavedIndexes[SelectedArea];
            }
        }
        //------!
        public int GetSelectedIndex
        {
            get { return lstMesh.SelectedIndex; }
        }
        MeshLibManager MeshManager = new MeshLibManager();
        string[] Names = null;
        Parameter[] TMBParams = new Parameter[3];
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
         public Parameter[] ParameterObject
        {
            get {
                List<Parameter> lp = new List<Parameter>();
                for (int i = 0; i < 3; i++)
                {
                    if (TMBParams[i] != null)
                    {
                        TMBParams[i].Nx /= parts;
                        lp.Add(TMBParams[i]);
                    }
                }
                return lp.ToArray();   
            }
            set
            {
                int ch = value.Length;
                int ch0 = 0;
                TMBParams = new Parameter[3];
                for (int i = 0; i < 3; i++)
                {
                    if (SavedIndexes[i] != -1)
                    {
                        if(value.Length==3)
                            TMBParams[i] = value[i];
                        else
                            TMBParams[i] = value[ch0];
                        ch0++;
                        if (ch == ch0)
                            break;
                        
                    }
                }
                //prpMeshParam.SelectedObject = TMBParams[0];
                //TMBParams = value;     
                prpMeshParam.SelectedObject = TMBParams[SelectedArea];
            }
        }
        public MeshLibControl()
        {
            InitializeComponent();
            Names = MeshManager.GetNamesGenerators();
            lstMesh.Items.AddRange(Names);
            //
            try
            {
                lstMesh.SelectedIndex = 0;
                TMBParams[SelectedArea] = MeshManager.CreateDetermParamChild(0);
                prpMeshParam.SelectedObject = TMBParams[SelectedArea];
                //
                lstMesh.Height = (int)(this.Height * 0.13);
                lstMesh.Width = this.Width - 5;
                //
                prpMeshParam.Height = (int)(this.Height * 0.6);
                prpMeshParam.Width = this.Width - 5;
                //
                btnBottom.Visible = false;
                btnTop.Visible = false;
            }
            catch
            {
            }
        }

        public MeshGenerator GetMeshObject()
        {
            return MeshManager.CreateDetermGenChild(lstMesh.SelectedIndex);
        }

        private void MeshLibControl_Load(object sender, EventArgs e)
        {

        }

        private void MeshLibControl_Resize(object sender, EventArgs e)
        {
            lstMesh.Height = (int)(this.Height * 0.13);
            lstMesh.Width = this.Width - 5;
            //
            prpMeshParam.Height = (int)(this.Height *0.6);
            prpMeshParam.Width = this.Width - 5;
            //
            Point newLocation = new Point(prpMeshParam.Location.X, (int)(this.Height * 0.4));
            prpMeshParam.Location = newLocation;
            lblParam.Location = new Point(newLocation.X, newLocation.Y - 10);
            //
        }

        private void lstMesh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMesh.SelectedIndex != SavedIndexes[SelectedArea])
            {
                SavedIndexes[SelectedArea] = lstMesh.SelectedIndex;
                TMBParams[SelectedArea] = MeshManager.CreateDetermParamChild(lstMesh.SelectedIndex);
                TMBParams[SelectedArea].Method = lstMesh.SelectedIndex;    
            }
            prpMeshParam.SelectedObject = TMBParams[SelectedArea];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TopLayer = Convert.ToDouble(txtTopLayer.Text);
                if (TopLayer != 0)
                {
                    btnTop.Visible = true;
                    SavedIndexes[0] = lstMesh.SelectedIndex;
                    TMBParams[0] = MeshManager.CreateDetermParamChild(lstMesh.SelectedIndex);
                    TMBParams[SelectedArea].Method = lstMesh.SelectedIndex;
                }
                else
                {
                    TMBParams[0] = null;
                    btnTop.Visible = false;
                }
            }
            catch
            { }
        }


        private void txtBottomLayer_TextChanged(object sender, EventArgs e)
        {
            try
            {
                BottomLayer = Convert.ToDouble(txtBottomLayer.Text);
                if (BottomLayer != 0)
                {
                    btnBottom.Visible = true;
                    SavedIndexes[2] = lstMesh.SelectedIndex;
                    TMBParams[2] = MeshManager.CreateDetermParamChild(lstMesh.SelectedIndex);
                    TMBParams[SelectedArea].Method = lstMesh.SelectedIndex;
                }
                else
                {
                    TMBParams[2] = null;
                    btnBottom.Visible = false;
                }
            }
            catch { }
        }

        private void btnBottom_CheckedChanged(object sender, EventArgs e)
        {
            if (btnBottom.Checked)
            {
                SelectedArea = 2;
                //TMBParams[SelectedArea] = MeshManager.CreateDetermParamChild(lstMesh.SelectedIndex);
                //TMBParams[SelectedArea].Method = lstMesh.SelectedIndex;
                lstMesh.SelectedIndex = SavedIndexes[2];
                prpMeshParam.SelectedObject = TMBParams[SelectedArea];
            }
        }

        private void btnMiddle_CheckedChanged(object sender, EventArgs e)
        {
            if (btnMiddle.Checked)
            {
                SelectedArea = 1;
                //TMBParams[SelectedArea] = MeshManager.CreateDetermParamChild(lstMesh.SelectedIndex);
                //TMBParams[SelectedArea].Method = lstMesh.SelectedIndex;
                lstMesh.SelectedIndex = SavedIndexes[1];
                prpMeshParam.SelectedObject = TMBParams[SelectedArea];
            }
        }

        private void btnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (btnTop.Checked)
            {
                SelectedArea = 0;
                //TMBParams[SelectedArea] = MeshManager.CreateDetermParamChild(lstMesh.SelectedIndex);
                //TMBParams[SelectedArea].Method = lstMesh.SelectedIndex;
                lstMesh.SelectedIndex = SavedIndexes[0];
                prpMeshParam.SelectedObject = TMBParams[SelectedArea];
            }
        }

        private void nudParts_ValueChanged(object sender, EventArgs e)
        {
            parts = (int)nudParts.Value;
        }
    }
}
