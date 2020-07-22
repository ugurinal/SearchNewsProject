using System;
using System.Collections.Generic;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;

namespace SearchNewsProject
{
    class News
    {
        ArticlesResult articleResult = new ArticlesResult();
        EverythingRequest everythingRequest = new EverythingRequest();
        NewsApiClient NewsApiClient = new NewsApiClient("f86bf25ce8854adf8463a757edc1765a");

        public void setEverythingRequest(string keyWords, int language, DateTime from, DateTime to, int searchSize, int sortBy)
        {
            everythingRequest.Q = keyWords;
            everythingRequest.From = from;
            everythingRequest.To = to;
            everythingRequest.PageSize = searchSize;

            switch (language)
            {
                case 0:
                    everythingRequest.Language = Languages.TR;
                    break;
                case 1:
                    everythingRequest.Language = Languages.EN;
                    break;
                case 2:
                    everythingRequest.Language = Languages.DE;
                    break;
                case 3:
                    everythingRequest.Language = Languages.RU;
                    break;
                default:
                    everythingRequest.Language = Languages.TR;
                    break;
            }

            switch (sortBy)
            {
                case 0:
                    everythingRequest.SortBy = SortBys.PublishedAt;
                    break;
                case 1:
                    everythingRequest.SortBy = SortBys.Popularity;
                    break;
                case 2:
                    everythingRequest.SortBy = SortBys.Relevancy;
                    break;
                default:
                    everythingRequest.SortBy = SortBys.PublishedAt;
                    break;

            }



        }

        public void searchNews()
        {
            articleResult = NewsApiClient.GetEverything(everythingRequest);
        }

        public string getStatus()
        {
            return articleResult.Status.ToString();
        }

        public int getTotalResults()
        {
            return articleResult.TotalResults;
        }
        public List<Article> getArticles()
        {
            return articleResult.Articles;
        }

    }
}
