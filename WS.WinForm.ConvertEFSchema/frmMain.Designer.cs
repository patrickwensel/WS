namespace WS.WinForm.ConvertEFSchema
{
    partial class frmMain
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
            this.txtEDMXPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConvertToDev = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtEDMXPath
            // 
            this.txtEDMXPath.Location = new System.Drawing.Point(12, 51);
            this.txtEDMXPath.Name = "txtEDMXPath";
            this.txtEDMXPath.Size = new System.Drawing.Size(672, 20);
            this.txtEDMXPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Location of EDMX file";
            // 
            // btnConvertToDev
            // 
            this.btnConvertToDev.Location = new System.Drawing.Point(12, 93);
            this.btnConvertToDev.Name = "btnConvertToDev";
            this.btnConvertToDev.Size = new System.Drawing.Size(199, 23);
            this.btnConvertToDev.TabIndex = 2;
            this.btnConvertToDev.Text = "Convert to Dev";
            this.btnConvertToDev.UseVisualStyleBackColor = true;
            this.btnConvertToDev.Click += new System.EventHandler(this.btnConvertToDev_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 428);
            this.Controls.Add(this.btnConvertToDev);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEDMXPath);
            this.Name = "frmMain";
            this.Text = "Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEDMXPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConvertToDev;
    }
}

