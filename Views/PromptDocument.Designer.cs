namespace Plugin_ICGFront.Views
{
    partial class PromptDocument
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
            this.ContinueButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DocSeries = new System.Windows.Forms.TextBox();
            this.DocNumber = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.DocNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // ContinueButton
            // 
            this.ContinueButton.Enabled = false;
            this.ContinueButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContinueButton.Location = new System.Drawing.Point(107, 75);
            this.ContinueButton.Name = "ContinueButton";
            this.ContinueButton.Size = new System.Drawing.Size(89, 28);
            this.ContinueButton.TabIndex = 2;
            this.ContinueButton.Text = "Aceptar";
            this.ContinueButton.UseVisualStyleBackColor = true;
            this.ContinueButton.Click += new System.EventHandler(this.ContinueButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Serie:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Número:";
            // 
            // DocSeries
            // 
            this.DocSeries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DocSeries.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.DocSeries.Location = new System.Drawing.Point(69, 18);
            this.DocSeries.Name = "DocSeries";
            this.DocSeries.Size = new System.Drawing.Size(127, 20);
            this.DocSeries.TabIndex = 5;
            this.DocSeries.TextChanged += new System.EventHandler(this.SeriesText_TextChanged);
            this.DocSeries.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DocSeries_KeyPress);
            // 
            // DocNumber
            // 
            this.DocNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DocNumber.Location = new System.Drawing.Point(69, 45);
            this.DocNumber.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.DocNumber.Name = "DocNumber";
            this.DocNumber.Size = new System.Drawing.Size(127, 20);
            this.DocNumber.TabIndex = 6;
            this.DocNumber.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.DocNumber.ValueChanged += new System.EventHandler(this.DocNumber_ValueChanged);
            this.DocNumber.Click += new System.EventHandler(this.DocNumber_Click);
            this.DocNumber.Enter += new System.EventHandler(this.DocNumber_Enter);
            this.DocNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DocNumber_KeyDown);
            this.DocNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DocNumber_KeyUp);
            // 
            // PromptDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(208, 115);
            this.Controls.Add(this.DocNumber);
            this.Controls.Add(this.DocSeries);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ContinueButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 525);
            this.MinimizeBox = false;
            this.Name = "PromptDocument";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serie y número del documento";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PromptDocument_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DocNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ContinueButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DocSeries;
        private System.Windows.Forms.NumericUpDown DocNumber;
    }
}