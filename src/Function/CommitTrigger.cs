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
        [CosmosDBOutput(
            "commitstore-db",
            "commits",
            Connection = "CosmosDBConnection",
            CreateIfNotExists = false
        )]
        public object Run(
            [QueueTrigger("commitqueue", Connection = "StorageQueueConnection")]
                QueueMessage message
        )
        {
            var json = message.MessageText;
            _logger.LogInformation("Received CommitData: {Json}", json);

            // connect to cosmosdb
            JsonDocument jsondoc;
            CommitData commit;
            try
            {
                jsondoc = JsonDocument.Parse(json);
                commit = new CommitData
                {
                    Id = GetStringFromJsonDocument(jsondoc, "commit_hash"),
                    Repository = GetStringFromJsonDocument(jsondoc, "repository"),
                    Branch = GetStringFromJsonDocument(jsondoc, "branch"),
                    Author = GetStringFromJsonDocument(jsondoc, "author"),
                    Message = GetStringFromJsonDocument(jsondoc, "commit_message"),
                    Timestamp = message.InsertedOn?.DateTime ?? DateTimeOffset.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to parse json:'{Json}'", json);
                throw;
            }
            _logger.LogInformation(
                "Saving this to Cosmos: {Json}",
                JsonSerializer.Serialize(commit)
            );

            return commit;
        }

        public static string GetStringFromJsonDocument(JsonDocument doc, string propertyName)
        {
            var prop = doc.RootElement.GetProperty(propertyName);
            return prop.GetString()
                ?? throw new InvalidOperationException($"Property {propertyName} not found");
        }
    }
}
