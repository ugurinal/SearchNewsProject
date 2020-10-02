using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SearchNewsProject
{
    internal class BingNews
    {
        private struct SearchResult
        {
            public String jsonResult;
            public Dictionary<String, String> relevantHeaders;
        }

        private const string accessKey = "819df7c3408d4eb7b15e7e2e9739c002";
        private const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/news/search";
        private string searchQuery = null;

        public dynamic getBingNews()
        {
            WebRequest request = WebRequest.Create(searchQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            if (!String.IsNullOrEmpty(json))
            {
                var searchResult = new SearchResult()
                {
                    jsonResult = json,
                    relevantHeaders = new Dictionary<String, String>()
                };

                foreach (String header in response.Headers)
                {
                    if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                        searchResult.relevantHeaders[header] = response.Headers[header];
                }

                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(searchResult.jsonResult);

                return jsonObj;
            }
            else
            {
                return null;
            }
        }

        public void setSearchQuery(string keywords, int language, int searchSize, int sortBy)
        {
            string lang;

            switch (language)
            {
                case 0:
                    lang = "tr-TR";
                    break;

                case 1:
                    lang = "en-US";
                    break;

                case 2:
                    lang = "de-DE";
                    break;

                case 3:
                    lang = "ru-RU";
                    break;

                default:
                    lang = "tr-TR";
                    break;
            }

            if (sortBy == 0)
            {
                searchQuery = uriBase + "?q=" + keywords + "&sortBy=Date" + "&mkt=" + lang + "&count=" + searchSize;
            }
            else
            {
                searchQuery = uriBase + "?q=" + keywords + "&mkt=" + lang + "&count=" + searchSize;
            }
        }
    }
}