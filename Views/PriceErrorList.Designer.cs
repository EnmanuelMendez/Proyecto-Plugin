namespace Plugin_ICGFront.Views
{
    partial class PriceErrorList
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
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErroredPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SuggestedPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CopyPrice = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(734, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "A continuacion, se muestra un listado de articulos con errores en los precios:";
            // 
            // ContinueButton
            // 
            this.ContinueButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContinueButton.Location = new System.Drawing.Point(606, 281);
            this.ContinueButton.Name = "ContinueButton";
            this.ContinueButton.Size = new System.Drawing.Size(134, 34);
            this.ContinueButton.TabIndex = 2;
            this.ContinueButton.Text = "Continuar";
            this.ContinueButton.UseVisualStyleBackColor = true;
            this.ContinueButton.Click += new System.EventHandler(this.ContinueButton_Click);
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvItems.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvItems.ColumnHeadersHeight = 24;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemCode,
            this.Description,
            this.ErroredPrice,
            this.SuggestedPrice,
            this.CopyPrice});
            this.dgvItems.Enabled = false;
            this.dgvItems.EnableHeadersVisualStyles = false;
            this.dgvItems.Location = new System.Drawing.Point(12, 41);
            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.ShowEditingIcon = false;
            this.dgvItems.Size = new System.Drawing.Size(728, 227);
            this.dgvItems.TabIndex = 3;
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Dgv_items_CellContentClick);
            // 
            // ItemCode
            // 
            this.ItemCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ItemCode.DataPropertyName = "ItemCode";
            this.ItemCode.Frozen = true;
            this.ItemCode.HeaderText = "Codigo articulo";
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.ReadOnly = true;
            this.ItemCode.Width = 102;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Description.DataPropertyName = "Description";
            this.Description.Frozen = true;
            this.Description.HeaderText = "Descripcion";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 300;
            // 
            // ErroredPrice
            // 
            this.ErroredPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ErroredPrice.DataPropertyName = "ErroredPrice";
            this.ErroredPrice.Frozen = true;
            this.ErroredPrice.HeaderText = "Precio Erroneo";
            this.ErroredPrice.Name = "ErroredPrice";
            this.ErroredPrice.ReadOnly = true;
            this.ErroredPrice.Width = 102;
            // 
            // SuggestedPrice
            // 
            this.SuggestedPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.SuggestedPrice.DataPropertyName = "SuggestedPrice";
            this.SuggestedPrice.Frozen = true;
            this.SuggestedPrice.HeaderText = "Precio Sugerido";
            this.SuggestedPrice.Name = "SuggestedPrice";
            this.SuggestedPrice.ReadOnly = true;
            this.SuggestedPrice.Width = 107;
            // 
            // CopyPrice
            // 
            this.CopyPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.CopyPrice.FillWeight = 50F;
            this.CopyPrice.HeaderText = "";
            this.CopyPrice.MinimumWidth = 25;
            this.CopyPrice.Name = "CopyPrice";
            this.CopyPrice.ReadOnly = true;
            this.CopyPrice.Text = "Copiar";
            this.CopyPrice.UseColumnTextForButtonValue = true;
            this.CopyPrice.Width = 25;
            // 
            // PriceErrorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(752, 327);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.ContinueButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 525);
            this.MinimizeBox = false;
            this.Name = "PriceErrorList";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Listado de articulos con errores";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PriceErrorList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ContinueButton;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErroredPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn SuggestedPrice;
        private System.Windows.Forms.DataGridViewButtonColumn CopyPrice;
    }
}