using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CommitStore.Functions
{
    public class CommitTrigger
    {
        private readonly ILogger<CommitTrigger> _logger;

        public CommitTrigger(ILogger<CommitTrigger> logger)
        {
            _logger = logger;
        }

        [Function(nameof(CommitTrigger))]
        [CosmosDBOutput("commitstore-db", "commits", Connection = "CosmosDBConnection", CreateIfNotExists = false)]
        public object Run([QueueTrigger("commitqueue", Connection = "StorageQueueConnection")] QueueMessage message)
        {
            _logger.LogInformation($"Processing commit from CommitLogger queue");
            var json = message.MessageText;
            _logger.LogInformation($"Received CommitData: {json}");

            // connect to cosmosdb
            JsonDocument jsondoc;
            try
            {
                jsondoc = System.Text.Json.JsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to parse json: {ex.Message}, jsonvalue:'{json}'");
                throw;
            }

            if (jsondoc != null)
            {
                var commit = new CommitData
                {
                    Id = jsondoc.RootElement.GetProperty("commit_hash").GetString(),
                    Repository = jsondoc.RootElement.GetProperty("repository").GetString(),
                    Branch = jsondoc.RootElement.GetProperty("branch").GetString(),
                    Author = jsondoc.RootElement.GetProperty("author").GetString(),
                    Message = jsondoc.RootElement.GetProperty("commit_message").GetString(),
                    Timestamp = message.InsertedOn?.DateTime ?? DateTimeOffset.UtcNow
                };

                var jsoncommit = JsonSerializer.Serialize(commit);
                _logger.LogInformation($"Saving this to Cosmos: {jsoncommit}");
                return commit;
            }
            else
            {
                throw new Exception("Unable to parse json");
            }
        }
    }

    public class CommitData
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("repository")]
        public string Repository { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("branch")]
        public string Branch { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("author")]
        public string Author { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public string Message { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}
