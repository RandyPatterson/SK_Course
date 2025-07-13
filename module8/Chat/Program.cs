// dotnet add package Microsoft.SemanticKernel
using System.ClientModel;
using Azure;
using Azure.Search.Documents.Indexes;
using Chat.ModelBinders;
using Chat.Plugins;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//dotnet add package Microsoft.SemanticKernel.Connectors.AzureAISearch --prerelease

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

        //Add RAG Plugin
        kernelBuilder.Plugins.AddFromType<ContosoHealth>();

        //Add Azure OpenAI Service
        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: builder.Configuration.GetValue<string>("AZURE_OPENAI_CHAT_DEPLOYMENT")!,
            endpoint: builder.Configuration.GetValue<string>("AZURE_OPENAI_ENDPOINT")!,
            apiKey: builder.Configuration.GetValue<string>("AZURE_OPENAI_KEY")!);


        builder.Services.AddAzureOpenAIEmbeddingGenerator(
            deploymentName: builder.Configuration.GetValue<string>("EMBEDDING_DEPLOYNAME")!,
            endpoint: builder.Configuration.GetValue<string>("AZURE_OPENAI_ENDPOINT")!,
            apiKey: builder.Configuration.GetValue<string>("AZURE_OPENAI_KEY")!);


        builder.Services.AddSingleton(
            sp => new SearchIndexClient(
                new Uri(builder.Configuration.GetValue<string>("AI_SEARCH_ENDOINT")!),
                new AzureKeyCredential(builder.Configuration.GetValue<string>("AI_SEARCH_KEY")!)));
        
        builder.Services.AddAzureAISearchVectorStore();

        
        // disable concurrent invocation of functions to get the latest news and the current time
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
}
