namespace CommitStore.Functions
{
    /// <summary>
    /// Represents a commit data object
    /// </summary>
    public class CommitData
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public required string Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("repository")]
        public required string Repository { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("branch")]
        public required string Branch { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("author")]
        public required string Author { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public required string Message { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}
