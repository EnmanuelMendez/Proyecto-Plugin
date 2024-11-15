namespace Plugin_ICGFront.Views
{
    partial class DiscountCalculator
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
            this.label1 = new System.Windows.Forms.Label();
            this.nupDefaultPrice = new System.Windows.Forms.NumericUpDown();
            this.nupNewPrice = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nupDiscount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nupDefaultPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupNewPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupDiscount)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Precio por defecto:";
            // 
            // nupDefaultPrice
            // 
            this.nupDefaultPrice.DecimalPlaces = 2;
            this.nupDefaultPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nupDefaultPrice.Location = new System.Drawing.Point(236, 13);
            this.nupDefaultPrice.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nupDefaultPrice.Name = "nupDefaultPrice";
            this.nupDefaultPrice.Size = new System.Drawing.Size(253, 35);
            this.nupDefaultPrice.TabIndex = 1;
            this.nupDefaultPrice.ThousandsSeparator = true;
            this.nupDefaultPrice.ValueChanged += new System.EventHandler(this.NupDefaultPrice_ValueChanged);
            // 
            // nupNewPrice
            // 
            this.nupNewPrice.DecimalPlaces = 2;
            this.nupNewPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nupNewPrice.Location = new System.Drawing.Point(236, 61);
            this.nupNewPrice.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nupNewPrice.Name = "nupNewPrice";
            this.nupNewPrice.Size = new System.Drawing.Size(253, 35);
            this.nupNewPrice.TabIndex = 3;
            this.nupNewPrice.ThousandsSeparator = true;
            this.nupNewPrice.ValueChanged += new System.EventHandler(this.NupNewPrice_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "Precio nuevo:";
            // 
            // nupDiscount
            // 
            this.nupDiscount.BackColor = System.Drawing.Color.Bisque;
            this.nupDiscount.DecimalPlaces = 8;
            this.nupDiscount.Enabled = false;
            this.nupDiscount.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nupDiscount.InterceptArrowKeys = false;
            this.nupDiscount.Location = new System.Drawing.Point(236, 111);
            this.nupDiscount.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nupDiscount.Name = "nupDiscount";
            this.nupDiscount.ReadOnly = true;
            this.nupDiscount.Size = new System.Drawing.Size(212, 35);
            this.nupDiscount.TabIndex = 5;
            this.nupDiscount.ThousandsSeparator = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 29);
            this.label3.TabIndex = 4;
            this.label3.Text = "Descuento:";
            // 
            // btnCopy
            // 
            this.btnCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Location = new System.Drawing.Point(454, 111);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(35, 35);
            this.btnCopy.TabIndex = 6;
            this.btnCopy.Text = "C";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // DiscountCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 161);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.nupDiscount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nupNewPrice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nupDefaultPrice);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(517, 200);
            this.MinimumSize = new System.Drawing.Size(517, 200);
            this.Name = "DiscountCalculator";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calculadora de descuentos";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nupDefaultPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupNewPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupDiscount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nupDefaultPrice;
        private System.Windows.Forms.NumericUpDown nupNewPrice;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nupDiscount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCopy;
    }
}