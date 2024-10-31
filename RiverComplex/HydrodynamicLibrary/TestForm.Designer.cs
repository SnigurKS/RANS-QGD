namespace HydrodynamicLibrary
{
    partial class TestForm
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            //this.taskControl1 = new RiverTaskLibrary.RiverTaskControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ch_Tau = new System.Windows.Forms.CheckBox();
            this.chU = new System.Windows.Forms.CheckBox();
            this.chH = new System.Windows.Forms.CheckBox();
            this.chEta = new System.Windows.Forms.CheckBox();
            this.chQ = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(6, 20);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(244, 403);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(735, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(264, 455);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            //this.tabPage1.Controls.Add(this.taskControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(256, 429);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Дно";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // taskControl1
            //// 
            //this.taskControl1.Location = new System.Drawing.Point(3, 6);
            //this.taskControl1.Name = "taskControl1";
            //this.taskControl1.Size = new System.Drawing.Size(247, 417);
            //this.taskControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.propertyGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(256, 429);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Поток";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Гидродинамика - метод обратного хода";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(739, 473);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 36);
            this.button1.TabIndex = 3;
            this.button1.Text = "Размыв";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(829, 473);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 21);
            this.button2.TabIndex = 4;
            this.button2.Text = "Отрисовка";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ch_Tau
            // 
            this.ch_Tau.AutoSize = true;
            this.ch_Tau.Location = new System.Drawing.Point(739, 530);
            this.ch_Tau.Name = "ch_Tau";
            this.ch_Tau.Size = new System.Drawing.Size(45, 17);
            this.ch_Tau.TabIndex = 5;
            this.ch_Tau.Text = "Tau";
            this.ch_Tau.UseVisualStyleBackColor = true;
            // 
            // chU
            // 
            this.chU.AutoSize = true;
            this.chU.Location = new System.Drawing.Point(790, 530);
            this.chU.Name = "chU";
            this.chU.Size = new System.Drawing.Size(34, 17);
            this.chU.TabIndex = 6;
            this.chU.Text = "U";
            this.chU.UseVisualStyleBackColor = true;
            // 
            // chH
            // 
            this.chH.AutoSize = true;
            this.chH.Location = new System.Drawing.Point(829, 530);
            this.chH.Name = "chH";
            this.chH.Size = new System.Drawing.Size(34, 17);
            this.chH.TabIndex = 7;
            this.chH.Text = "H";
            this.chH.UseVisualStyleBackColor = true;
            // 
            // chEta
            // 
            this.chEta.AutoSize = true;
            this.chEta.Location = new System.Drawing.Point(869, 530);
            this.chEta.Name = "chEta";
            this.chEta.Size = new System.Drawing.Size(42, 17);
            this.chEta.TabIndex = 8;
            this.chEta.Text = "Eta";
            this.chEta.UseVisualStyleBackColor = true;
            // 
            // chQ
            // 
            this.chQ.AutoSize = true;
            this.chQ.Location = new System.Drawing.Point(917, 530);
            this.chQ.Name = "chQ";
            this.chQ.Size = new System.Drawing.Size(32, 17);
            this.chQ.TabIndex = 9;
            this.chQ.Text = "q";
            this.chQ.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(919, 473);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(84, 36);
            this.button3.TabIndex = 10;
            this.button3.Text = "Бугор";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(829, 500);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(84, 21);
            this.button4.TabIndex = 11;
            this.button4.Text = "График";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 559);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chQ);
            this.Controls.Add(this.chEta);
            this.Controls.Add(this.chH);
            this.Controls.Add(this.chU);
            this.Controls.Add(this.ch_Tau);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TestForm_Paint);
            this.Resize += new System.EventHandler(this.TestForm_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        //private RiverTaskLibrary.RiverTaskControl taskControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox ch_Tau;
        private System.Windows.Forms.CheckBox chU;
        private System.Windows.Forms.CheckBox chH;
        private System.Windows.Forms.CheckBox chEta;
        private System.Windows.Forms.CheckBox chQ;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}