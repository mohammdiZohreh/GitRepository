namespace TesterIKBCM
{
    partial class resultMBox
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
            this.lblPass = new System.Windows.Forms.Label();
            this.lblresult = new System.Windows.Forms.Label();
            this.lblresult2 = new System.Windows.Forms.Label();
            this.lblresult3 = new System.Windows.Forms.Label();
            this.lblresult4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblPass.Location = new System.Drawing.Point(278, 36);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(145, 31);
            this.lblPass.TabIndex = 1;
            this.lblPass.Text = "Test Pass";
            this.lblPass.Click += new System.EventHandler(this.lblPass_Click);
            // 
            // lblresult
            // 
            this.lblresult.AutoSize = true;
            this.lblresult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblresult.Location = new System.Drawing.Point(12, 124);
            this.lblresult.Name = "lblresult";
            this.lblresult.Size = new System.Drawing.Size(11, 15);
            this.lblresult.TabIndex = 2;
            this.lblresult.Text = ".";
            // 
            // lblresult2
            // 
            this.lblresult2.AutoSize = true;
            this.lblresult2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblresult2.Location = new System.Drawing.Point(10, 148);
            this.lblresult2.Name = "lblresult2";
            this.lblresult2.Size = new System.Drawing.Size(15, 15);
            this.lblresult2.TabIndex = 3;
            this.lblresult2.Text = "..";
            // 
            // lblresult3
            // 
            this.lblresult3.AutoSize = true;
            this.lblresult3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblresult3.Location = new System.Drawing.Point(10, 174);
            this.lblresult3.Name = "lblresult3";
            this.lblresult3.Size = new System.Drawing.Size(15, 15);
            this.lblresult3.TabIndex = 4;
            this.lblresult3.Text = "..";
            // 
            // lblresult4
            // 
            this.lblresult4.AutoSize = true;
            this.lblresult4.Location = new System.Drawing.Point(7, 195);
            this.lblresult4.Name = "lblresult4";
            this.lblresult4.Size = new System.Drawing.Size(13, 13);
            this.lblresult4.TabIndex = 5;
            this.lblresult4.Text = "..";
            this.lblresult4.Visible = false;
            // 
            // resultMBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(674, 311);
            this.Controls.Add(this.lblresult4);
            this.Controls.Add(this.lblresult3);
            this.Controls.Add(this.lblresult2);
            this.Controls.Add(this.lblresult);
            this.Controls.Add(this.lblPass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "resultMBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Result";
            this.Load += new System.EventHandler(this.resultMBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Label lblresult;
        private System.Windows.Forms.Label lblresult2;
        private System.Windows.Forms.Label lblresult3;
        private System.Windows.Forms.Label lblresult4;
    }
}