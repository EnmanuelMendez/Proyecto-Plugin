namespace Plugin_ICGFront.Views
{
    partial class RefundAlert
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
            this.components = new System.ComponentModel.Container();
            this.DismissButton = new System.Windows.Forms.Button();
            this.DismissButtonArea = new System.Windows.Forms.Panel();
            this.WaitingText = new System.Windows.Forms.Label();
            this.AlertPictureBox = new System.Windows.Forms.PictureBox();
            this.WarningText = new System.Windows.Forms.Label();
            this.CountdownTimer = new System.Windows.Forms.Timer(this.components);
            this.DismissButtonArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlertPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // DismissButton
            // 
            this.DismissButton.BackColor = System.Drawing.Color.LightGreen;
            this.DismissButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DismissButton.Enabled = false;
            this.DismissButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DismissButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DismissButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DismissButton.Location = new System.Drawing.Point(3, 3);
            this.DismissButton.Name = "DismissButton";
            this.DismissButton.Size = new System.Drawing.Size(209, 62);
            this.DismissButton.TabIndex = 0;
            this.DismissButton.Text = "OK";
            this.DismissButton.UseVisualStyleBackColor = false;
            this.DismissButton.Visible = false;
            this.DismissButton.Click += new System.EventHandler(this.DismissButton_Click);
            // 
            // DismissButtonArea
            // 
            this.DismissButtonArea.BackColor = System.Drawing.Color.PeachPuff;
            this.DismissButtonArea.Controls.Add(this.WaitingText);
            this.DismissButtonArea.Controls.Add(this.DismissButton);
            this.DismissButtonArea.Location = new System.Drawing.Point(0, 410);
            this.DismissButtonArea.Name = "DismissButtonArea";
            this.DismissButtonArea.Size = new System.Drawing.Size(775, 178);
            this.DismissButtonArea.TabIndex = 2;
            // 
            // WaitingText
            // 
            this.WaitingText.BackColor = System.Drawing.Color.Coral;
            this.WaitingText.Font = new System.Drawing.Font("Calibri", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WaitingText.Location = new System.Drawing.Point(0, -3);
            this.WaitingText.Name = "WaitingText";
            this.WaitingText.Size = new System.Drawing.Size(775, 181);
            this.WaitingText.TabIndex = 1;
            this.WaitingText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AlertPictureBox
            // 
            this.AlertPictureBox.BackColor = System.Drawing.Color.Coral;
            this.AlertPictureBox.Image = global::Plugin_ICGFront.Properties.Resources.warning_icon;
            this.AlertPictureBox.Location = new System.Drawing.Point(0, 10);
            this.AlertPictureBox.Name = "AlertPictureBox";
            this.AlertPictureBox.Size = new System.Drawing.Size(775, 229);
            this.AlertPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.AlertPictureBox.TabIndex = 1;
            this.AlertPictureBox.TabStop = false;
            // 
            // WarningText
            // 
            this.WarningText.BackColor = System.Drawing.Color.Coral;
            this.WarningText.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WarningText.Location = new System.Drawing.Point(0, 247);
            this.WarningText.Name = "WarningText";
            this.WarningText.Size = new System.Drawing.Size(775, 165);
            this.WarningText.TabIndex = 0;
            this.WarningText.Text = "La factura fue creada hace mas de 30 dias. Se procedera a descontar el ITBIS.";
            this.WarningText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CountdownTimer
            // 
            this.CountdownTimer.Interval = 1000;
            this.CountdownTimer.Tick += new System.EventHandler(this.CountdownTimer_Tick);
            // 
            // RefundAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Coral;
            this.ClientSize = new System.Drawing.Size(778, 600);
            this.Controls.Add(this.DismissButtonArea);
            this.Controls.Add(this.WarningText);
            this.Controls.Add(this.AlertPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RefundAlert";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.RefundAlert_Load);
            this.DismissButtonArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AlertPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button DismissButton;
        private System.Windows.Forms.PictureBox AlertPictureBox;
        private System.Windows.Forms.Panel DismissButtonArea;
        private System.Windows.Forms.Label WarningText;
        private System.Windows.Forms.Timer CountdownTimer;
        private System.Windows.Forms.Label WaitingText;
    }
}