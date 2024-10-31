namespace MeshLibrary
{
    partial class MeshLibControl
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
            this.lstMesh = new System.Windows.Forms.ListBox();
            this.prpMeshParam = new System.Windows.Forms.PropertyGrid();
            this.lblGen = new System.Windows.Forms.Label();
            this.lblParam = new System.Windows.Forms.Label();
            this.txtTopLayer = new System.Windows.Forms.TextBox();
            this.txtBottomLayer = new System.Windows.Forms.TextBox();
            this.lblm1 = new System.Windows.Forms.Label();
            this.lblTopLayer = new System.Windows.Forms.Label();
            this.lblBottomLayer = new System.Windows.Forms.Label();
            this.lblm2 = new System.Windows.Forms.Label();
            this.btnTop = new System.Windows.Forms.RadioButton();
            this.btnMiddle = new System.Windows.Forms.RadioButton();
            this.btnBottom = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.nudParts = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudParts)).BeginInit();
            this.SuspendLayout();
            // 
            // lstMesh
            // 
            this.lstMesh.FormattingEnabled = true;
            this.lstMesh.Location = new System.Drawing.Point(3, 112);
            this.lstMesh.Name = "lstMesh";
            this.lstMesh.Size = new System.Drawing.Size(250, 69);
            this.lstMesh.TabIndex = 0;
            this.lstMesh.SelectedIndexChanged += new System.EventHandler(this.lstMesh_SelectedIndexChanged);
            // 
            // prpMeshParam
            // 
            this.prpMeshParam.Location = new System.Drawing.Point(3, 215);
            this.prpMeshParam.Name = "prpMeshParam";
            this.prpMeshParam.Size = new System.Drawing.Size(250, 305);
            this.prpMeshParam.TabIndex = 1;
            // 
            // lblGen
            // 
            this.lblGen.AutoSize = true;
            this.lblGen.Location = new System.Drawing.Point(4, 93);
            this.lblGen.Name = "lblGen";
            this.lblGen.Size = new System.Drawing.Size(54, 13);
            this.lblGen.TabIndex = 2;
            this.lblGen.Text = "Generator";
            // 
            // lblParam
            // 
            this.lblParam.AutoSize = true;
            this.lblParam.Location = new System.Drawing.Point(4, 199);
            this.lblParam.Name = "lblParam";
            this.lblParam.Size = new System.Drawing.Size(60, 13);
            this.lblParam.TabIndex = 3;
            this.lblParam.Text = "Parameters";
            // 
            // txtTopLayer
            // 
            this.txtTopLayer.Location = new System.Drawing.Point(81, 19);
            this.txtTopLayer.Name = "txtTopLayer";
            this.txtTopLayer.Size = new System.Drawing.Size(47, 20);
            this.txtTopLayer.TabIndex = 9;
            this.txtTopLayer.Text = "0";
            this.txtTopLayer.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtBottomLayer
            // 
            this.txtBottomLayer.Location = new System.Drawing.Point(81, 62);
            this.txtBottomLayer.Name = "txtBottomLayer";
            this.txtBottomLayer.Size = new System.Drawing.Size(47, 20);
            this.txtBottomLayer.TabIndex = 10;
            this.txtBottomLayer.Text = "0";
            this.txtBottomLayer.TextChanged += new System.EventHandler(this.txtBottomLayer_TextChanged);
            // 
            // lblm1
            // 
            this.lblm1.AutoSize = true;
            this.lblm1.Location = new System.Drawing.Point(130, 20);
            this.lblm1.Name = "lblm1";
            this.lblm1.Size = new System.Drawing.Size(15, 13);
            this.lblm1.TabIndex = 11;
            this.lblm1.Text = "m";
            // 
            // lblTopLayer
            // 
            this.lblTopLayer.AutoSize = true;
            this.lblTopLayer.Location = new System.Drawing.Point(3, 19);
            this.lblTopLayer.Name = "lblTopLayer";
            this.lblTopLayer.Size = new System.Drawing.Size(55, 13);
            this.lblTopLayer.TabIndex = 12;
            this.lblTopLayer.Text = "Top Layer";
            // 
            // lblBottomLayer
            // 
            this.lblBottomLayer.AutoSize = true;
            this.lblBottomLayer.Location = new System.Drawing.Point(4, 62);
            this.lblBottomLayer.Name = "lblBottomLayer";
            this.lblBottomLayer.Size = new System.Drawing.Size(69, 13);
            this.lblBottomLayer.TabIndex = 13;
            this.lblBottomLayer.Text = "Bottom Layer";
            // 
            // lblm2
            // 
            this.lblm2.AutoSize = true;
            this.lblm2.Location = new System.Drawing.Point(130, 69);
            this.lblm2.Name = "lblm2";
            this.lblm2.Size = new System.Drawing.Size(15, 13);
            this.lblm2.TabIndex = 14;
            this.lblm2.Text = "m";
            // 
            // btnTop
            // 
            this.btnTop.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnTop.AutoSize = true;
            this.btnTop.Location = new System.Drawing.Point(165, 15);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(74, 23);
            this.btnTop.TabIndex = 15;
            this.btnTop.Text = " Top   Layer";
            this.btnTop.UseVisualStyleBackColor = true;
            this.btnTop.CheckedChanged += new System.EventHandler(this.btnTop_CheckedChanged);
            // 
            // btnMiddle
            // 
            this.btnMiddle.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnMiddle.AutoSize = true;
            this.btnMiddle.Checked = true;
            this.btnMiddle.Location = new System.Drawing.Point(165, 38);
            this.btnMiddle.Name = "btnMiddle";
            this.btnMiddle.Size = new System.Drawing.Size(74, 23);
            this.btnMiddle.TabIndex = 16;
            this.btnMiddle.TabStop = true;
            this.btnMiddle.Text = "MiddleLayer";
            this.btnMiddle.UseVisualStyleBackColor = true;
            this.btnMiddle.CheckedChanged += new System.EventHandler(this.btnMiddle_CheckedChanged);
            // 
            // btnBottom
            // 
            this.btnBottom.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnBottom.AutoSize = true;
            this.btnBottom.Location = new System.Drawing.Point(165, 61);
            this.btnBottom.Name = "btnBottom";
            this.btnBottom.Size = new System.Drawing.Size(76, 23);
            this.btnBottom.TabIndex = 17;
            this.btnBottom.Text = "BottomLayer";
            this.btnBottom.UseVisualStyleBackColor = true;
            this.btnBottom.CheckedChanged += new System.EventHandler(this.btnBottom_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Parts";
            // 
            // nudParts
            // 
            this.nudParts.Location = new System.Drawing.Point(191, 90);
            this.nudParts.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudParts.Name = "nudParts";
            this.nudParts.Size = new System.Drawing.Size(48, 20);
            this.nudParts.TabIndex = 19;
            this.nudParts.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudParts.ValueChanged += new System.EventHandler(this.nudParts_ValueChanged);
            // 
            // MeshLibControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudParts);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBottom);
            this.Controls.Add(this.btnMiddle);
            this.Controls.Add(this.btnTop);
            this.Controls.Add(this.lblm2);
            this.Controls.Add(this.lblBottomLayer);
            this.Controls.Add(this.lblTopLayer);
            this.Controls.Add(this.lblm1);
            this.Controls.Add(this.txtBottomLayer);
            this.Controls.Add(this.txtTopLayer);
            this.Controls.Add(this.lblParam);
            this.Controls.Add(this.lblGen);
            this.Controls.Add(this.prpMeshParam);
            this.Controls.Add(this.lstMesh);
            this.Name = "MeshLibControl";
            this.Size = new System.Drawing.Size(255, 530);
            this.Load += new System.EventHandler(this.MeshLibControl_Load);
            this.Resize += new System.EventHandler(this.MeshLibControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.nudParts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstMesh;
        private System.Windows.Forms.PropertyGrid prpMeshParam;
        private System.Windows.Forms.Label lblGen;
        private System.Windows.Forms.Label lblParam;
        private System.Windows.Forms.TextBox txtTopLayer;
        private System.Windows.Forms.TextBox txtBottomLayer;
        private System.Windows.Forms.Label lblm1;
        private System.Windows.Forms.Label lblTopLayer;
        private System.Windows.Forms.Label lblBottomLayer;
        private System.Windows.Forms.Label lblm2;
        private System.Windows.Forms.RadioButton btnTop;
        private System.Windows.Forms.RadioButton btnMiddle;
        private System.Windows.Forms.RadioButton btnBottom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudParts;
    }
}
