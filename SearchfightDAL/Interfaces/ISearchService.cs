using System.Collections.Generic;
using System.Threading.Tasks;
using SearchfightDAL;
using SearchfightDAL.Models;

namespace SearchfightEngine.Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchResultDto>> Search(SearchRequestModel request);
    }
}
