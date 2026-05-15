namespace Plugin_ICGFront.Views
{
    partial class ChangeRateCreditMemo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeRateCreditMemo));
            this.txt_tasa_sistema = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_tasa_factura = new System.Windows.Forms.TextBox();
            this.btn_cambiar_tasa = new System.Windows.Forms.Button();
            this.btn_cancelar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lb_pregunta = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_tasa_sistema
            // 
            this.txt_tasa_sistema.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_tasa_sistema.Location = new System.Drawing.Point(117, 7);
            this.txt_tasa_sistema.Name = "txt_tasa_sistema";
            this.txt_tasa_sistema.ReadOnly = true;
            this.txt_tasa_sistema.Size = new System.Drawing.Size(66, 22);
            this.txt_tasa_sistema.TabIndex = 0;
            this.txt_tasa_sistema.TextChanged += new System.EventHandler(this.txt_serie_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tasa del sistema:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(187, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tasa Factura:";
            // 
            // txt_tasa_factura
            // 
            this.txt_tasa_factura.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_tasa_factura.Location = new System.Drawing.Point(278, 8);
            this.txt_tasa_factura.Name = "txt_tasa_factura";
            this.txt_tasa_factura.ReadOnly = true;
            this.txt_tasa_factura.Size = new System.Drawing.Size(67, 22);
            this.txt_tasa_factura.TabIndex = 3;
            // 
            // btn_cambiar_tasa
            // 
            this.btn_cambiar_tasa.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_cambiar_tasa.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cambiar_tasa.Location = new System.Drawing.Point(178, 142);
            this.btn_cambiar_tasa.Name = "btn_cambiar_tasa";
            this.btn_cambiar_tasa.Size = new System.Drawing.Size(167, 23);
            this.btn_cambiar_tasa.TabIndex = 4;
            this.btn_cambiar_tasa.Text = "Cambiar Tasa";
            this.btn_cambiar_tasa.UseVisualStyleBackColor = true;
            this.btn_cambiar_tasa.Click += new System.EventHandler(this.btn_cambiar_tasa_Click);
            // 
            // btn_cancelar
            // 
            this.btn_cancelar.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_cancelar.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cancelar.Location = new System.Drawing.Point(5, 142);
            this.btn_cancelar.Name = "btn_cancelar";
            this.btn_cancelar.Size = new System.Drawing.Size(167, 23);
            this.btn_cancelar.TabIndex = 5;
            this.btn_cancelar.Text = "Cancelar";
            this.btn_cancelar.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(336, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "Existe una diferencia entre la tasa del sistema";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(79, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 14);
            this.label4.TabIndex = 8;
            this.label4.Text = "y la tasa del documento";
            // 
            // lb_pregunta
            // 
            this.lb_pregunta.AutoSize = true;
            this.lb_pregunta.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_pregunta.ForeColor = System.Drawing.Color.Red;
            this.lb_pregunta.Location = new System.Drawing.Point(12, 90);
            this.lb_pregunta.Name = "lb_pregunta";
            this.lb_pregunta.Size = new System.Drawing.Size(0, 15);
            this.lb_pregunta.TabIndex = 9;
            // 
            // ChangeRateCreditMemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 168);
            this.Controls.Add(this.lb_pregunta);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_cancelar);
            this.Controls.Add(this.btn_cambiar_tasa);
            this.Controls.Add(this.txt_tasa_factura);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_tasa_sistema);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangeRateCreditMemo";
            this.Text = "Advertencia";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_tasa_sistema;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_tasa_factura;
        private System.Windows.Forms.Button btn_cambiar_tasa;
        private System.Windows.Forms.Button btn_cancelar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_pregunta;
    }
}