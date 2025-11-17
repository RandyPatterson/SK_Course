// dotnet add package Microsoft.SemanticKernel
using System.ComponentModel;
using Chat.ModelBinders;
using Chat.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace Chat;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        //Add Model Binder for SK AuthorRole
        builder.Services.AddControllersWithViews(options => {
            options.ModelBinderProviders.Insert(0, new AuthorRoleBinderProvider());
        }).AddRazorRuntimeCompilation();

        //Add Semantic Kernel
        var kernelBuilder = builder.Services.AddKernel();

        //Add MCP Servers
        await AddFileSystemMcpServer(kernelBuilder);
        await AddGitHubMcpServer(kernelBuilder, builder.Configuration.GetValue<string>("GITHUB_PAT"));

        //Add Azure OpenAI Service
        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: builder.Configuration.GetValue<string>("AZURE_OPENAI_CHAT_DEPLOYMENT")!,
            endpoint: builder.Configuration.GetValue<string>("AZURE_OPENAI_ENDPOINT")!,
            apiKey: builder.Configuration.GetValue<string>("AZURE_OPENAI_KEY")!);


        // Disable (default) concurrent invocation of functions
        FunctionChoiceBehaviorOptions options = new() { AllowConcurrentInvocation = false };


        builder.Services.AddTransient<PromptExecutionSettings>( _ => new OpenAIPromptExecutionSettings {
            Temperature = 0.75,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: options)
        });

        var app = builder.Build();




        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}");

        app.Run();
    }

    /// <summary>
    /// Add Local MCP Server
    /// See: https://github.com/modelcontextprotocol/servers/tree/main/src/filesystem
    /// </summary>
    /// <param name="kernelBuilder"></param>
    /// <returns></returns>
    private static async Task  AddFileSystemMcpServer(IKernelBuilder kernelBuilder )
    {
        // Create an MCPClient for the GitHub server
        var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
        {
            Name = "FileSystem",
            Command = "npx",
            Arguments = ["-y", "@modelcontextprotocol/server-filesystem", "D:\\udemy\\module7\\Chat\\data\\"],
        }));

        // Retrieve the list of tools available on the GitHub server
        var tools = await mcpClient.ListToolsAsync();
        kernelBuilder.Plugins.AddFromFunctions("FS", tools.Select(skFunction => skFunction.AsKernelFunction()));
    }


    /// <summary>
    /// Add Remote Github MCP Server
    /// See: https://docs.github.com/en/copilot/how-tos/context/model-context-protocol/using-the-github-mcp-server#remote-mcp-server-configuration-with-oauth
    /// </summary>
    /// <param name="kernelBuilder"></param>
    /// <param name="github_apikey"></param>
    /// <returns></returns>
    private static async Task AddGitHubMcpServer(IKernelBuilder kernelBuilder, string github_apikey)
    {
        // Create an MCPClient for the GitHub server
        var mcpClient = await McpClient.CreateAsync(new HttpClientTransport(new()
        {
            Name = "GitHub",
            Endpoint = new Uri("https://api.githubcopilot.com/mcp/"),
            AdditionalHeaders = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {github_apikey}"
            }
        }));

        // Retrieve the list of tools available on the GitHub server
        var tools = await mcpClient.ListToolsAsync();
        kernelBuilder.Plugins.AddFromFunctions("GH", tools.Select(skFunction => skFunction.AsKernelFunction()));
    }
}
