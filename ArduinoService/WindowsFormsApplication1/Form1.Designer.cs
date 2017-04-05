namespace WindowsFormsApplication1
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
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.BtnClear = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbEnableLogs = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 50);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(338, 471);
            this.textBox1.TabIndex = 1;
            // 
            // BtnClear
            // 
            this.BtnClear.Location = new System.Drawing.Point(275, 16);
            this.BtnClear.Name = "BtnClear";
            this.BtnClear.Size = new System.Drawing.Size(75, 23);
            this.BtnClear.TabIndex = 2;
            this.BtnClear.Text = "Clear";
            this.BtnClear.UseVisualStyleBackColor = true;
            this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbEnableLogs
            // 
            this.cbEnableLogs.AutoSize = true;
            this.cbEnableLogs.Location = new System.Drawing.Point(12, 20);
            this.cbEnableLogs.Name = "cbEnableLogs";
            this.cbEnableLogs.Size = new System.Drawing.Size(91, 17);
            this.cbEnableLogs.TabIndex = 3;
            this.cbEnableLogs.Text = "Logs Enabled";
            this.cbEnableLogs.UseVisualStyleBackColor = true;
            this.cbEnableLogs.CheckedChanged += new System.EventHandler(this.cbEnableLogs_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 533);
            this.Controls.Add(this.cbEnableLogs);
            this.Controls.Add(this.BtnClear);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Arduio Service";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button BtnClear;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbEnableLogs;
    }
}

