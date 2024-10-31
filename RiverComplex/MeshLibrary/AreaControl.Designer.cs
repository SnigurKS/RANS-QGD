namespace MeshLibrary
{
    partial class AreaControl
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
            this.rbtnBottom = new System.Windows.Forms.RadioButton();
            this.rbtnTop = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtH = new System.Windows.Forms.TextBox();
            this.txtL = new System.Windows.Forms.TextBox();
            this.lstSpline = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // rbtnBottom
            // 
            this.rbtnBottom.AutoSize = true;
            this.rbtnBottom.Checked = true;
            this.rbtnBottom.Location = new System.Drawing.Point(3, 6);
            this.rbtnBottom.Name = "rbtnBottom";
            this.rbtnBottom.Size = new System.Drawing.Size(109, 17);
            this.rbtnBottom.TabIndex = 0;
            this.rbtnBottom.TabStop = true;
            this.rbtnBottom.Text = "Нижняя граница";
            this.rbtnBottom.UseVisualStyleBackColor = true;
            this.rbtnBottom.CheckedChanged += new System.EventHandler(this.rbtnBottom_CheckedChanged);
            // 
            // rbtnTop
            // 
            this.rbtnTop.AutoSize = true;
            this.rbtnTop.Location = new System.Drawing.Point(3, 29);
            this.rbtnTop.Name = "rbtnTop";
            this.rbtnTop.Size = new System.Drawing.Size(111, 17);
            this.rbtnTop.TabIndex = 1;
            this.rbtnTop.Text = "Верхняя граница";
            this.rbtnTop.UseVisualStyleBackColor = true;
            this.rbtnTop.CheckedChanged += new System.EventHandler(this.rbtnTop_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(115, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "H";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "L";
            // 
            // txtH
            // 
            this.txtH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtH.Location = new System.Drawing.Point(136, 28);
            this.txtH.Name = "txtH";
            this.txtH.Size = new System.Drawing.Size(48, 20);
            this.txtH.TabIndex = 4;
            this.txtH.Text = "1";
            this.txtH.TextChanged += new System.EventHandler(this.txtH_TextChanged);
            // 
            // txtL
            // 
            this.txtL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtL.Location = new System.Drawing.Point(136, 5);
            this.txtL.Name = "txtL";
            this.txtL.Size = new System.Drawing.Size(48, 20);
            this.txtL.TabIndex = 5;
            this.txtL.Text = "2";
            this.txtL.TextChanged += new System.EventHandler(this.txtL_TextChanged);
            // 
            // lstSpline
            // 
            this.lstSpline.FormattingEnabled = true;
            this.lstSpline.Items.AddRange(new object[] {
            "Нет",
            "Линейный",
            "Кубический",
            "Эрмитов",
            "Акимы"});
            this.lstSpline.Location = new System.Drawing.Point(3, 74);
            this.lstSpline.Name = "lstSpline";
            this.lstSpline.Size = new System.Drawing.Size(195, 69);
            this.lstSpline.TabIndex = 6;
            this.lstSpline.SelectedIndexChanged += new System.EventHandler(this.lstSpline_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Сплайн";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 158);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(191, 150);
            this.dataGridView1.TabIndex = 8;
            // 
            // AreaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lstSpline);
            this.Controls.Add(this.txtL);
            this.Controls.Add(this.txtH);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbtnTop);
            this.Controls.Add(this.rbtnBottom);
            this.Name = "AreaControl";
            this.Size = new System.Drawing.Size(208, 334);
            this.Resize += new System.EventHandler(this.AreaControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbtnBottom;
        private System.Windows.Forms.RadioButton rbtnTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtH;
        private System.Windows.Forms.TextBox txtL;
        private System.Windows.Forms.ListBox lstSpline;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
