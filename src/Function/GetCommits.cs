using System.Net;
using System.Text.Json;
using CommitStore.Functions;
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
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            CommitData[] commits =
            [
                new CommitData
                {
                    Id = "1",
                    Repository = "Repo1",
                    Branch = "main",
                    Author = "Author1",
                    Message = "Initial commit",
                    Timestamp = System.DateTimeOffset.UtcNow
                },
                new CommitData
                {
                    Id = "2",
                    Repository = "Repo2",
                    Branch = "main",
                    Author = "Author2",
                    Message = "Initial commit",
                    Timestamp = System.DateTimeOffset.UtcNow
                }
            ];
            response.WriteString(JsonSerializer.Serialize(commits));

            return response;
        }
    }
}
