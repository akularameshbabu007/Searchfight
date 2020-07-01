using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SearchfightDAL;
using SearchfightDAL.Models;
using SearchfightEngine.Interfaces;

namespace Searchfight.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchfightController : ControllerBase
    {
        private readonly ILogger<SearchfightController> _logger;
        private readonly ISearchfightSummaryCounter _summaryCounter;
        private readonly ISearchService _searchService;
        private readonly IConfiguration _configuration;

        public SearchfightController(
            ILogger<SearchfightController> logger,
            ISearchfightSummaryCounter summaryCounter,
            ISearchService searchService,
            IConfiguration configuration)
        {
            _logger = logger;
            _summaryCounter = summaryCounter;
            _searchService = searchService;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("/search")]
        public async Task<IEnumerable<string>> Get([FromQuery(Name = "args")] string[] args)
        {
            var result = new List<string>();
            var googleQueryString = _configuration.GetSection("Searchfight:SearchEngines:Google").Value;
            var bingQueryString = _configuration.GetSection("Searchfight:SearchEngines:Bing").Value;

            if (args.Length == 0)
            {
                _logger.LogInformation($"Empty request comes in {DateTime.Now}");

                return new List<string> { "Request can't be empty" };
            }

            var searchRequest = new SearchRequestModel
            {
                SearchEngineEntities = new List<SearchEngineEntity>
                {
                    new SearchEngineEntity
                    {
                        SearchEngine = SearchEngine.Google,
                        SearchEngineQuery = googleQueryString
                    },
                    new SearchEngineEntity
                    {
                        SearchEngine = SearchEngine.Bing,
                        SearchEngineQuery = bingQueryString
                    }
                },
                SearchRequests = args.ToList()
            };

            var searchResult = await _searchService.Search(searchRequest);


            foreach (var resultEntity in searchResult)
            {
                result.Add($"{resultEntity.RequestValue}: {resultEntity.SearchEngineName}: {resultEntity.ResultCount}");
            }

            var summaryResults = _summaryCounter.ProcessSummary(searchResult);

            foreach (var summaryResult in summaryResults)
            {
                result.Add(summaryResult);
            }

            return result;
        }
    }
}
