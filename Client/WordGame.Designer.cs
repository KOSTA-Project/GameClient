
namespace Client
{
    partial class WordGame
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
            this.wordlist = new System.Windows.Forms.TextBox();
            this.wordInput = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // wordlist
            // 
            this.wordlist.Location = new System.Drawing.Point(48, 37);
            this.wordlist.Multiline = true;
            this.wordlist.Name = "wordlist";
            this.wordlist.Size = new System.Drawing.Size(204, 251);
            this.wordlist.TabIndex = 0;
            // 
            // wordInput
            // 
            this.wordInput.Location = new System.Drawing.Point(48, 311);
            this.wordInput.Name = "wordInput";
            this.wordInput.Size = new System.Drawing.Size(204, 21);
            this.wordInput.TabIndex = 1;
            this.wordInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.wordInput_KeyDown);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("굴림", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.ForeColor = System.Drawing.Color.Red;
            this.textBox1.Location = new System.Drawing.Point(270, 141);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(44, 39);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "5";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // WordGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 392);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.wordInput);
            this.Controls.Add(this.wordlist);
            this.Name = "WordGame";
            this.Text = "WordGame";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WordGame_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox wordlist;
        private System.Windows.Forms.TextBox wordInput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
    }
}