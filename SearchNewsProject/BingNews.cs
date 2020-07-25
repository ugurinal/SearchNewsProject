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

        public dynamic getBingNews()
        {
            const string accessKey = "bed27930d54c41098a15e25f235cb2a8";
            const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/news/search";
            const string searchTerm = "Microsoft";

            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchTerm) + "&mkt=en-US" + "&count=80";

            WebRequest request = WebRequest.Create(uriQuery);
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
    }
}