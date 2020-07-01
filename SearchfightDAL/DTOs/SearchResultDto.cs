using System;

namespace SearchfightDAL
{
    public class SearchResultDto
    {
        public Guid RequestId { get; set; }

        public string RequestValue { get; set; }

        public double ResultCount { get; set; }

        public SearchEngine SearchEngineName { get; set; }
    }
}
