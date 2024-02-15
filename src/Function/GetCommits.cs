using System.Net;
using System.Text.Json;
using CommitStore.Functions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function
{
    public class GetCommits
    {
        private readonly ILogger _logger;

        public GetCommits(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetCommits>();
        }

        [Function("GetCommits")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", "post")] HttpRequestData req
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            // get cosmosdbconnectionstring
            var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
            var client = new CosmosClient(cosmosDbConnectionString);
            var database = client.GetDatabase("commitstore-db");
            var container = database.GetContainer("commits");

            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            var feedIterator = container
                .GetItemLinqQueryable<CommitData>(
                    allowSynchronousQueryExecution: true,
                    requestOptions: new QueryRequestOptions { MaxItemCount = 1000 }
                )
                .ToFeedIterator();

            var commits = new List<CommitData>();
            while (feedIterator.HasMoreResults)
            {
                var commitResponses = await feedIterator.ReadNextAsync();
                commits.AddRange(commitResponses);
            }
            response.WriteString(JsonSerializer.Serialize(commits));

            return response;
        }
    }
}
