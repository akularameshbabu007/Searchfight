using System;
using System.Collections.Generic;
using System.Linq;
using SearchfightDAL;
using SearchfightEngine.Interfaces;

namespace SearchfightEngine.Services
{
    public class SearchfightSummaryCounter : ISearchfightSummaryCounter
    {
        public IEnumerable<string> ProcessSummary(List<SearchResultDto> searchResult)
        {
            var countingResults = new List<string>();

            //ToDo: Code here is more readable and maintanable, but performance of it is worse, wrtie alternative high speed service
            foreach (SearchEngine searchEngine in Enum.GetValues(typeof(SearchEngine)))
            {
                var winner = searchResult
                    .Where(se => se.SearchEngineName == searchEngine)
                    .OrderByDescending(c => c.ResultCount)
                    .FirstOrDefault();

                countingResults.Add($"{winner.SearchEngineName} winner: {winner.RequestValue}");
            }

            var totalWinner = searchResult
                .OrderBy(c => c.ResultCount)
                .FirstOrDefault();

            countingResults.Add($"Total winner: {totalWinner.RequestValue}");

            return countingResults;
        }
    }
}