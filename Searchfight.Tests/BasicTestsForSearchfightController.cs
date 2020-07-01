using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Searchfight.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SearchfightDAL;
using SearchfightDAL.Models;
using SearchfightEngine.Interfaces;
using Xunit;

namespace Searchfight.Tests
{
    [NonController]
    public class BasicTestsForSearchfightController
    {
        [Fact]
        public async Task ListReturnAllValuesFromRepo()
        {
            var requestedData = new List<string> { "test1", "test2" };
            var expectedResult = new List<string> { "test: Google: 1" };
            var searchResponse = new List<SearchResultDto>
            {
                new SearchResultDto
                {
                    RequestValue = "test",
                    ResultCount = 1,
                    SearchEngineName = SearchEngine.Google,
                    RequestId = Guid.Empty
                }
            };
            Mock<ISearchfightSummaryCounter> mockSearchfightSummaryCounter = new Mock<ISearchfightSummaryCounter>();
            Mock<ISearchService> mockSearchService = new Mock<ISearchService>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();

            mockSearchService.CallBase = true;
            mockSearchService.Setup(x => x.Search(It.IsAny<SearchRequestModel>())).Returns(Task.FromResult(searchResponse));
            mockConfiguration.Setup(x => x.GetSection("Searchfight:SearchEngines:Google").Value).Returns("SearchEngnieQueryString");
            mockConfiguration.Setup(x => x.GetSection("Searchfight:SearchEngines:Bing").Value).Returns("SearchEngnieQueryString");
            SearchfightController controller = new SearchfightController( 
                null, 
                mockSearchfightSummaryCounter.Object,
                mockSearchService.Object,
                mockConfiguration.Object);

            var result = await controller.Get(requestedData.ToArray());

            Assert.IsType<List<string>>(result);
            Assert.Equal(expectedResult, result);
        }
    }
}
