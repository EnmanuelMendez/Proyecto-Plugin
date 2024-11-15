namespace Plugin_ICGFront
{
    partial class Main
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
            this.loadingText = new System.Windows.Forms.Label();
            this.PrintDialog = new System.Windows.Forms.PrintDialog();
            this.loadingSpinner = new System.Windows.Forms.PictureBox();
            this.LoadingPanel = new System.Windows.Forms.Panel();
            this.MessagePanel = new System.Windows.Forms.Panel();
            this.EasterEggButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.loadingSpinner)).BeginInit();
            this.LoadingPanel.SuspendLayout();
            this.MessagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadingText
            // 
            this.loadingText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingText.Location = new System.Drawing.Point(3, 6);
            this.loadingText.Name = "loadingText";
            this.loadingText.Size = new System.Drawing.Size(811, 84);
            this.loadingText.TabIndex = 0;
            this.loadingText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.loadingText.Click += new System.EventHandler(this.loadingText_Click);
            // 
            // PrintDialog
            // 
            this.PrintDialog.AllowPrintToFile = false;
            this.PrintDialog.AllowSelection = true;
            // 
            // loadingSpinner
            // 
            this.loadingSpinner.Image = global::Plugin_ICGFront.Properties.Resources.loading;
            this.loadingSpinner.Location = new System.Drawing.Point(358, 7);
            this.loadingSpinner.Name = "loadingSpinner";
            this.loadingSpinner.Size = new System.Drawing.Size(107, 111);
            this.loadingSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.loadingSpinner.TabIndex = 1;
            this.loadingSpinner.TabStop = false;
            // 
            // LoadingPanel
            // 
            this.LoadingPanel.Controls.Add(this.loadingSpinner);
            this.LoadingPanel.Location = new System.Drawing.Point(12, 12);
            this.LoadingPanel.Name = "LoadingPanel";
            this.LoadingPanel.Size = new System.Drawing.Size(817, 124);
            this.LoadingPanel.TabIndex = 2;
            // 
            // MessagePanel
            // 
            this.MessagePanel.Controls.Add(this.EasterEggButton);
            this.MessagePanel.Controls.Add(this.loadingText);
            this.MessagePanel.Location = new System.Drawing.Point(12, 142);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Size = new System.Drawing.Size(817, 96);
            this.MessagePanel.TabIndex = 3;
            // 
            // EasterEggButton
            // 
            this.EasterEggButton.BackColor = System.Drawing.Color.White;
            this.EasterEggButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.EasterEggButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EasterEggButton.ForeColor = System.Drawing.Color.White;
            this.EasterEggButton.Location = new System.Drawing.Point(799, 78);
            this.EasterEggButton.Name = "EasterEggButton";
            this.EasterEggButton.Size = new System.Drawing.Size(15, 15);
            this.EasterEggButton.TabIndex = 1;
            this.EasterEggButton.UseVisualStyleBackColor = false;
            this.EasterEggButton.Click += new System.EventHandler(this.EasterEggButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(841, 249);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.LoadingPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.loadingSpinner)).EndInit();
            this.LoadingPanel.ResumeLayout(false);
            this.MessagePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label loadingText;
        private System.Windows.Forms.PrintDialog PrintDialog;
        private System.Windows.Forms.PictureBox loadingSpinner;
        private System.Windows.Forms.Panel LoadingPanel;
        private System.Windows.Forms.Panel MessagePanel;
        private System.Windows.Forms.Button EasterEggButton;
    }
}

