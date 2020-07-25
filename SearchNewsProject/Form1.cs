﻿using MaterialSkin;
using MaterialSkin.Controls;
using NewsAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SearchNewsProject
{
    public partial class Form1 : MaterialForm
    {
        private struct SearchResult
        {
            public String jsonResult;
            public Dictionary<String, String> relevantHeaders;
        }

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
            sourceComboBox.SelectedIndex = 0;

            languageComboBox.Items.Add("Turkish");
            languageComboBox.Items.Add("English");
            languageComboBox.Items.Add("German");
            languageComboBox.Items.Add("Russian");
            languageComboBox.SelectedIndex = 0;

            sortByComboBox.Items.Add("Newest");
            sortByComboBox.Items.Add("Popularity");
            sortByComboBox.Items.Add("Relevancy");
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
            else
            {
                searchByNewsAPI();
            }
        }

        private void searchByNewsAPI()
        {
            News news = new News();

            news.setEverythingRequest(keywordTextBox.Text, languageComboBox.SelectedIndex, fromDateTimePicker.Value
                , toDateTimePicker.Value, Convert.ToInt32(searchSizeTextBox.Text), sortByComboBox.SelectedIndex);

            news.searchNews();

            if (news.getStatus().Equals("Ok"))
            {
                List<Article> articles = news.getArticles();

                ListItem[] listItems = new ListItem[articles.Count];
                int i = 0;

                var sw = new StreamWriter(@"D:\searchNewsProject.txt");

                foreach (var n in articles)
                {
                    sw.WriteLine("Başlık :  " + n.Title + " - Yazar : " + n.Author + " - Haber : " + n.Description + " - Link : " + n.Url + " - Tarih : " + n.PublishedAt + " URL : " + n.UrlToImage + "    status : " + news.getStatus());
                    populateList(listItems, n.Title, n.Description, n.Author, n.UrlToImage, n.Url, n.PublishedAt.Value.ToString("dd/MM/yyyy HH:mm"), i);
                    i++;
                }
            }
            else
            {
                MessageBox.Show("Status : " + news.getStatus());
            }
        }

        private void searchByBingAPI()
        {
            BingNews bingNews = new BingNews();

            dynamic jsonObj = bingNews.getBingNews();

            int size = Convert.ToInt32(searchSizeTextBox.Text);

            ListItem[] listItems = new ListItem[size];

            for (int i = 0; i < size; i++)
            {
                string title = jsonObj.value[i].name;
                string url = jsonObj.value[i].url;
                string imgLink = jsonObj.value[i].image.thumbnail.contentUrl;
                string content = jsonObj.value[i].description;
                string author = jsonObj.value[i].provider[0].name;
                string publishedAt = jsonObj.value[0].datePublished.ToString("dd/MM/yyyy HH:mm");

                populateList(listItems, title, content, author, imgLink, url, publishedAt, i);
            }
        }

        private void populateList(ListItem[] list, string title, string content, string author, string imgLink, string newLink, string date, int current)
        {
            list[current] = new ListItem();
            list[current].setTitle(title);
            list[current].setContent(content);
            list[current].setAuthor(author);
            list[current].setImage(imgLink);
            list[current].setLink(newLink);
            list[current].setDate(date);

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
                        flowLayoutPanel1.Controls.Add(list[current]);
                    });
                }
                else
                {
                    flowLayoutPanel1.Controls.Add(list[current]);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Background worker complete.");
        }

        private void sourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sourceComboBox.SelectedIndex == 0)
            {
                fromDateTimePicker.Enabled = false;
                toDateTimePicker.Enabled = false;
            }
            else
            {
                fromDateTimePicker.Enabled = true;
                toDateTimePicker.Enabled = true;
            }
        }
    }
}