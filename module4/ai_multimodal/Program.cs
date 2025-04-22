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


            var executionSettings = new OpenAIPromptExecutionSettings
            {
                ResponseFormat = typeof(CameraResult)
            };

            // Get all image files from the "images" directory
            var imageFiles = Directory.GetFiles("images", "*.jpg");
            foreach (var imageFile in imageFiles)
            {
                //Load Image into memory
                Console.WriteLine($"Image: {imageFile}");
                byte[] bytes = File.ReadAllBytes(imageFile);

                // Create a chat history with an initial system message
                ChatHistory history = new ChatHistory(
                    @"you are a traffic analyzer AI that monitors traffic congestion images and congestion level.
Heavy congestion level is when there is very little room between cars and vehicles are breaking.
Medium congestion is when there is a lot of cars but they are not braking.
Low traffic is when there are few cars on the road
In addition, attempt to determine if the image was taken with a malfunctioning camera by looking for distorted image or missing content  
");

                // Add user messages to the chat history
                history.AddUserMessage(
                    [
                        new ImageContent(bytes, "image/jpeg"),
                        new TextContent("Analyze the image and determine the traffic congestion level. Also determine if the camera is malfunctioning")
                    ]
                    );

                // Get the chat message content from the chat completion service
                var response = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory: history,
                    executionSettings: executionSettings
                );

                // Deserialize the response content into a CameraResult object
                var options = new JsonSerializerOptions
                {
                    // Configure the JSON serializer to convert strings to enums when needed
                    Converters = { new JsonStringEnumConverter() }
                };
                CameraResult result = JsonSerializer.Deserialize<CameraResult>(response.Content, options);


                // Display the results with appropriate console colors
                Console.ForegroundColor = result.IsBroken ? ConsoleColor.Red : ConsoleColor.Green;
                Console.WriteLine($"IsBroken: {result.IsBroken}");
                Console.WriteLine($"TrafficCongestionLevel: {result.TrafficCongestionLevel}");
                Console.WriteLine($"Analysis: {result.Analysis}");
                Console.ResetColor();
                Console.WriteLine(new string('-', 40));

                //artificial delay
                await Task.Delay(1000);
            }
        }
    }


    public class CameraResult
    {
        public bool IsBroken { get; set; }
        /// <summary>
        /// The level of traffic congestion in the image represented as an enum to bound the possible values
        /// </summary>
        public TrafficCongestionLevel TrafficCongestionLevel { get; set; }
        public string Analysis { get; set; }

    }

    public enum TrafficCongestionLevel
    {
        Light,
        Moderate,
        Heavy,
        Unknown
    }



}
