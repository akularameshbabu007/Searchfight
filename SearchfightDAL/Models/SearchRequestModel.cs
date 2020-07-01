using System.Collections.Generic;

namespace SearchfightDAL.Models
{
    public class SearchRequestModel
    {
        public List<string> SearchRequests { get; set; }

        public List<SearchEngineEntity> SearchEngineEntities { get; set; }
    }
}
