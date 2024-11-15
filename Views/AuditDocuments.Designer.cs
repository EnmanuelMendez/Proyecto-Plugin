namespace Plugin_ICGFront.Views
{
    partial class AuditDocuments
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
            this.ContinueButton = new System.Windows.Forms.Button();
            this.dgv_items = new System.Windows.Forms.DataGridView();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Numero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DocDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodCliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomCliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DocCur = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Monto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_items)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(734, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "A continuacion, se muestra un listado de documentos sin NCF:";
            // 
            // ContinueButton
            // 
            this.ContinueButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContinueButton.Location = new System.Drawing.Point(809, 281);
            this.ContinueButton.Name = "ContinueButton";
            this.ContinueButton.Size = new System.Drawing.Size(134, 34);
            this.ContinueButton.TabIndex = 2;
            this.ContinueButton.Text = "Continuar";
            this.ContinueButton.UseVisualStyleBackColor = true;
            this.ContinueButton.Click += new System.EventHandler(this.ContinueButton_Click);
            // 
            // dgv_items
            // 
            this.dgv_items.AllowUserToAddRows = false;
            this.dgv_items.AllowUserToDeleteRows = false;
            this.dgv_items.AllowUserToResizeRows = false;
            this.dgv_items.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgv_items.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_items.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv_items.ColumnHeadersHeight = 24;
            this.dgv_items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv_items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemCode,
            this.Numero,
            this.DocDate,
            this.CodCliente,
            this.NomCliente,
            this.DocCur,
            this.Monto});
            this.dgv_items.Enabled = false;
            this.dgv_items.EnableHeadersVisualStyles = false;
            this.dgv_items.Location = new System.Drawing.Point(12, 41);
            this.dgv_items.MultiSelect = false;
            this.dgv_items.Name = "dgv_items";
            this.dgv_items.ReadOnly = true;
            this.dgv_items.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_items.ShowEditingIcon = false;
            this.dgv_items.Size = new System.Drawing.Size(931, 227);
            this.dgv_items.TabIndex = 3;
            // 
            // ItemCode
            // 
            this.ItemCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ItemCode.DataPropertyName = "Series";
            this.ItemCode.Frozen = true;
            this.ItemCode.HeaderText = "Serie";
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.ReadOnly = true;
            this.ItemCode.Width = 56;
            // 
            // Numero
            // 
            this.Numero.DataPropertyName = "Number";
            this.Numero.Frozen = true;
            this.Numero.HeaderText = "Numero";
            this.Numero.Name = "Numero";
            this.Numero.ReadOnly = true;
            // 
            // DocDate
            // 
            this.DocDate.DataPropertyName = "DocDate";
            this.DocDate.Frozen = true;
            this.DocDate.HeaderText = "Fecha";
            this.DocDate.Name = "DocDate";
            this.DocDate.ReadOnly = true;
            // 
            // CodCliente
            // 
            this.CodCliente.DataPropertyName = "CardCode";
            this.CodCliente.HeaderText = "Codigo cliente";
            this.CodCliente.Name = "CodCliente";
            this.CodCliente.ReadOnly = true;
            // 
            // NomCliente
            // 
            this.NomCliente.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NomCliente.DataPropertyName = "CardName";
            this.NomCliente.HeaderText = "Nombre cliente";
            this.NomCliente.Name = "NomCliente";
            this.NomCliente.ReadOnly = true;
            // 
            // DocCur
            // 
            this.DocCur.DataPropertyName = "DocCur";
            this.DocCur.HeaderText = "Moneda";
            this.DocCur.Name = "DocCur";
            this.DocCur.ReadOnly = true;
            // 
            // Monto
            // 
            this.Monto.DataPropertyName = "DocTotal";
            this.Monto.HeaderText = "Monto";
            this.Monto.Name = "Monto";
            this.Monto.ReadOnly = true;
            // 
            // AuditDocuments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(955, 327);
            this.Controls.Add(this.dgv_items);
            this.Controls.Add(this.ContinueButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 525);
            this.MinimizeBox = false;
            this.Name = "AuditDocuments";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Listado de documentos sin NCF";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AuditDocuments_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_items)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ContinueButton;
        private System.Windows.Forms.DataGridView dgv_items;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Numero;
        private System.Windows.Forms.DataGridViewTextBoxColumn DocDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomCliente;
        private System.Windows.Forms.DataGridViewTextBoxColumn DocCur;
        private System.Windows.Forms.DataGridViewTextBoxColumn Monto;
    }
}