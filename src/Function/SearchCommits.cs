using System.Net;
using System.Text.Json;
using CommitStore.Functions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function
{
    public class SearchCommits
    {
        private readonly ILogger _logger;

        public SearchCommits(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SearchCommits>();
        }

        [Function("SearchCommits")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            // get search param querystring 'q'
            var queryString = req.Url.Query;
            var searchParam = System.Web.HttpUtility.ParseQueryString(queryString).Get("q") ?? "";
            _logger.LogInformation("Search param: {SearchParam}", searchParam);

            // get cosmosdbconnectionstring
            var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
            var client = new CosmosClient(cosmosDbConnectionString);
            var database = client.GetDatabase("commitstore-db");
            var container = database.GetContainer("commits");

            QueryDefinition qdef = new QueryDefinition(
                $"SELECT * FROM c where CONTAINS(c.message, @searchParam,true) or CONTAINS(c.branch, @searchParam,true) order by c.timestamp desc"
            ).WithParameter("@searchParam", searchParam);

            var feedIterator = container.GetItemQueryIterator<CommitData>(qdef);

            var commits = new List<CommitData>();
            while (feedIterator.HasMoreResults)
            {
                var commitResponses = await feedIterator.ReadNextAsync();
                commits.AddRange(commitResponses);
            }

            await response.WriteStringAsync(JsonSerializer.Serialize(commits));

            return response;
        }
    }
}
