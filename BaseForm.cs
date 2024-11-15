using System.Windows.Forms;

namespace Plugin_ICGFront
{
    public class BaseForm : Form
    {
        protected bool AppClosing = false;

        protected void OnFormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    e.Cancel = AppClosing;
                    break;
                case CloseReason.None:
                case CloseReason.TaskManagerClosing:
                    e.Cancel = true;
                    break;
            }
            base.OnFormClosing(e);
#else
            base.OnFormClosing(e);
#endif
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "BaseForm";
            this.ResumeLayout(false);

        }
    }
}