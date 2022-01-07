namespace App
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
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tmrRelease = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Speed = new System.Windows.Forms.Label();
            this.txtBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(9, 10);
            this.tbLog.Margin = new System.Windows.Forms.Padding(2);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(412, 165);
            this.tbLog.TabIndex = 0;
            this.tbLog.Text = "Выберите midi файл.";
            // 
            // tmrRelease
            // 
            this.tmrRelease.Interval = 1000;
            this.tmrRelease.Tick += new System.EventHandler(this.tmrRelease_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 180);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 70);
            this.button1.TabIndex = 1;
            this.button1.Text = "Выбрать .midi";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(133, 180);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 70);
            this.button2.TabIndex = 2;
            this.button2.Text = "Играть";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Speed
            // 
            this.Speed.AutoSize = true;
            this.Speed.Location = new System.Drawing.Point(270, 186);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(41, 13);
            this.Speed.TabIndex = 4;
            this.Speed.Text = "Speed:";
            // 
            // txtBox1
            // 
            this.txtBox1.Location = new System.Drawing.Point(308, 183);
            this.txtBox1.Name = "txtBox1";
            this.txtBox1.Size = new System.Drawing.Size(100, 20);
            this.txtBox1.TabIndex = 3;
            this.txtBox1.Text = "100";
            this.txtBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBox1.TextChanged += new System.EventHandler(this.txtBox1_TextChanged);
            this.txtBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBox1_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 262);
            this.Controls.Add(this.Speed);
            this.Controls.Add(this.txtBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbLog);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Piano cheat by Anal Network";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Timer tmrRelease;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label Speed;
        private System.Windows.Forms.TextBox txtBox1;
    }
}

