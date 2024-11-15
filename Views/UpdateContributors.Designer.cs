namespace Plugin_ICGFront.Views
{
    partial class UpdateContributors
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
            this.TextFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SelectFile = new System.Windows.Forms.Button();
            this.SelectedFilePath = new System.Windows.Forms.TextBox();
            this.InstructionsLabel = new System.Windows.Forms.Label();
            this.DowloadFileLink = new System.Windows.Forms.LinkLabel();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextFileDialog
            // 
            this.TextFileDialog.FileName = "DGII_RNC.TXT";
            this.TextFileDialog.Filter = "Archivos de texto|*.txt|Todos los archivos|*.*";
            this.TextFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.TextFileDialog_FileOk);
            // 
            // SelectFile
            // 
            this.SelectFile.Location = new System.Drawing.Point(544, 63);
            this.SelectFile.Name = "SelectFile";
            this.SelectFile.Size = new System.Drawing.Size(127, 23);
            this.SelectFile.TabIndex = 0;
            this.SelectFile.Text = "Seleccionar archivo...";
            this.SelectFile.UseVisualStyleBackColor = true;
            this.SelectFile.Click += new System.EventHandler(this.SelectFile_Click);
            // 
            // SelectedFilePath
            // 
            this.SelectedFilePath.Location = new System.Drawing.Point(12, 65);
            this.SelectedFilePath.Name = "SelectedFilePath";
            this.SelectedFilePath.ReadOnly = true;
            this.SelectedFilePath.Size = new System.Drawing.Size(526, 20);
            this.SelectedFilePath.TabIndex = 1;
            // 
            // InstructionsLabel
            // 
            this.InstructionsLabel.AutoSize = true;
            this.InstructionsLabel.Location = new System.Drawing.Point(9, 36);
            this.InstructionsLabel.Name = "InstructionsLabel";
            this.InstructionsLabel.Size = new System.Drawing.Size(569, 13);
            this.InstructionsLabel.TabIndex = 2;
            this.InstructionsLabel.Text = "A continuacion, seleccione el archivo de texto con el listado actualizado de RNC " +
    "(Registro Nacional del Contribuyente).";
            // 
            // DowloadFileLink
            // 
            this.DowloadFileLink.AutoSize = true;
            this.DowloadFileLink.Location = new System.Drawing.Point(9, 9);
            this.DowloadFileLink.Name = "DowloadFileLink";
            this.DowloadFileLink.Size = new System.Drawing.Size(164, 13);
            this.DowloadFileLink.TabIndex = 3;
            this.DowloadFileLink.TabStop = true;
            this.DowloadFileLink.Text = "Descargar archivo DGII_RNC.zip";
            this.DowloadFileLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DowloadFileLink_LinkClicked);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripProgressBar,
            this.ToolStripStatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 106);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(684, 22);
            this.StatusStrip.TabIndex = 4;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(240, 16);
            // 
            // ToolStripStatusLabel
            // 
            this.ToolStripStatusLabel.Name = "ToolStripStatusLabel";
            this.ToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // UpdateContributors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 128);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.DowloadFileLink);
            this.Controls.Add(this.InstructionsLabel);
            this.Controls.Add(this.SelectedFilePath);
            this.Controls.Add(this.SelectFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(700, 167);
            this.MinimumSize = new System.Drawing.Size(700, 139);
            this.Name = "UpdateContributors";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Actualizar base de datos de contribuyentes";
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog TextFileDialog;
        private System.Windows.Forms.Button SelectFile;
        private System.Windows.Forms.TextBox SelectedFilePath;
        private System.Windows.Forms.Label InstructionsLabel;
        private System.Windows.Forms.LinkLabel DowloadFileLink;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel;
    }
}