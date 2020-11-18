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
        #region global variables

        public List<ListItem> listItems = new List<ListItem>();         // A list to keep all the news
        public progressBarForm progressBarForm = new progressBarForm(); // Prograss bar to show progression
        public Classify classify = new Classify();                      // Classify object to classify the news
        private int buttonCounter = 0;  // a counter that count how many times back and next button clicked.

        #endregion global variables

        /* Main form*/

        public Form1()
        {
            InitializeComponent();
            setSkin();              // Theme of forms
            setDateTimePickers();   // Sets date time pickers
            setButton();            // Sets buttons
            setComboBoxes();        // Sets comboboxes
            setLabels(false);       // Sets labels
        }

        /* Function to change the theme of forms*/

        public void setSkin()
        {
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal800, Primary.Teal800,
                Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
        }

        /*  Function to correct the date time piclers.
            Date time picker must be in range of last 30 days.*/

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

        /* Initialize the buttons. Which sould be shown or not.*/

        public void setButton()
        {
            Font font = new Font("Microsoft Sans Serif", 18, FontStyle.Regular);
            searchButton.Font = font;

            backButton.Visible = false;
            nextButton.Visible = false;
        }

        /* Initialize the comboboxes. Addes choices in them.*/

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

        /* Sets the total result and showing results labels.*/

        public void setLabels(bool value)
        {
            totalResultLabel.Visible = value;
            ShowingResultLabel.Visible = value;
            totalResultNum.Visible = value;
            ShowingResultNum.Visible = value;
        }

        /* Updates the results labels.*/

        public void refreshLabels(List<ListItem> list)
        {
            int currentFirstResults = 150 * buttonCounter;          // if the button counter is zero it means
            int currentLastResults = (150 * (buttonCounter + 1));   // the user is in the first page

            if (currentLastResults > list.Count)                    // At the end of pages if the list count is
            {                                                       // less than current result, list count must be
                currentLastResults = list.Count;                    // displayed. Eg. if there is 736 news, it must
            }                                                       // be showing result: 600-736

            totalResultNum.Text = list.Count.ToString();
            ShowingResultNum.Text = "" + currentFirstResults + " - " + currentLastResults;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            setLabels(false);                   // if the back button is visible it resets the labels, buttons
            backButton.Visible = false;         // buttonCounter, in short it resets evertything.
            nextButton.Visible = false;         // Making it look like it is opened new.
            topicComboBox.Visible = false;
            buttonCounter = 0;
            topicComboBox.SelectedIndex = 0;

            flowLayoutPanel1.Controls.Clear();      // Clear the content of content panel
            flowLayoutPanel1.Refresh();

            listItems.Clear();                      // Clear the items in the list item which is
            classify.clear();                       // used for storing news. And the classify lists.

            backgroundWorker1.RunWorkerAsync();     // Starts the background worker. Background worker
                                                    // will do everything.
            UseWaitCursor = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            topicComboBox.SelectedIndex = 0;
            int selectedIndex = sourceComboBox.SelectedIndex;

            if (selectedIndex == 0)
            {
                searchByBingAPI();
            }
            else if (selectedIndex == 1)
            {
                searchByNewsAPI();
            }
            else
            {
                progressBarForm.Show();
                SearchByCustom();
                progressBarForm.closeForm();
            }

            classify.setMainList(listItems);    // sends the main list to the classfy class
            classify.categorise();              // Categorise the news
        }

        /* If sourse combobox selected index changes it will modify the other elemets like textbox,
         date time pickers, sortby comboboxes...*/

        private void sourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = sourceComboBox.SelectedIndex;

            if (selectedIndex == 0)
            {
                fromDateTimePicker.Enabled = false;
                toDateTimePicker.Enabled = false;
                sortByComboBox.Enabled = true;
                languageComboBox.Enabled = true;
                searchSizeTextBox.Enabled = true;

                sortByComboBox.Items.Clear();
                sortByComboBox.Items.Add("Newest");
                sortByComboBox.Items.Add("None");
                sortByComboBox.SelectedIndex = 1;
                sortByComboBox.Refresh();
            }
            else if (selectedIndex == 1)
            {
                fromDateTimePicker.Enabled = true;
                toDateTimePicker.Enabled = true;
                sortByComboBox.Enabled = true;
                languageComboBox.Enabled = true;
                searchSizeTextBox.Enabled = true;

                sortByComboBox.Items.Clear();
                sortByComboBox.Items.Add("Newest");
                sortByComboBox.Items.Add("Popularity");
                sortByComboBox.Items.Add("Relevancy");
                sortByComboBox.SelectedIndex = 0;
                sortByComboBox.Refresh();
            }
            else
            {
                fromDateTimePicker.Enabled = false;
                toDateTimePicker.Enabled = false;
                sortByComboBox.Enabled = false;
                languageComboBox.Enabled = false;
                searchSizeTextBox.Enabled = false;
            }
        }

        private void searchByNewsAPI()
        {
            /* Checks the keyword and searchsize text boxes if they are valid or not*/
            if (checkTextBoxes())
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

                    foreach (var newsResult in articles)
                    {
                        ListItem temp = new ListItem();
                        temp.Title = newsResult.Title;
                        temp.Content = newsResult.Description;
                        temp.Author = newsResult.Author;
                        temp.Image = newsResult.UrlToImage;
                        temp.Link = newsResult.Url;
                        temp.Date = newsResult.PublishedAt.Value.ToString("dd/MM/yyyy HH:mm");
                        listItems.Add(temp);

                        if (listItems.Count >= searchSize)
                        {
                            break;
                        }
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
            /* Checks the keyword and searchsize text boxes if they are valid or not*/
            if (checkTextBoxes())
            {
                string keyWords = keywordTextBox.Text;
                int language = languageComboBox.SelectedIndex;
                int searchSize = Convert.ToInt32(searchSizeTextBox.Text);
                int sortBy = sortByComboBox.SelectedIndex;

                BingNews bingNews = new BingNews();
                bingNews.setSearchQuery(keyWords, language, searchSize, sortBy);

                dynamic jsonObj = bingNews.getBingNews();

                if (jsonObj.totalEstimatedMatches < searchSize)     // if there are less news than search size
                {                                                   // assign totalestimatedmatches to searchsize
                    searchSize = jsonObj.totalEstimatedMatches; ;   // to use it in for loop in the below
                }

                for (int i = 0; i < searchSize; i++)
                {
                    try
                    {
                        string title = jsonObj.value[i].name;
                        string url = jsonObj.value[i].url;
                        string imgLink = jsonObj.value[i].image.thumbnail.contentUrl;
                        string content = jsonObj.value[i].description;
                        string author = jsonObj.value[i].provider[0].name;
                        string publishedAt = jsonObj.value[i].datePublished.ToString("dd/MM/yyyy HH:mm");

                        ListItem temp = new ListItem();

                        temp.Title = title;
                        temp.Content = content;
                        temp.Author = author;
                        temp.Image = imgLink;
                        temp.Link = url;
                        temp.Date = publishedAt;
                        listItems.Add(temp);
                    }
                    catch (Exception)
                    {
                    }
                }

                for (int i = 0; i < listItems.Count; i++)
                {
                    addNewsToLayoutPanel(listItems, i);
                }
            }
        }

        private void SearchByCustom()
        {
            string[] sourceUrls = {
                "https://www.sabah.com.tr/rss/anasayfa.xml",
                "https://www.sabah.com.tr/rss/sondakika.xml",
                "https://www.takvim.com.tr/rss/anasayfa.xml",
                "https://www.takvim.com.tr/rss/guncel.xml",
                "https://www.fotomac.com.tr/rss/anasayfa.xml",
                "https://rss.haberler.com/rss.asp",
                "https://www.haber.com/news-rss/",
                "https://tr.sputniknews.com/export/rss2/archive/index.xml",
                "http://www.star.com.tr/rss/rss.asp",
                "https://www.ensonhaber.com/rss/mansetler.xml",
                "https://www.ensonhaber.com/rss/ensonhaber.xml",
                "http://www.haberturk.com/rss",
                "https://www.cnnturk.com/feed/rss/all/news",
                "https://www.aa.com.tr/tr/rss/default?cat=guncel",
                "http://www.mynet.com/haber/rss/sondakika",
                "https://www.cumhuriyet.com.tr/rss",
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

                progress += (100.0 / sourceUrls.Length);        // updates the progress
                backgroundWorker1.ReportProgress(Convert.ToInt32(progress));    // updates the progress bar
            }

            /* Fill the list items which stores the news */

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
                    string content = null;

                    if (i == 1)
                    {
                        content = singleNode.SelectSingleNode("description").InnerText;
                    }
                    else if (i < 5)
                    {
                        int startIndex = 0, lastIndex = 0;

                        startIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("<br />") + 6;
                        lastIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("<a href");

                        if (lastIndex - startIndex > 0)
                        {
                            content = singleNode.SelectSingleNode("description").InnerText.Substring(startIndex, (lastIndex - startIndex));
                        }
                    }
                    else if (i >= 5 && i <= 8)
                    {
                        content = singleNode.SelectSingleNode("description").InnerText;
                    }
                    else if (i >= 9 && i <= 11)
                    {
                        int startIndex = 0, lastIndex = 0;

                        startIndex = singleNode.SelectSingleNode("description").InnerText.IndexOf("</a>") + 4;
                        lastIndex = singleNode.SelectSingleNode("description").InnerText.Length;
                        content = singleNode.SelectSingleNode("description").InnerText.Substring(startIndex, (lastIndex - startIndex));
                    }
                    else if (i >= 12 && i <= 16)
                    {
                        content = singleNode.SelectSingleNode("description").InnerText;
                    }

                    /* Checks if the rss feed contains the image link or not
                     if it doesn't we assign it*/
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
                    listItem.Content = content;
                    listItem.Date = pubDate;
                    listItem.Image = imgLink;
                    listItem.Link = link;
                    listItem.Title = title;

                    if (content.IndexOf(keywordTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)  // checks if the content
                    {                                                                                       // of the news contains
                        listItems.Add(listItem);                                                            // the keyword or not
                    }
                }
            }

            populateNewsList(listItems, 0);

            progress = 99;
            backgroundWorker1.ReportProgress(Convert.ToInt32(progress));
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (listItems.Count > 0)
            {
                setLabels(true);
                refreshLabels(listItems);
                flowLayoutPanel1.Refresh();

                topicComboBox.Enabled = true;
                topicComboBox.Visible = true;
            }
            UseWaitCursor = false;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarForm.updateProgressBar(e.ProgressPercentage);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            int selectedTopicIndex = topicComboBox.SelectedIndex;
            buttonCounter++;
            backAndNextButton(selectedTopicIndex);
            backButton.Enabled = true;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            int selectedTopicIndex = topicComboBox.SelectedIndex;
            buttonCounter--;
            backAndNextButton(selectedTopicIndex);
            nextButton.Enabled = true;
        }

        /*
         * Updates the news panel when topic is changed
         * 0 = All items
         * 1 = Gundem
         * 2 = Education
         * 3 = Economy
         * 4 = World
         * 5 = Sport
         * 6 = Health
         * 7 = Technology
         */

        private void topicComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = topicComboBox.SelectedIndex;

            flowLayoutPanel1.Controls.Clear();
            buttonCounter = 0;  // when the news panel resets , also reset the button counter

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

            refreshLabels(list);    // updates the result labels
            initBNButton(list);     // checks the back and next button if it should be visible, enable or not

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

        private void backAndNextButton(int index)
        {
            if (buttonCounter >= 0)
            {
                backButton.Enabled = true;
            }
            else
            {
                backButton.Enabled = false;
            }

            flowLayoutPanel1.Controls.Clear();

            int indexOfFirstNew = 150 * buttonCounter;

            /*
             *Refresh the news panel by checking the index of topic combo box.
             *0 = All news
             *1 = Gundem
             *2 = Education
             *3 = Economy
             *4 = World
             *5 = Sport
             *6 = Health
             *7 = Technology
             */
            switch (index)
            {
                case 0:
                    refreshLabels(listItems);
                    populateNewsList(listItems, indexOfFirstNew);
                    BackAndNextButController(listItems);

                    break;

                case 1:
                    refreshLabels(classify.getGundemList());
                    populateNewsList(classify.getGundemList(), indexOfFirstNew);
                    BackAndNextButController(classify.getGundemList());

                    break;

                case 2:
                    refreshLabels(classify.getEduList());
                    populateNewsList(classify.getEduList(), indexOfFirstNew);
                    BackAndNextButController(classify.getEduList());
                    break;

                case 3:
                    refreshLabels(classify.getEcoList());
                    populateNewsList(classify.getEcoList(), indexOfFirstNew);
                    BackAndNextButController(classify.getEcoList());
                    break;

                case 4:
                    refreshLabels(classify.getWorldList());
                    populateNewsList(classify.getWorldList(), indexOfFirstNew);
                    BackAndNextButController(classify.getWorldList());
                    break;

                case 5:
                    refreshLabels(classify.getSportList());
                    populateNewsList(classify.getSportList(), indexOfFirstNew);
                    BackAndNextButController(classify.getSportList());
                    break;

                case 6:
                    refreshLabels(classify.getHealthList());
                    populateNewsList(classify.getHealthList(), indexOfFirstNew);
                    BackAndNextButController(classify.getHealthList());
                    break;

                case 7:
                    refreshLabels(classify.getTechList());
                    populateNewsList(classify.getTechList(), indexOfFirstNew);
                    BackAndNextButController(classify.getTechList());
                    break;
            }
        }

        private void initBNButton(List<ListItem> list)   // for back and next button to be visible, enabled or not
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

        private void BackAndNextButController(List<ListItem> list)
        {
            if ((list.Count / (150 * (buttonCounter + 1))) < 1)     // if there are no unlisted news
            {                                                       // make forward button disabled
                nextButton.Enabled = false;
            }

            if (buttonCounter > 0)                                  // if button counter is higher than0
            {                                                       // make back button enabled
                backButton.Enabled = true;
            }
        }

        private bool checkTextBoxes()
        {
            if (String.IsNullOrWhiteSpace(keywordTextBox.Text) || String.IsNullOrWhiteSpace(searchSizeTextBox.Text))
            {
                MessageBox.Show("Search size or keywords fields can not be empty.");
                return false;
            }
            else if (Convert.ToInt32(searchSizeTextBox.Text) < 0 || Convert.ToInt32(searchSizeTextBox.Text) > 100)
            {
                MessageBox.Show("Search size can not be lower than 0 or higher than 100.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}