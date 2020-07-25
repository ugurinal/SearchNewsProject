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

        public void setTitle(string title)
        {
            labelTitle.Text = title;
        }

        public void setLink(string link)
        {
            this.link = link;
        }

        public void setContent(string content)
        {
            labelContent.Text = content;
        }

        public void setImage(string link)
        {
            pictureBox1.ImageLocation = link;
        }

        public void setAuthor(string aurhor)
        {
            labelAuthor.Text = aurhor;
        }

        public void setDate(string date)
        {
            labelDate.Text = date;
        }

        private void labelTitle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(link);
            labelTitle.LinkVisited = true;
        }
    }
}