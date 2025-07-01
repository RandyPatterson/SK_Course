using System.ComponentModel;
using System.Text.Json.Serialization;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

namespace Chat.Plugins
{
    public class ContosoHealth
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly SearchIndexClient _indexClient;

        public ContosoHealth(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, SearchIndexClient indexClient)
        {
            _embeddingGenerator = embeddingGenerator;
            _indexClient = indexClient;
        }

        [KernelFunction("contoso_search")]
        [Description("use to search Contoso company documents for the given query.")]
        [return:Description("returns a list of results where the Content is the data found from the search, Citation is the name of the document where the result was found and Score decimal percentage of is how confident the result matches the query ")]
        public async Task<IList<ContosoSearchResults>> SearchAsync(string query)
        {
            // Convert string query to vector
            var embedding = await _embeddingGenerator.GenerateAsync(query);

            // Get client for search operations
            SearchClient searchClient = _indexClient.GetSearchClient("benifits");

            // Configure request parameters
            VectorizedQuery vectorQuery = new(embedding.Vector);
            vectorQuery.Fields.Add("contentVector");

            SearchOptions searchOptions = new() {Size=5, VectorSearch = new() { Queries = { vectorQuery } } };

            // Perform search request
            Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(searchOptions);

            // Collect search results
            var searchResults = new List<ContosoSearchResults>();
            await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
            {
                //skip if confidence score less than 80%
                if (result.Score < .8)
                    continue;

                searchResults.Add(new ContosoSearchResults()
                {
                    Content = result.Document.Content,
                    Citation = result.Document.FilePath,
                    Score = result.Score
                });
            }

            return searchResults;
        }

        private sealed class IndexSchema
        {
            [JsonPropertyName("parent_id")]
            public string? ParentId { get; set; }

            [JsonPropertyName("content")]
            public string? Content { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("url")]
            public string? Url { get; set; }

            [JsonPropertyName("filepath")]
            public string? FilePath { get; set; }
        }
    }

    public class ContosoSearchResults
    {
        public string? Content { get; set; }
        public string? Citation { get; set; }
        public double? Score { get; set; }

    }
}
