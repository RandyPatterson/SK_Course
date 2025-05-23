using Customer.API.Services;
using Scalar.AspNetCore;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
}); // Ensure the required package is installed

// Register the CustomerRepository as a singleton since it's our in-memory database
builder.Services.AddSingleton<CustomerRepository>();

var app = builder.Build();
app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});
app.MapScalarApiReference(options =>
{
    //Configure Scalar OpenAPI UI
    options
        .WithTitle("Customer API")
        .WithSidebar(true)
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDarkMode(false)
        .WithClientButton(true);
});

// Redirect root to Scalar UI
app.MapGet("/", () => Results.Redirect("/scalar/v1"));
app.UseAuthorization();
app.MapControllers();
app.Run();
