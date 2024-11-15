namespace Plugin_ICGFront.Views
{
    partial class PasteSerialNumbers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasteSerialNumbers));
            this.txtSerialNumbers = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPasteSN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtSerialNumbers
            // 
            this.txtSerialNumbers.Location = new System.Drawing.Point(13, 95);
            this.txtSerialNumbers.Multiline = true;
            this.txtSerialNumbers.Name = "txtSerialNumbers";
            this.txtSerialNumbers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSerialNumbers.Size = new System.Drawing.Size(572, 343);
            this.txtSerialNumbers.TabIndex = 0;
            this.txtSerialNumbers.TextChanged += new System.EventHandler(this.TxtSerialNumbers_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(572, 70);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnPasteSN
            // 
            this.btnPasteSN.Location = new System.Drawing.Point(451, 445);
            this.btnPasteSN.Name = "btnPasteSN";
            this.btnPasteSN.Size = new System.Drawing.Size(134, 34);
            this.btnPasteSN.TabIndex = 2;
            this.btnPasteSN.Text = "Pegar numeros de serie";
            this.btnPasteSN.UseVisualStyleBackColor = true;
            this.btnPasteSN.Click += new System.EventHandler(this.BtnPasteSN_Click);
            // 
            // PasteSerialNumbers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 486);
            this.Controls.Add(this.btnPasteSN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSerialNumbers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "PasteSerialNumbers";
            this.ShowIcon = false;
            this.Text = "Pegar numeros de series";
            this.Load += new System.EventHandler(this.PasteSerialNumbers_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSerialNumbers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPasteSN;
    }
}