using MaterialSkin.Controls;
using System.Windows.Forms;

namespace SearchNewsProject
{
    public partial class progressBarForm : MaterialForm
    {
        public progressBarForm()
        {
            InitializeComponent();
        }

        public void updateProgressBar(int value)
        {
            Invoke((MethodInvoker)delegate
            {
                progressBar1.Value = value;
            });
        }

        private void progressBarForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            Parent = null;

            Invoke((MethodInvoker)delegate
            {
                progressBar1.Value = 0;
            });
        }

        public void closeForm()
        {
            Close();
        }

        private void materialButton1_Click_1(object sender, System.EventArgs e)
        {
            closeForm();
        }
    }
}