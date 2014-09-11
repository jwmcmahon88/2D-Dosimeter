namespace DosimeterController
{
    partial class Form1
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
            this.scanXMin = new System.Windows.Forms.NumericUpDown();
            this.scanXMax = new System.Windows.Forms.NumericUpDown();
            this.scanYMin = new System.Windows.Forms.NumericUpDown();
            this.scanYMax = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.scanLoadButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.scanSpeed = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.statusDataFile = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.statusLaser = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.statusCounter = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.statusPrinter = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.logText = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.scanXMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanXMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanYMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanYMax)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scanSpeed)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // scanXMin
            // 
            this.scanXMin.DecimalPlaces = 1;
            this.scanXMin.Location = new System.Drawing.Point(26, 19);
            this.scanXMin.Maximum = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.scanXMin.Name = "scanXMin";
            this.scanXMin.Size = new System.Drawing.Size(50, 20);
            this.scanXMin.TabIndex = 1;
            // 
            // scanXMax
            // 
            this.scanXMax.DecimalPlaces = 1;
            this.scanXMax.Location = new System.Drawing.Point(98, 19);
            this.scanXMax.Maximum = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.scanXMax.Name = "scanXMax";
            this.scanXMax.Size = new System.Drawing.Size(50, 20);
            this.scanXMax.TabIndex = 2;
            // 
            // scanYMin
            // 
            this.scanYMin.DecimalPlaces = 1;
            this.scanYMin.Location = new System.Drawing.Point(26, 45);
            this.scanYMin.Maximum = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.scanYMin.Name = "scanYMin";
            this.scanYMin.Size = new System.Drawing.Size(50, 20);
            this.scanYMin.TabIndex = 3;
            // 
            // scanYMax
            // 
            this.scanYMax.DecimalPlaces = 1;
            this.scanYMax.Location = new System.Drawing.Point(98, 45);
            this.scanYMax.Maximum = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.scanYMax.Name = "scanYMax";
            this.scanYMax.Size = new System.Drawing.Size(50, 20);
            this.scanYMax.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.scanLoadButton);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.scanSpeed);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.scanYMax);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.scanYMin);
            this.groupBox1.Controls.Add(this.scanXMin);
            this.groupBox1.Controls.Add(this.scanXMax);
            this.groupBox1.Location = new System.Drawing.Point(12, 102);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 130);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Readout Geometry";
            // 
            // scanLoadButton
            // 
            this.scanLoadButton.Location = new System.Drawing.Point(26, 101);
            this.scanLoadButton.Name = "scanLoadButton";
            this.scanLoadButton.Size = new System.Drawing.Size(151, 23);
            this.scanLoadButton.TabIndex = 15;
            this.scanLoadButton.Text = "Load from Image";
            this.scanLoadButton.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(51, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Speed:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(154, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "mm/min";
            // 
            // scanSpeed
            // 
            this.scanSpeed.DecimalPlaces = 1;
            this.scanSpeed.Location = new System.Drawing.Point(98, 71);
            this.scanSpeed.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.scanSpeed.Name = "scanSpeed";
            this.scanSpeed.Size = new System.Drawing.Size(50, 20);
            this.scanSpeed.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(154, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "mm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(154, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "mm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(82, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "X:";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(135, 238);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 6;
            this.startButton.Text = "Start Scan";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButtonClicked);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.statusDataFile);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.statusLaser);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.statusCounter);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.statusPrinter);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 84);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // statusDataFile
            // 
            this.statusDataFile.AutoSize = true;
            this.statusDataFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusDataFile.ForeColor = System.Drawing.Color.Black;
            this.statusDataFile.Location = new System.Drawing.Point(51, 61);
            this.statusDataFile.Name = "statusDataFile";
            this.statusDataFile.Size = new System.Drawing.Size(53, 13);
            this.statusDataFile.TabIndex = 7;
            this.statusDataFile.Text = "Inactive";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 61);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 6;
            this.label16.Text = "Data:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // statusLaser
            // 
            this.statusLaser.AutoSize = true;
            this.statusLaser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLaser.ForeColor = System.Drawing.Color.Maroon;
            this.statusLaser.Location = new System.Drawing.Point(51, 46);
            this.statusLaser.Name = "statusLaser";
            this.statusLaser.Size = new System.Drawing.Size(53, 13);
            this.statusLaser.TabIndex = 5;
            this.statusLaser.Text = "Inactive";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Laser:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // statusCounter
            // 
            this.statusCounter.AutoSize = true;
            this.statusCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusCounter.ForeColor = System.Drawing.Color.Maroon;
            this.statusCounter.Location = new System.Drawing.Point(51, 31);
            this.statusCounter.Name = "statusCounter";
            this.statusCounter.Size = new System.Drawing.Size(53, 13);
            this.statusCounter.TabIndex = 3;
            this.statusCounter.Text = "Inactive";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Counter:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // statusPrinter
            // 
            this.statusPrinter.AutoSize = true;
            this.statusPrinter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusPrinter.ForeColor = System.Drawing.Color.Maroon;
            this.statusPrinter.Location = new System.Drawing.Point(51, 16);
            this.statusPrinter.Name = "statusPrinter";
            this.statusPrinter.Size = new System.Drawing.Size(53, 13);
            this.statusPrinter.TabIndex = 1;
            this.statusPrinter.Text = "Inactive";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Printer:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 238);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Data File";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // logText
            // 
            this.logText.Location = new System.Drawing.Point(221, 12);
            this.logText.Multiline = true;
            this.logText.Name = "logText";
            this.logText.ReadOnly = true;
            this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logText.Size = new System.Drawing.Size(249, 249);
            this.logText.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 273);
            this.Controls.Add(this.logText);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.scanXMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanXMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanYMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scanYMax)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scanSpeed)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown scanXMin;
        private System.Windows.Forms.NumericUpDown scanXMax;
        private System.Windows.Forms.NumericUpDown scanYMin;
        private System.Windows.Forms.NumericUpDown scanYMax;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown scanSpeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label statusPrinter;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label statusLaser;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label statusCounter;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox logText;
        private System.Windows.Forms.Button scanLoadButton;
        private System.Windows.Forms.Label statusDataFile;
        private System.Windows.Forms.Label label16;

    }
}

