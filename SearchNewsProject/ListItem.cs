using System.Windows.Forms;

namespace SearchNewsProject
{
    public partial class ListItem : UserControl
    {
        public ListItem()
        {
            InitializeComponent();
        }

        private string link;    // link for title

        public string Link { get => link; set => link = value; }
        public string Content { get => labelContent.Text; set => labelContent.Text = value; }
        public string Image { get => pictureBox1.ImageLocation; set => pictureBox1.ImageLocation = value; }
        public string Author { get => labelAuthor.Text; set => labelAuthor.Text = value; }
        public string Date { get => labelDate.Text; set => labelDate.Text = value; }
        public string Title { get => labelTitle.Text; set => labelTitle.Text = value; }

        private void labelTitle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(link);
            labelTitle.LinkVisited = true;
        }
    }
}