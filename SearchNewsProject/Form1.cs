using MaterialSkin;
using MaterialSkin.Controls;
using NewsAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace SearchNewsProject
{
    public partial class Form1 : MaterialForm
    {
        private List<ListItem> listItems = new List<ListItem>();
        public progressBarForm progressBarForm = new progressBarForm();

        private int buttonCounter = 0;  // a counter that count how many times back and forward button clicked.

        public Form1()
        {
            InitializeComponent();
            setSkin();
            setDateTimePickers();
            setButton();
            setComboBoxes();
            setLabels(false);
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

            backButton.Visible = false;
            forwardButton.Visible = false;
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

        public void setLabels(bool value)
        {
            label1.Visible = value;
            label2.Visible = value;
            label3.Visible = value;
            label4.Visible = value;
        }

        public void refreshLabels()
        {
            int currentFirstResults = 150 * buttonCounter;
            int currentLastResults = (150 * (buttonCounter + 1));

            if (currentLastResults > listItems.Count)
            {
                currentLastResults = listItems.Count;
            }

            label3.Text = listItems.Count.ToString();
            label4.Text = "" + currentFirstResults + " - " + currentLastResults;
        }

        private void xuiButton1_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            listItems.Clear();

            //progressBarForm.Show();

            backgroundWorker1.RunWorkerAsync();

            UseWaitCursor = true;
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
                progressBarForm.Show();
                getCustomNews();
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
                        populeNewsList(listItems, i);
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
                        MessageBox.Show(e.Message);
                    }
                }

                if (listItems.Count < searchSize)
                {
                    searchSize = listItems.Count;
                }

                for (int i = 0; i < searchSize; i++)
                {
                    populeNewsList(listItems, i);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setLabels(true);
            refreshLabels();
            flowLayoutPanel1.Refresh();

            progressBarForm.closeForm();

            UseWaitCursor = false;

            if (listItems.Count > 150)
            {
                backButton.Visible = true;
                backButton.Enabled = false;
                forwardButton.Visible = true;
            }
            MessageBox.Show("Background worker done.");
            MessageBox.Show("Flow layout panel count: " + flowLayoutPanel1.Controls.Count);
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
            string[] sourceUrls = {
                "https://www.sabah.com.tr/rss/anasayfa.xml",        // same 0
                "https://www.sabah.com.tr/rss/sondakika.xml",       // same 0
                "https://www.takvim.com.tr/rss/anasayfa.xml",       // same 0
                "https://www.takvim.com.tr/rss/guncel.xml",         // same 0
                "https://www.fotomac.com.tr/rss/anasayfa.xml",      // same 0
                "https://rss.haberler.com/rss.asp",                 // same 1
                "https://www.haber.com/news-rss/",                  // same 1
                "https://tr.sputniknews.com/export/rss2/archive/index.xml",// same1
                "http://www.star.com.tr/rss/rss.asp",               // same 1
                "https://www.ensonhaber.com/rss/mansetler.xml",     // same 2
                "https://www.ensonhaber.com/rss/ensonhaber.xml",    // same 2
                "http://www.haberturk.com/rss",                     // same 2
                "https://www.cnnturk.com/feed/rss/all/news",        // same 3
                /*"https://www.hurriyet.com.tr/rss/gundem",*/       // same 3
                "https://www.ntv.com.tr/son-dakika.rss",            // same 4
                "http://www.mynet.com/haber/rss/sondakika",         // same 5
                "https://www.cumhuriyet.com.tr/rss",                // same 6
                "https://www.aa.com.tr/tr/rss/default?cat=guncel"   // same 7
            };

            int nodeCounter = 0;
            double progress = 0;
            /*Find how many item nodes does the rss feed have.
             * For updating progress bar we have to know how many nodes there are.
            */
            for (int i = 0; i < 5; i++)
            {
                XmlDocument rssXmlDoc = new XmlDocument();

                rssXmlDoc.Load(sourceUrls[i]);

                XmlNodeList itemNode = rssXmlDoc.SelectNodes("rss/channel/item");

                nodeCounter += itemNode.Count;
            }

            for (int i = 0; i < 5; i++)
            {
                XmlDocument rssXmlDoc = new XmlDocument();

                rssXmlDoc.Load(sourceUrls[i]);

                XmlNodeList channelNode = rssXmlDoc.SelectNodes("rss/channel");
                XmlNodeList itemNode = rssXmlDoc.SelectNodes("rss/channel/item");

                string author = channelNode.Item(0).SelectSingleNode("title").InnerText;

                foreach (XmlNode singleNode in itemNode)
                {
                    ListItem listItem = new ListItem();

                    int startIndex = 0, lastIndex = 0;

                    string title = null;
                    string description = null;
                    string link = null;
                    string pubDate = null;
                    string imgLink = null;

                    if (i < 5)
                    {
                        startIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("<br />") + 6;
                        lastIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("<a href");

                        title = singleNode.SelectSingleNode("title").InnerText;
                        link = singleNode.SelectSingleNode("link").InnerText;
                        pubDate = singleNode.SelectSingleNode("pubDate").InnerText;
                        description = singleNode.SelectSingleNode("description").InnerText.Substring(startIndex, (lastIndex - startIndex));
                    }

                    if (i == 1)
                    {
                        imgLink = "https://isbh.tmgrup.com.tr/sbh/v5/i/logoBig.png";
                    }
                    else
                    {
                        imgLink = singleNode.SelectSingleNode("enclosure").Attributes["url"].Value;
                    }

                    listItem.setAuthor(author);
                    listItem.setContent(description);
                    listItem.setDate(pubDate);
                    listItem.setImage(imgLink);
                    listItem.setLink(link);
                    listItem.setTitle(title);
                    listItems.Add(listItem);

                    if (progress < 99)
                    {
                        progress += (100.0 / nodeCounter);
                        backgroundWorker1.ReportProgress(Convert.ToInt32(progress));
                    }
                }
            }

            for (int i = 0; i < listItems.Count; i++)
            {
                if (i == 150)
                {
                    break;
                }
                populeNewsList(listItems, i);
            }

            progress = 100;
            backgroundWorker1.ReportProgress(Convert.ToInt32(progress));

            MessageBox.Show("ListItem count :" + listItems.Count);
        }

        private void populeNewsList(List<ListItem> listItems, int current)
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
                else
                {
                    flowLayoutPanel1.Controls.Add(listItems.ElementAt(current));
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarForm.updateProgressBar(e.ProgressPercentage);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            int counter = 0;    // for a loop to show exactly 150 news in the flowlayout panel
            buttonCounter++;

            if ((listItems.Count / (150 * (buttonCounter + 1))) < 1)    // if there are still unlisted news
            {                                                           // make forward button enable
                forwardButton.Enabled = false;
            }

            backButton.Enabled = true;                                  // if forward button clicked
                                                                        // this means back button must be enabled
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Refresh();

            for (int i = 150 * buttonCounter; i < listItems.Count; i++)
            {
                if (counter == 150)
                {
                    break;
                }

                populeNewsList(listItems, i);
                counter++;
            }

            refreshLabels();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            int counter = 0;

            buttonCounter--;

            forwardButton.Enabled = true;   // if back button cliked this means forward button must be enabled

            if (buttonCounter <= 0)         // if button counter is less or equal to zero(0) disable the back button.
            {
                backButton.Enabled = false;
            }

            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Refresh();
            for (int i = 150 * buttonCounter; i < listItems.Count; i++)
            {
                if (counter == 150)
                {
                    break;
                }
                populeNewsList(listItems, i);
                counter++;
            }

            refreshLabels();
        }
    }
}