namespace ShowGraphic
{
    partial class FaceGraph
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FaceGraph));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtZoomY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtZoomX = new System.Windows.Forms.TextBox();
            this.btn_zoom = new System.Windows.Forms.Button();
            this.chLogY = new System.Windows.Forms.CheckBox();
            this.chLogX = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rY = new System.Windows.Forms.RadioButton();
            this.rX = new System.Windows.Forms.RadioButton();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMult = new System.Windows.Forms.TextBox();
            this.chMult = new System.Windows.Forms.ComboBox();
            this.txtPointSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnY = new System.Windows.Forms.Button();
            this.chExp = new System.Windows.Forms.CheckBox();
            this.btnX = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chPaletteLine = new System.Windows.Forms.ComboBox();
            this.chWidthLine = new System.Windows.Forms.ComboBox();
            this.btnVis = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.fontDialog2 = new System.Windows.Forms.FontDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StripStatusLabel_c2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButton_C2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cmb_box_C2 = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(9, 10);
            this.chart1.Margin = new System.Windows.Forms.Padding(2);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(650, 463);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtZoomY);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.txtZoomX);
            this.panel1.Controls.Add(this.btn_zoom);
            this.panel1.Controls.Add(this.chLogY);
            this.panel1.Controls.Add(this.chLogX);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.rY);
            this.panel1.Controls.Add(this.rX);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtMult);
            this.panel1.Controls.Add(this.chMult);
            this.panel1.Controls.Add(this.txtPointSize);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnY);
            this.panel1.Controls.Add(this.chExp);
            this.panel1.Controls.Add(this.btnX);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.chPaletteLine);
            this.panel1.Controls.Add(this.chWidthLine);
            this.panel1.Location = new System.Drawing.Point(486, 146);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(163, 326);
            this.panel1.TabIndex = 5;
            this.panel1.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(87, 307);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "y-";
            // 
            // txtZoomY
            // 
            this.txtZoomY.Location = new System.Drawing.Point(105, 304);
            this.txtZoomY.Name = "txtZoomY";
            this.txtZoomY.Size = new System.Drawing.Size(49, 20);
            this.txtZoomY.TabIndex = 23;
            this.txtZoomY.Text = "0.1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 283);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "x-";
            // 
            // btnSave
            // 
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(10, 257);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(73, 22);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtZoomX
            // 
            this.txtZoomX.Location = new System.Drawing.Point(105, 281);
            this.txtZoomX.Name = "txtZoomX";
            this.txtZoomX.Size = new System.Drawing.Size(49, 20);
            this.txtZoomX.TabIndex = 21;
            this.txtZoomX.Text = "0.1";
            // 
            // btn_zoom
            // 
            this.btn_zoom.Location = new System.Drawing.Point(19, 285);
            this.btn_zoom.Name = "btn_zoom";
            this.btn_zoom.Size = new System.Drawing.Size(55, 30);
            this.btn_zoom.TabIndex = 17;
            this.btn_zoom.Text = "Zoom";
            this.btn_zoom.UseVisualStyleBackColor = true;
            this.btn_zoom.Click += new System.EventHandler(this.btn_zoom_Click);
            // 
            // chLogY
            // 
            this.chLogY.AutoSize = true;
            this.chLogY.Location = new System.Drawing.Point(93, 58);
            this.chLogY.Name = "chLogY";
            this.chLogY.Size = new System.Drawing.Size(51, 17);
            this.chLogY.TabIndex = 20;
            this.chLogY.Text = "LogY";
            this.chLogY.UseVisualStyleBackColor = true;
            this.chLogY.CheckedChanged += new System.EventHandler(this.chLogY_CheckedChanged);
            // 
            // chLogX
            // 
            this.chLogX.AutoSize = true;
            this.chLogX.Location = new System.Drawing.Point(10, 59);
            this.chLogX.Name = "chLogX";
            this.chLogX.Size = new System.Drawing.Size(51, 17);
            this.chLogX.TabIndex = 19;
            this.chLogX.Text = "LogX";
            this.chLogX.UseVisualStyleBackColor = true;
            this.chLogX.CheckedChanged += new System.EventHandler(this.chLogX_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(93, 80);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(61, 20);
            this.textBox1.TabIndex = 18;
            this.textBox1.Text = "0";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // rY
            // 
            this.rY.AutoSize = true;
            this.rY.Location = new System.Drawing.Point(50, 81);
            this.rY.Margin = new System.Windows.Forms.Padding(2);
            this.rY.Name = "rY";
            this.rY.Size = new System.Drawing.Size(32, 17);
            this.rY.TabIndex = 17;
            this.rY.Text = "Y";
            this.rY.UseVisualStyleBackColor = true;
            this.rY.CheckedChanged += new System.EventHandler(this.rY_CheckedChanged);
            // 
            // rX
            // 
            this.rX.AutoSize = true;
            this.rX.Checked = true;
            this.rX.Location = new System.Drawing.Point(10, 81);
            this.rX.Margin = new System.Windows.Forms.Padding(2);
            this.rX.Name = "rX";
            this.rX.Size = new System.Drawing.Size(32, 17);
            this.rX.TabIndex = 16;
            this.rX.TabStop = true;
            this.rX.Text = "X";
            this.rX.UseVisualStyleBackColor = true;
            this.rX.CheckedChanged += new System.EventHandler(this.rX_CheckedChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnLoad.Location = new System.Drawing.Point(86, 257);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(68, 22);
            this.btnLoad.TabIndex = 15;
            this.btnLoad.Text = "Загрузить";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 103);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Обезразмеривающий коэфф.";
            // 
            // txtMult
            // 
            this.txtMult.Location = new System.Drawing.Point(124, 116);
            this.txtMult.Margin = new System.Windows.Forms.Padding(2);
            this.txtMult.Name = "txtMult";
            this.txtMult.Size = new System.Drawing.Size(30, 20);
            this.txtMult.TabIndex = 12;
            this.txtMult.Text = "1";
            this.txtMult.TextChanged += new System.EventHandler(this.txtMult_TextChanged);
            // 
            // chMult
            // 
            this.chMult.FormattingEnabled = true;
            this.chMult.Location = new System.Drawing.Point(10, 116);
            this.chMult.Margin = new System.Windows.Forms.Padding(2);
            this.chMult.Name = "chMult";
            this.chMult.Size = new System.Drawing.Size(100, 21);
            this.chMult.TabIndex = 11;
            this.chMult.SelectedIndexChanged += new System.EventHandler(this.chMult_SelectedIndexChanged);
            // 
            // txtPointSize
            // 
            this.txtPointSize.Location = new System.Drawing.Point(124, 236);
            this.txtPointSize.Margin = new System.Windows.Forms.Padding(2);
            this.txtPointSize.Name = "txtPointSize";
            this.txtPointSize.Size = new System.Drawing.Size(30, 20);
            this.txtPointSize.TabIndex = 10;
            this.txtPointSize.Text = "8";
            this.txtPointSize.Click += new System.EventHandler(this.txtPointSize_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 239);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Размер эксп. точек";
            // 
            // btnY
            // 
            this.btnY.Location = new System.Drawing.Point(10, 197);
            this.btnY.Margin = new System.Windows.Forms.Padding(2);
            this.btnY.Name = "btnY";
            this.btnY.Size = new System.Drawing.Size(144, 19);
            this.btnY.TabIndex = 7;
            this.btnY.Text = "Настройка шрифта оси Y";
            this.btnY.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnY.UseVisualStyleBackColor = true;
            this.btnY.Click += new System.EventHandler(this.btnY_Click);
            // 
            // chExp
            // 
            this.chExp.AutoSize = true;
            this.chExp.Location = new System.Drawing.Point(16, 220);
            this.chExp.Margin = new System.Windows.Forms.Padding(2);
            this.chExp.Name = "chExp";
            this.chExp.Size = new System.Drawing.Size(94, 17);
            this.chExp.TabIndex = 1;
            this.chExp.Text = "Эксперимент";
            this.chExp.UseVisualStyleBackColor = true;
            this.chExp.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btnX
            // 
            this.btnX.Location = new System.Drawing.Point(10, 173);
            this.btnX.Margin = new System.Windows.Forms.Padding(2);
            this.btnX.Name = "btnX";
            this.btnX.Size = new System.Drawing.Size(144, 19);
            this.btnX.TabIndex = 6;
            this.btnX.Text = "Настройка шрифта оси X";
            this.btnX.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnX.UseVisualStyleBackColor = true;
            this.btnX.Click += new System.EventHandler(this.btnX_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 138);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "толщина";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 138);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Палитра линий";
            // 
            // chPaletteLine
            // 
            this.chPaletteLine.FormattingEnabled = true;
            this.chPaletteLine.Items.AddRange(new object[] {
            "Цветные",
            "Черные"});
            this.chPaletteLine.Location = new System.Drawing.Point(10, 151);
            this.chPaletteLine.Margin = new System.Windows.Forms.Padding(2);
            this.chPaletteLine.Name = "chPaletteLine";
            this.chPaletteLine.Size = new System.Drawing.Size(100, 21);
            this.chPaletteLine.TabIndex = 2;
            this.chPaletteLine.TextChanged += new System.EventHandler(this.chPaletteLine_SelectedIndexChanged);
            // 
            // chWidthLine
            // 
            this.chWidthLine.FormattingEnabled = true;
            this.chWidthLine.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.chWidthLine.Location = new System.Drawing.Point(124, 151);
            this.chWidthLine.Margin = new System.Windows.Forms.Padding(2);
            this.chWidthLine.Name = "chWidthLine";
            this.chWidthLine.Size = new System.Drawing.Size(30, 21);
            this.chWidthLine.TabIndex = 3;
            this.chWidthLine.TextChanged += new System.EventHandler(this.chWidthLine_SelectedIndexChanged);
            // 
            // btnVis
            // 
            this.btnVis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVis.Location = new System.Drawing.Point(648, 368);
            this.btnVis.Margin = new System.Windows.Forms.Padding(2);
            this.btnVis.Name = "btnVis";
            this.btnVis.Size = new System.Drawing.Size(10, 19);
            this.btnVis.TabIndex = 16;
            this.btnVis.Text = "<";
            this.btnVis.UseVisualStyleBackColor = true;
            this.btnVis.Click += new System.EventHandler(this.btnVis_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripStatusLabel_c2,
            this.toolStripDropDownButton_C2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 471);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(668, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StripStatusLabel_c2
            // 
            this.StripStatusLabel_c2.Name = "StripStatusLabel_c2";
            this.StripStatusLabel_c2.Size = new System.Drawing.Size(91, 17);
            this.StripStatusLabel_c2.Text = "C2 Discrepancy:";
            // 
            // toolStripDropDownButton_C2
            // 
            this.toolStripDropDownButton_C2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton_C2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmb_box_C2});
            this.toolStripDropDownButton_C2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton_C2.Image")));
            this.toolStripDropDownButton_C2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton_C2.Name = "toolStripDropDownButton_C2";
            this.toolStripDropDownButton_C2.Size = new System.Drawing.Size(29, 20);
            this.toolStripDropDownButton_C2.Text = "toolStripDropDownButton1";
            // 
            // cmb_box_C2
            // 
            this.cmb_box_C2.Items.AddRange(new object[] {
            "U",
            "V",
            "K",
            "E",
            "Nu",
            "Tau",
            "Pk"});
            this.cmb_box_C2.MaxDropDownItems = 7;
            this.cmb_box_C2.Name = "cmb_box_C2";
            this.cmb_box_C2.Size = new System.Drawing.Size(121, 23);
            // 
            // FaceGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 493);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnVis);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FaceGraph";
            this.Text = "FaceGraph";
            this.Shown += new System.EventHandler(this.ChartForm_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChartForm_Paint);
            this.Resize += new System.EventHandler(this.ChartForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMult;
        private System.Windows.Forms.ComboBox chMult;
        private System.Windows.Forms.TextBox txtPointSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnY;
        private System.Windows.Forms.CheckBox chExp;
        private System.Windows.Forms.Button btnX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox chPaletteLine;
        private System.Windows.Forms.ComboBox chWidthLine;
        private System.Windows.Forms.Button btnVis;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.FontDialog fontDialog2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton rY;
        private System.Windows.Forms.RadioButton rX;
        private System.Windows.Forms.CheckBox chLogY;
        private System.Windows.Forms.CheckBox chLogX;
        private System.Windows.Forms.Button btn_zoom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtZoomY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtZoomX;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StripStatusLabel_c2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton_C2;
        private System.Windows.Forms.ToolStripComboBox cmb_box_C2;
    }
}