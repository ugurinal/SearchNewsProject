using MaterialSkin;
using MaterialSkin.Controls;
using NewsAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Windows.Forms;
using System.Xml;

namespace SearchNewsProject
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();
            setSkin();
            setDateTimePickers();
            setButton();
            setComboBoxes();
        }

        public void setSkin()
        {
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal800,
                Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
        }

        public void setDateTimePickers()
        {
            fromDateTimePicker.CustomFormat = "dd.MM.yyyy - dddd";
            fromDateTimePicker.Format = DateTimePickerFormat.Custom;
            toDateTimePicker.CustomFormat = "dd.MM.yyyy - dddd";
            toDateTimePicker.Format = DateTimePickerFormat.Custom;

            DateTimePicker dtp = new DateTimePicker();
            dtp.Font = new Font("Arial", 14, FontStyle.Regular);
            fromDateTimePicker.Font = dtp.Font;
            toDateTimePicker.Font = dtp.Font;

            Size size = new Size(260, 35);

            fromDateTimePicker.Size = size;
            toDateTimePicker.Size = size;

            TimeSpan month = new System.TimeSpan(29, 0, 0, 0);
            fromDateTimePicker.MinDate = DateTime.Today.Subtract(month);
            fromDateTimePicker.MaxDate = DateTime.Today;
            toDateTimePicker.MinDate = DateTime.Today.Subtract(month);
            toDateTimePicker.MaxDate = DateTime.Today;
        }

        public void setButton()
        {
            Font font = new Font("Microsoft Sans Serif", 18, FontStyle.Regular);
            xuiButton1.Font = font;
        }

        public void setComboBoxes()
        {
            sourceComboBox.Items.Add("Bing API");
            sourceComboBox.Items.Add("News API");
            sourceComboBox.Items.Add("Custom");
            sourceComboBox.SelectedIndex = 1;

            languageComboBox.Items.Add("Turkish");
            languageComboBox.Items.Add("English");
            languageComboBox.Items.Add("German");
            languageComboBox.Items.Add("Russian");
            languageComboBox.SelectedIndex = 0;

            sortByComboBox.SelectedIndex = 0;
        }

        private void xuiButton1_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sourceComboBox.SelectedIndex == 0)
            {
                searchByBingAPI();
            }
            else if (sourceComboBox.SelectedIndex == 1)
            {
                searchByNewsAPI();
            }
            else
            {
                getCustomNews2();
                //getCustomNews();
            }
        }

        private void searchByNewsAPI()
        {
            if (String.IsNullOrWhiteSpace(keywordTextBox.Text) || String.IsNullOrEmpty(searchSizeTextBox.Text))
            {
                MessageBox.Show("Search size or keywords fields can not be empty.");
            }
            else if (Convert.ToInt32(searchSizeTextBox.Text) < 0)
            {
                MessageBox.Show("Search size can not be lower than 0.");
            }
            else
            {
                string keyWords = keywordTextBox.Text;
                int searchSize = Convert.ToInt32(searchSizeTextBox.Text);
                int language = languageComboBox.SelectedIndex;
                DateTime from = fromDateTimePicker.Value;
                DateTime to = toDateTimePicker.Value;
                int sortBy = sortByComboBox.SelectedIndex;

                News news = new News();
                news.setEverythingRequest(keyWords, language, from, to, searchSize, sortBy);

                news.searchNews();

                if (news.getStatus().Equals("Ok"))
                {
                    List<Article> articles = news.getArticles();
                    List<ListItem> listItems = new List<ListItem>();

                    int counter = 0;

                    foreach (var newsResult in articles)
                    {
                        if (counter >= searchSize)
                        {
                            break;
                        }

                        ListItem temp = new ListItem();
                        temp.setTitle(newsResult.Title);
                        temp.setContent(newsResult.Description);
                        temp.setAuthor(newsResult.Author);
                        temp.setImage(newsResult.UrlToImage);
                        temp.setLink(newsResult.Url);
                        temp.setDate(newsResult.PublishedAt.Value.ToString("dd/MM/yyyy HH:mm"));
                        listItems.Add(temp);
                        counter++;
                    }

                    for (int i = 0; i < listItems.Count; i++)
                    {
                        populateList(listItems, i);
                    }
                }
                else
                {
                    MessageBox.Show("Status : " + news.getStatus());
                }
            }
        }

        private void searchByBingAPI()
        {
            if (String.IsNullOrWhiteSpace(keywordTextBox.Text) || String.IsNullOrWhiteSpace(searchSizeTextBox.Text))
            {
                MessageBox.Show("Search size or keywords fields can not be empty.");
            }
            else if (Convert.ToInt32(searchSizeTextBox.Text) < 0)
            {
                MessageBox.Show("Search size can not be lower or equal to 0.");
            }
            else
            {
                string keyWords = keywordTextBox.Text;
                int language = languageComboBox.SelectedIndex;
                int searchSize = Convert.ToInt32(searchSizeTextBox.Text);
                int sortBy = sortByComboBox.SelectedIndex;

                BingNews bingNews = new BingNews();
                bingNews.setSearchQuery(keyWords, language, searchSize, sortBy);

                dynamic jsonObj = bingNews.getBingNews();

                List<ListItem> listItems = new List<ListItem>();

                int counter = 0;

                for (int i = 0; i < searchSize; i++)
                {
                    try
                    {
                        string title = jsonObj.value[i].name;
                        string url = jsonObj.value[i].url;
                        string imgLink = jsonObj.value[i].image.thumbnail.contentUrl;
                        string content = jsonObj.value[i].description;
                        string author = jsonObj.value[i].provider[0].name;
                        string publishedAt = jsonObj.value[0].datePublished.ToString("dd/MM/yyyy HH:mm");

                        ListItem temp = new ListItem();

                        temp.setTitle(title);
                        temp.setContent(content);
                        temp.setAuthor(author);
                        temp.setImage(imgLink);
                        temp.setLink(url);
                        temp.setDate(publishedAt);
                        listItems.Add(temp);
                        counter++;
                    }
                    catch (Exception e)
                    {
                    }
                }

                if (listItems.Count < searchSize)
                {
                    searchSize = listItems.Count;
                }

                for (int i = 0; i < searchSize; i++)
                {
                    populateList(listItems, i);
                }
            }
        }

        private void populateList(List<ListItem> listItems, int current)
        {
            try
            {
                if (InvokeRequired && listItems.ElementAt(current) != null)
                {
                    BeginInvoke((MethodInvoker)delegate ()
                    {
                        flowLayoutPanel1.Controls.Add(listItems.ElementAt(current));
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Background worker done.");
        }

        private void sourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sourceComboBox.SelectedIndex == 0)
            {
                fromDateTimePicker.Enabled = false;
                toDateTimePicker.Enabled = false;
                sortByComboBox.Enabled = true;
                languageComboBox.Enabled = true;

                sortByComboBox.Items.Clear();
                sortByComboBox.Items.Add("Newest");
                sortByComboBox.Items.Add("None");
                sortByComboBox.SelectedIndex = 1;
                sortByComboBox.Refresh();
            }
            else if (sourceComboBox.SelectedIndex == 1)
            {
                fromDateTimePicker.Enabled = true;
                toDateTimePicker.Enabled = true;
                sortByComboBox.Enabled = true;
                languageComboBox.Enabled = true;

                sortByComboBox.Items.Clear();
                sortByComboBox.Items.Add("Newest");
                sortByComboBox.Items.Add("Popularity");
                sortByComboBox.Items.Add("Relevancy");
                sortByComboBox.SelectedIndex = 0;
                sortByComboBox.Refresh();
            }
            else
            {
                fromDateTimePicker.Enabled = true;
                toDateTimePicker.Enabled = true;
                sortByComboBox.Enabled = false;
                languageComboBox.Enabled = false;
            }
        }

        private void getCustomNews()
        {
            string[] url = {
                /*"https://www.hurriyet.com.tr/rss/gundem",*/
                "https://www.sabah.com.tr/rss/anasayfa.xml",
                "https://www.sabah.com.tr/rss/sondakika.xml",
                "https://rss.haberler.com/rss.asp",
                "https://www.ensonhaber.com/rss/mansetler.xml",
                "https://www.ensonhaber.com/rss/ensonhaber.xml",
                "http://www.haberturk.com/rss",
                "https://www.cnnturk.com/feed/rss/all/news",
                "https://www.ntv.com.tr/son-dakika.rss",
                "https://www.takvim.com.tr/rss/anasayfa.xml",
                "https://www.takvim.com.tr/rss/guncel.xml",
                "http://www.mynet.com/haber/rss/sondakika",
                "https://www.haber.com/news-rss/",
                "https://www.cumhuriyet.com.tr/rss",
                "https://onedio.com/secure/sitemap/news48.xml",
                "https://tr.sputniknews.com/export/rss2/archive/index.xml",
                "https://www.fotomac.com.tr/rss/anasayfa.xml",
                "http://www.star.com.tr/rss/rss.asp",
                "https://www.aa.com.tr/tr/rss/default?cat=guncel"
            };

            List<ListItem> listItems = new List<ListItem>();

            try
            {
                for (int i = 0; i < url.Length; i++)
                {
                    XmlReader reader = XmlReader.Create(url[i]);
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    reader.Close();

                    foreach (SyndicationItem item in feed.Items)
                    {
                        ListItem listItem = new ListItem();
                        listItem.setTitle(item.Title.Text);
                        listItem.setDate(item.PublishDate.ToString());
                        listItem.setAuthor(feed.Title.Text);
                        listItem.setContent(item.Summary.Text);
                        listItem.setLink(item.Links.ElementAt(0).Uri.ToString());
                        listItem.setImage(item.Links.ElementAt(1).Uri.ToString());
                        listItems.Add(listItem);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            for (int i = 0; i < listItems.Count; i++)
            {
                populateListNew(listItems, i);
            }
        }

        private void getCustomNews2()
        {
            XmlDocument rssXmlDoc = new XmlDocument();

            rssXmlDoc.Load("https://www.sabah.com.tr/rss/gundem.xml");

            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

            XmlNode xmlNode = rssNodes.Item(1);

            int subIndex = xmlNode.SelectSingleNode("description").InnerText.IndexOf("<br />") + 6;
            int lastIndex = xmlNode.SelectSingleNode("description").InnerText.IndexOf("...<a href");
            string title = xmlNode.SelectSingleNode("description").InnerText.Substring(subIndex, (lastIndex - subIndex) + 1);
            MessageBox.Show(title);
        }

        private void populateListNew(List<ListItem> listItems, int current)
        {
            if (flowLayoutPanel1.Controls.Count < 0)
            {
                flowLayoutPanel1.Controls.Clear();
            }
            else
            {
                if (InvokeRequired)
                {
                    BeginInvoke((MethodInvoker)delegate ()
                    {
                        flowLayoutPanel1.Controls.Add(listItems.ElementAt(current));
                    });
                }
            }
        }
    }
}