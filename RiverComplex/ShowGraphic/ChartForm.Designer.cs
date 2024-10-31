namespace ShowGraphic
{
    partial class ChartForm
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
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chExp = new System.Windows.Forms.CheckBox();
            this.chWidthLine = new System.Windows.Forms.ComboBox();
            this.chPaletteLine = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.chLineType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtZoomY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtZoomX = new System.Windows.Forms.TextBox();
            this.btnZoom = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMult = new System.Windows.Forms.TextBox();
            this.chMult = new System.Windows.Forms.ComboBox();
            this.txtPointSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnY = new System.Windows.Forms.Button();
            this.btnX = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.fontDialog2 = new System.Windows.Forms.FontDialog();
            this.btnVis = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StripStatuslbl_C2 = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(788, 508);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart2";
            // 
            // chExp
            // 
            this.chExp.AutoSize = true;
            this.chExp.Location = new System.Drawing.Point(13, 220);
            this.chExp.Name = "chExp";
            this.chExp.Size = new System.Drawing.Size(94, 17);
            this.chExp.TabIndex = 1;
            this.chExp.Text = "Эксперимент";
            this.chExp.UseVisualStyleBackColor = true;
            this.chExp.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
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
            this.chWidthLine.Location = new System.Drawing.Point(154, 98);
            this.chWidthLine.Name = "chWidthLine";
            this.chWidthLine.Size = new System.Drawing.Size(34, 21);
            this.chWidthLine.TabIndex = 3;
            this.chWidthLine.SelectedIndexChanged += new System.EventHandler(this.chWidthLine_SelectedIndexChanged);
            // 
            // chPaletteLine
            // 
            this.chPaletteLine.FormattingEnabled = true;
            this.chPaletteLine.Items.AddRange(new object[] {
            "Цветные",
            "Черные"});
            this.chPaletteLine.Location = new System.Drawing.Point(5, 98);
            this.chPaletteLine.Name = "chPaletteLine";
            this.chPaletteLine.Size = new System.Drawing.Size(121, 21);
            this.chPaletteLine.TabIndex = 2;
            this.chPaletteLine.SelectedIndexChanged += new System.EventHandler(this.chPaletteLine_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Палитра линий";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Толщина линий";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.chLineType);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtZoomY);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtZoomX);
            this.panel1.Controls.Add(this.btnZoom);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Controls.Add(this.btnSave);
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
            this.panel1.Location = new System.Drawing.Point(601, 191);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(199, 319);
            this.panel1.TabIndex = 4;
            this.panel1.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Тип отрисовки";
            // 
            // chLineType
            // 
            this.chLineType.FormattingEnabled = true;
            this.chLineType.Items.AddRange(new object[] {
            "Цветные",
            "Черные"});
            this.chLineType.Location = new System.Drawing.Point(5, 135);
            this.chLineType.Name = "chLineType";
            this.chLineType.Size = new System.Drawing.Size(181, 21);
            this.chLineType.TabIndex = 21;
            this.chLineType.SelectedIndexChanged += new System.EventHandler(this.chLineType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(133, 294);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "y-";
            // 
            // txtZoomY
            // 
            this.txtZoomY.Location = new System.Drawing.Point(149, 291);
            this.txtZoomY.Name = "txtZoomY";
            this.txtZoomY.Size = new System.Drawing.Size(43, 20);
            this.txtZoomY.TabIndex = 19;
            this.txtZoomY.Text = "0.1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(65, 294);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "x-";
            // 
            // txtZoomX
            // 
            this.txtZoomX.Location = new System.Drawing.Point(83, 292);
            this.txtZoomX.Name = "txtZoomX";
            this.txtZoomX.Size = new System.Drawing.Size(43, 20);
            this.txtZoomX.TabIndex = 17;
            this.txtZoomX.Text = "0.1";
            // 
            // btnZoom
            // 
            this.btnZoom.Location = new System.Drawing.Point(13, 291);
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(43, 20);
            this.btnZoom.TabIndex = 16;
            this.btnZoom.Text = "Zoom";
            this.btnZoom.UseVisualStyleBackColor = true;
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(113, 265);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 15;
            this.btnLoad.Text = "Загрузить";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(13, 265);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(190, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Обезразмеривающий коэффициент";
            // 
            // txtMult
            // 
            this.txtMult.Location = new System.Drawing.Point(149, 55);
            this.txtMult.Name = "txtMult";
            this.txtMult.Size = new System.Drawing.Size(39, 20);
            this.txtMult.TabIndex = 12;
            this.txtMult.Text = "1";
            this.txtMult.TextChanged += new System.EventHandler(this.txtMult_TextChanged);
            // 
            // chMult
            // 
            this.chMult.FormattingEnabled = true;
            this.chMult.Location = new System.Drawing.Point(7, 54);
            this.chMult.Name = "chMult";
            this.chMult.Size = new System.Drawing.Size(121, 21);
            this.chMult.TabIndex = 11;
            this.chMult.SelectedIndexChanged += new System.EventHandler(this.chMult_SelectedIndexChanged);
            // 
            // txtPointSize
            // 
            this.txtPointSize.Location = new System.Drawing.Point(147, 243);
            this.txtPointSize.Name = "txtPointSize";
            this.txtPointSize.Size = new System.Drawing.Size(39, 20);
            this.txtPointSize.TabIndex = 10;
            this.txtPointSize.Text = "8";
            this.txtPointSize.TextChanged += new System.EventHandler(this.txtPointSize_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Размер эксп. точек";
            // 
            // btnY
            // 
            this.btnY.Location = new System.Drawing.Point(11, 191);
            this.btnY.Name = "btnY";
            this.btnY.Size = new System.Drawing.Size(175, 23);
            this.btnY.TabIndex = 7;
            this.btnY.Text = "Настройка шрифта оси Y";
            this.btnY.UseVisualStyleBackColor = true;
            this.btnY.Click += new System.EventHandler(this.btnY_Click);
            // 
            // btnX
            // 
            this.btnX.Location = new System.Drawing.Point(11, 162);
            this.btnX.Name = "btnX";
            this.btnX.Size = new System.Drawing.Size(175, 23);
            this.btnX.TabIndex = 6;
            this.btnX.Text = "Настройка шрифта оси X";
            this.btnX.UseVisualStyleBackColor = true;
            this.btnX.Click += new System.EventHandler(this.btnX_Click);
            // 
            // btnVis
            // 
            this.btnVis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVis.Location = new System.Drawing.Point(793, 414);
            this.btnVis.Name = "btnVis";
            this.btnVis.Size = new System.Drawing.Size(14, 23);
            this.btnVis.TabIndex = 5;
            this.btnVis.Text = "<";
            this.btnVis.UseVisualStyleBackColor = true;
            this.btnVis.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripStatuslbl_C2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 510);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(812, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StripStatuslbl_C2
            // 
            this.StripStatuslbl_C2.Name = "StripStatuslbl_C2";
            this.StripStatuslbl_C2.Size = new System.Drawing.Size(90, 17);
            this.StripStatuslbl_C2.Text = "C2 discrepancy:";
            // 
            // ChartForm
            // 
            this.ClientSize = new System.Drawing.Size(812, 532);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnVis);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart1);
            this.Name = "ChartForm";
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
        private System.Windows.Forms.CheckBox chExp;
        private System.Windows.Forms.ComboBox chWidthLine;
        private System.Windows.Forms.ComboBox chPaletteLine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnY;
        private System.Windows.Forms.Button btnX;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.FontDialog fontDialog2;
        private System.Windows.Forms.Button btnVis;
        private System.Windows.Forms.TextBox txtPointSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMult;
        private System.Windows.Forms.ComboBox chMult;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtZoomX;
        private System.Windows.Forms.Button btnZoom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtZoomY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox chLineType;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StripStatuslbl_C2;
    }
}