namespace ShowGraphic
{
    partial class FShow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cListBoxFiltr = new System.Windows.Forms.CheckedListBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.topdY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cb2K = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbdY = new System.Windows.Forms.TextBox();
            this.cbRevers = new System.Windows.Forms.CheckBox();
            this.ch_Ex = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cListBoxFiltr
            // 
            this.cListBoxFiltr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cListBoxFiltr.FormattingEnabled = true;
            this.cListBoxFiltr.Location = new System.Drawing.Point(698, 12);
            this.cListBoxFiltr.Name = "cListBoxFiltr";
            this.cListBoxFiltr.Size = new System.Drawing.Size(120, 199);
            this.cListBoxFiltr.TabIndex = 0;
            this.cListBoxFiltr.SelectedIndexChanged += new System.EventHandler(this.cListBoxFiltr_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(703, 246);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(81, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Отрисовка";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(703, 441);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Перерисовка";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // topdY
            // 
            this.topdY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.topdY.Location = new System.Drawing.Point(700, 312);
            this.topdY.Name = "topdY";
            this.topdY.Size = new System.Drawing.Size(115, 20);
            this.topdY.TabIndex = 3;
            this.topdY.Text = "0.05";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(700, 296);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Масштаб";
            // 
            // cb2K
            // 
            this.cb2K.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb2K.AutoSize = true;
            this.cb2K.Location = new System.Drawing.Point(703, 418);
            this.cb2K.Name = "cb2K";
            this.cb2K.Size = new System.Drawing.Size(106, 17);
            this.cb2K.TabIndex = 5;
            this.cb2K.Text = "Полный график";
            this.cb2K.UseVisualStyleBackColor = true;
            this.cb2K.CheckedChanged += new System.EventHandler(this.cb2K_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(700, 343);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Смещение по Y";
            // 
            // tbdY
            // 
            this.tbdY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbdY.Location = new System.Drawing.Point(700, 359);
            this.tbdY.Name = "tbdY";
            this.tbdY.Size = new System.Drawing.Size(115, 20);
            this.tbdY.TabIndex = 6;
            this.tbdY.Text = "0";
            // 
            // cbRevers
            // 
            this.cbRevers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRevers.AutoSize = true;
            this.cbRevers.Location = new System.Drawing.Point(703, 395);
            this.cbRevers.Name = "cbRevers";
            this.cbRevers.Size = new System.Drawing.Size(63, 17);
            this.cbRevers.TabIndex = 8;
            this.cbRevers.Text = "Реверс";
            this.cbRevers.UseVisualStyleBackColor = true;
            this.cbRevers.CheckedChanged += new System.EventHandler(this.cbRevers_CheckedChanged);
            // 
            // ch_Ex
            // 
            this.ch_Ex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ch_Ex.AutoSize = true;
            this.ch_Ex.Location = new System.Drawing.Point(702, 270);
            this.ch_Ex.Name = "ch_Ex";
            this.ch_Ex.Size = new System.Drawing.Size(94, 17);
            this.ch_Ex.TabIndex = 9;
            this.ch_Ex.Text = "Эксперимент";
            this.ch_Ex.UseVisualStyleBackColor = true;
            // 
            // FShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(830, 539);
            this.Controls.Add(this.ch_Ex);
            this.Controls.Add(this.cbRevers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbdY);
            this.Controls.Add(this.cb2K);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.topdY);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.cListBoxFiltr);
            this.Name = "FShow";
            this.Text = "FShow";
            this.Load += new System.EventHandler(this.FShow_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FShow_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox cListBoxFiltr;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox topdY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb2K;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbdY;
        private System.Windows.Forms.CheckBox cbRevers;
        private System.Windows.Forms.CheckBox ch_Ex;

    }
}