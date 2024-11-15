namespace Plugin_ICGFront.Views
{
    partial class CustomClient
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
            this.rncTxt = new System.Windows.Forms.TextBox();
            this.searchBtn = new System.Windows.Forms.Button();
            this.cdNameTxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cdStatusTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.paymentSchemeCb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.infoPane = new System.Windows.Forms.Panel();
            this.cdRncTxt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.continueBtn = new System.Windows.Forms.Button();
            this.Cancel_Btn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.infoPane.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "RNC / Cedula:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rncTxt
            // 
            this.rncTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.rncTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rncTxt.Location = new System.Drawing.Point(224, 33);
            this.rncTxt.Name = "rncTxt";
            this.rncTxt.Size = new System.Drawing.Size(586, 42);
            this.rncTxt.TabIndex = 1;
            this.rncTxt.TextChanged += new System.EventHandler(this.RNC_Txt_TextChanged);
            this.rncTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RNC_Txt_KeyPress);
            this.rncTxt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RNC_Txt_KeyUp);
            // 
            // searchBtn
            // 
            this.searchBtn.BackColor = System.Drawing.Color.PeachPuff;
            this.searchBtn.Enabled = false;
            this.searchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBtn.Location = new System.Drawing.Point(816, 33);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(84, 43);
            this.searchBtn.TabIndex = 2;
            this.searchBtn.Text = "Buscar";
            this.searchBtn.UseVisualStyleBackColor = false;
            this.searchBtn.Click += new System.EventHandler(this.Search_Btn_Click);
            // 
            // cdNameTxt
            // 
            this.cdNameTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.cdNameTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cdNameTxt.Location = new System.Drawing.Point(258, 88);
            this.cdNameTxt.Name = "cdNameTxt";
            this.cdNameTxt.ReadOnly = true;
            this.cdNameTxt.Size = new System.Drawing.Size(615, 42);
            this.cdNameTxt.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 29);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nombre:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cdStatusTxt
            // 
            this.cdStatusTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.cdStatusTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cdStatusTxt.Location = new System.Drawing.Point(258, 137);
            this.cdStatusTxt.Name = "cdStatusTxt";
            this.cdStatusTxt.ReadOnly = true;
            this.cdStatusTxt.Size = new System.Drawing.Size(615, 42);
            this.cdStatusTxt.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(236, 29);
            this.label3.TabIndex = 5;
            this.label3.Text = "Estado:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // paymentSchemeCb
            // 
            this.paymentSchemeCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paymentSchemeCb.Font = new System.Drawing.Font("Microsoft Sans Serif", 23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paymentSchemeCb.FormattingEnabled = true;
            this.paymentSchemeCb.Location = new System.Drawing.Point(258, 212);
            this.paymentSchemeCb.Name = "paymentSchemeCb";
            this.paymentSchemeCb.Size = new System.Drawing.Size(615, 43);
            this.paymentSchemeCb.TabIndex = 7;
            this.paymentSchemeCb.SelectedIndexChanged += new System.EventHandler(this.PaymentScheme_Cb_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 219);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(241, 29);
            this.label4.TabIndex = 8;
            this.label4.Text = "Tipo comprobante:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // infoPane
            // 
            this.infoPane.Controls.Add(this.cdRncTxt);
            this.infoPane.Controls.Add(this.label5);
            this.infoPane.Controls.Add(this.label4);
            this.infoPane.Controls.Add(this.cdNameTxt);
            this.infoPane.Controls.Add(this.paymentSchemeCb);
            this.infoPane.Controls.Add(this.label2);
            this.infoPane.Controls.Add(this.cdStatusTxt);
            this.infoPane.Controls.Add(this.label3);
            this.infoPane.Controls.Add(this.groupBox1);
            this.infoPane.Location = new System.Drawing.Point(12, 111);
            this.infoPane.Name = "infoPane";
            this.infoPane.Size = new System.Drawing.Size(891, 277);
            this.infoPane.TabIndex = 9;
            this.infoPane.Visible = false;
            // 
            // cdRncTxt
            // 
            this.cdRncTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.cdRncTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cdRncTxt.Location = new System.Drawing.Point(258, 39);
            this.cdRncTxt.Name = "cdRncTxt";
            this.cdRncTxt.ReadOnly = true;
            this.cdRncTxt.Size = new System.Drawing.Size(615, 42);
            this.cdRncTxt.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(236, 29);
            this.label5.TabIndex = 9;
            this.label5.Text = "RNC / Cedula:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // continueBtn
            // 
            this.continueBtn.BackColor = System.Drawing.Color.PaleGreen;
            this.continueBtn.Enabled = false;
            this.continueBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.continueBtn.Location = new System.Drawing.Point(764, 427);
            this.continueBtn.Name = "continueBtn";
            this.continueBtn.Size = new System.Drawing.Size(139, 43);
            this.continueBtn.TabIndex = 10;
            this.continueBtn.Text = "Continuar";
            this.continueBtn.UseVisualStyleBackColor = false;
            this.continueBtn.Click += new System.EventHandler(this.Continue_Btn_Click);
            // 
            // Cancel_Btn
            // 
            this.Cancel_Btn.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Cancel_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel_Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel_Btn.Location = new System.Drawing.Point(12, 427);
            this.Cancel_Btn.Name = "Cancel_Btn";
            this.Cancel_Btn.Size = new System.Drawing.Size(139, 43);
            this.Cancel_Btn.TabIndex = 11;
            this.Cancel_Btn.Text = "Cancelar";
            this.Cancel_Btn.UseVisualStyleBackColor = false;
            this.Cancel_Btn.Click += new System.EventHandler(this.Cancel_Btn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(883, 268);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resultado";
            // 
            // CustomClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(915, 482);
            this.Controls.Add(this.Cancel_Btn);
            this.Controls.Add(this.continueBtn);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.rncTxt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.infoPane);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomClient";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CustomClient_Load);
            this.infoPane.ResumeLayout(false);
            this.infoPane.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox rncTxt;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.TextBox cdNameTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox cdStatusTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox paymentSchemeCb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel infoPane;
        private System.Windows.Forms.Button continueBtn;
        private System.Windows.Forms.Button Cancel_Btn;
        private System.Windows.Forms.TextBox cdRncTxt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}