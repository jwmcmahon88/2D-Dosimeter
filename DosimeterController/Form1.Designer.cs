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
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.logText = new System.Windows.Forms.TextBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.reinitializeButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(212, 281);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 6;
            this.startButton.Text = "Start Scan";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButtonClicked);
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.ForeColor = System.Drawing.Color.Maroon;
            this.status.Location = new System.Drawing.Point(55, 286);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(53, 13);
            this.status.TabIndex = 1;
            this.status.Text = "Inactive";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 286);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Status:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // logText
            // 
            this.logText.Location = new System.Drawing.Point(293, 12);
            this.logText.Multiline = true;
            this.logText.Name = "logText";
            this.logText.ReadOnly = true;
            this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logText.Size = new System.Drawing.Size(250, 292);
            this.logText.TabIndex = 10;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(12, 12);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(275, 263);
            this.propertyGrid.TabIndex = 11;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // reinitializeButton
            // 
            this.reinitializeButton.Location = new System.Drawing.Point(212, 281);
            this.reinitializeButton.Name = "reinitializeButton";
            this.reinitializeButton.Size = new System.Drawing.Size(75, 23);
            this.reinitializeButton.TabIndex = 13;
            this.reinitializeButton.Text = "Reinitialize";
            this.reinitializeButton.UseVisualStyleBackColor = true;
            this.reinitializeButton.Click += new System.EventHandler(this.ReinitializeButtonClicked);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(212, 281);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 14;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButtonClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 316);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.reinitializeButton);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.status);
            this.Controls.Add(this.logText);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.startButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "2D Dosimeter Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox logText;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button reinitializeButton;
        private System.Windows.Forms.Button stopButton;
    }
}