using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SearchfightDAL;
using SearchfightDAL.Models;

namespace SearchfightEngine.Interfaces
{
    public class SearchService : ISearchService
    {
        public async Task<List<SearchResultDto>> Search(SearchRequestModel request)
        {
            var searchResult = new List<SearchResultDto>();

            foreach (var requestEntity in request.SearchRequests)
            {
                foreach (var searchEngineEntity in request.SearchEngineEntities)
                {
                    double amountOfResults = Double.MinValue;

                    if (searchEngineEntity.SearchEngine == SearchEngine.Google)
                    {
                        amountOfResults = ExtractGoogleAmountOfResults(searchEngineEntity.SearchEngineQuery, requestEntity);
                    }
                    else if (searchEngineEntity.SearchEngine == SearchEngine.Bing)
                    {
                        amountOfResults = await ExtractBingAmountOfResults(searchEngineEntity.SearchEngineQuery, requestEntity);
                    }

                    var requestId = Guid.NewGuid();

                    searchResult.Add(new SearchResultDto
                    {
                        RequestId = requestId,
                        RequestValue = requestEntity,
                        ResultCount = amountOfResults,
                        SearchEngineName = searchEngineEntity.SearchEngine
                    });
                }
            }

            return searchResult;
        }

        private double ExtractGoogleAmountOfResults(string searchEngineQuery, string requestEntity)
        {
            var doc = new HtmlWeb().Load(searchEngineQuery + requestEntity);
            HtmlNode div = new HtmlNode(HtmlNodeType.Document, new HtmlDocument(), 0);
            div = doc.DocumentNode.SelectSingleNode("//div[@id='result-stats']");

            string responseBody = div.InnerText;

            var matches = Regex.Matches(responseBody, @"[0-9].+?(?=\()");
            var total = matches[0].Value;
            var parsedstr = total.Replace(",", "");            
            var textWithResultNumberWithoutCommas = parsedstr.Replace(",", "");
            var onlySearchResultNumber = Regex.Matches(textWithResultNumberWithoutCommas, @"[0-9]+")[0].Value;
             var resultAmount = double.Parse(onlySearchResultNumber);

            return resultAmount;
        }

        private async Task<double> ExtractBingAmountOfResults(string searchEngineQuery, string requestEntity)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(searchEngineQuery + requestEntity);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            int firstIndexOfBingResultNumber = responseBody.IndexOf(@"span class=""sb_count") + 22;
            int lastIndexOfBingResultNumber = firstIndexOfBingResultNumber + 15;
            string textWithResultNumber = responseBody.Substring(firstIndexOfBingResultNumber, lastIndexOfBingResultNumber - firstIndexOfBingResultNumber);
            var textWithResultNumberWithoutCommas = textWithResultNumber.Replace(",","");
            textWithResultNumberWithoutCommas = textWithResultNumberWithoutCommas.Replace(" ", "");
            var onlySearchResultNumber = Regex.Matches(textWithResultNumberWithoutCommas, @"[0-9]+")[0].Value;

            var resultAmount = double.Parse(onlySearchResultNumber);

            return resultAmount;
        }
    }
}
