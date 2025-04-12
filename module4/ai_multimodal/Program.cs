using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable SKEXP0010 

namespace ai_multimodal
{
    /// <summary>
    /// Show the concepts of using a Multi-Modal AI model to analyze traffic congestion images.
    /// Also shows how to use Formated Output to get a deterministic response.
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration to load environment variables
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            // Initialize the Azure OpenAI Chat Completion Service with necessary parameters
            AzureOpenAIChatCompletionService chatCompletionService = new(
                deploymentName: configuration["DEPLOYMENT_NAME"]!,
                apiKey: configuration["API_KEY"]!,
                endpoint: configuration["ENDPOINT"]!,
                modelId: configuration["MODEL_ID"]!
            );

            // Get all image files from the "images" directory
            var imageFiles = Directory.GetFiles("images", "*.jpg");
            foreach (var imageFile in imageFiles)
            {
                //Load Image into memory
                Console.WriteLine($"Image: {imageFile}");
                byte[] bytes = File.ReadAllBytes(imageFile);

                // Create a chat history with an initial system message
                ChatHistory history = new ChatHistory();

                // Add user messages to the chat history
                history.AddUserMessage("[Replace]");

                // Get the chat message content from the chat completion service
                var response = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory: history
                );

                Console.WriteLine(response.Content);
                Console.WriteLine(new string('-', 40));
            }
        }
    }


}
