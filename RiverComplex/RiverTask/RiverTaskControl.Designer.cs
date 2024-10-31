namespace RiverTaskLibrary
{
    partial class RiverTaskControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstModel = new System.Windows.Forms.ListBox();
            this.prpModelParameters = new System.Windows.Forms.PropertyGrid();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblParam = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstModel
            // 
            this.lstModel.FormattingEnabled = true;
            this.lstModel.ItemHeight = 16;
            this.lstModel.Location = new System.Drawing.Point(4, 36);
            this.lstModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstModel.Name = "lstModel";
            this.lstModel.Size = new System.Drawing.Size(331, 84);
            this.lstModel.TabIndex = 0;
            this.lstModel.SelectedIndexChanged += new System.EventHandler(this.lstModel_SelectedIndexChanged);
            // 
            // prpModelParameters
            // 
            this.prpModelParameters.Location = new System.Drawing.Point(4, 162);
            this.prpModelParameters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.prpModelParameters.Name = "prpModelParameters";
            this.prpModelParameters.Size = new System.Drawing.Size(332, 386);
            this.prpModelParameters.TabIndex = 1;
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(5, 5);
            this.lblModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(46, 17);
            this.lblModel.TabIndex = 2;
            this.lblModel.Text = "Model";
            // 
            // lblParam
            // 
            this.lblParam.AutoSize = true;
            this.lblParam.Location = new System.Drawing.Point(4, 141);
            this.lblParam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblParam.Name = "lblParam";
            this.lblParam.Size = new System.Drawing.Size(81, 17);
            this.lblParam.TabIndex = 3;
            this.lblParam.Text = "Parameters";
            // 
            // RiverTaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblParam);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.prpModelParameters);
            this.Controls.Add(this.lstModel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "RiverTaskControl";
            this.Size = new System.Drawing.Size(340, 549);
            this.Load += new System.EventHandler(this.RiverTaskControl_Load);
            this.Resize += new System.EventHandler(this.RiverTaskControl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstModel;
        private System.Windows.Forms.PropertyGrid prpModelParameters;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblParam;
    }
}
