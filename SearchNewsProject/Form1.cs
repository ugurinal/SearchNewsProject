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
        public List<ListItem> listItems = new List<ListItem>();
        public progressBarForm progressBarForm = new progressBarForm();
        public Classify classify = new Classify();

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
            nextButton.Visible = false;
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

            topicComboBox.Enabled = false;
            topicComboBox.Visible = false;
            topicComboBox.Items.Add("All");
            topicComboBox.Items.Add("Gündem");
            topicComboBox.Items.Add("Eğitim");
            topicComboBox.Items.Add("Ekonomi");
            topicComboBox.Items.Add("Dünya");
            topicComboBox.Items.Add("Spor");
            topicComboBox.Items.Add("Sağlık");
            topicComboBox.Items.Add("Teknoloji");
            topicComboBox.SelectedIndex = 0;

            sortByComboBox.SelectedIndex = 0;
        }

        public void setLabels(bool value)
        {
            label1.Visible = value;
            label2.Visible = value;
            label3.Visible = value;
            label4.Visible = value;
        }

        public void refreshLabels(List<ListItem> list)
        {
            int currentFirstResults = 150 * buttonCounter;
            int currentLastResults = (150 * (buttonCounter + 1));

            if (currentLastResults > list.Count)
            {
                currentLastResults = list.Count;
            }

            label3.Text = list.Count.ToString();
            label4.Text = "" + currentFirstResults + " - " + currentLastResults;
        }

        private void xuiButton1_Click(object sender, EventArgs e)
        {
            if (backButton.Visible)
            {
                setLabels(false);
                backButton.Visible = false;
                nextButton.Visible = false;
                topicComboBox.Visible = false;
                buttonCounter = 0;
                topicComboBox.SelectedIndex = 0;
            }

            flowLayoutPanel1.Controls.Clear();

            listItems.Clear();

            backgroundWorker1.RunWorkerAsync();

            UseWaitCursor = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Refresh();
            listItems.Clear();

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
                        temp.Title = newsResult.Title;
                        temp.Content = newsResult.Description;
                        temp.Author = newsResult.Author;
                        temp.Image = newsResult.UrlToImage;
                        temp.Link = newsResult.Url;
                        temp.Date = newsResult.PublishedAt.Value.ToString("dd/MM/yyyy HH:mm");
                        listItems.Add(temp);
                        counter++;
                    }

                    for (int i = 0; i < listItems.Count; i++)
                    {
                        addNewsToLayoutPanel(listItems, i);
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

                        temp.Title = title;
                        temp.Content = content;
                        temp.Author = author;
                        temp.Image = imgLink;
                        temp.Link = url;
                        temp.Date = publishedAt;
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
                    addNewsToLayoutPanel(listItems, i);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setLabels(true);
            refreshLabels(listItems);
            flowLayoutPanel1.Refresh();

            progressBarForm.closeForm();

            UseWaitCursor = false;

            classify.setMainList(listItems);
            classify.categorise();

            topicComboBox.Enabled = true;
            topicComboBox.Visible = true;

            BNButtonHandler(listItems);
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
                "https://www.aa.com.tr/tr/rss/default?cat=guncel",   // same 3
                "http://www.mynet.com/haber/rss/sondakika",         // same 4
                "https://www.cumhuriyet.com.tr/rss",                // same 5
                "http://feeds.bbci.co.uk/turkce/rss.xml"
            };

            List<XmlDocument> xmlDocuments = new List<XmlDocument>();

            int nodeCounter = 0;
            double progress = 0;

            /*Find how many item nodes does the rss feed have.
             * For updating progress bar we have to know how many nodes there are.
            */
            for (int i = 0; i < sourceUrls.Length; i++)
            {
                XmlDocument rssXmlDoc = new XmlDocument();

                rssXmlDoc.Load(sourceUrls[i]);

                xmlDocuments.Add(rssXmlDoc);

                XmlNodeList itemNode = rssXmlDoc.SelectNodes("rss/channel/item");

                nodeCounter += itemNode.Count;

                progress += (100.0 / sourceUrls.Length);
                backgroundWorker1.ReportProgress(Convert.ToInt32(progress));
            }

            for (int i = 0; i < xmlDocuments.Count; i++)
            {
                XmlDocument rssXmlDoc = new XmlDocument();

                rssXmlDoc = xmlDocuments.ElementAt(i);

                XmlNodeList channelNode = rssXmlDoc.SelectNodes("rss/channel");
                XmlNodeList itemNode = rssXmlDoc.SelectNodes("rss/channel/item");

                string author = channelNode.Item(0).SelectSingleNode("title").InnerText;

                foreach (XmlNode singleNode in itemNode)
                {
                    ListItem listItem = new ListItem();

                    string title = singleNode.SelectSingleNode("title").InnerText; ;

                    string link = singleNode.SelectSingleNode("link").InnerText;
                    string pubDate = singleNode.SelectSingleNode("pubDate").InnerText;
                    string imgLink = null;
                    string description = null;

                    if (i < 5)
                    {
                        int startIndex = 0, lastIndex = 0;

                        startIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("<br />") + 6;
                        lastIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("<a href");

                        description = singleNode.SelectSingleNode("description").InnerText.Substring(startIndex, (lastIndex - startIndex));
                    }
                    else if (i >= 5 && i <= 8)
                    {
                        description = singleNode.SelectSingleNode("description").InnerText;
                    }
                    else if (i >= 9 && i <= 11)
                    {
                        int startIndex = 0, lastIndex = 0;

                        startIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("</a>") + 4;
                        lastIndex = singleNode.SelectSingleNode("description").InnerText.Length;
                        description = singleNode.SelectSingleNode("description").InnerText.Substring(startIndex, (lastIndex - startIndex));
                    }
                    else if (i >= 12 && i <= 16)
                    {
                        description = singleNode.SelectSingleNode("description").InnerText;
                    }

                    if (singleNode.SelectSingleNode("enclosure") != null)
                    {
                        imgLink = singleNode.SelectSingleNode("enclosure").Attributes["url"].Value;
                    }
                    else if (singleNode.SelectSingleNode("image") != null)
                    {
                        imgLink = singleNode.SelectSingleNode("image").InnerText;
                    }
                    else if (singleNode.SelectSingleNode("ipimage") != null)
                    {
                        imgLink = singleNode.SelectSingleNode("ipimage").InnerText;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                imgLink = "https://isbh.tmgrup.com.tr/sbh/v5/i/logoBig.png";
                                break;

                            case 1:
                                imgLink = "https://isbh.tmgrup.com.tr/sbh/v5/i/logoBig.png";
                                break;

                            case 2:
                                imgLink = "https://www.takvim.com.tr/c/tk/i/i/takvim_logo2.gif";
                                break;

                            case 3:
                                imgLink = "https://www.takvim.com.tr/c/tk/i/i/takvim_logo2.gif";
                                break;

                            case 4:
                                imgLink = "https://www.fotomac.com.tr/Content/v1/i/n-logo.png";
                                break;

                            case 5:
                                imgLink = "https://www.haberler.com/static/img/tasarim/haberler-logo.svg";
                                break;

                            case 6:
                                imgLink = "https://www.haber.com/wp-content/uploads/2020/01/80518294-55099622.png";
                                break;

                            case 7:
                                imgLink = "https://tr.sputniknews.com/i/logo-social.png";
                                break;

                            case 8:
                                imgLink = "https://www.turkmedya.com.tr/assets/img/logo/logo_star.png?v=1.0";
                                break;

                            case 9:
                                imgLink = "https://icdn.ensonhaber.com/cdn/desktop/img/logo.png";
                                break;

                            case 10:
                                imgLink = "https://icdn.ensonhaber.com/cdn/desktop/img/logo.png";
                                break;

                            case 11:
                                imgLink = "https://im.haberturk.com/assets/images/logo/haberturk-logo-v1.svg";
                                break;

                            case 12:
                                imgLink = "https://pbs.twimg.com/profile_images/879963335898406916/jU6vr8mb_400x400.jpg";
                                break;

                            case 13:
                                imgLink = "https://cdnassets.aa.com.tr/assets/newVersion/images/logo1_b_yan_100.png";
                                break;

                            case 14:
                                imgLink = "https://img7.mynet.com/mynet-logo.png";
                                break;

                            case 15:
                                imgLink = "https://pbs.twimg.com/profile_images/1192128310119215104/gb55ML1j_400x400.jpg";
                                break;

                            case 16:
                                imgLink = "https://upload.wikimedia.org/wikipedia/en/thumb/f/ff/BBC_News.svg/1200px-BBC_News.svg.png";
                                break;

                            default:
                                imgLink = "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png";
                                break;
                        }
                    }

                    listItem.Author = author;
                    listItem.Content = description;
                    listItem.Date = pubDate;
                    listItem.Image = imgLink;
                    listItem.Link = link;
                    listItem.Title = title;
                    listItems.Add(listItem);
                }
            }

            populateNewsList(listItems, 0);

            progress = 100;
            backgroundWorker1.ReportProgress(Convert.ToInt32(progress));
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarForm.updateProgressBar(e.ProgressPercentage);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            buttonCounter++;
            backAndNextButton(topicComboBox.SelectedIndex);
            backButton.Enabled = true;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            buttonCounter--;
            backAndNextButton(topicComboBox.SelectedIndex);
            nextButton.Enabled = true;
        }

        private void topicComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = topicComboBox.SelectedIndex;

            flowLayoutPanel1.Controls.Clear();
            buttonCounter = 0;

            switch (index)
            {
                case 0:
                    populateNewsList(listItems, 0);
                    break;

                case 1:
                    populateNewsList(classify.getGundemList(), 0);
                    break;

                case 2:
                    populateNewsList(classify.getEduList(), 0);
                    break;

                case 3:
                    populateNewsList(classify.getEcoList(), 0);
                    break;

                case 4:
                    populateNewsList(classify.getWorldList(), 0);
                    break;

                case 5:
                    populateNewsList(classify.getSportList(), 0);
                    break;

                case 6:
                    populateNewsList(classify.getHealthList(), 0);
                    break;

                case 7:
                    populateNewsList(classify.getTechList(), 0);
                    break;

                default:
                    break;
            }
        }

        private void populateNewsList(List<ListItem> list, int current)
        {
            if (list == null)
            {
                return;
            }

            int counter = 0;

            refreshLabels(list);
            BNButtonHandler(list);

            for (int i = current; i < list.Count; i++)
            {
                if (counter == 150)
                {
                    break;
                }
                addNewsToLayoutPanel(list, i);
                counter++;
            }

            flowLayoutPanel1.Refresh();
        }

        private void addNewsToLayoutPanel(List<ListItem> listItems, int current)
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

        private void backAndNextButton(int index)
        {
            if (buttonCounter <= 0)         // if button counter is less or equal to zero(0) disable the back button.
            {
                backButton.Enabled = false;
            }

            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Refresh();

            switch (index)
            {
                case 0:
                    refreshLabels(listItems);
                    populateNewsList(listItems, 150 * buttonCounter);
                    nextButtonHandler(listItems);
                    break;

                case 1:
                    refreshLabels(classify.getGundemList());
                    populateNewsList(classify.getGundemList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getGundemList());
                    break;

                case 2:
                    refreshLabels(classify.getEduList());
                    populateNewsList(classify.getEduList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getEduList());
                    break;

                case 3:
                    refreshLabels(classify.getEcoList());
                    populateNewsList(classify.getEcoList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getEcoList());
                    break;

                case 4:
                    refreshLabels(classify.getWorldList());
                    populateNewsList(classify.getWorldList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getWorldList());
                    break;

                case 5:
                    refreshLabels(classify.getSportList());
                    populateNewsList(classify.getSportList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getSportList());
                    break;

                case 6:
                    refreshLabels(classify.getHealthList());
                    populateNewsList(classify.getHealthList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getHealthList());
                    break;

                case 7:
                    refreshLabels(classify.getTechList());
                    populateNewsList(classify.getTechList(), 150 * buttonCounter);
                    nextButtonHandler(classify.getTechList());
                    break;
            }
        }

        private void BNButtonHandler(List<ListItem> list)   // for back and next button to be visible, enabled or not
        {
            if (list.Count > 150)
            {
                backButton.Visible = true;
                backButton.Enabled = false;
                nextButton.Visible = true;
                nextButton.Enabled = true;
            }
            else
            {
                backButton.Visible = false;
                backButton.Enabled = false;
                nextButton.Visible = false;
                nextButton.Enabled = false;
            }
        }

        private void nextButtonHandler(List<ListItem> list)
        {
            if ((list.Count / (150 * (buttonCounter + 1))) < 1)    // if there are no unlisted news
            {                                                           // make forward button disable
                nextButton.Enabled = false;
            }
        }
    }
}