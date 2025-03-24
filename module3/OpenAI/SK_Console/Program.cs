using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0001 

namespace SK_DEV
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Build and get configuration from appsettings.json, environment variables, and user secrets
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            // Create a kernel builder and add Azure OpenAI chat completion service
            var builder = Kernel.CreateBuilder();
            
            //Azure OpenAI
            //builder.AddAzureOpenAIChatCompletion(config["modelid"], config["endpoint"], config["apikey"]);

            //OpenAI
            builder.AddOpenAIChatCompletion(config["OpenAI:modelid"], config["OpenAI:apikey"]);

            // Build the kernel
            Kernel kernel = builder.Build();

            // Create chat history
            var history = new ChatHistory();

            // Get reference to chat completion service
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Define settings for OpenAI prompt execution
            OpenAIPromptExecutionSettings settings = new()
            {
                ChatSystemPrompt = "You are a friendly AI Assistant that answers in a friendly manner",
                Temperature = 0.9,
                MaxTokens = 1500,
            };

            // Create a chat history truncation reducer
            var reducer = new ChatHistoryTruncationReducer(targetCount: 10);
            // var reducer = new ChatHistorySummarizationReducer(chatCompletionService, 2, 2);

            foreach (var attr in chatCompletionService.Attributes)
                Console.WriteLine($"{attr.Key} \t\t{attr.Value}");

            // Control loop for user interaction
            while (true)
            {
                // Get input from user
                Console.Write("\nEnter your prompt: ");
                var prompt = Console.ReadLine();

                // Exit if prompt is null or empty
                if (string.IsNullOrEmpty(prompt))
                    break;

                string fullMessage = "";
                OpenAI.Chat.ChatTokenUsage usage = null;

                history.AddUserMessage(prompt);
                // Get streaming response from chat completion service
                await foreach (StreamingChatMessageContent responseChunk in chatCompletionService.GetStreamingChatMessageContentsAsync(history, settings))
                {
                    // Print response to console
                    Console.Write(responseChunk.Content);
                    fullMessage += responseChunk.Content;
                    usage = ((OpenAI.Chat.StreamingChatCompletionUpdate)responseChunk.InnerContent).Usage;
                }
                // Add response to chat history
                history.AddAssistantMessage(fullMessage);


                //get non-streaming result from chat completion setvice
                //var response = await chatCompletionService.GetChatMessageContentAsync(history, settings);
                //add response to chat history
                //history.Add(response);
                
                // Display number of tokens used (model specific)
                Console.WriteLine($"\n\tInput Tokens: \t{usage?.InputTokenCount}");
                Console.WriteLine($"\tOutput Tokens: \t{usage?.OutputTokenCount}");
                Console.WriteLine($"\tTotal Tokens: \t{usage?.TotalTokenCount}");

                // Reduce chat history if necessary
                var reduceMessages = await reducer.ReduceAsync(history);
                if (reduceMessages is not null)
                    history = new(reduceMessages);
            }
        }
    }
}