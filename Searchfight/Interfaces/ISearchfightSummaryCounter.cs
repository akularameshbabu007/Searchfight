using System.Collections.Generic;
using SearchfightDAL;

namespace SearchfightEngine.Interfaces
{
    public interface ISearchfightSummaryCounter
    {
        IEnumerable<string> ProcessSummary(List<SearchResultDto> searchResult);
    }
}
