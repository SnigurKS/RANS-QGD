namespace AlgebraLibrary
{
    partial class AlgebraLibControl
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
            this.lstAlgorythm = new System.Windows.Forms.ListBox();
            this.lstSystemType = new System.Windows.Forms.ListBox();
            this.lblAlg = new System.Windows.Forms.Label();
            this.lblSys = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstAlgorythm
            // 
            this.lstAlgorythm.FormattingEnabled = true;
            this.lstAlgorythm.Location = new System.Drawing.Point(3, 25);
            this.lstAlgorythm.Name = "lstAlgorythm";
            this.lstAlgorythm.Size = new System.Drawing.Size(200, 82);
            this.lstAlgorythm.TabIndex = 0;
            // 
            // lstSystemType
            // 
            this.lstSystemType.FormattingEnabled = true;
            this.lstSystemType.Location = new System.Drawing.Point(3, 143);
            this.lstSystemType.Name = "lstSystemType";
            this.lstSystemType.Size = new System.Drawing.Size(200, 82);
            this.lstSystemType.TabIndex = 0;
            // 
            // lblAlg
            // 
            this.lblAlg.AutoSize = true;
            this.lblAlg.Location = new System.Drawing.Point(4, 4);
            this.lblAlg.Name = "lblAlg";
            this.lblAlg.Size = new System.Drawing.Size(53, 13);
            this.lblAlg.TabIndex = 1;
            this.lblAlg.Text = "Algorythm";
            // 
            // lblSys
            // 
            this.lblSys.AutoSize = true;
            this.lblSys.Location = new System.Drawing.Point(3, 127);
            this.lblSys.Name = "lblSys";
            this.lblSys.Size = new System.Drawing.Size(65, 13);
            this.lblSys.TabIndex = 2;
            this.lblSys.Text = "SystemType";
            // 
            // AlgebraLibControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblSys);
            this.Controls.Add(this.lblAlg);
            this.Controls.Add(this.lstAlgorythm);
            this.Controls.Add(this.lstSystemType);
            this.Name = "AlgebraLibControl";
            this.Size = new System.Drawing.Size(206, 242);
            this.Load += new System.EventHandler(this.AlgebraLibControl_Load);
            this.Resize += new System.EventHandler(this.AlgebraLibControl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstAlgorythm;
        private System.Windows.Forms.ListBox lstSystemType;
        private System.Windows.Forms.Label lblAlg;
        private System.Windows.Forms.Label lblSys;
    }
}
