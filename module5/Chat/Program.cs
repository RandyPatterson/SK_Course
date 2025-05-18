// dotnet add package Microsoft.SemanticKernel
using Chat.ModelBinders;
using Chat.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace Chat;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        //Add Model Binder for SK AuthorRole
        builder.Services.AddControllersWithViews(options => {
            options.ModelBinderProviders.Insert(0, new AuthorRoleBinderProvider());
        }).AddRazorRuntimeCompilation();

        //Add Semantic Kernel
        var kernelBuilder = builder.Services.AddKernel();

        kernelBuilder.Plugins.AddFromType<GetDateTime>();

        //Add Azure OpenAI Service
        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: builder.Configuration.GetValue<string>("AZURE_OPENAI_CHAT_DEPLOYMENT")!,
            endpoint: builder.Configuration.GetValue<string>("AZURE_OPENAI_ENDPOINT")!,
            apiKey: builder.Configuration.GetValue<string>("AZURE_OPENAI_KEY")!);

        builder.Services.AddTransient<PromptExecutionSettings>( _ => new OpenAIPromptExecutionSettings {
            Temperature = 0.75,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
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
